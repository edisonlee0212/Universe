using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Universe
{
    [Serializable]
    public struct Position : IComponentData
    {
        public double3 Value;
    }

    [Serializable]
    public struct SelectionStatus : IComponentData
    {
        public byte Value;

    }

    [Serializable]
    public struct RenderContent : ISharedComponentData, IEquatable<RenderContent>
    {
        public MeshMaterial MeshMaterial;
        public bool Equals(RenderContent other)
        {
            return MeshMaterial == other.MeshMaterial;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MeshMaterial
    {
        public Mesh Mesh;
        public Material Material;
    }

    [Serializable]
    public unsafe struct RenderMeshIndex : ISharedComponentData, IEquatable<RenderMeshIndex>
    {
        public Indexer Value;

        public bool Equals(RenderMeshIndex other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public class Indexer
    {
        public int Index;
        public Indexer(int index)
        {
            Index = index;
        }
    }

    [Serializable]
    public struct StarSeed : IComponentData
    {
        /// <summary>
        /// The seed of the star, use this to calculate initial position.
        /// </summary>
        public float Value;
        
    }
    /// <summary>
    /// This keep track of the position of the star in the list.
    /// </summary>
    [Serializable]
    public struct Index : IComponentData
    {
        public int Value;
    }

    /// <summary>
    /// Original color of the star
    /// </summary>
    [Serializable]
    public struct OriginalColor : IComponentData
    {
        public Vector4 Value;
    }

    /// <summary>
    /// The deviation of its orbit
    /// </summary>
    [Serializable]
    public struct StarOrbitOffset : IComponentData, IStarOrbitOffset
    {
        public double3 Value;
        public double3 Offset { get => Value; set => Value = value; }
    }

    /// <summary>
    /// This will help calculate the orbit. Smaller = close to center, bigger = close to disk
    /// </summary>
    [Serializable]
    public struct StarOrbitProportion : IComponentData
    {
        public double Value;
    }

    /// <summary>
    /// The calculated surface color by the distance to the camera.
    /// </summary>
    [Serializable]
    public struct SurfaceColor : IComponentData
    {
        public Vector4 Value;
    }

    [Serializable]
    public struct DisplayColor : IComponentData
    {
        public Vector4 Value;
    }

    [Serializable]
    public struct StarOrbit : IComponentData, IStarOrbit
    {
        public StarOrbitOffset OrbitOffset;
        public float A;
        public float B;
        public double TiltY;
        public double TiltX;
        public double TiltZ;
        public double SpeedMultiplier;
        public double3 Center;
        public IStarOrbitOffset _StarOrbitOffset { get => OrbitOffset; set => OrbitOffset = (StarOrbitOffset)value; }
        public double3 _CenterPosition { get => Center; set => Center = value; }
        

        public double3 GetPoint(double time, bool isStar = true)
        {
            double angle = isStar ? time / Math.Sqrt(A + B) * SpeedMultiplier : time;

            double3 point = default;
            point.x = Math.Sin(angle) * A + OrbitOffset.Value.x;
            point.y = OrbitOffset.Value.y;
            point.z = Math.Cos(angle) * B + OrbitOffset.Value.z;

            point = Rotate(Quaternion.AngleAxis((float)TiltX, Vector3.right), point);
            point = Rotate(Quaternion.AngleAxis((float)TiltY, Vector3.up), point);
            point = Rotate(Quaternion.AngleAxis((float)TiltZ, Vector3.forward), point);

            point.x += _CenterPosition.x;
            point.y += _CenterPosition.y;
            point.z += _CenterPosition.z;
            return point;
        }

        private double3 Rotate(Quaternion rotation, double3 point)
        {
            double x = rotation.x * 2D;
            double y = rotation.y * 2D;
            double z = rotation.z * 2D;
            double xx = rotation.x * x;
            double yy = rotation.y * y;
            double zz = rotation.z * z;
            double xy = rotation.x * y;
            double xz = rotation.x * z;
            double yz = rotation.y * z;
            double wx = rotation.w * x;
            double wy = rotation.w * y;
            double wz = rotation.w * z;
            double3 res;
            res.x = (1D - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1D - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1D - (xx + yy)) * point.z;
            return res;
        }
    }

    [Serializable]
    public struct StarClusterIndex : IComponentData
    {
        public int Value;
    }

    [Serializable]
    public struct StarClusterPattern : ISharedComponentData, IStarClusterPattern
    {
        public StarClusterIndex Index;
        public bool Equals(StarClusterPattern other)
        {
            return Index.Value == other.Index.Value;
        }

        public override int GetHashCode()
        {
            return Index.Value;
        }

        #region public
        public double YSpread;
        public double XZSpread;

        public double DiskAB;
        public double DiskEccentricity;
        public double DiskA;
        public double DiskB;

        public double CoreProportion;
        public double CoreAB;
        public double CoreEccentricity;
        public double CoreA;
        public double CoreB;

        public double CenterAB;
        public double CenterEccentricity;
        public double CenterA;
        public double CenterB;


        public double DiskSpeed;
        public double CoreSpeed;
        public double CenterSpeed;

        public double DiskTiltX;
        public double DiskTiltZ;
        public double CoreTiltX;
        public double CoreTiltZ;
        public double CenterTiltX;
        public double CenterTiltZ;

        public Color DiskColor;
        public Color CoreColor;
        public Color CenterColor;

        public double Rotation;
        public double3 CenterPosition;

        public double3 _CenterPosition { get => CenterPosition; set => CenterPosition = value; }
        public IStarClusterPatternOffset _StarClusterPatternOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Private

        #endregion

        public void SetAB()
        {
            DiskA = DiskAB * DiskEccentricity;
            DiskB = DiskAB * (1 - DiskEccentricity);
            CenterA = CenterAB * CenterEccentricity;
            CenterB = CenterAB * (1 - CenterEccentricity);
            CoreAB = CenterAB / 2 + CenterAB / 2 +
                ((DiskA + DiskB) - CenterAB / 2 - CenterAB / 2)
                * CoreProportion;
            CoreA = CoreAB * CoreEccentricity;
            CoreB = CoreAB * (1 - CoreEccentricity);
            
            
        }

        /// <summary>
        /// Set the ellipse by the proportion.
        /// </summary>
        /// <param name="proportion">
        /// The position of the ellipse in the density waves, range is from 0 to 1
        /// </param>
        /// <param name="orbit">
        /// The ellipse will be reset by the proportion and the density wave properties.
        /// </param>
        public IStarOrbit GetOrbit(double starOrbitProportion, ref IStarOrbitOffset orbitOffset)
        {
            StarOrbit orbit = default;
            if (starOrbitProportion > CoreProportion)
            {
                //If the wave is outside the disk;
                double actualProportion = (starOrbitProportion - CoreProportion) / (1 - CoreProportion);
                orbit.A = (float)(CoreA + (DiskA - CoreA) * actualProportion);
                orbit.B = (float)(CoreB + (DiskB - CoreB) * actualProportion);
                orbit.TiltX = CoreTiltX - (CoreTiltX - DiskTiltX) * actualProportion;
                orbit.TiltZ = CoreTiltZ - (CoreTiltZ - DiskTiltZ) * actualProportion;
                orbit.SpeedMultiplier = CoreSpeed + (DiskSpeed - CoreSpeed) * actualProportion;
            }
            else
            {
                double actualProportion = starOrbitProportion / CoreProportion;
                orbit.A = (float)(CenterA + (CoreA - CenterA) * actualProportion);
                orbit.B = (float)(CenterB + (CoreB - CenterB) * actualProportion);
                orbit.TiltX = CenterTiltX - (CenterTiltX - CoreTiltX) * actualProportion;
                orbit.TiltZ = CenterTiltZ - (CenterTiltZ - CoreTiltZ) * actualProportion;
                orbit.SpeedMultiplier = CenterSpeed + (CoreSpeed - CenterSpeed) * actualProportion;
            }
            orbit.TiltY = -Rotation * starOrbitProportion;
            orbit._CenterPosition = _CenterPosition * (1 - starOrbitProportion);
            orbit._StarOrbitOffset = orbitOffset;
            return orbit;
        }

        public IStarOrbitOffset GetOrbitOffset(double proportion)
        {
            double offset;
            offset = Math.Sqrt(1 - proportion);
            IStarOrbitOffset orbitOffset = new StarOrbitOffset();
            double3 d3 = default;
            d3.y = Random.NextGaussianDouble(offset) * (DiskA + DiskB) * YSpread;
            d3.x = Random.NextGaussianDouble(offset) * (DiskA + DiskB) * XZSpread;
            d3.z = Random.NextGaussianDouble(offset) * (DiskA + DiskB) * XZSpread;
            orbitOffset.Offset = d3;
            return orbitOffset;
        }

        public Color GetColor(double proportion)
        {
            Color color = new Color { };
            if (proportion > CoreProportion)
            {
                //If the wave is outside the disk;
                double actualProportion = (proportion - CoreProportion) / (1 - CoreProportion);
                color = CoreColor * (1 - (float)actualProportion) + DiskColor * (float)actualProportion;
            }
            else
            {
                double actualProportion = proportion / CoreProportion;
                color = CoreColor * (float)actualProportion + CenterColor * (1 - (float)actualProportion);
            }
            color = Vector4.Normalize(color);
            return color;
        }

        public IStarOrbit GetOrbit(double3 position)
        {
            throw new NotImplementedException();
        }
    }
}