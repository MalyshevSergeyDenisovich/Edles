using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bin.Global.Inputs
{
    public class PlayerCameraController : MonoBehaviour
    {
        private PlayerInputActions _inputActionsAsset;
        private InputAction _movement;
        private InputAction _rotation;

        private void Awake()
        {
            _inputActionsAsset = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _movement = _inputActionsAsset.PLayerActions.Movement;
            _movement.Enable();

            _rotation = _inputActionsAsset.PLayerActions.Rotation;
            _rotation.Enable();
        }

        private void OnDisable()
        {
            _movement.Disable();
            
            _rotation.Disable();
        }

        private void FixedUpdate()
        {
            Debug.Log("Value _movement - " + _movement.ReadValue<Vector2>()); 
            Debug.Log("Value _rotation - " + _rotation.ReadValue<float>()); 
        }

        private void OnMovement(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();
        }
    }
}
