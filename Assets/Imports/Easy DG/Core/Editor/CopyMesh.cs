using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CopyMesh : MonoBehaviour {

    [MenuItem("Assets/CopyMesh")]
    static void Copy()
    {
        Mesh mesh = Selection.activeObject as Mesh;
        Mesh newmesh = new Mesh();
        newmesh.vertices = mesh.vertices;
        newmesh.triangles = mesh.triangles;
        newmesh.uv = mesh.uv;
        newmesh.normals = mesh.normals;
        newmesh.colors = mesh.colors;
        newmesh.tangents = mesh.tangents;
        AssetDatabase.CreateAsset(newmesh, AssetDatabase.GetAssetPath(mesh) + " copy.asset");
    }
}
