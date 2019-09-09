using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Universe
{
    [CreateAssetMenu]
    public class RenderMeshResources : ScriptableObject
    {
        public Material[] Materials;
        public Mesh[] Meshes;
    }
}
