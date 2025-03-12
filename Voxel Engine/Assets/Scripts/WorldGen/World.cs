using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;

    // TODO: find a better method for this
    [Header("Material Dictionary")]
    public List<Material> _materialsTEMP;

    [Header("WorldGen Settings")]
    [SerializeField] private int _seed = 0;
    [SerializeField] private int _worldSizeX = 5;
    [SerializeField] private int _worldSizeY = 5;
    [SerializeField] private int _worldSizeZ = 5;
    public int _chunkSize = 16;
    [SerializeField] private int _noiseScale = 10;
    [SerializeField] private int _heightScale = 32;

    private WorldNoise _noise;
    private Dictionary<Vector3Int, Chunk> _chunks;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NewWorld();

        WorldSettingsManager.instance.CloseMenu();
    }

    public void NewWorld()
    {
        // Delete old world
        if (_chunks != null)
            DeleteAllChunks();

        // Setup world gen
        _chunks = new Dictionary<Vector3Int, Chunk>();
        _noise = new WorldNoise(_noiseScale, _heightScale);
        _noise.GenerateNoise_Random();
        //_noise.GenerateNoise_Seeded(_seed);
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        for (int x = 0; x < _worldSizeX; x++)
        {
            for (int y = 0; y < _worldSizeY; y++)
            {
                for (int z = 0; z < _worldSizeZ; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x * _chunkSize, y * _chunkSize, z * _chunkSize);
                    GameObject chunkObj = new GameObject($"Chunk ({x}, {y}, {z})");
                    chunkObj.transform.position = chunkPos;
                    chunkObj.transform.parent = transform;

                    Chunk chunk = chunkObj.AddComponent<Chunk>();
                    chunk.InitChunk(_chunkSize);
                    _chunks.Add(chunkPos, chunk);
                }
            }
        }

        // Setup player spawn point
        int playerSpawnX = _worldSizeX * _chunkSize / 2;
        int playerSpawnZ = _worldSizeZ * _chunkSize / 2;
        FindObjectOfType<PlayerController>().gameObject.transform.position = new Vector3(playerSpawnX, GetHeight(playerSpawnX, playerSpawnZ) + 5, playerSpawnZ);
        FindObjectOfType<PlayerController>().gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void RegenerateWorld()
    {
        for (int x = 0; x < _worldSizeX; x++)
        {
            for (int y = 0; y < _worldSizeY; y++)
            {
                for (int z = 0; z < _worldSizeZ; z++)
                {
                    _chunks[new Vector3Int(x, y, z)].ReloadChunk();
                }
            }
        }
    }

    public Chunk GetChunkAt(Vector3 pos)
    {
        Vector3Int chunkCoords = new Vector3Int(Mathf.FloorToInt(pos.x / _chunkSize) * _chunkSize,
                                                Mathf.FloorToInt(pos.y / _chunkSize) * _chunkSize,
                                                Mathf.FloorToInt(pos.z / _chunkSize) * _chunkSize);

        if (_chunks.TryGetValue(chunkCoords, out Chunk chunk))
            return chunk;

        return null;
    }

    public void DeleteAllChunks()
    {
        // TODO: replace this
        Chunk[] chunks = FindObjectsOfType<Chunk>();

        foreach (Chunk c in chunks)
            c.DeleteChunk();

        _chunks = null;
    }

    public float GetHeight(int x, int y)
    {
        return _noise.GetHeight(x, y);
    }

    public void SetSeed(int seed) { _seed = seed; }
    public void SetWorldSizeX(int x) { _worldSizeX = x; }
    public void SetWorldSizeY(int y) { _worldSizeY = y; }
    public void SetWorldSizeZ(int z) { _worldSizeZ = z; }
    public void SetChunkSize(int size) { _chunkSize = size; }
    public void SetMountainWidth(int scale) { _noiseScale = scale; }
    public void SetMountainHeight(int scale) { _heightScale = scale; }
}
