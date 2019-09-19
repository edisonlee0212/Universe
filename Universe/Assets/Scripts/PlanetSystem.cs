﻿using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Universe
{
    public enum PlanetType
    {
        Solar,
        Planet
    }
    public struct PlanetInfo
    {
        public PlanetType PlanetType;
        public double3 Position;
        public double Radius;
    }

    public struct MeshInfo
    {
        public int level, index;
        public bool enabled;
    }
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlanetarySystem : JobComponentSystem
    {
        #region Private
        private static List<NativeQueue<int>> _DestroyQueueList;
        private static List<NativeQueue<int>> _CreateQueueList;
        private static List<NativeQueue<MeshInfo>> _GenerateMeshQueueList;
        private static List<TerrainMesh[]> _RemoveMeshQueueList;
        private static List<List<TerrainMesh>> _MeshWaitingToDisplayList;
        private static List<int> _LastChildLevelList;
        private Light _SolarLight;
        #endregion

        #region Public
        private static double3 _FloatingOrigin;
        private static List<Planet> _Planets;
        private static int _LodDistance;
        private static int[] _LodLevelScanTime;
        private static int _Counter;
        private static float _Timer;
        public static int LodDistance { get => _LodDistance; set => _LodDistance = value; }
        public static List<Planet> Planets { get => _Planets; set => _Planets = value; }
        public static double3 FloatingOrigin { get => _FloatingOrigin; set => _FloatingOrigin = value; }
        public static int[] LodLevelScanTime { get => _LodLevelScanTime; set => _LodLevelScanTime = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _LodDistance = 8;
            FloatingOrigin = double3.zero;
            _LodLevelScanTime = new int[10];
            _LodLevelScanTime[0] = 10;
            _LodLevelScanTime[1] = 9;
            _LodLevelScanTime[2] = 8;
            _LodLevelScanTime[3] = 7;
            _LodLevelScanTime[4] = 6;
            _LodLevelScanTime[5] = 5;
            _LodLevelScanTime[6] = 4;
            _LodLevelScanTime[7] = 3;
            _LodLevelScanTime[8] = 2;
            _LodLevelScanTime[9] = 1;
            _GenerateMeshQueueList = new List<NativeQueue<MeshInfo>>();
            _RemoveMeshQueueList = new List<TerrainMesh[]>();
        }

        public void Init()
        {
            ShutDown();
            _DestroyQueueList = new List<NativeQueue<int>>();
            _CreateQueueList = new List<NativeQueue<int>>();
            _LastChildLevelList = new List<int>();
            _Planets = new List<Planet>();
            _MeshWaitingToDisplayList = new List<List<TerrainMesh>>();

            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;


            if (_Planets != null && _Planets.Count != 0)
            {
                while (_Planets.Count != 0)
                {
                    UnLoadPlanet(0);
                }
                _Planets = null;
            }
            if (_DestroyQueueList != null)
            {
                foreach (var i in _DestroyQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
                _DestroyQueueList = null;
            }

            if (_CreateQueueList != null)
            {
                foreach (var i in _CreateQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
                _CreateQueueList = null;
            }

            _LastChildLevelList = null;
            _MeshWaitingToDisplayList = null;
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods
        public static int LoadPlanet(PlanetInfo planetInfo)
        {
            var planet = new Planet(planetInfo, _LodLevelScanTime.Length, 64, new Material(Shader.Find("Standard")));
            planet.Index = _Planets.Count;
            _Planets.Add(planet);
            _LastChildLevelList.Add(0);
            _MeshWaitingToDisplayList.Add(new List<TerrainMesh>());
            _CreateQueueList.Add(new NativeQueue<int>(Allocator.Persistent));
            _DestroyQueueList.Add(new NativeQueue<int>(Allocator.Persistent));
            _GenerateMeshQueueList.Add(new NativeQueue<MeshInfo>(Allocator.Persistent));
            _RemoveMeshQueueList.Add(null);
            planet.Init(_GenerateMeshQueueList[planet.Index]);
            return _Planets.Count - 1;
        }

        public static bool UnLoadPlanet(int index)
        {
            Debug.Assert(index >= 0 && index < _Planets.Count);

            _Planets[index].ShutDown();
            if (_GenerateMeshQueueList[index].IsCreated) _GenerateMeshQueueList[index].Dispose();
            _GenerateMeshQueueList.RemoveAtSwapBack(index);
            _RemoveMeshQueueList.RemoveAtSwapBack(index);
            _MeshWaitingToDisplayList.RemoveAtSwapBack(index);
            _Planets.RemoveAtSwapBack(index);
            _LastChildLevelList.RemoveAtSwapBack(index);
            if (_CreateQueueList[index].IsCreated) _CreateQueueList[index].Dispose();
            _CreateQueueList.RemoveAtSwapBack(index);
            if (_DestroyQueueList[index].IsCreated) _DestroyQueueList[index].Dispose();
            _DestroyQueueList.RemoveAtSwapBack(index);
            if (index < _Planets.Count) _Planets[index].Index = index;
            return true;
        }
        #endregion

        #region Jobs

        [BurstCompile]
        protected struct EvaluateDistance : IJobParallelFor
        {
            public int currentLevel;
            public double3 centerPosition;
            public double3 floatingOrigin;
            public double radius;
            public int lodDistance;
            [NativeDisableParallelForRestriction]
            public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                if (info.IsMesh && Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin) > (lodDistance * radius / Mathf.Pow(2, currentLevel + 1)))
                {
                    info.TooFar = true;
                    targetTerrainChunkInfos[index] = info;
                }
                else
                {
                    info.TooFar = false;
                    targetTerrainChunkInfos[index] = info;
                }
            }
        }

        [BurstCompile]
        protected struct ScanJob : IJobParallelFor
        {
            public int currentLevel;
            public double3 centerPosition;
            public double3 floatingOrigin;
            public double radius;
            public int lodDistance;
            [ReadOnly] public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            [ReadOnly] public NativeArray<TerrainChunkInfo> childTerrainChunkInfos;
            [WriteOnly] public NativeQueue<int>.ParallelWriter destroyChildrenQueue;
            [WriteOnly] public NativeQueue<int>.ParallelWriter createChildrenQueue;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                //If the info has children and all children is agreed to collapse, we collapse.
                if (!info.IsMesh && childTerrainChunkInfos[info.Child0].TooFar && childTerrainChunkInfos[info.Child1].TooFar && childTerrainChunkInfos[info.Child2].TooFar && childTerrainChunkInfos[info.Child3].TooFar)
                {
                    if ((Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin)) > (lodDistance * radius / Mathf.Pow(2, currentLevel)))
                    {
                        destroyChildrenQueue.Enqueue(index);
                    }
                }
                else if (info.IsMesh && !info.TooFar)
                {
                    createChildrenQueue.Enqueue(index);
                }
            }
        }

        [BurstCompile]
        protected struct ResetChunkIndex : IJobParallelFor
        {
            public void Execute(int index)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        private void OnFixedUpdate(ref JobHandle inputDeps)
        {
            _Counter++;
            foreach (var planet in _Planets)
            {
                int index = planet.Index;
                if (_GenerateMeshQueueList[index].Count == 0 && _RemoveMeshQueueList[index] == null)
                {
                    for (int i = 0; i < _LodLevelScanTime.Length; i++)
                    {

                        if (_CreateQueueList[index].Count == 0 && _DestroyQueueList[index].Count == 0 && i > 0 && _Counter % _LodLevelScanTime[i] == 0)
                        {
                            inputDeps = new EvaluateDistance
                            {
                                currentLevel = i - 1,
                                centerPosition = planet.Position,
                                floatingOrigin = FloatingOrigin,
                                radius = planet.Radius,
                                lodDistance = _LodDistance,
                                targetTerrainChunkInfos = planet.TerrainChunkInfos[i - 1].AsDeferredJobArray(),
                            }.Schedule(planet.TerrainChunkInfos[i - 1].Length, 1, inputDeps);
                            inputDeps.Complete();

                            inputDeps = new EvaluateDistance
                            {
                                currentLevel = i,
                                centerPosition = planet.Position,
                                floatingOrigin = FloatingOrigin,
                                radius = planet.Radius,
                                lodDistance = _LodDistance,
                                targetTerrainChunkInfos = planet.TerrainChunkInfos[i].AsDeferredJobArray(),
                            }.Schedule(planet.TerrainChunkInfos[i].Length, 1, inputDeps);
                            inputDeps.Complete();
                            _LastChildLevelList[planet.Index] = i;
                            Scan(ref inputDeps, planet, _LastChildLevelList[index]);
                        }
                    }
                }
            }
            return;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _Timer += Time.deltaTime;
            if (_Timer > 0.05f)
            {
                _Timer = 0;
                OnFixedUpdate(ref inputDeps);
            }

            if (true)
            {
                var xz = ControlSystem.InputSystem.PlanetarySystem.MoveCamera.ReadValue<Vector2>();
                Vector3 forward = CameraModule.MainCameraTransform.forward;
                Vector3 right = CameraModule.MainCameraTransform.right;
                var y = ControlSystem.InputSystem.PlanetarySystem.AltCamera.ReadValue<float>();
                var delta = forward * xz.y + right * xz.x;
                _FloatingOrigin += new double3(delta.x, delta.y, delta.z) * 1000;
                _FloatingOrigin.y += y * 1000;
            }
            foreach (var planet in _Planets)
            {
                var index = planet.Index;
                if (_GenerateMeshQueueList[index].Count != 0)
                {
                    var meshInfo = _GenerateMeshQueueList[index].Dequeue();
                    _MeshWaitingToDisplayList[index].Add(planet.AddMesh(ref inputDeps, ref meshInfo));
                }
                else if (_RemoveMeshQueueList[planet.Index] != null)
                {
                    _RemoveMeshQueueList[index] = null;
                    for(int i = 0; i < _MeshWaitingToDisplayList[index].Count; i++)
                    {
                        var list = _MeshWaitingToDisplayList[index];
                        var terrainMesh = list[i];
                        terrainMesh.Enable = true;
                    }
                    _MeshWaitingToDisplayList[index].Clear();
                }
                else if (_CreateQueueList[index].Count != 0) ContinueAdd(ref inputDeps, planet);
                else if (_DestroyQueueList[index].Count != 0) ContinueRemove(ref inputDeps, planet);
                foreach (var i in planet.PlanetMeshes)
                {
                    Quaternion.LookRotation((float3)planet.TerrainChunkInfos[i.DetailLevel][i.TerrainChunkInfoIndex].LocalUp);
                    //If the mesh is within the view frustum and its facing the player we draw it.
                    var drawPosition = (float3)(planet.Position - FloatingOrigin);

                    if (_RemoveMeshQueueList[index] != null) foreach (var j in _RemoveMeshQueueList[index]) Graphics.DrawMesh(j.Mesh, drawPosition, Quaternion.identity, planet.SurfaceMaterial, Vector3.Distance(Vector3.zero, drawPosition) < 10000 ? 0 : 8);
                    if (i.Enable) Graphics.DrawMesh(i.Mesh, drawPosition, Quaternion.identity, planet.SurfaceMaterial, Vector3.Distance(Vector3.zero, drawPosition) < 10000 ? 0 : 8);

                }
            }
            return inputDeps;
        }

        private bool CheckFrustum(float3 position)
        {
            bool culled = false;

            return culled;
        }

        private void ContinueAdd(ref JobHandle inputDeps, Planet planet)
        {
            var index = planet.Index;
            int currentLevel = _LastChildLevelList[index] - 1;
            var createQueue = _CreateQueueList[index];
            for (int i = 0; i < createQueue.Count && i < 1; i++)
            {
                int chunkIndex = createQueue.Dequeue();
                var infoList = planet.TerrainChunkInfos;
                var terrainMeshes = new TerrainMesh[1];
                terrainMeshes[0] = planet.PlanetMeshes[infoList[currentLevel][chunkIndex].MeshIndex];
                _RemoveMeshQueueList[index] = terrainMeshes;

                planet.RemoveMesh(chunkIndex, currentLevel);

                var info = infoList[currentLevel][chunkIndex];
                info.IsMesh = false;

                double3 localUp = info.LocalUp;
                info.Child0 = infoList[currentLevel + 1].Length;
                var newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(info.ChunkCoordinate.x * 2, info.ChunkCoordinate.y * 2 + 1), true, true, localUp, planet.Resolution);
                infoList[currentLevel + 1].Add(newInfo);
                info.Child1 = infoList[currentLevel + 1].Length;
                newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(info.ChunkCoordinate.x * 2 + 1, info.ChunkCoordinate.y * 2 + 1), false, true, localUp, planet.Resolution);
                infoList[currentLevel + 1].Add(newInfo);
                info.Child2 = infoList[currentLevel + 1].Length;
                newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(info.ChunkCoordinate.x * 2, info.ChunkCoordinate.y * 2), true, false, localUp, planet.Resolution);
                infoList[currentLevel + 1].Add(newInfo);
                info.Child3 = infoList[currentLevel + 1].Length;
                newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(info.ChunkCoordinate.x * 2 + 1, info.ChunkCoordinate.y * 2), false, false, localUp, planet.Resolution);
                infoList[currentLevel + 1].Add(newInfo);

                _GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
                {
                    level = currentLevel + 1,
                    index = info.Child0
                });

                _GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
                {
                    level = currentLevel + 1,
                    index = info.Child1
                });
                _GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
                {
                    level = currentLevel + 1,
                    index = info.Child2
                });
                _GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
                {
                    level = currentLevel + 1,
                    index = info.Child3
                });

                infoList[currentLevel][chunkIndex] = info;
            }
        }

        private void ContinueRemove(ref JobHandle inputDeps, Planet planet)
        {
            var index = planet.Index;
            var lastChildLevelList = _LastChildLevelList[index];
            var childChunkList = planet.TerrainChunkInfos[lastChildLevelList];
            var parentChunkList = planet.TerrainChunkInfos[lastChildLevelList - 1];
            var destroyQueue = _DestroyQueueList[index];

            for (int i = 0; i < destroyQueue.Count && i < 1; i++)
            {
                int parentChunkIndex = destroyQueue.Dequeue();
                var info = parentChunkList[parentChunkIndex];
                var terrainMeshes = new TerrainMesh[4];
                terrainMeshes[0] = planet.PlanetMeshes[childChunkList[info.Child0].MeshIndex];
                terrainMeshes[1] = planet.PlanetMeshes[childChunkList[info.Child1].MeshIndex];
                terrainMeshes[2] = planet.PlanetMeshes[childChunkList[info.Child2].MeshIndex];
                terrainMeshes[3] = planet.PlanetMeshes[childChunkList[info.Child3].MeshIndex];
                _RemoveMeshQueueList[index] = terrainMeshes;

                planet.RemoveMesh(info.Child0, lastChildLevelList);
                planet.RemoveMesh(info.Child1, lastChildLevelList);
                planet.RemoveMesh(info.Child2, lastChildLevelList);
                planet.RemoveMesh(info.Child3, lastChildLevelList);

                planet.RemoveChunkInfo(lastChildLevelList, parentChunkList[parentChunkIndex].Child0);
                planet.RemoveChunkInfo(lastChildLevelList, parentChunkList[parentChunkIndex].Child1);
                planet.RemoveChunkInfo(lastChildLevelList, parentChunkList[parentChunkIndex].Child2);
                planet.RemoveChunkInfo(lastChildLevelList, parentChunkList[parentChunkIndex].Child3);

                for (int j = 0; j < childChunkList.Length; j++)
                {
                    if (childChunkList[j].IsMesh)
                    {
                        var terrainMesh = planet.PlanetMeshes[childChunkList[j].MeshIndex];
                        terrainMesh.TerrainChunkInfoIndex = j;
                        planet.PlanetMeshes[childChunkList[j].MeshIndex] = terrainMesh;
                    }
                }
                _GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
                {
                    level = lastChildLevelList - 1,
                    index = parentChunkIndex
                });
            }
        }

        private void Scan(ref JobHandle inputDeps, Planet planet, int childLevel)
        {
            inputDeps = new ScanJob
            {
                currentLevel = childLevel,
                centerPosition = planet.Position,
                floatingOrigin = FloatingOrigin,
                radius = planet.Radius,
                lodDistance = _LodDistance,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel - 1].AsDeferredJobArray(),
                childTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
                destroyChildrenQueue = _DestroyQueueList[planet.Index].AsParallelWriter(),
                createChildrenQueue = _CreateQueueList[planet.Index].AsParallelWriter()
            }.Schedule(planet.TerrainChunkInfos[childLevel - 1].Length, 1, inputDeps);
            inputDeps.Complete();
        }
    }
    public class Planet
    {
        #region Private
        private NativeArray<Vector3> _Vertices;
        private int[] _SharedTriangles;

        #endregion

        #region Public
        private PlanetType _PlanetType;
        private NativeList<ShapeConstructionStage> _ShapeConstructionPipeline;
        private bool _Enabled;
        private int _Index;
        private Material _SurfaceMaterial;
        private int _Resolution;
        private Noise _Noise;
        private double _Radius;
        private double3 _Position;
        private int _LevelAmount;
        //The lists of chunks in different levels.
        private NativeList<TerrainChunkInfo>[] _TerrainChunkInfos;
        //The list of meshes waiting to be rendered.
        private List<TerrainMesh> m_PlanetMeshes;
        private bool _Debug;
        public int LevelAmount { get => _LevelAmount; set => _LevelAmount = value; }
        public List<TerrainMesh> PlanetMeshes { get => m_PlanetMeshes; set => m_PlanetMeshes = value; }
        public NativeList<TerrainChunkInfo>[] TerrainChunkInfos { get => _TerrainChunkInfos; set => _TerrainChunkInfos = value; }
        public double3 Position { get => _Position; set => _Position = value; }
        public bool DebugMode { get => _Debug; set => _Debug = value; }
        public double Radius { get => _Radius; set => _Radius = value; }
        public int Resolution { get => _Resolution; set => _Resolution = value; }
        public Material SurfaceMaterial { get => _SurfaceMaterial; set => _SurfaceMaterial = value; }
        public NativeArray<Vector3> Vertices { get => _Vertices; set => _Vertices = value; }
        public int Index { get => _Index; set => _Index = value; }
        public bool Enabled { get => _Enabled; }
        public Noise Noise { get => _Noise; set => _Noise = value; }
        public NativeList<ShapeConstructionStage> ShapeConstructionPipeline { get => _ShapeConstructionPipeline; set => _ShapeConstructionPipeline = value; }
        public PlanetType PlanetType { get => _PlanetType; set => _PlanetType = value; }
        #endregion

        #region Managers
        public Planet(PlanetInfo planetInfo, int levelAmount, int resolution, Material surfaceMaterial)
        {
            _PlanetType = planetInfo.PlanetType;
            _Radius = planetInfo.Radius;
            _Position = planetInfo.Position;
            _LevelAmount = levelAmount;
            _Resolution = resolution;
            _SurfaceMaterial = surfaceMaterial;
            _Enabled = false;
        }

        public void Init(NativeQueue<MeshInfo> queue)
        {
            _ShapeConstructionPipeline = new NativeList<ShapeConstructionStage>(Allocator.Persistent);
            if (_PlanetType != PlanetType.Solar)
            {
                _ShapeConstructionPipeline.Add(new ShapeConstructionStage
                {
                    stageType = ShapeConstructionStageType.SimpleNoiseFilter,
                    strength = 0.07,
                    numLayers = 4,
                    baseRoughness = 1.07,
                    roughness = 2.2,
                    persistence = 0.5,
                    center = new double3(1.11, 0.92, -0.39),
                });
                _ShapeConstructionPipeline.Add(new ShapeConstructionStage
                {
                    stageType = ShapeConstructionStageType.SeaLevel,
                    minValue = 0
                });
            }
            else
            {
                _SurfaceMaterial = Resources.Load<Material>("Materials/PlanetSurface");
            }

            _Vertices = new NativeArray<Vector3>(_Resolution * _Resolution, Allocator.Persistent);
            _SharedTriangles = new int[_Resolution * _Resolution * 6];
            int triIndex = 0;
            for (int y = 0; y < _Resolution; y++)
            {
                for (int x = 0; x < _Resolution; x++)
                {
                    int i = x + y * _Resolution;

                    if (x != _Resolution - 1 && y != _Resolution - 1)
                    {
                        _SharedTriangles[triIndex] = i;
                        _SharedTriangles[triIndex + 1] = i + _Resolution + 1;
                        _SharedTriangles[triIndex + 2] = i + _Resolution;

                        _SharedTriangles[triIndex + 3] = i;
                        _SharedTriangles[triIndex + 4] = i + 1;
                        _SharedTriangles[triIndex + 5] = i + _Resolution + 1;
                        triIndex += 6;
                    }
                }
            }
            _TerrainChunkInfos = new NativeList<TerrainChunkInfo>[_LevelAmount];
            for (int i = 0; i < _LevelAmount; i++)
            {
                _TerrainChunkInfos[i] = new NativeList<TerrainChunkInfo>(Allocator.Persistent);
            }
            m_PlanetMeshes = new List<TerrainMesh>();
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(1, 0, 0), _Resolution));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 1, 0), _Resolution));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 0, 1), _Resolution));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(-1, 0, 0), _Resolution));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, -1, 0), _Resolution));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 0, -1), _Resolution));
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 0,
                enabled = true
            });
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 1,
                enabled = true
            });
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 2,
                enabled = true
            });
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 3,
                enabled = true
            });
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 4,
                enabled = true
            });
            queue.Enqueue(new MeshInfo
            {
                level = 0,
                index = 5,
                enabled = true
            });
            _Enabled = true;
        }

        public void ShutDown()
        {
            _Enabled = false;
            if (_ShapeConstructionPipeline.IsCreated) _ShapeConstructionPipeline.Dispose();
            if (_Vertices.IsCreated) _Vertices.Dispose();
            if (_TerrainChunkInfos != null)
            {
                foreach (var i in _TerrainChunkInfos)
                {
                    if (i.IsCreated) i.Dispose();
                }
            }
        }

        ~Planet()
        {
            if (Enabled) ShutDown();
        }
        #endregion

        public void RemoveChunkInfo(int level, int chunkInfoIndex)
        {
            //Update parent's index before Remove and swap back
            int parentLevel = level - 1;
            var lastInfoIndex = _TerrainChunkInfos[level].Length - 1;
            var lastInfo = _TerrainChunkInfos[level][lastInfoIndex];
            var parentInfo = _TerrainChunkInfos[parentLevel][lastInfo.ParentIndex];

            if (!lastInfo.IsMesh && level < _LevelAmount - 1)
            {
                var childChunkList = _TerrainChunkInfos[level + 1];
                var info = childChunkList[lastInfo.Child0];
                info.ParentIndex = chunkInfoIndex;
                childChunkList[lastInfo.Child0] = info;

                info = childChunkList[lastInfo.Child1];
                info.ParentIndex = chunkInfoIndex;
                childChunkList[lastInfo.Child1] = info;

                info = childChunkList[lastInfo.Child2];
                info.ParentIndex = chunkInfoIndex;
                childChunkList[lastInfo.Child2] = info;

                info = childChunkList[lastInfo.Child3];
                info.ParentIndex = chunkInfoIndex;
                childChunkList[lastInfo.Child3] = info;
            }

            if (lastInfo.IsUp)
            {
                if (lastInfo.IsLeft)
                {
                    parentInfo.Child0 = chunkInfoIndex;
                }
                else
                {
                    parentInfo.Child1 = chunkInfoIndex;
                }
            }
            else
            {
                if (lastInfo.IsLeft)
                {
                    parentInfo.Child2 = chunkInfoIndex;
                }
                else
                {
                    parentInfo.Child3 = chunkInfoIndex;
                }
            }
            _TerrainChunkInfos[parentLevel][lastInfo.ParentIndex] = parentInfo;
            _TerrainChunkInfos[level].RemoveAtSwapBack(chunkInfoIndex);
        }

        public void RemoveMesh(int chunkIndex, int level)
        {
            int meshIndex = _TerrainChunkInfos[level][chunkIndex].MeshIndex;
            m_PlanetMeshes.RemoveAtSwapBack(meshIndex);
            if (meshIndex < m_PlanetMeshes.Count)
            {
                var terrainMesh = m_PlanetMeshes[meshIndex];
                var info = _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex];
                info.MeshIndex = meshIndex;
                _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex] = info;
            }
        }

        public void RemoveMesh(int meshIndex)
        {
            m_PlanetMeshes.RemoveAtSwapBack(meshIndex);
            if (meshIndex < m_PlanetMeshes.Count)
            {
                var terrainMesh = m_PlanetMeshes[meshIndex];
                var info = _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex];
                info.MeshIndex = meshIndex;
                _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex] = info;
            }
        }


        //[BurstCompile]
        public struct ConstructVerticesPipeline : IJobParallelFor
        {
            [NativeDisableParallelForRestriction]
            [WriteOnly] public NativeArray<Vector3> vertices;
            public double radius;
            public TerrainChunkInfo terrainChunkInfo;
            [ReadOnly] public NativeArray<ShapeConstructionStage> shapeConstructionStages;
            [ReadOnly] public Noise noise;
            public void Execute(int index)
            {
                int actualDetailLevel = (int)math.pow(2, terrainChunkInfo.DetailLevel);
                int x = index % terrainChunkInfo.Resolution;
                int y = index / terrainChunkInfo.Resolution;
                double2 percent = new double2(x, y) / (terrainChunkInfo.Resolution - 1) / actualDetailLevel;
                double3 pointOnUnitCube = terrainChunkInfo.LocalUp + (percent.x + (double)terrainChunkInfo.ChunkCoordinate.x / actualDetailLevel - .5D) * 2 * terrainChunkInfo.AxisA + (percent.y + (double)terrainChunkInfo.ChunkCoordinate.y / actualDetailLevel - .5D) * 2 * terrainChunkInfo.AxisB;
                terrainChunkInfo.Normalize(ref pointOnUnitCube);
                double elevation = 0;
                double previousResult = 0;
                for (int i = 0; i < shapeConstructionStages.Length; i++)
                {
                    previousResult = shapeConstructionStages[i].Process(pointOnUnitCube, ref noise, previousResult, elevation);
                    elevation += previousResult;
                }
                vertices[index] = (float3)(pointOnUnitCube * radius * (1D + elevation));
            }
        }

        public TerrainMesh AddMesh(ref JobHandle inputDeps, ref MeshInfo meshInfo)
        {
            int currentLevel = meshInfo.level;
            int chunkIndex = meshInfo.index;
            var terrainChunkInfo = _TerrainChunkInfos[currentLevel][chunkIndex];
            
            terrainChunkInfo.IsMesh = true;
            inputDeps = new ConstructVerticesPipeline
            {
                vertices = _Vertices,
                radius = Radius,
                terrainChunkInfo = terrainChunkInfo,
                shapeConstructionStages = _ShapeConstructionPipeline.AsParallelReader(),
                noise = _Noise
            }.Schedule(_Vertices.Length, 1, inputDeps);
            
            inputDeps.Complete();
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = _Vertices.ToArray();
            mesh.triangles = _SharedTriangles;
            mesh.RecalculateNormals();
            terrainChunkInfo.MeshIndex = m_PlanetMeshes.Count;
            var terrainMesh = new TerrainMesh
            {
                DetailLevel = currentLevel,
                TerrainChunkInfoIndex = chunkIndex,
                Mesh = mesh,
                Enable = meshInfo.enabled
            };
            m_PlanetMeshes.Add(terrainMesh);
            _TerrainChunkInfos[currentLevel][chunkIndex] = terrainChunkInfo;
            return terrainMesh;
        }
    }
}

