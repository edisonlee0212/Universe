using System.Collections;
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

    public class Planet
    {
        #region Private
        private NativeArray<Vector3> _Vertices;
        private int[] _SharedTriangles;

        #endregion

        #region Public
        private Light m_SourceLight;
        private Quaternion _Rotation;
        private int _MaxLodLevel;
        private PlanetType _PlanetType;
        private NativeList<ShapeConstructionStage> _ShapeConstructionPipeline;
        private int _Index;
        private Material _SurfaceMaterial;
        private int _Resolution;
        private Noise _Noise;
        private double _Radius;
        private double3 _Position;
        //The lists of chunks in different levels.
        private NativeList<TerrainChunkInfo>[] _TerrainChunkInfos;
        //The list of meshes waiting to be rendered.
        private List<TerrainMesh> m_PlanetMeshes;
        private bool _Debug;
        public List<TerrainMesh> PlanetMeshes { get => m_PlanetMeshes; set => m_PlanetMeshes = value; }
        public NativeList<TerrainChunkInfo>[] TerrainChunkInfos { get => _TerrainChunkInfos; set => _TerrainChunkInfos = value; }
        public double3 Position { get => _Position; set => _Position = value; }
        public bool DebugMode { get => _Debug; set => _Debug = value; }
        public double Radius { get => _Radius; set => _Radius = value; }
        public int Resolution { get => _Resolution; set => _Resolution = value; }
        public Material SurfaceMaterial { get => _SurfaceMaterial; set => _SurfaceMaterial = value; }
        public NativeArray<Vector3> Vertices { get => _Vertices; set => _Vertices = value; }
        public int Index { get => _Index; set => _Index = value; }
        public Noise Noise { get => _Noise; set => _Noise = value; }
        public NativeList<ShapeConstructionStage> ShapeConstructionPipeline { get => _ShapeConstructionPipeline; set => _ShapeConstructionPipeline = value; }
        public PlanetType PlanetType { get => _PlanetType; set => _PlanetType = value; }
        public int MaxLodLevel { get => _MaxLodLevel; set => _MaxLodLevel = value; }
        public Quaternion Rotation { get => _Rotation; set => _Rotation = value; }
        public Light SourceLight { get => m_SourceLight; set => m_SourceLight = value; }
        #endregion

        #region Managers
        public Planet(PlanetInfo planetInfo, int maxLodLevel, int resolution, Material surfaceMaterial, Quaternion rotation, NativeQueue<MeshInfo> queue, int index)
        {
            _Rotation = rotation;
            _PlanetType = planetInfo.PlanetType;
            _Radius = planetInfo.Radius;
            _Position = planetInfo.Position;
            _MaxLodLevel = maxLodLevel;
            _Resolution = resolution;
            _SurfaceMaterial = surfaceMaterial;
            _MaxLodLevel = 10;
            _Index = index;
            Init(queue);
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
            _TerrainChunkInfos = new NativeList<TerrainChunkInfo>[_MaxLodLevel];
            for (int i = 0; i < _MaxLodLevel; i++)
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
        }

        public void ShutDown()
        {
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
            ShutDown();
        }
        #endregion

        public void RemoveChunkInfo(int level, int chunkInfoIndex)
        {
            //Update parent's index before Remove and swap back
            int parentLevel = level - 1;
            var lastInfoIndex = _TerrainChunkInfos[level].Length - 1;
            var lastInfo = _TerrainChunkInfos[level][lastInfoIndex];
            var parentInfo = _TerrainChunkInfos[parentLevel][lastInfo.ParentIndex];

            if (!lastInfo.IsMesh && level < _MaxLodLevel - 1)
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

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlanetarySystemSimulationSystem : JobComponentSystem
    {
        #region Private
        #endregion

        #region Public
        private static List<Planet> _Planets;
        public static List<Planet> Planets { get => _Planets; set => _Planets = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _Planets = new List<Planet>();
        }

        public void Init()
        {
            ShutDown();
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods
        public static int LoadPlanet(PlanetInfo planetInfo)
        {
            PlanetarySystemRenderSystem.LastChildLevelList.Add(0);
            PlanetarySystemRenderSystem.CreateQueueList.Add(new NativeQueue<int>(Allocator.Persistent));
            PlanetarySystemRenderSystem.DestroyQueueList.Add(new NativeQueue<int>(Allocator.Persistent));
            PlanetarySystemRenderSystem.GenerateMeshQueueList.Add(new NativeQueue<MeshInfo>(Allocator.Persistent));
            PlanetarySystemRenderSystem.RemoveMeshQueueList.Add(null);

            var planet = new Planet(planetInfo, maxLodLevel: 15, resolution: 64, new Material(Shader.Find("Standard")), Quaternion.identity, PlanetarySystemRenderSystem.GenerateMeshQueueList[_Planets.Count], _Planets.Count);

            _Planets.Add(planet);
           
            return _Planets.Count - 1;
        }

        public static bool UnLoadPlanet(int index)
        {
            Debug.Assert(index >= 0 && index < _Planets.Count);
            _Planets[index].ShutDown();
            if (PlanetarySystemRenderSystem.GenerateMeshQueueList[index].IsCreated) PlanetarySystemRenderSystem.GenerateMeshQueueList[index].Dispose();
            PlanetarySystemRenderSystem.GenerateMeshQueueList.RemoveAtSwapBack(index);
            PlanetarySystemRenderSystem.RemoveMeshQueueList.RemoveAtSwapBack(index);
            _Planets.RemoveAtSwapBack(index);
            PlanetarySystemRenderSystem.LastChildLevelList.RemoveAtSwapBack(index);
            if (PlanetarySystemRenderSystem.CreateQueueList[index].IsCreated) PlanetarySystemRenderSystem.CreateQueueList[index].Dispose();
            PlanetarySystemRenderSystem.CreateQueueList.RemoveAtSwapBack(index);
            if (PlanetarySystemRenderSystem.DestroyQueueList[index].IsCreated) PlanetarySystemRenderSystem.DestroyQueueList[index].Dispose();
            PlanetarySystemRenderSystem.DestroyQueueList.RemoveAtSwapBack(index);
            if (index < _Planets.Count) _Planets[index].Index = index;
            return true;
        }

        public static void Clear()
        {
            if (_Planets != null && _Planets.Count != 0)
            {
                while (_Planets.Count != 0)
                {
                    UnLoadPlanet(0);
                }
            }
        }
        #endregion

        #region Jobs
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }


    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class PlanetarySystemRenderSystem : JobComponentSystem
    {
        #region Private
        
        private Light _SolarLight;
        #endregion

        #region Public
        private static double3 _FloatingOrigin;
        private static int _LodDistance;
        private static int _Counter;
        private static float _Timer;
        private static List<NativeQueue<int>> _DestroyQueueList;
        private static List<NativeQueue<int>> _CreateQueueList;
        private static List<NativeQueue<MeshInfo>> _GenerateMeshQueueList;
        private static List<TerrainMesh[]> _RemoveMeshQueueList;
        private static List<int> _LastChildLevelList;
        public static int LodDistance { get => _LodDistance; set => _LodDistance = value; }
        public static double3 FloatingOrigin { get => _FloatingOrigin; set => _FloatingOrigin = value; }
        public static List<NativeQueue<int>> DestroyQueueList { get => _DestroyQueueList; set => _DestroyQueueList = value; }
        public static List<NativeQueue<int>> CreateQueueList { get => _CreateQueueList; set => _CreateQueueList = value; }
        public static List<NativeQueue<MeshInfo>> GenerateMeshQueueList { get => _GenerateMeshQueueList; set => _GenerateMeshQueueList = value; }
        public static List<TerrainMesh[]> RemoveMeshQueueList { get => _RemoveMeshQueueList; set => _RemoveMeshQueueList = value; }
        public static List<int> LastChildLevelList { get => _LastChildLevelList; set => _LastChildLevelList = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _LodDistance = 8;
            FloatingOrigin = double3.zero;
            GenerateMeshQueueList = new List<NativeQueue<MeshInfo>>();
            RemoveMeshQueueList = new List<TerrainMesh[]>();
            DestroyQueueList = new List<NativeQueue<int>>();
            CreateQueueList = new List<NativeQueue<int>>();
            LastChildLevelList = new List<int>();
        }

        public void Init()
        {
            ShutDown();
            Enabled = true;
            Clear();
        }

        public void ShutDown()
        {
            Enabled = false;
        }
        protected override void OnDestroy()
        {
            ShutDown();
            if (GenerateMeshQueueList != null)
            {
                foreach (var i in GenerateMeshQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
                GenerateMeshQueueList = null;
            }

            if (DestroyQueueList != null)
            {
                foreach (var i in DestroyQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
                DestroyQueueList = null;
            }

            if (CreateQueueList != null)
            {
                foreach (var i in CreateQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
                CreateQueueList = null;
            }
            RemoveMeshQueueList = null;
            LastChildLevelList = null;
        }
        #endregion

        #region Methods
        public void Clear()
        {
            if (GenerateMeshQueueList != null)
            {
                foreach (var i in GenerateMeshQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
            }

            if (DestroyQueueList != null)
            {
                foreach (var i in DestroyQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
            }

            if (CreateQueueList != null)
            {
                foreach (var i in CreateQueueList)
                {
                    if (i.IsCreated) i.Dispose();
                }
            }
            RemoveMeshQueueList.Clear();
            LastChildLevelList.Clear();
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
            public Quaternion rotation;
            public int lodDistance;
            [NativeDisableParallelForRestriction]
            public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                if (info.IsMesh && Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius, rotation), (float3)floatingOrigin) > (lodDistance * radius / Mathf.Pow(2, currentLevel + 1)))
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
            public Quaternion rotation;
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
                    if ((Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius, rotation), (float3)floatingOrigin)) > (lodDistance * radius / Mathf.Pow(2, currentLevel)))
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

        public JobHandle OnFixedUpdate(JobHandle inputDeps)
        {
            if (!Enabled) return inputDeps;
            _Counter++;
            foreach (var planet in PlanetarySystemSimulationSystem.Planets)
            {
                int index = planet.Index;
                if (GenerateMeshQueueList[index].Count == 0 && RemoveMeshQueueList[index] == null)
                {
                    for (int i = planet.MaxLodLevel - 1; i > 0; i--)
                    {
                        if (CreateQueueList[index].Count == 0 && DestroyQueueList[index].Count == 0)
                        {
                            inputDeps = new EvaluateDistance
                            {
                                currentLevel = i - 1,
                                centerPosition = planet.Position,
                                floatingOrigin = FloatingOrigin,
                                radius = planet.Radius,
                                rotation = planet.Rotation,
                                lodDistance = _LodDistance,
                                targetTerrainChunkInfos = planet.TerrainChunkInfos[i - 1].AsDeferredJobArray(),
                            }.Schedule(planet.TerrainChunkInfos[i - 1].Length, 1, inputDeps);
                            inputDeps.Complete();

                            inputDeps = new EvaluateDistance
                            {
                                currentLevel = i,
                                centerPosition = planet.Position,
                                floatingOrigin = FloatingOrigin,
                                rotation = planet.Rotation,
                                radius = planet.Radius,
                                lodDistance = _LodDistance,
                                targetTerrainChunkInfos = planet.TerrainChunkInfos[i].AsDeferredJobArray(),
                            }.Schedule(planet.TerrainChunkInfos[i].Length, 1, inputDeps);
                            inputDeps.Complete();
                            LastChildLevelList[planet.Index] = i;
                            Scan(ref inputDeps, planet, LastChildLevelList[index]);
                        }
                    }
                }
            }
            return inputDeps;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _Timer += Time.deltaTime;

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
            foreach (var planet in PlanetarySystemSimulationSystem.Planets)
            {
                var index = planet.Index;
                if (GenerateMeshQueueList[index].Count != 0)
                {
                    var meshInfo = GenerateMeshQueueList[index].Dequeue();
                    planet.AddMesh(ref inputDeps, ref meshInfo).Enable = true;
                }
                else if (RemoveMeshQueueList[planet.Index] != null)
                {
                    RemoveMeshQueueList[index] = null;
                }
                else if (CreateQueueList[index].Count != 0) ContinueAdd(ref inputDeps, planet);
                else if (DestroyQueueList[index].Count != 0) ContinueRemove(ref inputDeps, planet);
                foreach (var i in planet.PlanetMeshes)
                {
                    Quaternion.LookRotation((float3)planet.TerrainChunkInfos[i.DetailLevel][i.TerrainChunkInfoIndex].LocalUp);
                    //If the mesh is within the view frustum and its facing the player we draw it.
                    var drawPosition = (float3)(planet.Position - FloatingOrigin);

                    if (RemoveMeshQueueList[index] != null)
                        foreach (var j in RemoveMeshQueueList[index])
                        {
                            Graphics.DrawMesh(j.Mesh, drawPosition, planet.Rotation, planet.SurfaceMaterial, Vector3.Distance(Vector3.zero, drawPosition) < 10000 ? 0 : 0);
                        }
                    if (i.Enable)
                    {
                        Graphics.DrawMesh(i.Mesh, drawPosition, planet.Rotation, planet.SurfaceMaterial, Vector3.Distance(Vector3.zero, drawPosition) < 10000 ? 0 : 0);
                    }

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
            int currentLevel = LastChildLevelList[index] - 1;
            var createQueue = CreateQueueList[index];

            int chunkIndex = createQueue.Dequeue();
            var infoList = planet.TerrainChunkInfos;
            var terrainMeshes = new TerrainMesh[1];
            terrainMeshes[0] = planet.PlanetMeshes[infoList[currentLevel][chunkIndex].MeshIndex];
            RemoveMeshQueueList[index] = terrainMeshes;

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

            GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
            {
                level = currentLevel + 1,
                index = info.Child0
            });

            GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
            {
                level = currentLevel + 1,
                index = info.Child1
            });
            GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
            {
                level = currentLevel + 1,
                index = info.Child2
            });
            GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
            {
                level = currentLevel + 1,
                index = info.Child3
            });

            infoList[currentLevel][chunkIndex] = info;

        }

        private void ContinueRemove(ref JobHandle inputDeps, Planet planet)
        {
            var index = planet.Index;
            var lastChildLevelList = LastChildLevelList[index];
            var childChunkList = planet.TerrainChunkInfos[lastChildLevelList];
            var parentChunkList = planet.TerrainChunkInfos[lastChildLevelList - 1];
            var destroyQueue = DestroyQueueList[index];


            int parentChunkIndex = destroyQueue.Dequeue();
            var info = parentChunkList[parentChunkIndex];
            var terrainMeshes = new TerrainMesh[4];
            terrainMeshes[0] = planet.PlanetMeshes[childChunkList[info.Child0].MeshIndex];
            terrainMeshes[1] = planet.PlanetMeshes[childChunkList[info.Child1].MeshIndex];
            terrainMeshes[2] = planet.PlanetMeshes[childChunkList[info.Child2].MeshIndex];
            terrainMeshes[3] = planet.PlanetMeshes[childChunkList[info.Child3].MeshIndex];
            RemoveMeshQueueList[index] = terrainMeshes;

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
            GenerateMeshQueueList[planet.Index].Enqueue(new MeshInfo
            {
                level = lastChildLevelList - 1,
                index = parentChunkIndex
            });

        }

        private bool Scan(ref JobHandle inputDeps, Planet planet, int childLevel)
        {
            int index = planet.Index;
            inputDeps = new ScanJob
            {
                currentLevel = childLevel,
                centerPosition = planet.Position,
                floatingOrigin = FloatingOrigin,
                radius = planet.Radius,
                lodDistance = _LodDistance,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel - 1].AsDeferredJobArray(),
                childTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
                destroyChildrenQueue = DestroyQueueList[index].AsParallelWriter(),
                createChildrenQueue = CreateQueueList[index].AsParallelWriter()
            }.Schedule(planet.TerrainChunkInfos[childLevel - 1].Length, 1, inputDeps);
            inputDeps.Complete();
            return DestroyQueueList[index].Count == 0 && CreateQueueList[index].Count == 0;
        }
    }
    
}

