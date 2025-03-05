using UnityEngine;

public class WorldNoise
{
    private float[,] _noiseMap;

    private int _width = 256;
    private int _length = 256;
    private float _scale = 20f;
    private float _heightScale = 20f;

    private float _offsetX = 0f;
    private float _offsetY = 0f;

    public WorldNoise(float noiseScale = 20f, float heightScale = 20f)
    {
        _scale = noiseScale;
        _heightScale = heightScale;
    }

    public void GenerateNoise_Random()
    {
        _offsetX = Random.Range(0, 256);
        _offsetY = Random.Range(0, 256);

        float[,] map = new float[_width, _length];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _length; y++)
            {
                float height = CalculateHeight(x, y);

                map[x, y] = height;
            }
        }

        _noiseMap = map;
    }

    public void GenerateNoise_Seeded(int seed)
    {
        Random.InitState(seed);
        _offsetX = Random.Range(0, 256);
        _offsetY = Random.Range(0, 256);

        float[,] map = new float[_width, _length];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _length; y++)
            {
                float height = CalculateHeight(x, y);

                map[x, y] = height;
            }
        }

        _noiseMap = map;
    }

    private float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / _width * _scale + _offsetX;
        float yCoord = (float)y / _length * _scale + _offsetY;

        float height = Mathf.PerlinNoise(xCoord, yCoord) * _heightScale;

        return height;
    }

    public float GetHeight(int x, int y)
    {
        if (_noiseMap == null)
        {
            Debug.LogError("Noise map not generated");
            return 0;
        }

        return _noiseMap[x, y];
    }
}
