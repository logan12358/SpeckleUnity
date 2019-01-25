using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    public static class SpeckleMeshConverter
    {
        public static GameObject ToNative(this SpeckleCore.SpeckleMesh speckleMesh)
        {
            GameObject go = SpecklePrefabs.InstantiateMesh();
            
            go.GetComponent<UnitySpeckleObjectData>().Id = speckleMesh._id;
            go.GetComponent<UnitySpeckleObjectData>().SpeckleObject = speckleMesh;
        
            Mesh unityMesh = go.GetComponent<MeshFilter>().mesh;

            unityMesh.vertices = Util.SpecklePointsToUnityPoints(speckleMesh.Vertices);
            unityMesh.triangles = Util.SpeckleFacesToUnityTriangles(speckleMesh.Faces);

            unityMesh.RecalculateNormals();
            unityMesh.RecalculateTangents();

            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = unityMesh;

            return go;
        }
    }
}
