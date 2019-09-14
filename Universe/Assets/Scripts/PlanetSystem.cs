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
    //[DisableAutoCreation]
    public class PlanetSystem : JobComponentSystem
    {
        private static NativeQueue<int> _DestroyQueue;
        private Planet _Planet;
        private static double3 _FloatingOrigin;
        private float _z;
        private Material _Material;
        #region Managers
        protected override void OnCreate()
        {
            _z = 0;
            _FloatingOrigin = double3.zero;
            _Planet = new Planet(new double3(0, 0, 3000), 3000, 6);
            _Planet.Init();
            _Planet.ReEval(_FloatingOrigin);
            //Debug.Log(_Planet.TerrainChunkInfos[0].Length);
            //Debug.Log(_Planet.TerrainChunkInfos[1].Length);
            _Material = new Material(Shader.Find("Standard"));
            Init();

        }

        public void Init()
        {
            _DestroyQueue = new NativeQueue<int>(Allocator.Persistent);
        }

        public void ShutDown()
        {
            if (_DestroyQueue.IsCreated) _DestroyQueue.Dispose();
        }
        protected override void OnDestroy()
        {
            _Planet.ShutDown();
            ShutDown();
        }
        #endregion
        #region Methods

        #endregion

        #region Jobs
        [BurstCompile]
        protected struct ScanDistance : IJobParallelFor
        {
            public int currentLevel;
            public double3 centerPosition;
            public double3 floatingOrigin;
            public double radius;
            [NativeDisableParallelForRestriction]
            public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                if (info.IsMesh && Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin) > radius * 2 / (currentLevel + 1))
                {
                    info.TooFar = true;
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
            [ReadOnly] public NativeArray<TerrainChunkInfo> targetTerrainChunkInfos;
            [ReadOnly] public NativeArray<TerrainChunkInfo> childTerrainChunkInfos;
            [WriteOnly] public NativeQueue<int>.ParallelWriter destroyChildredQueue;
            public void Execute(int index)
            {
                var info = targetTerrainChunkInfos[index];
                //If the info has children and all children is agreed to collapse, we collapse.
                if (!info.IsMesh && childTerrainChunkInfos[info.Child0].TooFar && childTerrainChunkInfos[info.Child1].TooFar && childTerrainChunkInfos[info.Child2].TooFar && childTerrainChunkInfos[info.Child3].TooFar)
                {
                    if ((Vector3.Distance((float3)info.ChunkCenterPosition(centerPosition, radius), (float3)floatingOrigin)) > (radius * 2 / (currentLevel)))
                    {
                        destroyChildredQueue.Enqueue(index);
                    }
                }
            }
        }


        #endregion
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_z > -2000) _z -= Time.deltaTime * 500;
            _FloatingOrigin = new double3(0, 0, _z);
            _Planet.ReEval(_FloatingOrigin);
            ScanAndRemove(ref inputDeps, _Planet, _Planet.LevelAmount - 1);
            foreach (var i in _Planet.PlanetMeshes)
            {
                Quaternion.LookRotation((float3)_Planet.TerrainChunkInfos[i.DetailLevel][i.TerrainChunkInfoIndex].LocalUp);
                //If the mesh is within the view frustum and its facing the player we draw it.
                if (true) Graphics.DrawMesh(i.Mesh, (float3)(_Planet.Position - _FloatingOrigin), Quaternion.identity, _Material, 0);
            }
            return inputDeps;
        }

        private void ScanAndRemove(ref JobHandle inputDeps, Planet planet, int childLevel)
        {
            inputDeps = new ScanDistance
            {
                currentLevel = childLevel,
                centerPosition = _Planet.Position,
                floatingOrigin = _FloatingOrigin,
                radius = planet.Radius,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
            }.Schedule(planet.TerrainChunkInfos[childLevel].Length, 1, inputDeps);
            inputDeps.Complete();

            inputDeps = new ScanChild
            {
                currentLevel = childLevel,
                centerPosition = _Planet.Position,
                floatingOrigin = _FloatingOrigin,
                radius = planet.Radius,
                targetTerrainChunkInfos = planet.TerrainChunkInfos[childLevel - 1].AsDeferredJobArray(),
                childTerrainChunkInfos = planet.TerrainChunkInfos[childLevel].AsDeferredJobArray(),
                destroyChildredQueue = _DestroyQueue.AsParallelWriter()
            }.Schedule(planet.TerrainChunkInfos[childLevel - 1].Length, 1, inputDeps);
            inputDeps.Complete();

            var childChunkList = planet.TerrainChunkInfos[childLevel];
            var parentChunkList = planet.TerrainChunkInfos[childLevel - 1];
            while (_DestroyQueue.Count > 0)
            {
                int parentChunkIndex = _DestroyQueue.Dequeue();
                Debug.Log(parentChunkIndex);
                int childChunkIndex = parentChunkList[parentChunkIndex].Child0;
                int meshIndex = childChunkList[childChunkIndex].MeshIndex;
                planet.RemoveChunk(meshIndex, childChunkIndex, childLevel);
                childChunkIndex = parentChunkList[parentChunkIndex].Child1;
                meshIndex = childChunkList[childChunkIndex].MeshIndex;
                planet.RemoveChunk(meshIndex, childChunkIndex, childLevel);
                childChunkIndex = parentChunkList[parentChunkIndex].Child2;
                meshIndex = childChunkList[childChunkIndex].MeshIndex;
                planet.RemoveChunk(meshIndex, childChunkIndex, childLevel);
                childChunkIndex = parentChunkList[parentChunkIndex].Child3;
                meshIndex = childChunkList[childChunkIndex].MeshIndex;
                planet.RemoveChunk(meshIndex, childChunkIndex, childLevel);

                var info = parentChunkList[parentChunkIndex];
                planet.GenerateMesh(ref info, childLevel - 1, parentChunkIndex);
            }
        }
    }
    public class Planet
    {
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

        public Planet(double3 position, double radius, int levelAmount)
        {
            _Radius = radius;
            _Position = position;
            _LevelAmount = levelAmount;
        }

        public void Init()
        {
            _SharedTriangles = new int[1536];
            int triIndex = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int i = x + y * 16;

                    if (x != 16 - 1 && y != 16 - 1)
                    {
                        _SharedTriangles[triIndex] = i;
                        _SharedTriangles[triIndex + 1] = i + 16 + 1;
                        _SharedTriangles[triIndex + 2] = i + 16;

                        _SharedTriangles[triIndex + 3] = i;
                        _SharedTriangles[triIndex + 4] = i + 1;
                        _SharedTriangles[triIndex + 5] = i + 16 + 1;
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
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(1, 0, 0)));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 1, 0)));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 0, 1)));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(-1, 0, 0)));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, -1, 0)));
            _TerrainChunkInfos[0].Add(new TerrainChunkInfo(0, 0, new int2(0, 0), false, false, new double3(0, 0, -1)));
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

        private void RemoveChunkInfo(int level, int chunkInfoIndex)
        {
            //Update parent's index before Remove and swap back
            int parentLevel = level - 1;
            var lastInfo = _TerrainChunkInfos[level][_TerrainChunkInfos[level].Length - 1];
            if (_Debug) Debug.Log("lastinfo's index" + (_TerrainChunkInfos[level].Length - 1) + "," + lastInfo.DetailLevel);
            var parentInfo = _TerrainChunkInfos[parentLevel][lastInfo.ParentIndex];
            if (_Debug) Debug.Log("parentinfo's index" + lastInfo.ParentIndex + "," + parentInfo.DetailLevel);
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

        public void RemoveChunk(int meshIndex, int childChunkIndex, int childLevel)
        {
            //Remove mesh
            Debug.Log(meshIndex + "and length: " + m_PlanetMeshes.Count);
            if (meshIndex >= m_PlanetMeshes.Count) return;
            var lastMesh = m_PlanetMeshes[m_PlanetMeshes.Count - 1];
            Debug.Log(lastMesh.DetailLevel + " and index: " + lastMesh.TerrainChunkInfoIndex);
            var lastMeshChunkInfo = _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex];
            lastMeshChunkInfo.MeshIndex = meshIndex;
            _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex] = lastMeshChunkInfo;
            m_PlanetMeshes.RemoveAtSwapBack(meshIndex);
            //Remove Chunk
            RemoveChunkInfo(childLevel, childChunkIndex);
        }

        public void GenerateMesh(ref TerrainChunkInfo terrainChunkInfo, int currentLevel, int chunkIndex)
        {
            terrainChunkInfo.IsMesh = true;
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[256];
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
            if (_Debug) Debug.Log("Evaluating lvl:" + currentLevel + " index:" + chunkIndex);
            var terrainChunkInfo = _TerrainChunkInfos[currentLevel][chunkIndex];
            if (terrainChunkInfo.MeshIndex == -1)
            {
                if (_Debug) Debug.Log("generating mesh");
                GenerateMesh(ref terrainChunkInfo, currentLevel, chunkIndex);
            }
            //If this chunk is not a chunk with mesh, then we evaluate its children.
            var distance = Vector3.Distance((float3)terrainChunkInfo.ChunkCenterPosition(_Position, _Radius), (float3) fromPosition);
            if (_Debug) Debug.Log("distance" + distance);
            //If the distance is too far, we return true, let parent to decide if we need to join chunks.
            if (distance > _Radius * 2 / (currentLevel + 1))
            {
                return true;
            }
            else
            {
                if (currentLevel < _LevelAmount - 1)
                {
                    if (terrainChunkInfo.IsMesh)
                    {
                        if (_Debug) UnityEngine.Debug.Log("Removing current mesh");
                        var lastMesh = m_PlanetMeshes[m_PlanetMeshes.Count - 1];
                        var lastMeshInfo = _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex];
                        lastMeshInfo.MeshIndex = terrainChunkInfo.MeshIndex;
                        _TerrainChunkInfos[lastMesh.DetailLevel][lastMesh.TerrainChunkInfoIndex] = lastMeshInfo;
                        terrainChunkInfo.IsMesh = false;
                        m_PlanetMeshes.RemoveAtSwapBack(terrainChunkInfo.MeshIndex);
                        //create four children and further evaluate.
                        double3 localUp = terrainChunkInfo.LocalUp;
                        terrainChunkInfo.Child0 = _TerrainChunkInfos[currentLevel + 1].Length;
                        var newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2, terrainChunkInfo.ChunkCoordinate.y * 2 + 1), true, true, localUp);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child1 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2 + 1, terrainChunkInfo.ChunkCoordinate.y * 2 + 1), false, true, localUp);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child2 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2, terrainChunkInfo.ChunkCoordinate.y * 2), true, false, localUp);
                        _TerrainChunkInfos[currentLevel + 1].Add(newInfo);
                        terrainChunkInfo.Child3 = _TerrainChunkInfos[currentLevel + 1].Length;
                        newInfo = new TerrainChunkInfo(chunkIndex, currentLevel + 1, new int2(terrainChunkInfo.ChunkCoordinate.x * 2 + 1, terrainChunkInfo.ChunkCoordinate.y * 2), false, false, localUp);
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

