using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Universe
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class StarTransformSystem : JobComponentSystem, ISubSystem
    {
        #region Private
        private static float _FollowEntityTimer, _FollowEntityTotalTime;
        private static double3 _OriginalFloatingOrigin;

        #endregion

        #region Public
        private static Entity m_FollowingEntity;
        private static float _TimeSpeed;
        private static float _SimulatedTime;
        private static float _ScaleFactor;
        private static float _DistanceFactor;
        private static bool _StartFollow;
        private static double3 _FloatingOrigin;
        public static float SimulatedTime { get => _SimulatedTime; set => _SimulatedTime = value; }
        public static float ScaleFactor { get => _ScaleFactor; set => _ScaleFactor = value; }
        public static float DistanceFactor { get => _DistanceFactor; set => _DistanceFactor = value; }
        public static float TimeSpeed { get => _TimeSpeed; set => _TimeSpeed = value; }
        public static double3 FloatingOrigin { get => _FloatingOrigin; set => _FloatingOrigin = value; }
        public static Entity FollowingEntity { get => m_FollowingEntity; set => m_FollowingEntity = value; }
        public static bool StartFollow { get => _StartFollow; set => _StartFollow = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
        }
        public void Init()
        {
            ShutDown();
            _ScaleFactor = 1;
            _DistanceFactor = 0;
            _TimeSpeed = 0.1f;
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
        public static void FollowEntity(Entity entity)
        {
            _StartFollow = true;
            ControlSystem.Interrupt();
            _OriginalFloatingOrigin = _FloatingOrigin;
            _FollowEntityTimer = 0;
            _FollowEntityTotalTime = 1;
            m_FollowingEntity = entity;
        }

        public static void UnFollow()
        {
            m_FollowingEntity = Entity.Null;
        }

        public static JobHandle OnTransition(ComponentSystemBase system, JobHandle inputDeps)
        {
            inputDeps = new CalculateStarTranslationAndColorPlanetarySystemMode
            {
                followingEntity = m_FollowingEntity,
                floatingOrigin = _FloatingOrigin,
            }.Schedule(system, inputDeps);

            inputDeps.Complete();
            return inputDeps;
        }

        #endregion

        #region Jobs
        [BurstCompile]
        protected struct CalculateStarPositionAndScale : IJobForEach<StarSeed, Position, StarOrbit, Scale, StarClusterIndex, StarOrbitProportion>
        {
            [ReadOnly] public float time;
            [ReadOnly] public float scaleFactor;
            [ReadOnly] public float distanceFactor;
            [ReadOnly] public double3 floatingOrigin;
            [ReadOnly] public int patternIndex;
            public void Execute([ReadOnly] ref StarSeed c0, [WriteOnly] ref Position c1, [ReadOnly] ref StarOrbit c2, [WriteOnly] ref Scale c3, [ReadOnly] ref StarClusterIndex c4, [ReadOnly] ref StarOrbitProportion c5)
            {
                if (patternIndex != c4.Value) return;
                var position = c2.GetPoint(c0.Value * 360 + time);
                c1.Value = position;
                if (distanceFactor == 0) c3.Value = scaleFactor;
                else
                {
                    c3.Value = Mathf.Lerp(scaleFactor, 90000f / Vector3.Distance((float3)(position - floatingOrigin), Vector3.zero), distanceFactor);
                }
            }
        }

        [BurstCompile]
        protected struct CalculateStarTranslationAndColorStarClusterMode : IJobForEachWithEntity<Translation, Position, OriginalColor, DisplayColor, Rotation>
        {
            [ReadOnly] public double3 floatingOrigin;
            [ReadOnly] public float distanceFactor;
            [ReadOnly] public Entity followingEntity;
            public void Execute(Entity entity, int index, [WriteOnly] ref Translation c0, [ReadOnly] ref Position c1, [ReadOnly] ref OriginalColor c2, [WriteOnly] ref DisplayColor c3, [WriteOnly] ref Rotation c4)
            {
                var position = c1.Value;
                position -= floatingOrigin;
                if (distanceFactor == 0) c0.Value = (float3)position;
                else if (!followingEntity.Equals(entity))
                {
                    float fromDistance = Vector3.Distance(Vector3.zero, (float3)position);
                    c0.Value = Vector3.Normalize((float3)position) * Mathf.Lerp(fromDistance, 90000, distanceFactor);

                }
                else
                {
                    c0.Value = new float3(10000000, 0, 0);
                    return;
                }
                var color = c2.Value;
                c3.Value = color * 3;
            }
        }

        [BurstCompile]
        protected struct CalculateStarTranslationAndColorPlanetarySystemMode : IJobForEachWithEntity<Translation, Position, OriginalColor, DisplayColor, Rotation, Scale>
        {
            [ReadOnly] public double3 floatingOrigin;
            [ReadOnly] public Entity followingEntity;
            public void Execute(Entity entity, int index, [WriteOnly] ref Translation c0, [ReadOnly] ref Position c1, [ReadOnly] ref OriginalColor c2, [WriteOnly] ref DisplayColor c3, [WriteOnly] ref Rotation c4, [WriteOnly] ref Scale c5)
            {
                var position = c1.Value;
                position -= floatingOrigin;
                if (followingEntity.Equals(entity))
                {
                    c0.Value = new float3(10000000, 0, 0);
                    c5.Value = 0;
                    return;
                }
                else
                {
                    c0.Value = (float3)Vector3.Normalize((float3)position) * 90000;
                }
                var color = c2.Value;
                c3.Value = color * 3;
                c4.Value = Quaternion.FromToRotation(Vector3.forward, (float3)position);
                c5.Value = 90000f / Vector3.Distance((float3)position, Vector3.zero);
            }
        }

        protected struct RefreashStarOrbit : IJobForEach<StarClusterIndex, StarOrbit, StarOrbitOffset, StarOrbitProportion>
        {
            [ReadOnly] public StarClusterPattern pattern;
            public void Execute([ReadOnly]ref StarClusterIndex c0, [WriteOnly] ref StarOrbit c1, [ReadOnly] ref StarOrbitOffset c2, [ReadOnly] ref StarOrbitProportion c3)
            {
                if (pattern.Index.Value == c0.Value)
                {
                    IStarOrbitOffset offset = c2;
                    c1 = (StarOrbit)pattern.GetOrbit(c3.Value, ref offset);
                }
            }
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _SimulatedTime += Time.deltaTime * _TimeSpeed;
            if (_StartFollow)
            {
                _FollowEntityTimer += Time.deltaTime;
                double3 target = EntityManager.GetComponentData<Position>(m_FollowingEntity).Value;
                float proportion = _FollowEntityTimer / _FollowEntityTotalTime;
                _FloatingOrigin = target * proportion + _OriginalFloatingOrigin * (1 - proportion);
                if (_FollowEntityTimer > _FollowEntityTotalTime)
                {
                    _StartFollow = false;
                    ControlSystem.Resume();
                }
            }
            foreach (var pattern in WorldSystem.StarClusterPatterns)
            {
                inputDeps = new CalculateStarPositionAndScale
                {
                    time = _SimulatedTime,
                    scaleFactor = _ScaleFactor,
                    patternIndex = pattern.Index.Value,
                    distanceFactor = _DistanceFactor,
                    floatingOrigin = _FloatingOrigin
                }.Schedule(this, inputDeps);
                inputDeps.Complete();
                if (m_FollowingEntity != Entity.Null && !_StartFollow) _FloatingOrigin = EntityManager.GetComponentData<Position>(m_FollowingEntity).Value;


                inputDeps = new CalculateStarTranslationAndColorStarClusterMode
                {
                    distanceFactor = _DistanceFactor,
                    floatingOrigin = _FloatingOrigin,
                    followingEntity = m_FollowingEntity
                }.Schedule(this, inputDeps);

            }
            #region Floating Origin
            if (m_FollowingEntity == Entity.Null)
            {
                var xz = ControlSystem.InputSystem.StarCluster.MoveCamera.ReadValue<Vector2>();
                Vector3 forward = CameraModule.MainCameraTransform.forward;
                Vector3 right = CameraModule.MainCameraTransform.right;
                var y = ControlSystem.InputSystem.StarCluster.AltCamera.ReadValue<float>();
                var delta = forward * xz.y + right * xz.x;
                _FloatingOrigin += new double3(delta.x, delta.y, delta.z) * _ScaleFactor;
                _FloatingOrigin.y += y * _ScaleFactor;
            }
            #endregion

            #region Zoom
            _ScaleFactor += ControlSystem.InputSystem.StarCluster.Zoom.ReadValue<float>() / 50;
            _ScaleFactor = Mathf.Clamp(_ScaleFactor, 1f, 5);
            #endregion

            inputDeps.Complete();
            return inputDeps;
        }
    }

    [UpdateAfter(typeof(SelectionSystem))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class StarDisplayColorSystem : JobComponentSystem, ISubSystem
    {
        #region Private
        #endregion

        #region Public
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
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
        #endregion

        #region Jobs
        [BurstCompile]
        protected struct CalculateDisplayColor : IJobForEach<SelectionStatus, DisplayColor, SurfaceColor>
        {
            public void Execute([ReadOnly]ref SelectionStatus c0, [WriteOnly] ref DisplayColor c1, [ReadOnly] ref SurfaceColor c2)
            {
                if (c0.Value == 2) c1.Value = Color.red;
            }
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = new CalculateDisplayColor { }.Schedule(this, inputDeps);
            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(StarRenderSystem))]
    public class CopyStarColorSystem : JobComponentSystem, ISubSystem
    {
        #region Private
        private static EntityQuery _StarEntityQuery;
        private static NativeArray<DisplayColor> _DisplayColors;
        private static ComputeBuffer[] _DisplayColorBuffers;
        private List<RenderContent> _RenderMeshes;

        #endregion

        #region Public

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _RenderMeshes = new List<RenderContent>();
            _StarEntityQuery = EntityManager.CreateEntityQuery(typeof(DisplayColor), typeof(RenderContent));
            _DisplayColorBuffers = new ComputeBuffer[32];

        }

        public void Init()
        {
            ShutDown();
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_DisplayColorBuffers != null)
                foreach (var i in _DisplayColorBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_DisplayColors.IsCreated) _DisplayColors.Dispose();
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager.GetAllUniqueSharedComponentData(_RenderMeshes);
            int count = _RenderMeshes.Count;
            for (int i = 1; i < count; i++)
            {
                _StarEntityQuery.SetFilter(_RenderMeshes[i]);
                _DisplayColors = _StarEntityQuery.ToComponentDataArray<DisplayColor>(Allocator.TempJob);
                if (_DisplayColors.Length != 0)
                {
                    if (_DisplayColorBuffers[i - 1] != null) _DisplayColorBuffers[i - 1].Release();
                    _DisplayColorBuffers[i - 1] = new ComputeBuffer(_DisplayColors.Length, 16);
                    _DisplayColorBuffers[i - 1].SetData(_DisplayColors);
                    if (_RenderMeshes[i].MeshMaterial.Material != null) _RenderMeshes[i].MeshMaterial.Material.SetBuffer("_DisplayColorBuffer", _DisplayColorBuffers[i - 1]);
                }
                _DisplayColors.Dispose();
            }
            _RenderMeshes.Clear();

            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class StarRenderSystem : JobComponentSystem, ISubSystem
    {
        #region Private
        private EntityQuery _MeshRenderEntityQuery;
        private NativeArray<LocalToWorld> _LocalToWorlds;
        private List<RenderContent> _RenderMeshes;
        private ComputeBuffer[] _ArgsBuffers, _LocalToWorldBuffers;
        private uint[] args;
        #endregion

        #region Public
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _ArgsBuffers = new ComputeBuffer[32];
            _LocalToWorldBuffers = new ComputeBuffer[32];
            _RenderMeshes = new List<RenderContent>();
            _MeshRenderEntityQuery = EntityManager.CreateEntityQuery(typeof(RenderContent), typeof(LocalToWorld));
        }
        public void Init()
        {
            ShutDown();



            args = new uint[5] { 0, 0, 0, 0, 0 };
            Enabled = true;
        }

        public void ShutDown()
        {
            if (_LocalToWorlds.IsCreated) _LocalToWorlds.Dispose();
            if (_LocalToWorldBuffers != null)
                foreach (var i in _LocalToWorldBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_ArgsBuffers != null)
                foreach (var i in _ArgsBuffers)
                {
                    if (i != null) i.Release();
                }
            Enabled = false;
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager.GetAllUniqueSharedComponentData(_RenderMeshes);
            int count = _RenderMeshes.Count;
            for (int i = 1; i < count; i++)
            {
                _MeshRenderEntityQuery.SetFilter(_RenderMeshes[i]);
                _LocalToWorlds = _MeshRenderEntityQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
                int amount = _LocalToWorlds.Length;
                if (amount != 0)
                {
                    if (_ArgsBuffers[i - 1] != null) _ArgsBuffers[i - 1].Release();
                    if (_LocalToWorldBuffers[i - 1] != null) _LocalToWorldBuffers[i - 1].Release();
                    args[0] = _RenderMeshes[i].MeshMaterial.Mesh.GetIndexCount(0);
                    args[1] = (uint)amount;
                    _ArgsBuffers[i - 1] = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                    _ArgsBuffers[i - 1].SetData(args);

                    _LocalToWorldBuffers[i - 1] = new ComputeBuffer(amount, 64);
                    _LocalToWorldBuffers[i - 1].SetData(_LocalToWorlds);
                    _RenderMeshes[i].MeshMaterial.Material.SetBuffer("_LocalToWorldBuffer", _LocalToWorldBuffers[i - 1]);

                    Graphics.DrawMeshInstancedIndirect(_RenderMeshes[i].MeshMaterial.Mesh, 0, _RenderMeshes[i].MeshMaterial.Material, new Bounds(Vector3.zero, Vector3.one * 60000), _ArgsBuffers[i - 1], 0, null, 0, false, 0);
                }
                _LocalToWorlds.Dispose();
            }
            _RenderMeshes.Clear();
            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(StarRenderSystem))]
    public class BeaconRenderSystem : JobComponentSystem, ISubSystem
    {
        #region Attributes
        private static MaterialPropertyBlock m_MaterialPropertyBlock;
        #endregion

        #region Public
        private static Matrix4x4[] m_Matrices;
        private static Vector4[] m_Colors;
        private static UnityEngine.Mesh m_BeaconMesh;
        private static UnityEngine.Material m_BeaconMaterial;
        private static int m_BeaconAmount;
        private static bool m_DrawBeacon;
        public static int BeaconAmount { get => m_BeaconAmount; set => m_BeaconAmount = value; }
        public static UnityEngine.Mesh BeaconMesh { get => m_BeaconMesh; set => m_BeaconMesh = value; }
        public static UnityEngine.Material BeaconMaterial { get => m_BeaconMaterial; set => m_BeaconMaterial = value; }
        public static Matrix4x4[] Matrices { get => m_Matrices; set => m_Matrices = value; }
        public static Vector4[] Colors { get => m_Colors; set => m_Colors = value; }
        public static bool DrawBeacon { get => m_DrawBeacon; set => m_DrawBeacon = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Matrices = new Matrix4x4[1023];
            Colors = new Vector4[1023];
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
            Enabled = false;
        }

        public void Init()
        {
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
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_DrawBeacon && m_BeaconAmount != 0 && m_BeaconAmount < 1024)
            {
                m_MaterialPropertyBlock.SetVectorArray("_EmissionColor", Colors);
                Graphics.DrawMeshInstanced(m_BeaconMesh, 0, m_BeaconMaterial,
                    Matrices,
                    BeaconAmount, m_MaterialPropertyBlock, 0, false, 0, null);
            }
            if (m_BeaconAmount > 1023)
            {
                Debug.Log("Too many beacons! [" + m_BeaconAmount + "]");
            }
            return inputDeps;
        }
    }



}