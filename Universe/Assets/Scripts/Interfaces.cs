using System;
using Unity.Mathematics;
using UnityEngine;

namespace Universe{
    #region Astronomical object
    public interface IStarOrbit
    {
        IStarOrbitOffset _StarOrbitOffset { get; set; }
        double3 _CenterPosition { get; set; }
        double3 GetPoint(double time, bool isStar);
    }


    public interface IStarOrbitOffset
    {
        double3 Offset { get; set; }
    }

    public interface IStarClusterPattern
    {
        IStarClusterPatternOffset _StarClusterPatternOffset { get; set; }
        double3 _CenterPosition { get; set; }
        IStarOrbit GetOrbit(double proportion, ref IStarOrbitOffset offset);
        IStarOrbit GetOrbit(double3 position); 
        Color GetColor(double proportion);
    }

    public interface IStarClusterPatternOffset
    {
        double3 Offset { get; set; }
    }
    #endregion

    #region ECS
    public interface ISubSystem
    {
        void Init();
        void ShutDown();
    }
    #endregion
}