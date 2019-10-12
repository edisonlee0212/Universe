using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
/*
namespace Universe
{
    public unsafe struct StarData
    {
        public ushort Reference;
        public byte PlanetAmount;
        public double Proportion;
        public int FirstPlanetReference;
    }

    public struct PlanetData
    {
        public int Reference;
        public byte Index;
        public ushort StarReference;
        public float Seed;
        public float DistanceToStar;
    }


    public unsafe struct ResourceData
    {
        public int PlanetReference;
        public fixed uint ResourceList[Game.ElementSize];
    }

    public struct EnergyData
    {
        public ushort StarReference;
    }

    [DisableAutoCreation]
    public class DataSystem : JobComponentSystem
    {
        #region Attribute
        private static EntityQuery m_InstanceQuery;
        #endregion

        #region Public
        private static int m_StarAmount;
        private static NativeArray<StarData> m_StarDatas;
        private static NativeArray<PlanetData> m_PlanetDatas;
        private static NativeArray<ResourceData> m_ResourceDatas;
        public static int StarAmount { get => m_StarAmount; set => m_StarAmount = value; }
        public static NativeArray<StarData> StarDatas { get => m_StarDatas; }
        public static NativeArray<PlanetData> PlanetDatas { get => m_PlanetDatas; }
        public static NativeArray<ResourceData> ResourceDatas { get => m_ResourceDatas; set => m_ResourceDatas = value; }
        #endregion

        #region Managers

        protected override void OnCreate()
        {  
            if (m_InstanceQuery != null) m_InstanceQuery.Dispose();
            m_InstanceQuery = EntityManager.CreateEntityQuery(typeof(StarSeed), typeof(StarOrbitProportion), typeof(Index));
        }

        public void Init()
        {
            if (StarDatas.IsCreated) StarDatas.Dispose();
            if (PlanetDatas.IsCreated) PlanetDatas.Dispose();
            if (ResourceDatas.IsCreated) ResourceDatas.Dispose();
            m_StarDatas = new NativeArray<StarData>(m_StarAmount, Allocator.Persistent);

            Update();
        }

        protected override void OnDestroy()
        {
            if (StarDatas.IsCreated) StarDatas.Dispose();
            if (PlanetDatas.IsCreated) PlanetDatas.Dispose();
            if (ResourceDatas.IsCreated) ResourceDatas.Dispose();
            if (m_InstanceQuery != null) m_InstanceQuery.Dispose();
        }
        #endregion

        #region Methods


        #endregion

        #region Jobs
        public struct StarDataGenerator : IJobForEachWithEntity<StarSeed, StarOrbitProportion, Index>
        {
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<StarData> starDatas;

            public void Execute(Entity entity, int index, ref StarSeed c0, ref StarOrbitProportion c1, ref Index c2)
            {
                StarData starData = default;
                starData.Reference = (ushort)c2.Value;
                starData.Proportion = c1.Value;
                starDatas[index] = starData;
            }
        }

        public unsafe struct ResourceDataGenerator : IJobParallelFor
        {
            [ReadOnly] public NativeArray<PlanetData> planetDatas;
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<ResourceData> resourceDatas;
            public void Execute(int index)
            {
                ResourceData resourceData = default;
                resourceData.ResourceList[0] = 100;
                //TODO: Calculate resource list;
                resourceDatas[index] = resourceData;
            }
        }

        

        #endregion
        protected unsafe override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Random.seed = Game.Seed;
            var starProperties = m_InstanceQuery.ToComponentDataArray<StarProperties>(Allocator.TempJob, out inputDeps);
            m_InstanceQuery.CalculateEntityCount();
            inputDeps.Complete();
            inputDeps = new StarDataGenerator
            {
                starDatas = StarDatas,
            }.Schedule(m_StarAmount, 100, inputDeps);
            inputDeps.Complete();
            starProperties.Dispose();
            m_PlanetAmount = 0;


            for (int i = 0; i < m_StarAmount; i++)
            {
                var starData = m_StarDatas[i];
                var a = Random.Next();
                var b = Random.Next();
                starData.PlanetAmount = (byte)((a * a * 7) + (b * 3));
                m_PlanetAmount += starData.PlanetAmount;
                m_StarDatas[i] = starData;
            }

            Debug.Log("Total Star Amount: " + m_StarAmount + "\nTotal Planet Amount: " + m_PlanetAmount + ". Average planet amount: " + (float)m_PlanetAmount / m_StarAmount);

            m_PlanetDatas = new NativeArray<PlanetData>(m_PlanetAmount, Allocator.Persistent);

            int index = 0;
            for (int i = 0; i < m_StarAmount; i++)
            {
                StarData starData = m_StarDatas[i];
                int planetAmount = starData.PlanetAmount;
                for (int j = 0; j < planetAmount; j++)
                {
                    PlanetData planetData = default;
                    planetData.DistanceToStar = 6 + 2 * j;
                    planetData.Index = (byte)j;
                    planetData.StarReference = (ushort)i;
                    planetData.Reference = index;
                    planetData.Seed = Random.Next();
                    m_PlanetDatas[index] = planetData;
                    if (j == 0)
                    {
                        starData.FirstPlanetReference = index;
                        m_StarDatas[i] = starData;
                    }
                    index++;
                }
            }

            m_ResourceDatas = new NativeArray<ResourceData>(m_PlanetAmount, Allocator.Persistent);
            inputDeps = new ResourceDataGenerator
            {
                resourceDatas = m_ResourceDatas,
                planetDatas = m_PlanetDatas
            }.Schedule(m_StarAmount, 1000, inputDeps);
            inputDeps.Complete();

            return inputDeps;
        }
    }
}
*/