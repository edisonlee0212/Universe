
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
namespace Universe
{
    public class MeshFactory
    {
        private static List<Mesh> _Meshes;

        public Mesh GetMesh()
        {
            if (_Meshes == null) _Meshes = new List<Mesh>();
            if (_Meshes.Count > 0)
            {
                var mesh = _Meshes[0];
                _Meshes.RemoveAtSwapBack(0);
                return mesh;
            }
            else
            {
                return new Mesh();
            }
        }

        public void ReturnMesh(Mesh mesh)
        {
            _Meshes.Add(mesh);
        }
    }
}
