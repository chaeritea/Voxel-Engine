using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    // Setup params
    private Voxel[,,] _voxels;
    private int _chunkDimension = 16;

    // Mesh params
    private List<Vector3> _verts = new List<Vector3>();
    private List<int> _tris = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;

    // Editor params
    private Color _gizmoColor;

    private void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshCollider = gameObject.AddComponent<MeshCollider>();

        GenerateMesh();
    }

    //private void OnDrawGizmos()
    //{
    //    if (_voxels == null)
    //    {
    //        return;
    //    }

    //    Gizmos.color = _gizmoColor;
    //    Gizmos.DrawCube(transform.position + new Vector3(_chunkSize / 2, _chunkSize / 2, _chunkSize / 2), new Vector3(_chunkSize, _chunkSize, _chunkSize));
    //}

    // Sets up the chunk with dimensions of specified size
    public void InitChunk(int size)
    {
        _chunkDimension = size;
        _voxels = new Voxel[size, size, size];
        InitVoxels();

        _gizmoColor = Random.ColorHSV();
        _gizmoColor.a = 0.4f;
    }

    public void ReloadChunk()
    {
        _verts.Clear();
        _tris.Clear();
        _uvs.Clear();

        GenerateMesh();
    }

    // Populates the chunk with voxels
    private void InitVoxels()
    {
        for (int x = 0; x < _chunkDimension; x++)
        {
            for (int y = 0; y < _chunkDimension; y++)
            {
                for (int z = 0; z < _chunkDimension; z++)
                {
                    Vector3 pos = transform.position + new Vector3(x, y, z);
                    Voxel.VoxelType type = DetermineVoxelType(pos.x, pos.y, pos.z);

                    _voxels[x, y, z] = new Voxel(new Vector3(x, y, z), type, type != Voxel.VoxelType.Air);
                }
            }
        }
    }

    private void GenerateMesh()
    {
        ProcessChunk();

        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = _tris.ToArray();
        mesh.uv = _uvs.ToArray();

        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
        _meshRenderer.material = World.instance._defaultVoxelMaterial;
    }

    // Processes each of the voxels in the chunk iteratively
    public void ProcessChunk()
    {
        for (int x = 0; x < _chunkDimension; x++)
        {
            for (int y = 0; y < _chunkDimension; y++)
            {
                for (int z = 0; z < _chunkDimension; z++)
                {
                    ProcessVoxel(x, y, z);
                }
            }
        }
    }

    // Processes a voxel and generates its visible faces
    private void ProcessVoxel(int x, int y, int z)
    {
        if (_voxels == null ||
            x < 0 || x >= _voxels.GetLength(0) ||
            y < 0 || y >= _voxels.GetLength(1) ||
            z < 0 || z >= _voxels.GetLength(2))
        {
            return;
        }

        Voxel v = _voxels[x, y, z];

        if (v._isActive)
        {
            bool[] facesVisible = new bool[6];

            facesVisible[0] = IsFaceVisible(x + 1, y, z); // x+
            facesVisible[1] = IsFaceVisible(x - 1, y, z); // x-
            facesVisible[2] = IsFaceVisible(x, y + 1, z); // y+
            facesVisible[3] = IsFaceVisible(x, y - 1, z); // y-
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // z+
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // z-

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                {
                    GenerateFace(x, y, z, i);
                }
            }
        }
    }

    // Checks adjacent voxel to see if face is covered
    private bool IsFaceVisible(int x, int y, int z)
    {
        Vector3 globalPos = transform.position + new Vector3(x, y, z);

        return IsVoxelHiddenInChunk(x, y, z) && IsVoxelHiddenInWorld(globalPos);
    }

    // Checks adgacent voxel in chunk
    private bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= _chunkDimension ||
            y < 0 || y >= _voxels.GetLength(1) ||
            z < 0 || z >= _chunkDimension)
        {
            return true;
        }
        else
            return !_voxels[x, y, z]._isActive;
    }

    // Checks adjacent voxel in world
    private bool IsVoxelHiddenInWorld(Vector3 globalPos)
    {
        Chunk neighborChunk = World.instance.GetChunkAt(globalPos);
        if (neighborChunk == null)
            return true;

        Vector3 localPos = neighborChunk.transform.InverseTransformPoint(globalPos);

        return !neighborChunk.IsVoxelActiveAt(localPos);
    }

    // Check if a voxel is active at the specified position
    public bool IsVoxelActiveAt(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > _chunkDimension ||
            y < 0 || y > _chunkDimension ||
            z < 0 || z > _chunkDimension)
        {
            return false;
        }

        return _voxels[x, y, z]._isActive;
    }

    private Voxel GetVoxelAt(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > _chunkDimension ||
            y < 0 || y > _chunkDimension ||
            z < 0 || z > _chunkDimension)
        {
            Debug.LogError("No voxel at " + pos);
            return null;
        }

        return _voxels[x, y, z];
    }

    public void CreateVoxelAt(Vector3 pos)
    {
        Voxel v = GetVoxelAt(pos);

        if (v == null)
        {
            return;
        }

        if (v._type == Voxel.VoxelType.Ground)
        {
            Debug.LogError("Voxel at " + pos + " is already ground");
            return;
        }

        v._type = Voxel.VoxelType.Ground;
        v._isActive = true;
        ReloadChunk();
    }

    public void DestroyVoxelAt(Vector3 pos)
    {
        Voxel v = GetVoxelAt(pos);

        if (v == null)
        {
            return;
        }

        if (v._type == Voxel.VoxelType.Air)
        {
            Debug.LogError("Voxel at " + pos + " is already air");
            return;
        }

        v._type = Voxel.VoxelType.Air;
        v._isActive = false;
        ReloadChunk();

        // TODO: find a more efficient way to solve this problem with chunk borders
        Vector3 globalPos = transform.position + pos;

        if (pos.x < 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(-1, 0, 0));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
        if (pos.x >= _chunkDimension - 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(1, 0, 0));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
        if (pos.y < 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(0, -1, 0));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
        if (pos.y >= _chunkDimension - 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(0, 1, 0));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
        if (pos.z < 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(0, 0, -1));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
        if (pos.z >= _chunkDimension - 1)
        {
            Chunk neighborChunk = World.instance.GetChunkAt(globalPos + new Vector3(0, 0, 1));
            if (neighborChunk != null)
                neighborChunk.ReloadChunk();
        }
    }

    public void DestroyChunk()
    {
        for (int x = 0; x < _chunkDimension; x++)
        {
            for (int y = 0; y < _chunkDimension; y++)
            {
                for (int z = 0; z < _chunkDimension; z++)
                {
                    if (_voxels[x, y, z]._type != Voxel.VoxelType.Air)
                    {
                        _voxels[x, y, z]._type = Voxel.VoxelType.Air;
                        _voxels[x, y, z]._isActive = false;
                    }
                }
            }
        }

        Debug.Log("Destroying Chunk " + gameObject.name);
        ReloadChunk();
    }

    // Clear all chunk Data
    public void DeleteChunk()
    {
        _voxels = null;

        _meshFilter.mesh = null;
        _meshCollider.sharedMesh = null;

        Destroy(gameObject);
    }

    // Generates vertices and UVs for a specified face
    private void GenerateFace(int x, int y, int z, int faceIndex)
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
    private void AddTriangleIndices()
    {
        int vertCount = _verts.Count;

        _tris.Add(vertCount - 4);
        _tris.Add(vertCount - 3);
        _tris.Add(vertCount - 2);

        _tris.Add(vertCount - 4);
        _tris.Add(vertCount - 2);
        _tris.Add(vertCount - 1);
    }

    private Voxel.VoxelType DetermineVoxelType(float x, float y, float z)
    {
        if (y > World.instance.GetHeight((int)x, (int)z))
        {
            return Voxel.VoxelType.Air;
        }
        else
        {
            return Voxel.VoxelType.Ground;
        }
    }
}
