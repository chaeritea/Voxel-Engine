using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelectUI : MonoBehaviour
{
    [SerializeField] private Transform _playerCam;
    [SerializeField] private Transform _cubeCam;
    [SerializeField] private PlayerController _player;
    [SerializeField] private MeshRenderer _renderer;

    void Update()
    {
        _cubeCam.rotation = _playerCam.rotation;
        _cubeCam.position = transform.position - (3 * _cubeCam.forward);

        Material mat = World.instance._materialsTEMP[_player.GetSelectedType().GetHashCode()];
        if (_renderer.material != mat)
            _renderer.material = mat;
    }
}
