// GENERATED AUTOMATICALLY FROM 'Assets/Bin/Global/Inputs/CameraInGameAsset.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Bin.Global.Inputs
{
    public class @CameraInGameAsset : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CameraInGameAsset()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraInGameAsset"",
    ""maps"": [
        {
            ""name"": ""InGame"",
            ""id"": ""6f223625-3681-4918-8311-4f40b4494b5b"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e219317f-869e-43f6-8705-7a5742610b03"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Value"",
                    ""id"": ""4d794488-aa55-4806-bb8e-646da1dc1d23"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""52c68b16-9b25-44b2-bd00-b01e0ba0f304"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""37705c5f-b220-49d3-a07b-e7b46355744f"",
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
                    ""id"": ""799e71de-8249-4978-b3ef-460aba66f64e"",
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
                    ""id"": ""f7905f1d-b7c7-462e-a8fc-24057c605c46"",
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
                    ""id"": ""426b7a26-6ff6-4707-9948-982fb50c6c7e"",
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
                    ""id"": ""b8590753-0b9b-43dc-8770-4a201962c987"",
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
                    ""id"": ""d1c8517a-7b39-4b4f-8567-9e249793c34b"",
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
                    ""id"": ""f0f6a8b7-88f8-48a6-9906-d3597d393237"",
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
                    ""id"": ""ff4aae17-b435-49ee-9d4a-064a27c9b7e1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""443201ad-8023-4506-a862-c3be818721ec"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // InGame
            m_InGame = asset.FindActionMap("InGame", throwIfNotFound: true);
            m_InGame_Movement = m_InGame.FindAction("Movement", throwIfNotFound: true);
            m_InGame_Rotation = m_InGame.FindAction("Rotation", throwIfNotFound: true);
            m_InGame_Scroll = m_InGame.FindAction("Scroll", throwIfNotFound: true);
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

        // InGame
        private readonly InputActionMap m_InGame;
        private IInGameActions m_InGameActionsCallbackInterface;
        private readonly InputAction m_InGame_Movement;
        private readonly InputAction m_InGame_Rotation;
        private readonly InputAction m_InGame_Scroll;
        public struct InGameActions
        {
            private @CameraInGameAsset m_Wrapper;
            public InGameActions(@CameraInGameAsset wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_InGame_Movement;
            public InputAction @Rotation => m_Wrapper.m_InGame_Rotation;
            public InputAction @Scroll => m_Wrapper.m_InGame_Scroll;
            public InputActionMap Get() { return m_Wrapper.m_InGame; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
            public void SetCallbacks(IInGameActions instance)
            {
                if (m_Wrapper.m_InGameActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                    @Rotation.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnRotation;
                    @Rotation.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnRotation;
                    @Rotation.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnRotation;
                    @Scroll.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnScroll;
                    @Scroll.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnScroll;
                    @Scroll.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnScroll;
                }
                m_Wrapper.m_InGameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Rotation.started += instance.OnRotation;
                    @Rotation.performed += instance.OnRotation;
                    @Rotation.canceled += instance.OnRotation;
                    @Scroll.started += instance.OnScroll;
                    @Scroll.performed += instance.OnScroll;
                    @Scroll.canceled += instance.OnScroll;
                }
            }
        }
        public InGameActions @InGame => new InGameActions(this);
        public interface IInGameActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnRotation(InputAction.CallbackContext context);
            void OnScroll(InputAction.CallbackContext context);
        }
    }
}
