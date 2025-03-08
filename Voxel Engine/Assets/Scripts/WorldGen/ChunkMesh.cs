using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh : MonoBehaviour
{
    public Voxel.VoxelType _cmKey;

    public MeshFilter _meshFilter;
    public MeshRenderer _meshRenderer;
    public MeshCollider _meshCollider;

    private List<Vector3> _verts = new List<Vector3>();
    private List<int> _tris = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    private void Awake()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = _tris.ToArray();
        mesh.uv = _uvs.ToArray();

        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
        _meshRenderer.material = World.instance._materialsTEMP[_cmKey.GetHashCode()];
    }

    // Generates vertices and UVs for a specified face
    public void GenerateFace(int x, int y, int z, int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: // x+
                _verts.Add(new Vector3(x + 1, y, z + 1));
                _verts.Add(new Vector3(x + 1, y, z));
                _verts.Add(new Vector3(x + 1, y + 1, z));
                _verts.Add(new Vector3(x + 1, y + 1, z + 1));
                _uvs.Add(new Vector2(1, 0));
                _uvs.Add(new Vector2(1, 1));
                _uvs.Add(new Vector2(1, 1));
                _uvs.Add(new Vector2(1, 0));
                break;
            case 1: // x-
                _verts.Add(new Vector3(x, y, z));
                _verts.Add(new Vector3(x, y, z + 1));
                _verts.Add(new Vector3(x, y + 1, z + 1));
                _verts.Add(new Vector3(x, y + 1, z));
                _uvs.Add(new Vector2(0, 0));
                _uvs.Add(new Vector2(0, 0));
                _uvs.Add(new Vector2(0, 1));
                _uvs.Add(new Vector2(0, 1));
                break;
            case 2: // y+
                _verts.Add(new Vector3(x, y + 1, z));
                _verts.Add(new Vector3(x, y + 1, z + 1));
                _verts.Add(new Vector3(x + 1, y + 1, z + 1));
                _verts.Add(new Vector3(x + 1, y + 1, z));
                _uvs.Add(new Vector2(0, 0));
                _uvs.Add(new Vector2(1, 0));
                _uvs.Add(new Vector2(1, 1));
                _uvs.Add(new Vector2(0, 1));
                break;
            case 3: // y-
                _verts.Add(new Vector3(x, y, z));
                _verts.Add(new Vector3(x + 1, y, z));
                _verts.Add(new Vector3(x + 1, y, z + 1));
                _verts.Add(new Vector3(x, y, z + 1));
                _uvs.Add(new Vector2(0, 0));
                _uvs.Add(new Vector2(0, 1));
                _uvs.Add(new Vector2(1, 1));
                _uvs.Add(new Vector2(1, 0));
                break;
            case 4: // z+
                _verts.Add(new Vector3(x, y, z + 1));
                _verts.Add(new Vector3(x + 1, y, z + 1));
                _verts.Add(new Vector3(x + 1, y + 1, z + 1));
                _verts.Add(new Vector3(x, y + 1, z + 1));
                _uvs.Add(new Vector2(0, 1));
                _uvs.Add(new Vector2(0, 1));
                _uvs.Add(new Vector2(1, 1));
                _uvs.Add(new Vector2(1, 1));
                break;
            case 5: // z-
                _verts.Add(new Vector3(x + 1, y, z));
                _verts.Add(new Vector3(x, y, z));
                _verts.Add(new Vector3(x, y + 1, z));
                _verts.Add(new Vector3(x + 1, y + 1, z));
                _uvs.Add(new Vector2(0, 0));
                _uvs.Add(new Vector2(1, 0));
                _uvs.Add(new Vector2(1, 0));
                _uvs.Add(new Vector2(0, 0));
                break;
        }

        AddTriangleIndices();
    }

    // Adds indices of a tri's vertices to the tris list
    public void AddTriangleIndices()
    {
        int vertCount = _verts.Count;

        _tris.Add(vertCount - 4);
        _tris.Add(vertCount - 3);
        _tris.Add(vertCount - 2);

        _tris.Add(vertCount - 4);
        _tris.Add(vertCount - 2);
        _tris.Add(vertCount - 1);
    }

    public void ClearMesh()
    {
        _verts.Clear();
        _tris.Clear();
        _uvs.Clear();
    }
}