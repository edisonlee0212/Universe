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
    public enum ControlMode
    {
        NoControl,
        StarCluster,
        PlanetarySystem,
        Planet
    }
    public class ControlSystem
    {
        #region Private
        #endregion

        #region Public
        private static ControlMode _ControlMode;
        private static Controls _InputSystem;
        public static Controls InputSystem { get => _InputSystem; set => _InputSystem = value; }
        public static ControlMode ControlMode
        {
            get => _ControlMode;
            set
            {
                if (_InputSystem != null)
                {
                    switch (_ControlMode)
                    {
                        case ControlMode.StarCluster:
                            _InputSystem.StarCluster.Disable();
                            break;
                        case ControlMode.PlanetarySystem:
                            _InputSystem.PlanetarySystem.Disable();
                            break;
                        case ControlMode.Planet:
                            _InputSystem.Planet.Disable();
                            break;
                        default:
                            break;
                    }
                    switch (value)
                    {
                        case ControlMode.StarCluster:
                            _InputSystem.StarCluster.Enable();
                            break;
                        case ControlMode.PlanetarySystem:
                            _InputSystem.PlanetarySystem.Enable();
                            break;
                        case ControlMode.Planet:
                            _InputSystem.Planet.Enable();
                            break;
                        default:
                            break;
                    }
                }

                _ControlMode = value;
            }
        }
        #endregion

        #region Managers
        public ControlSystem()
        {
            _InputSystem = new Controls();
            _InputSystem.StarCluster.Disable();
            _InputSystem.PlanetarySystem.Disable();
            _InputSystem.Planet.Disable();
            //Star Cluster
            _InputSystem.StarCluster.Cancel.performed += ctx => CentralSystem.OnStarClusterCancel(ctx);
            _InputSystem.StarCluster.BoxSelectionStart.performed += ctx => SelectionSystem.OnBoxSelectEnter(ctx);
            _InputSystem.StarCluster.BoxSelectionRelease.performed += ctx => SelectionSystem.OnBoxSelectRelease(ctx);
            _InputSystem.StarCluster.ToPlanetarySystem.performed += ctx => CentralSystem.StarClusterModeToPlanetarySystemMode(ctx);
            //Planetary System
            _InputSystem.PlanetarySystem.Cancel.performed += ctx => CentralSystem.OnPlanetarySystemCancel(ctx);

            //Planet
        }
        #endregion
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class CentralSystem : JobComponentSystem
    {
        #region Resources
        private static RenderMeshResources m_RenderMeshResources;
        public static RenderMeshResources RenderMeshResources { get => m_RenderMeshResources; set => m_RenderMeshResources = value; }

        #endregion

        #region Subsystems
        #region Non-ECS
        private static ControlSystem m_ControlSystem;
        public static ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }
        #endregion
        #region ECS
        private static WorldSystem m_WorldSystem;
        private static StarTransformSystem m_StarTransformSystem;
        private static SelectionSystem m_SelectionSystem;
        private static StarDisplayColorSystem m_StarDisplayColorSystem;
        private static MeshRenderSystem m_RenderSystem;
        private static CopyStarColorSystem m_CopyStarColorSystem;
        public static WorldSystem WorldSystem { get => m_WorldSystem; set => m_WorldSystem = value; }
        public static StarTransformSystem StarTransformSystem { get => m_StarTransformSystem; set => m_StarTransformSystem = value; }
        public static SelectionSystem SelectionSystem { get => m_SelectionSystem; set => m_SelectionSystem = value; }
        public static MeshRenderSystem RenderSystem { get => m_RenderSystem; set => m_RenderSystem = value; }
        public static CopyStarColorSystem CopyStarColorSystem { get => m_CopyStarColorSystem; set => m_CopyStarColorSystem = value; }
        public static StarDisplayColorSystem StarDisplayColorSystem { get => m_StarDisplayColorSystem; set => m_StarDisplayColorSystem = value; }
        #endregion
        #endregion

        #region Private
        private static RenderContent _CurrentStarRenderContent;
        #endregion

        #region Public
        public struct SwitchingModeInfo
        {
            public float TotalTime, Time;
            public float FromDistanceFactor, ToDistanceFactor;
            public float FromScaleFactor, ToScaleFactor;
            public float FromTimeSpeed, ToTimeSpeed;
            public ControlMode TargetControlMode;
            public Mesh TargetMesh;
            public Material TargetMaterial;
        }
        private static SwitchingModeInfo _SwitchingModeInfo;
        private static bool _IsSwitching;

        private static float _SystemTime;
        public static float SystemTime { get => _SystemTime; set => _SystemTime = value; }
        public static SwitchingModeInfo SwitchingMode { get => _SwitchingModeInfo; set => _SwitchingModeInfo = value; }
        public static bool IsSwitching { get => _IsSwitching; set => _IsSwitching = value; }
        public static RenderContent CurrentStarRenderContent { get => _CurrentStarRenderContent; set => _CurrentStarRenderContent = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            m_RenderMeshResources = Resources.Load<RenderMeshResources>("RenderMeshResources");
            SelectionSystem.Camera = Camera.main;
            m_WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            m_StarTransformSystem = World.Active.GetOrCreateSystem<StarTransformSystem>();
            m_SelectionSystem = World.Active.GetOrCreateSystem<SelectionSystem>();
            m_RenderSystem = World.Active.GetOrCreateSystem<MeshRenderSystem>();
            m_StarDisplayColorSystem = World.Active.GetOrCreateSystem<StarDisplayColorSystem>();
            m_CopyStarColorSystem = World.Active.GetOrCreateSystem<CopyStarColorSystem>();


            m_WorldSystem.Init();
            m_StarTransformSystem.Init();
            m_SelectionSystem.Init();
            m_StarDisplayColorSystem.Init();
            m_CopyStarColorSystem.Init();
            m_RenderSystem.Init();
            m_ControlSystem = new ControlSystem();

            ControlSystem.ControlMode = ControlMode.StarCluster;

            StarClusterPattern pattern = new StarClusterPattern
            {
                Index = new StarClusterIndex { Value = 0 },
                YSpread = 0.05D,
                XZSpread = 0.015D,
                DiskAB = 2000D,
                DiskEccentricity = 0.5D,
                CoreProportion = 0.4D,
                CoreEccentricity = 0.8D,
                CenterAB = 10D,
                CenterEccentricity = 0.5D,
                DiskSpeed = 1,
                CoreSpeed = 5,
                CenterSpeed = 10,
                DiskTiltX = 0,
                DiskTiltZ = 0,
                CoreTiltX = 0,
                CoreTiltZ = 0,
                CenterTiltX = 0,
                CenterTiltZ = 0,
                DiskColor = Color.blue,
                CoreColor = Color.yellow,
                CenterColor = Color.white,
                Rotation = 360,
                CenterPosition = new double3(0)
            };
            pattern.SetAB();
            WorldSystem.AddStarClusterPattern(pattern);

            _CurrentStarRenderContent = new RenderContent { MeshMaterial = new MeshMaterial() };
            _CurrentStarRenderContent.MeshMaterial.Material = m_RenderMeshResources.Materials[0];
            _CurrentStarRenderContent.MeshMaterial.Mesh = m_RenderMeshResources.Meshes[0];

            for (int i = 0; i < 6000; i++)
            {
                var value = Random.Next();
                var proportion = new StarOrbitProportion { Value = System.Math.Sqrt(value) };
                WorldSystem.AddStar(pattern.Index, proportion, _CurrentStarRenderContent, new StarSeed { Value = Random.Next() });
            }
        }

        protected override void OnDestroy()
        {
            m_WorldSystem.ShutDown();
            m_StarTransformSystem.ShutDown();
            m_SelectionSystem.ShutDown();
            m_RenderSystem.ShutDown();
        }
        #endregion

        #region Methods
        public static void StarClusterModeToPlanetarySystemMode(InputAction.CallbackContext ctx)
        {
            if (_IsSwitching) return;
            ControlSystem.ControlMode = ControlMode.NoControl;
            //TODO: Switch to planetary system.
            _SwitchingModeInfo.TargetControlMode = ControlMode.PlanetarySystem;
            _SwitchingModeInfo.TotalTime = 2f;
            _SwitchingModeInfo.Time = 0;
            _SwitchingModeInfo.FromDistanceFactor = 0;
            _SwitchingModeInfo.ToDistanceFactor = 1;
            _SwitchingModeInfo.FromScaleFactor = StarTransformSystem.ScaleFactor;
            _SwitchingModeInfo.ToScaleFactor = 1;
            _SwitchingModeInfo.FromTimeSpeed = StarTransformSystem.TimeSpeed;
            _SwitchingModeInfo.ToTimeSpeed = 0;
            _SwitchingModeInfo.TargetMaterial = m_RenderMeshResources.Materials[1];
            _SwitchingModeInfo.TargetMesh = m_RenderMeshResources.Meshes[1];
            _IsSwitching = true;
        }

        public static void OnStarClusterCancel(InputAction.CallbackContext ctx)
        {
            SelectionSystem.OnSelectionStatusReset();
        }

        public static void OnPlanetarySystemCancel(InputAction.CallbackContext ctx)
        {
            if (_IsSwitching) return;
            StarTransformSystem.Enabled = true;
            ControlSystem.ControlMode = ControlMode.NoControl;
            //TODO: Switch to star cluster.
            _SwitchingModeInfo.TargetControlMode = ControlMode.StarCluster;
            _SwitchingModeInfo.TotalTime = 2f;
            _SwitchingModeInfo.Time = 0;
            _SwitchingModeInfo.FromDistanceFactor = 1;
            _SwitchingModeInfo.ToDistanceFactor = 0;
            _SwitchingModeInfo.FromScaleFactor = StarTransformSystem.ScaleFactor;
            _SwitchingModeInfo.ToScaleFactor = 1;
            _SwitchingModeInfo.FromTimeSpeed = 0;
            _SwitchingModeInfo.ToTimeSpeed = 1;
            _SwitchingModeInfo.TargetMaterial = m_RenderMeshResources.Materials[0];
            _SwitchingModeInfo.TargetMesh = m_RenderMeshResources.Meshes[0];
            _CurrentStarRenderContent.MeshMaterial.Mesh = _SwitchingModeInfo.TargetMesh;
            _CurrentStarRenderContent.MeshMaterial.Material = _SwitchingModeInfo.TargetMaterial;
            _IsSwitching = true;
        }

        public static void OnPlanetCancel(InputAction.CallbackContext ctx)
        {
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _SystemTime += Time.deltaTime;

            if (_IsSwitching)
            {
                StarTransformSystem.DistanceFactor = Mathf.Lerp(_SwitchingModeInfo.FromDistanceFactor, _SwitchingModeInfo.ToDistanceFactor, (_SwitchingModeInfo.Time / _SwitchingModeInfo.TotalTime));
                StarTransformSystem.TimeSpeed = Mathf.Lerp(_SwitchingModeInfo.FromTimeSpeed, _SwitchingModeInfo.ToTimeSpeed, (_SwitchingModeInfo.Time / _SwitchingModeInfo.TotalTime));
                StarTransformSystem.ScaleFactor = Mathf.Lerp(_SwitchingModeInfo.FromScaleFactor, _SwitchingModeInfo.ToScaleFactor, (_SwitchingModeInfo.Time / _SwitchingModeInfo.TotalTime));
                _SwitchingModeInfo.Time += Time.deltaTime;
                if (_SwitchingModeInfo.Time >= _SwitchingModeInfo.TotalTime)
                {
                    _IsSwitching = false;
                    StarTransformSystem.DistanceFactor = _SwitchingModeInfo.ToDistanceFactor;
                    ControlSystem.ControlMode = _SwitchingModeInfo.TargetControlMode;
                    _CurrentStarRenderContent.MeshMaterial.Mesh = _SwitchingModeInfo.TargetMesh;
                    _CurrentStarRenderContent.MeshMaterial.Material = _SwitchingModeInfo.TargetMaterial;
                }
            }

            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldSystem : JobComponentSystem, ISubSystem
    {
        #region Archetypes
        private static EntityArchetype _StarEntityArchetype;
        #endregion

        #region Private
        private static int _LastStarClusterIndex;
        private static EntityManager m_EntityManager;
        private struct StarInfo
        {
            public RenderContent RenderMesh;
            public StarClusterIndex StarClusterIndex;
            public StarOrbitProportion StarOrbitProportion;
            public StarSeed StarSeed;
        }

        private static Queue<StarInfo> _StarCreationQueue;
        private static Queue<Entity> _StarDestructionQueue;

        #endregion

        #region Public

        private static List<StarClusterPattern> _StarClusterPatterns;
        private static int _StarAmount;
        public static List<StarClusterPattern> StarClusterPatterns { get => _StarClusterPatterns; set => _StarClusterPatterns = value; }
        public static int StarAmount { get => _StarAmount; set => _StarAmount = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _LastStarClusterIndex = 0;
            m_EntityManager = World.Active.EntityManager;
            _StarClusterPatterns = new List<StarClusterPattern>();
            _StarCreationQueue = new Queue<StarInfo>();
            _StarDestructionQueue = new Queue<Entity>();
            _StarEntityArchetype = EntityManager.CreateArchetype(
                typeof(Position),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(Index),
                typeof(StarSeed),
                typeof(StarOrbitProportion),
                typeof(OriginalColor),
                typeof(SurfaceColor),
                typeof(DisplayColor),
                typeof(SelectionStatus),
                typeof(StarOrbit),
                typeof(StarClusterIndex),
                typeof(RenderContent)
                );
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

        #region Method
        public static void AddStarClusterPattern(StarClusterPattern starClusterPattern)
        {
            _StarClusterPatterns.Add(starClusterPattern);
            _LastStarClusterIndex++;
        }

        public static void AddStar(StarClusterIndex starClusterIndex, StarOrbitProportion proportion, RenderContent renderMesh, StarSeed starSeed)
        {
            _StarCreationQueue.Enqueue(new StarInfo
            {
                StarSeed = starSeed,
                RenderMesh = renderMesh,
                StarClusterIndex = starClusterIndex,
                StarOrbitProportion = proportion,
            });
        }

        public static void RemoveStar(Entity starEntity)
        {
            _StarDestructionQueue.Enqueue(starEntity);
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            if (_StarCreationQueue.Count != 0)
            {
                int count = _StarCreationQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var starInfo = _StarCreationQueue.Dequeue();
                    CreateStar(inputDeps, starInfo);
                }
            }
            else if (_StarDestructionQueue.Count != 0)
            {
                int count = _StarDestructionQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var starEntity = _StarDestructionQueue.Dequeue();
                    DestroyStar(inputDeps, starEntity);
                }
            }
            return inputDeps;
        }

        private void CreateStar(JobHandle inputDeps, StarInfo starInfo)
        {
            Entity instance = EntityManager.CreateEntity(_StarEntityArchetype);
            var orbitOffset = _StarClusterPatterns[starInfo.StarClusterIndex.Value].GetOrbitOffset(starInfo.StarOrbitProportion.Value);
            //Debug.Log("Proportion: " + starInfo.StarOrbitProportion.Value);
            EntityManager.SetSharedComponentData(instance, starInfo.RenderMesh);
            EntityManager.SetComponentData(instance, starInfo.StarClusterIndex);
            StarOrbit orbit = (StarOrbit)_StarClusterPatterns[starInfo.StarClusterIndex.Value].GetOrbit(starInfo.StarOrbitProportion.Value, ref orbitOffset);
            EntityManager.SetComponentData(instance, orbit);
            //Debug.Log(orbit.A + " + " + orbit.B);
            EntityManager.SetComponentData(instance, starInfo.StarOrbitProportion);
            EntityManager.SetComponentData(instance, starInfo.StarSeed);
            EntityManager.SetComponentData(instance, new DisplayColor { Value = Color.red });
            OriginalColor originalColor = default;
            originalColor.Value = new Color(Random.Next(), Random.Next(), Random.Next(), 1);
            EntityManager.SetComponentData(instance, originalColor);
            _StarAmount++;
        }

        private void DestroyStar(JobHandle inputDeps, Entity starEntity)
        {
            EntityManager.DestroyEntity(starEntity);
            _StarAmount--;
        }
    }

    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class StarTransformSystem : JobComponentSystem, ISubSystem
    {
        #region Private


        #endregion

        #region Public
        private static float _TimeSpeed;
        private static float _SimulatedTime;
        private float _ScaleFactor;
        private float _DistanceFactor;
        private float _floatingOrigin;
        public static float SimulatedTime { get => _SimulatedTime; set => _SimulatedTime = value; }
        public float ScaleFactor { get => _ScaleFactor; set => _ScaleFactor = value; }
        public float DistanceFactor { get => _DistanceFactor; set => _DistanceFactor = value; }
        public static float TimeSpeed { get => _TimeSpeed; set => _TimeSpeed = value; }
        public float FloatingOrigin { get => _floatingOrigin; set => _floatingOrigin = value; }
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
            _TimeSpeed = 1;
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
                    c3.Value = Mathf.Lerp(scaleFactor, 100f / Vector3.Distance((float3)position, Vector3.zero), distanceFactor);
                }
            }
        }

        [BurstCompile]
        protected struct CalculateStarTranslationAndColorStarClusterMode : IJobForEach<Translation, Position, OriginalColor, DisplayColor, Rotation>
        {
            [ReadOnly] public double3 floatingOrigin;
            [ReadOnly] public float distanceFactor;
            public void Execute([WriteOnly] ref Translation c0, [ReadOnly] ref Position c1, [ReadOnly] ref OriginalColor c2, [WriteOnly] ref DisplayColor c3, [WriteOnly] ref Rotation c4)
            {
                var position = c1.Value;
                position -= floatingOrigin;
                if (distanceFactor == 0) c0.Value = (float3)position;
                else
                {
                    float fromDistance = Vector3.Distance(Vector3.zero, (float3)position);
                    c0.Value = Vector3.Normalize((float3)position) * Mathf.Lerp(fromDistance, 500, distanceFactor);
                }
                
                c3.Value = c2.Value;
            }
        }

        [BurstCompile]
        protected struct CalculateStarTranslationAndColorPlanetarySystemMode : IJobForEach<Translation, Position, OriginalColor, DisplayColor, Rotation, Scale>
        {
            [ReadOnly] public double3 floatingOrigin;
            public void Execute([WriteOnly] ref Translation c0, [ReadOnly] ref Position c1, [ReadOnly] ref OriginalColor c2, [WriteOnly] ref DisplayColor c3, [WriteOnly] ref Rotation c4, [WriteOnly] ref Scale c5)
            {
                var position = c1.Value;
                position -= floatingOrigin;
                c0.Value = (float3)Vector3.Normalize((float3)position) * 10000;
                c3.Value = c2.Value;
                c4.Value = Quaternion.FromToRotation(Vector3.forward, (float3)position);
                c5.Value = 2000f / Vector3.Distance((float3)position, Vector3.zero);
            }
        }

        [BurstCompile]
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
            foreach (var pattern in WorldSystem.StarClusterPatterns)
            {
                inputDeps = new CalculateStarPositionAndScale
                {
                    time = _SimulatedTime,
                    scaleFactor = _ScaleFactor,
                    patternIndex = pattern.Index.Value,
                    distanceFactor = _DistanceFactor,
                    floatingOrigin = _floatingOrigin
                }.Schedule(this, inputDeps);
                inputDeps.Complete();
                if (ControlSystem.ControlMode == ControlMode.PlanetarySystem) {
                    inputDeps = new CalculateStarTranslationAndColorPlanetarySystemMode
                    {
                        floatingOrigin = _floatingOrigin
                    }.Schedule(this, inputDeps);
                    Enabled = false;
                }
                else
                {
                    inputDeps = new CalculateStarTranslationAndColorStarClusterMode
                    {
                        distanceFactor = _DistanceFactor,
                        floatingOrigin = _floatingOrigin
                    }.Schedule(this, inputDeps);
                }
            }
            inputDeps.Complete();
            return inputDeps;
        }
    }


    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class SelectionSystem : JobComponentSystem, ISubSystem
    {
        #region Private
        private static NativeQueue<Entity> _RayCastResultEntities;
        private static NativeQueue<float> _Distances;
        private static Vector3 _SavedMousePosition;

        #endregion

        #region Public
        private static Camera m_Camera;
        private static float _MaxRayCastDistance;
        private static Entity m_RayCastSelectedEntity;
        private static bool _EnableRaySelection, _EnableBoxSelection, _Reset;
        public static Camera Camera { get => m_Camera; set => m_Camera = value; }
        public static float MaxRayCastDistance { get => _MaxRayCastDistance; set => _MaxRayCastDistance = value; }
        public static Entity RayCastSelectedEntity { get => m_RayCastSelectedEntity; set => m_RayCastSelectedEntity = value; }
        public static bool EnableRaySelection { get => _EnableRaySelection; }
        public static bool EnableBoxSelection { get => _EnableBoxSelection; }
        public static bool Reset { get => _Reset; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            m_Camera = Camera.main;
        }
        public void Init()
        {
            ShutDown();
            _RayCastResultEntities = new NativeQueue<Entity>(Allocator.Persistent);
            _Distances = new NativeQueue<float>(Allocator.Persistent);

            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_RayCastResultEntities.IsCreated) _RayCastResultEntities.Dispose();
            if (_Distances.IsCreated) _Distances.Dispose();
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods
        public static void OnBoxSelectEnter(InputAction.CallbackContext ctx)
        {
            _EnableRaySelection = false;
            _SavedMousePosition = Input.mousePosition;
            _EnableBoxSelection = true;
        }

        public static void OnBoxSelectRelease(InputAction.CallbackContext ctx)
        {
            _EnableBoxSelection = false;
        }

        public static void OnSelectionStatusReset()
        {
            _Reset = true;
        }
        #endregion

        #region Jobs
        [BurstCompile]
        struct RaySelectionJob : IJobForEachWithEntity<Scale, LocalToWorld>
        {
            [ReadOnly] public Vector3 start;
            [ReadOnly] public Vector3 end;
            [ReadOnly] public float rayCastDistance;
            [WriteOnly] public NativeQueue<Entity>.ParallelWriter rayCastResultEntities;
            [WriteOnly] public NativeQueue<float>.ParallelWriter rayCastDistances;

            public void Execute([ReadOnly] Entity entity, [ReadOnly] int index, [ReadOnly] ref Scale c1, [ReadOnly] ref LocalToWorld c3)
            {
                float d;
                float scale = c1.Value;
                float3 position = c3.Position;
                if (Vector3.Distance(position, start) <= rayCastDistance && Vector3.Angle(end - start, position - (float3)start) < 80)
                {
                    d = Vector3.Dot((position - (float3)start), (end - start)) / rayCastDistance;
                    float ap = Vector3.Distance(position, (float3)start);
                    if ((ap + d) * (ap - d) < scale * scale)
                    {
                        rayCastResultEntities.Enqueue(entity);
                        rayCastDistances.Enqueue(ap);
                    }
                }
                d = Vector3.Distance(Vector3.zero, position);
            }
        }

        [BurstCompile]
        public struct BoxSelectionJob : IJobForEach<LocalToWorld, SelectionStatus>
        {
            [ReadOnly] public float4 p0, p1, p2, p3, p4;
            [ReadOnly] public int mode;
            public void Execute([ReadOnly] ref LocalToWorld c0, [WriteOnly] ref SelectionStatus c1)
            {

                float3 pos = c0.Position;
                float radius = c0.Value.c0.x;
                bool selected = true;
                if (p0.x * pos.x + p0.y * pos.y + p0.z * pos.z + p0.w <= radius) selected = false;
                if (p1.x * pos.x + p1.y * pos.y + p1.z * pos.z + p1.w <= radius) selected = false;
                if (p2.x * pos.x + p2.y * pos.y + p2.z * pos.z + p2.w <= radius) selected = false;
                if (p3.x * pos.x + p3.y * pos.y + p3.z * pos.z + p3.w <= radius) selected = false;
                if (p4.x * pos.x + p4.y * pos.y + p4.z * pos.z + p4.w <= radius) selected = false;
                if (selected) c1.Value = 2;
                else c1.Value = 0;
            }
        }

        [BurstCompile]
        public struct ResetSelectionStatus : IJobForEach<SelectionStatus>
        {
            public void Execute([WriteOnly] ref SelectionStatus c0)
            {
                c0.Value = 0;
            }
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_EnableRaySelection)
            {
                UnityEngine.Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
                //If we are in galaxy view, then scan for stars. Else scan for planets.
                inputDeps = new RaySelectionJob
                {
                    start = ray.origin,
                    end = ray.origin + ray.direction * _MaxRayCastDistance,
                    rayCastResultEntities = _RayCastResultEntities.AsParallelWriter(),
                    rayCastDistances = _Distances.AsParallelWriter(),
                }.Schedule(this, inputDeps);
                inputDeps.Complete();
                float min = MaxRayCastDistance;
                while (_Distances.Count > 0)
                {
                    Entity e = _RayCastResultEntities.Dequeue();
                    float f = _Distances.Dequeue();
                    if (f < min)
                    {
                        min = f;
                        m_RayCastSelectedEntity = e;
                        EntityManager.SetComponentData(e, new SelectionStatus { Value = 1 });
                    }
                }
            }
            else if (_EnableBoxSelection)
            {
                float4[] clippingPlanes = new float4[5];
                CalculateClippingPlanes(ref clippingPlanes, -1);
                inputDeps = new BoxSelectionJob
                {
                    p0 = clippingPlanes[0],
                    p1 = clippingPlanes[1],
                    p2 = clippingPlanes[2],
                    p3 = clippingPlanes[3],
                    p4 = clippingPlanes[4]
                }.Schedule(this, inputDeps);
                inputDeps.Complete();
            }
            if (_Reset)
            {
                _Reset = false;
                inputDeps = new ResetSelectionStatus { }.Schedule(this, inputDeps);
                inputDeps.Complete();
            }
            return inputDeps;
        }

        private static void CalculateClippingPlanes(ref float4[] clippingPlanes, float farDistance = -1)
        {
            m_Camera = Camera.main;

            if (farDistance == -1) farDistance = m_Camera.farClipPlane;
            UnityEngine.Plane plane = default;
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3[] points = new Vector3[4];
            if ((currentMousePosition.x - _SavedMousePosition.x) * (currentMousePosition.y - _SavedMousePosition.y) < 0)
            {
                points[0] = m_Camera.ScreenToWorldPoint(new Vector3(_SavedMousePosition.x, _SavedMousePosition.y, farDistance));
                points[1] = m_Camera.ScreenToWorldPoint(new Vector3(_SavedMousePosition.x, currentMousePosition.y, farDistance));
                points[2] = m_Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, farDistance));
                points[3] = m_Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, _SavedMousePosition.y, farDistance));
            }
            else
            {
                points[3] = m_Camera.ScreenToWorldPoint(new Vector3(_SavedMousePosition.x, _SavedMousePosition.y, farDistance));
                points[2] = m_Camera.ScreenToWorldPoint(new Vector3(_SavedMousePosition.x, currentMousePosition.y, farDistance));
                points[1] = m_Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, farDistance));
                points[0] = m_Camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, _SavedMousePosition.y, farDistance));
            }
            plane.Set3Points(m_Camera.transform.position, points[0], points[1]);
            clippingPlanes[0] = ToFloat4(ref plane);
            plane.Set3Points(m_Camera.transform.position, points[1], points[2]);
            clippingPlanes[1] = ToFloat4(ref plane);
            plane.Set3Points(m_Camera.transform.position, points[2], points[3]);
            clippingPlanes[2] = ToFloat4(ref plane);
            plane.Set3Points(m_Camera.transform.position, points[3], points[0]);
            clippingPlanes[3] = ToFloat4(ref plane);
            plane.Set3Points(points[2], points[1], points[0]);
            clippingPlanes[4] = ToFloat4(ref plane);
        }

        private static float4 ToFloat4(ref UnityEngine.Plane plane)
        {
            float4 ret = default;
            ret.x = plane.normal.x;
            ret.y = plane.normal.y;
            ret.z = plane.normal.z;
            ret.w = plane.distance;
            return ret;
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
    [UpdateBefore(typeof(MeshRenderSystem))]
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
    [UpdateBefore(typeof(MeshRenderSystem))]
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

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class MeshRenderSystem : JobComponentSystem, ISubSystem
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
}
