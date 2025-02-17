﻿
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Universe
{
    public class TerrainMesh
    {
        private bool _Enable;
        private Mesh _Mesh;
        private int _DetailLevel;
        private int _TerrainChunkInfoIndex;
        public Mesh Mesh { get => _Mesh; set => _Mesh = value; }
        public int DetailLevel { get => _DetailLevel; set => _DetailLevel = value; }
        public int TerrainChunkInfoIndex { get => _TerrainChunkInfoIndex; set => _TerrainChunkInfoIndex = value; }
        public bool Enable { get => _Enable; set => _Enable = value; }
    }

    public struct TerrainChunkInfo
    {
        //The level of detail, the larger the detail, the smaller the chunk will be.
        private int _DetailLevel;
        //Indicate whether this is a chunk with mesh or is a chunk that is deformed.
        private bool _IsMesh;
        //We don't store the mesh in the chunk, the chunk only keep track of the mesh information. This points directly to the mesh.
        private int _MeshIndex;
        //The chunk coordinate in which a chunk belongs to the face
        private int2 _ChunkCoordinate;
        //The index of the parent, if index is -1, then this is a root.
        private int _ParentIndex;
        private bool _TooFar;
        private bool _IsUp, _IsLeft;
        //The index of four children, upperleft = 0, upperright = 1, lower left = 2, lower right = 3.
        private int _Child0, _Child1, _Child2, _Child3;

        private int _Resolution;

        private double3 _LocalUp, _AxisA, _AxisB;
        public int DetailLevel { get => _DetailLevel; set => _DetailLevel = value; }
        public bool IsMesh { get => _IsMesh; set => _IsMesh = value; }
        public int MeshIndex { get => _MeshIndex; set => _MeshIndex = value; }
        public int2 ChunkCoordinate { get => _ChunkCoordinate; set => _ChunkCoordinate = value; }
        public int ParentIndex { get => _ParentIndex; set => _ParentIndex = value; }
        public bool IsUp { get => _IsUp; set => _IsUp = value; }
        public bool IsLeft { get => _IsLeft; set => _IsLeft = value; }
        public int Child0 { get => _Child0; set => _Child0 = value; }
        public int Child1 { get => _Child1; set => _Child1 = value; }
        public int Child2 { get => _Child2; set => _Child2 = value; }
        public int Child3 { get => _Child3; set => _Child3 = value; }
        public double3 LocalUp { get => _LocalUp; set => _LocalUp = value; }
        public double3 AxisA { get => _AxisA; set => _AxisA = value; }
        public double3 AxisB { get => _AxisB; set => _AxisB = value; }
        public bool TooFar { get => _TooFar; set => _TooFar = value; }
        public int Resolution { get => _Resolution; set => _Resolution = value; }

        public double3 ChunkCenterPosition(double3 planetPosition, double radius, Quaternion rotation)
        {
            int actualDetailLevel = (int)math.pow(2, _DetailLevel);
            double2 percent = new double2(0.5, 0.5) / actualDetailLevel;
            double3 point = _LocalUp + (percent.x + (double)_ChunkCoordinate.x / actualDetailLevel - 0.5d) * 2 * _AxisA + (percent.y + (double)_ChunkCoordinate.y / actualDetailLevel - 0.5d) * 2 * _AxisB;
            double x = rotation.x * 2F;
            double y = rotation.y * 2F;
            double z = rotation.z * 2F;
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
            res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
            Normalize(ref res);
            double3 ret = res * radius + planetPosition;
            return ret;
        }

        private double3 dot(Quaternion rotation, double3 point)
        {
            double x = rotation.x * 2F;
            double y = rotation.y * 2F;
            double z = rotation.z * 2F;
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
            res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
            return res;
        }

        public TerrainChunkInfo(int parentIndex, int detailLevel, int2 chunkCoordinate, bool isLeft, bool isUp, double3 localUp, int resolution)
        {
            _IsMesh = false;
            _MeshIndex = -1;
            _ChunkCoordinate = chunkCoordinate;
            _DetailLevel = detailLevel;
            _Child0 = 0;
            _Child1 = 0;
            _Child2 = 0;
            _Child3 = 0;
            _ParentIndex = parentIndex;
            _IsLeft = isLeft;
            _IsUp = isUp;
            _LocalUp = localUp;
            _AxisA = new double3(localUp.y, localUp.z, localUp.x);
            _AxisB = (float3)Vector3.Cross((float3)localUp, (float3)_AxisA);
            _TooFar = false;
            _Resolution = resolution;
            Normalize(ref _LocalUp);
        }

        public void Normalize(ref double3 d3)
        {
            double x, y, z;
            x = d3.x;
            y = d3.y;
            z = d3.z;
            d3 /= math.sqrt(x * x + y * y + z * z);
        }
    }
}