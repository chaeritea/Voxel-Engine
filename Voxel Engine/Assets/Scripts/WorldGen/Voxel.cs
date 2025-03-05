using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{

    public enum VoxelType
    {
        Air,
        Ground
    }

    public Vector3 _position;
    public VoxelType _type;
    public bool _isActive;

    public Voxel(Vector3 position, VoxelType type, bool isActive)
    {
        _position = position;
        _type = type;
        _isActive = isActive;
    }
}
