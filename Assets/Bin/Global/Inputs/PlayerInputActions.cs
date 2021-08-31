// GENERATED AUTOMATICALLY FROM 'Assets/Bin/Global/Inputs/PlayerInputAction.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Bin.Global.Inputs
{
    public class PlayerInputActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public PlayerInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputAction"",
    ""maps"": [
        {
            ""name"": ""PLayerActions"",
            ""id"": ""9af42938-9c9c-4d2f-a020-1eda56e93589"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""0e67c57a-f296-496f-b1e1-991478092968"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Button"",
                    ""id"": ""ed6081d7-9cce-4f69-a59c-5d297251d93b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""0fc6b911-0875-4f6f-8b2f-f5bb79bab9bb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d96612e9-83d1-4d7e-91db-3b1cafeba030"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""efdfac9d-3a23-4c17-95ad-0da5b0216dc0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""041bc428-849f-4f0f-85e1-f572bd066d03"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""189d83d7-cfec-48a0-8960-d0b65d746096"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ed3bc20d-b882-4fea-a77e-1b74846a4354"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""b845e67f-18bc-4beb-afde-708866f1855b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b219e502-0f03-456c-8061-17ac0f49770a"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // PLayerActions
            m_PLayerActions = asset.FindActionMap("PLayerActions", throwIfNotFound: true);
            m_PLayerActions_Movement = m_PLayerActions.FindAction("Movement", throwIfNotFound: true);
            m_PLayerActions_Rotation = m_PLayerActions.FindAction("Rotation", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // PLayerActions
        private readonly InputActionMap m_PLayerActions;
        private IPLayerActionsActions m_PLayerActionsActionsCallbackInterface;
        private readonly InputAction m_PLayerActions_Movement;
        private readonly InputAction m_PLayerActions_Rotation;
        public struct PLayerActionsActions
        {
            private PlayerInputActions m_Wrapper;
            public PLayerActionsActions(PlayerInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_PLayerActions_Movement;
            public InputAction @Rotation => m_Wrapper.m_PLayerActions_Rotation;
            public InputActionMap Get() { return m_Wrapper.m_PLayerActions; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PLayerActionsActions set) { return set.Get(); }
            public void SetCallbacks(IPLayerActionsActions instance)
            {
                if (m_Wrapper.m_PLayerActionsActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnMovement;
                    @Rotation.started -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnRotation;
                    @Rotation.performed -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnRotation;
                    @Rotation.canceled -= m_Wrapper.m_PLayerActionsActionsCallbackInterface.OnRotation;
                }
                m_Wrapper.m_PLayerActionsActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Rotation.started += instance.OnRotation;
                    @Rotation.performed += instance.OnRotation;
                    @Rotation.canceled += instance.OnRotation;
                }
            }
        }
        public PLayerActionsActions @PLayerActions => new PLayerActionsActions(this);
        public interface IPLayerActionsActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnRotation(InputAction.CallbackContext context);
        }
    }
}
