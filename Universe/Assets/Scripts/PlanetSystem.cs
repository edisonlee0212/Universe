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
    public struct PlanetInfo
    {
        public double3 Position;
        public double Radius;
    }
    public class PlanetarySystem : JobComponentSystem
    {
        #region Private
        private static NativeQueue<int> _DestroyQueue;
        private float _z;
        private float _x;
        #endregion

        #region Public
        private bool _EnableFrustumCulling;
        private static double3 _FloatingOrigin;
        private static List<Planet> _Planets;
        private static int _LodDistance;
        public static int LodDistance { get => _LodDistance; set => _LodDistance = value; }
        public static List<Planet> Planets { get => _Planets; set => _Planets = value; }
        public static double3 FloatingOrigin { get => _FloatingOrigin; set => _FloatingOrigin = value; }
        public bool EnableFrustumCulling { get => _EnableFrustumCulling; set => _EnableFrustumCulling = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _z = 0;
            _x = 0;
            _LodDistance = 100000;
            FloatingOrigin = double3.zero;

            _Planets = new List<Planet>();
        }

        public void Init()
        {
            ShutDown();
            _DestroyQueue = new NativeQueue<int>(Allocator.Persistent);
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_DestroyQueue.IsCreated) _DestroyQueue.Dispose();

            if (_Planets != null && _Planets.Count != 0)
            {
                foreach (var i in _Planets)
                {
                    i.ShutDown();
                }
            }
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods
        public static int LoadPlanet(PlanetInfo planetInfo)
        {
            var planet = new Planet(planetInfo, 6, 64, new Material(Shader.Find("Standard")));
            planet.Init();
            _Planets.Add(planet);
            return _Planets.Count - 1;
        }

        public static void UnLoadPlanet(int index)
        {
            Debug.Assert(index > 0 && index < _Planets.Count);
            _Planets[index].ShutDown();
            _Planets.RemoveAtSwapBack(index);
        }
        #endregion

        #region Jobs
        [BurstCompile]
        protected struct ScanDistance : IJobParallelFor
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
                if (info.IsMesh && Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin) > (lodDistance / Mathf.Pow(2, currentLevel + 1)))
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
        protected struct ScanChild : IJobParallelFor
        {
            public int currentLevel;
            public double3 centerPosition;
            public double3 floatingOrigin;
            public double radius;
            public int lodDistance;
            [ReadOnly] public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            [ReadOnly] public NativeArray<TerrainChunkInfo> childTerrainChunkInfos;
            [WriteOnly] public NativeQueue<int>.ParallelWriter destroyChildrenQueue;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                //If the info has children and all children is agreed to collapse, we collapse.
                if (!info.IsMesh && childTerrainChunkInfos[info.Child0].TooFar && childTerrainChunkInfos[info.Child1].TooFar && childTerrainChunkInfos[info.Child2].TooFar && childTerrainChunkInfos[info.Child3].TooFar)
                {
                    if ((Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin)) > (lodDistance / Mathf.Pow(2, currentLevel)))
                    {
                        destroyChildrenQueue.Enqueue(index);
                    }
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

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetKey(KeyCode.S)) _z -= Time.deltaTime * 15000;
            else if (Input.GetKey(KeyCode.W)) _z += Time.deltaTime * 15000;
            if (Input.GetKey(KeyCode.D)) _x += Time.deltaTime * 15000;
            else if (Input.GetKey(KeyCode.A)) _x -= Time.deltaTime * 15000;
            FloatingOrigin = new double3(_x, 0, _z);

            foreach (var planet in _Planets)
            {
                planet.ReEval(FloatingOrigin);

                ScanAndRemove(ref inputDeps, planet, planet.LevelAmount - 1);
                ScanAndRemove(ref inputDeps, planet, planet.LevelAmount - 2);
                ScanAndRemove(ref inputDeps, planet, planet.LevelAmount - 3);
                ScanAndRemove(ref inputDeps, planet, planet.LevelAmount - 4);
                ScanAndRemove(ref inputDeps, planet, planet.LevelAmount - 5);
                foreach (var i in planet.PlanetMeshes)
                {
                    Quaternion.LookRotation((float3)planet.TerrainChunkInfos[i.DetailLevel][i.TerrainChunkInfoIndex].LocalUp);
                    //If the mesh is within the view frustum and its facing the player we draw it.
                    bool culled = false;
                    var drawPosition = (float3)(planet.Position - FloatingOrigin);
                    if (_EnableFrustumCulling)
                    {
                        culled = CheckFrustum(drawPosition);
                    }
                    if (!culled) Graphics.DrawMesh(i.Mesh, drawPosition, Quaternion.identity, planet.SurfaceMaterial, 0);
                }
            }
            return inputDeps;
        }

        private bool CheckFrustum(float3 position)
        {
            bool culled = false;

            return culled;
        }

        private void ScanAndRemove(ref JobHandle inputDeps, Planet planet, int childLevel)
        {
            inputDeps = new ScanDistance
            {
                currentLevel = childLevel,
                centerPosition = planet.Position,
                floatingOrigin = FloatingOrigin,
                radius = planet.Radius,
                lodDistance = _LodDistance,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
            }.Schedule(planet.TerrainChunkInfos[childLevel].Length, 1, inputDeps);
            inputDeps.Complete();

            inputDeps = new ScanChild
            {
                currentLevel = childLevel,
                centerPosition = planet.Position,
                floatingOrigin = FloatingOrigin,
                radius = planet.Radius,
                lodDistance = _LodDistance,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel - 1].AsDeferredJobArray(),
                childTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
                destroyChildrenQueue = _DestroyQueue.AsParallelWriter()
            }.Schedule(planet.TerrainChunkInfos[childLevel - 1].Length, 1, inputDeps);
            inputDeps.Complete();

            var childChunkList = planet.TerrainChunkInfos[childLevel];
            var parentChunkList = planet.TerrainChunkInfos[childLevel - 1];
            int[] children = new int[4];
            while (_DestroyQueue.Count > 0)
            {
                int parentChunkIndex = _DestroyQueue.Dequeue();
                children[0] = parentChunkList[parentChunkIndex].Child0;
                children[1] = parentChunkList[parentChunkIndex].Child1;
                children[2] = parentChunkList[parentChunkIndex].Child2;
                children[3] = parentChunkList[parentChunkIndex].Child3;
                planet.RemoveMesh(children[0], childLevel);
                planet.RemoveMesh(children[1], childLevel);
                planet.RemoveMesh(children[2], childLevel);
                planet.RemoveMesh(children[3], childLevel);

                children[0] = parentChunkList[parentChunkIndex].Child0;
                planet.RemoveChunkInfo(childLevel, children[0]);
                children[1] = parentChunkList[parentChunkIndex].Child1;
                planet.RemoveChunkInfo(childLevel, children[1]);
                children[2] = parentChunkList[parentChunkIndex].Child2;
                planet.RemoveChunkInfo(childLevel, children[2]);
                children[3] = parentChunkList[parentChunkIndex].Child3;
                planet.RemoveChunkInfo(childLevel, children[3]);
                var info = parentChunkList[parentChunkIndex];

                for (int i = 0; i < childChunkList.Length; i++)
                {
                    if (childChunkList[i].IsMesh)
                    {
                        var terrainMesh = planet.PlanetMeshes[childChunkList[i].MeshIndex];
                        terrainMesh.TerrainChunkInfoIndex = i;
                        planet.PlanetMeshes[childChunkList[i].MeshIndex] = terrainMesh;
                    }
                }

                planet.GenerateMesh(ref info, childLevel - 1, parentChunkIndex);

            }
        }
    }
    public class Planet
    {
        private Material _SurfaceMaterial;
        private int _Resolution;
        private double _Radius;
        private double3 _Position;
        private float[] _RefreshTimeStep;
        private int _LevelAmount;
        //The lists of chunks in different levels.
        private NativeList<TerrainChunkInfo>[] _TerrainChunkInfos;
        //The list of meshes waiting to be rendered.
        private List<TerrainMesh> m_PlanetMeshes;
        private int[] _SharedTriangles;
        private bool _Debug;
        public int LevelAmount { get => _LevelAmount; set => _LevelAmount = value; }
        public List<TerrainMesh> PlanetMeshes { get => m_PlanetMeshes; set => m_PlanetMeshes = value; }
        public NativeList<TerrainChunkInfo>[] TerrainChunkInfos { get => _TerrainChunkInfos; set => _TerrainChunkInfos = value; }
        public double3 Position { get => _Position; set => _Position = value; }
        public bool DebugMode { get => _Debug; set => _Debug = value; }
        public double Radius { get => _Radius; set => _Radius = value; }
        public int Resolution { get => _Resolution; set => _Resolution = value; }
        public Material SurfaceMaterial { get => _SurfaceMaterial; set => _SurfaceMaterial = value; }

        public Planet(PlanetInfo planetInfo, int levelAmount, int resolution, Material surfaceMaterial)
        {
            _Radius = planetInfo.Radius;
            _Position = planetInfo.Position;
            _LevelAmount = levelAmount;
            _Resolution = resolution;
            _SurfaceMaterial = surfaceMaterial;
        }

        public void Init()
        {
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
            ReEval(double3.zero);
        }

        public void ReEval(double3 originPosition)
        {
            EvaluateChunk(ref originPosition, 0, 0);
            EvaluateChunk(ref originPosition, 0, 1);
            EvaluateChunk(ref originPosition, 0, 2);
            EvaluateChunk(ref originPosition, 0, 3);
            EvaluateChunk(ref originPosition, 0, 4);
            EvaluateChunk(ref originPosition, 0, 5);
        }

        public void ShutDown()
        {
            if (_TerrainChunkInfos != null)
            {
                foreach (var i in _TerrainChunkInfos)
                {
                    if (i.IsCreated) i.Dispose();
                }
            }
        }

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

        public void RemoveMesh(int childChunkIndex, int childLevel)
        {
            int meshIndex = _TerrainChunkInfos[childLevel][childChunkIndex].MeshIndex; 
            m_PlanetMeshes.RemoveAtSwapBack(meshIndex);
            if (meshIndex < m_PlanetMeshes.Count)
            {
                var terrainMesh = m_PlanetMeshes[meshIndex];
                var info = _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex];
                info.MeshIndex = meshIndex;
                _TerrainChunkInfos[terrainMesh.DetailLevel][terrainMesh.TerrainChunkInfoIndex] = info;
            }
        }

        public void GenerateMesh(ref TerrainChunkInfo terrainChunkInfo, int currentLevel, int chunkIndex)
        {
            terrainChunkInfo.IsMesh = true;
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[_Resolution * _Resolution];
            terrainChunkInfo.ConstructVertices(_Radius, ref vertices);
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = _SharedTriangles;
            mesh.RecalculateNormals();
            terrainChunkInfo.MeshIndex = m_PlanetMeshes.Count;
            m_PlanetMeshes.Add(new TerrainMesh
            {
                DetailLevel = currentLevel,
                TerrainChunkInfoIndex = chunkIndex,
                Mesh = mesh
            });
            _TerrainChunkInfos[currentLevel][chunkIndex] = terrainChunkInfo;
        }

        public bool EvaluateChunk(ref double3 fromPosition, int currentLevel, int chunkIndex)
        {
            var terrainChunkInfo = _TerrainChunkInfos[currentLevel][chunkIndex];
            if (terrainChunkInfo.MeshIndex == -1)
            {
                GenerateMesh(ref terrainChunkInfo, currentLevel, chunkIndex);
            }
            //If this chunk is not a chunk with mesh, then we evaluate its children.
            var distance = Vector3.Distance((float3)terrainChunkInfo.ChunkCenterPosition(_Position, _Radius), (float3)fromPosition);
            //If the distance is too far, we return true, let parent to decide if we need to join chunks.
            if (distance > PlanetarySystem.LodDistance / Mathf.Pow(2, (currentLevel + 1)))
            {
                return true;
            }
            else
            {
                if (currentLevel < _LevelAmount - 1)
                {
                    if (terrainChunkInfo.IsMesh)
                    {
                        var lastMesh = m_PlanetMeshes[m_PlanetMeshes.Count - 1];
                        var lastMeshInfo = _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex];
                        lastMeshInfo.MeshIndex = terrainChunkInfo.MeshIndex;
                        _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex] = lastMeshInfo;
                        terrainChunkInfo.IsMesh = false;
                        m_PlanetMeshes.RemoveAtSwapBack(terrainChunkInfo.MeshIndex);

                        //create four children and further evaluate.
                        double3 localUp = terrainChunkInfo.LocalUp;
                        terrainChunkInfo.Child0 = _TerrainChunkInfos[currentLevel + 1].Length;
                        var newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2, terrainChunkInfo.ChunkCoordinate.y * 2 + 1), true, true, localUp, _Resolution);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child1 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2 + 1, terrainChunkInfo.ChunkCoordinate.y * 2 + 1), false, true, localUp, _Resolution);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child2 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2, terrainChunkInfo.ChunkCoordinate.y * 2), true, false, localUp, _Resolution);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child3 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2 + 1, terrainChunkInfo.ChunkCoordinate.y * 2), false, false, localUp, _Resolution);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                    }
                    EvaluateChunk(ref fromPosition, currentLevel + 1, terrainChunkInfo.Child0);
                    EvaluateChunk(ref fromPosition, currentLevel + 1, terrainChunkInfo.Child1);
                    EvaluateChunk(ref fromPosition, currentLevel + 1, terrainChunkInfo.Child2);
                    EvaluateChunk(ref fromPosition, currentLevel + 1, terrainChunkInfo.Child3);
                }
                _TerrainChunkInfos[currentLevel][chunkIndex] = terrainChunkInfo;
                return false;
            }
        }
    }

}

