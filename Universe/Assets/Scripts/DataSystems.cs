using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Universe
{
    [Serializable]
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
        public fixed uint ResourceList[108];
    }

    [Serializable]
    public struct EnergyData : IComponentData
    {
        public long TotalEnergy;
        public int ReleaseSpeed;
    }

    public class DataSystem
    {
        #region Attribute
        private EntityManager m_EntityManager;
        private int _LastTurnCount;
        #endregion

        #region Public
        private static NativeArray<PlanetData> m_PlanetDatas;
        private static NativeArray<ResourceData> m_ResourceDatas;
        public static NativeArray<PlanetData> PlanetDatas { get => m_PlanetDatas; }
        public static NativeArray<ResourceData> ResourceDatas { get => m_ResourceDatas; set => m_ResourceDatas = value; }
        #endregion

        #region Managers

        public DataSystem()
        {  
            m_EntityManager = World.Active.EntityManager;

        }

        ~DataSystem()
        {
            if (PlanetDatas.IsCreated) PlanetDatas.Dispose();
            if (ResourceDatas.IsCreated) ResourceDatas.Dispose();
        }
        #endregion

        #region Methods


        #endregion

        #region Jobs


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

        [BurstCompile]
        public struct EnergyRelease : IJobForEach<EnergyData>
        {
            public int timeFactor;
            public void Execute(ref EnergyData c0)
            {
                c0.TotalEnergy -= c0.ReleaseSpeed * timeFactor;
            }
        }

        public JobHandle OnEnterNextTurn(ComponentSystemBase system, JobHandle inputDeps, int turnCount)
        {
            inputDeps = new EnergyRelease
            {
                timeFactor = turnCount - _LastTurnCount
            }.Schedule(system, inputDeps);

            
            return inputDeps;
        }

        public JobHandle OnExitNextTurn(ComponentSystemBase system, JobHandle inputDeps, int turnCount)
        {
            _LastTurnCount = turnCount;
            return inputDeps;
        }

        #endregion

    }
}
