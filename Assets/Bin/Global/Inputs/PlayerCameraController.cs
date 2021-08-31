using Bin.Global.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bin.Global.Inputs
{
    public class PlayerCameraController : MonoBehaviour
    {
        private CameraInGameAsset _inputActionsAsset;
        private InputAction _movement;
        private InputAction _rotation;
        private InputAction _scroll;

        private Vector3 _forceDirection = Vector3.zero;
        
        private Transform _playerCamera;
        private PlayerInputManager _playerInputManager;
        
        private void Awake() => OnAwake();

        private void OnEnable() => Enable();

        private void OnDisable() => Disable();

        private void FixedUpdate() => OnFixedUpdate();

        
        private Vector3 GetCameraForward()
        {
            var forward = _playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        private Vector3 GetCameraRight()
        {
            var right = _playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        private void OnAwake()
        {
            _playerInputManager = new PlayerInputManager();
            _playerCamera = transform.GetChild(0);
            _inputActionsAsset = new CameraInGameAsset();
            HashFields();
        }

        private void OnFixedUpdate()
        {
            Move();
            Rotate();
            Scroll();
        }

        private void Scroll()
        {
            var y = _scroll.ReadValue<Vector2>().y / 10;
            transform.position += new Vector3(0, y, 0);
        }

        private void Move()
        {
            _forceDirection += GetCameraRight() * (_movement.ReadValue<Vector2>().x * CameraSettings.MovementSpeed);
            _forceDirection += GetCameraForward() * (_movement.ReadValue<Vector2>().y * CameraSettings.MovementSpeed);
            transform.position += _forceDirection;
            _forceDirection = Vector3.zero;
        }

        private void Rotate()
        {
            var inputAction = _rotation.ReadValue<float>() * CameraSettings.RotationSpeed;
            transform.Rotate(Vector3.up * inputAction);
        }

        private void HashFields()
        {
            _movement = _inputActionsAsset.InGame.Movement;
            _rotation = _inputActionsAsset.InGame.Rotation;
            _scroll = _inputActionsAsset.InGame.Scroll;
        }

        private void Enable()
        {
            _movement.Enable();
            _rotation.Enable();
            _scroll.Enable();
        }

        private void Disable()
        {
            _movement.Disable();
            _rotation.Disable();
            _scroll.Disable();
        }
    }
}
