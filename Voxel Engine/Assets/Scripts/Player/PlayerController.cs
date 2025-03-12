using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // References
    private Rigidbody _rb;
    [SerializeField] private GameObject _cam;

    // Input params
    private InputActionMap _playerController;
    private InputAction _move;
    private InputAction _look;
    private InputAction _scroll;

    // Movement params
    private float _cameraH = 0;
    private float _cameraV = 0;
    private Vector3 _moveForward = Vector3.zero;
    private Vector3 _moveSideways = Vector3.zero;
    private bool _isGrounded = true;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 20f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _mouseSensitivity = 1f;

    // Other params
    private int _hotbarIndex = 0;
    [SerializeField] private List<Voxel.VoxelType> _hotbar;
    [SerializeField] private float _reachDistance = 5f;
    [SerializeField] private LayerMask _voxelMask;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerInput>().actions.FindActionMap("PlayerController");

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _playerController.Enable();

        _move = _playerController.FindAction("Move");
        _look = _playerController.FindAction("Look");
        _scroll = _playerController.FindAction("Scroll");

        _playerController.FindAction("Jump").performed += Jump;
        // TODO: add this
        //_playerController.FindAction("Fly").performed += ToggleFly;
        _playerController.FindAction("Create").performed += CreateVoxel;
        _playerController.FindAction("Destroy").performed += DestroyVoxel;
    }

    // Camera gets very choppy if in FixedUpdate
    private void Update()
    {
        if (WorldSettingsManager.instance._isMenuOpen)
            return;

        // Camera control
        _cameraH += _mouseSensitivity * _look.ReadValue<Vector2>().x;
        _cameraV -= _mouseSensitivity * _look.ReadValue<Vector2>().y;

        _cameraV = Mathf.Clamp(_cameraV, -89f, 89f);

        _cam.transform.eulerAngles = new Vector3(_cameraV, _cameraH, 0.0f);

        // Hotbar scroll
        float scrollValue = _scroll.ReadValue<Vector2>().y;

        if (scrollValue > 0)
        {
            if (++_hotbarIndex >= _hotbar.Count)
                _hotbarIndex = 0;
        }
        else if (scrollValue < 0)
        {
            if (--_hotbarIndex < 0)
                _hotbarIndex = _hotbar.Count - 1;
        }
    }

    // FixedUpdate for anything involving physics to stay framerate agnostic
    private void FixedUpdate()
    {
        if (WorldSettingsManager.instance._isMenuOpen)
            return;

        // Movement
        _moveForward = _cam.transform.forward * _move.ReadValue<Vector2>().y;
        _moveSideways = _cam.transform.right * _move.ReadValue<Vector2>().x;

        Vector3 movementVec = (_moveForward + _moveSideways);
        movementVec = new Vector3(movementVec.x, 0, movementVec.z).normalized * _moveSpeed;

        _rb.velocity = new Vector3(movementVec.x, _rb.velocity.y, movementVec.z);

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * _fallMultiplier * Time.deltaTime;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (WorldSettingsManager.instance._isMenuOpen)
            return;

        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }

    private void CreateVoxel(InputAction.CallbackContext context)
    {
        if (WorldSettingsManager.instance._isMenuOpen)
            return;

        Debug.Log("Trying to create...");

        RaycastHit hit;

        // TODO: figure out and add _reachDistance
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _voxelMask))
        {
            Vector3 globalPos = hit.point;
            // TODO: fix this terrible terrible line
            globalPos -= _cam.transform.forward.normalized * 0.01f;
            Chunk hitChunk = World.instance.GetChunkAt(globalPos);
            Vector3 localPos = hitChunk.transform.InverseTransformPoint(globalPos);

            hitChunk.CreateVoxelAt(localPos, _hotbar[_hotbarIndex]);
            Debug.Log("Creating voxel at " + globalPos);
        }
    }

    private void DestroyVoxel(InputAction.CallbackContext context)
    {
        if (WorldSettingsManager.instance._isMenuOpen)
            return;

        Debug.Log("Trying to destroy...");

        RaycastHit hit;

        // TODO: figure out and add _reachDistance
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _voxelMask))
        {
            Vector3 globalPos = hit.point;
            // TODO: fix this terrible terrible line
            globalPos += _cam.transform.forward.normalized * 0.01f;
            Chunk hitChunk = World.instance.GetChunkAt(globalPos);
            Vector3 localPos = hitChunk.transform.InverseTransformPoint(globalPos);

            hitChunk.DestroyVoxelAt(localPos);
            Debug.Log("Destroying voxel at " + globalPos);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Voxel.VoxelType GetSelectedType()
    {
        return _hotbar[_hotbarIndex];
    }

    private void OnCollisionStay(Collision collision)
    {
        _isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
