// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Universe
{
    public class Controls : IInputActionCollection
    {
        private InputActionAsset asset;
        public Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""StarCluster"",
            ""id"": ""1e379cb5-136c-4c23-b089-5ab26ebfda1b"",
            ""actions"": [
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""7f4e2e5a-afad-49e2-bda4-e31236d33189"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""a8ee1b80-5fed-4118-8f10-f5774703a872"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AltCamera"",
                    ""type"": ""Button"",
                    ""id"": ""d217f86b-4591-477c-8093-63c4f0cb4c36"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BoxSelectionStart"",
                    ""type"": ""Button"",
                    ""id"": ""a17aebd5-0b67-4228-a649-7bad75f523a4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""BoxSelectionRelease"",
                    ""type"": ""Button"",
                    ""id"": ""f2b005ed-a27a-4340-a77f-c83d10eb7de2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""a9d3c9ae-5064-480b-8368-c68cc7d3e36d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""ToPlanetarySystem"",
                    ""type"": ""Button"",
                    ""id"": ""c670d114-9a37-4a0b-9841-065c210bc6d4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""65cc6c5b-198e-49c0-8049-29a845474e17"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""548d0d36-899e-4f13-8d15-d2e5caceb5a5"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""74957de3-2c42-433c-abd5-f335493e5080"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""0095915a-1bca-439c-9e84-87e67817b42a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ab280041-8a98-4fa1-8f7d-d47cb97e58f4"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""07f16ef2-1fd9-4d81-b6b6-09e2a44acfb8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""587376aa-c78d-44ba-b284-445fa969e707"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""26562bd1-6fec-4394-88d7-b53ae9c2fe96"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ed380f68-9eb6-4b26-9464-af59a1dfed1d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d4422ac8-c49a-45c3-9826-0ce9698f29cd"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""75daf927-6db5-4f6a-8465-b88a12700e6c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f0faf8a0-79c2-4879-aebc-2a8b3e1b3984"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BoxSelectionStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0087bb96-834c-4839-9b9b-3ca9fc0b16c0"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BoxSelectionRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f9e7c23-8a5c-4a88-98b2-adf550176af8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9327fc0b-1309-43bb-93d4-e10d33daa7b6"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToPlanetarySystem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Planet"",
            ""id"": ""92c4ead4-63ef-447c-b33e-48f81858a610"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""84569a08-d645-4348-bf24-5ec08b7026fa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""59ddcc16-c63d-4c47-a43c-b7ccad1a26d5"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlanetarySystem"",
            ""id"": ""113b2517-3090-48a1-843d-d0d4ecc69426"",
            ""actions"": [
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""0622222b-7652-4baf-978b-b813932c8552"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b59261fb-a849-49c8-a930-683e04efc2fd"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // StarCluster
            m_StarCluster = asset.GetActionMap("StarCluster");
            m_StarCluster_Zoom = m_StarCluster.GetAction("Zoom");
            m_StarCluster_MoveCamera = m_StarCluster.GetAction("MoveCamera");
            m_StarCluster_AltCamera = m_StarCluster.GetAction("AltCamera");
            m_StarCluster_BoxSelectionStart = m_StarCluster.GetAction("BoxSelectionStart");
            m_StarCluster_BoxSelectionRelease = m_StarCluster.GetAction("BoxSelectionRelease");
            m_StarCluster_Cancel = m_StarCluster.GetAction("Cancel");
            m_StarCluster_ToPlanetarySystem = m_StarCluster.GetAction("ToPlanetarySystem");
            // Planet
            m_Planet = asset.GetActionMap("Planet");
            m_Planet_Newaction = m_Planet.GetAction("New action");
            // PlanetarySystem
            m_PlanetarySystem = asset.GetActionMap("PlanetarySystem");
            m_PlanetarySystem_Cancel = m_PlanetarySystem.GetAction("Cancel");
        }

        ~Controls()
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

        // StarCluster
        private readonly InputActionMap m_StarCluster;
        private IStarClusterActions m_StarClusterActionsCallbackInterface;
        private readonly InputAction m_StarCluster_Zoom;
        private readonly InputAction m_StarCluster_MoveCamera;
        private readonly InputAction m_StarCluster_AltCamera;
        private readonly InputAction m_StarCluster_BoxSelectionStart;
        private readonly InputAction m_StarCluster_BoxSelectionRelease;
        private readonly InputAction m_StarCluster_Cancel;
        private readonly InputAction m_StarCluster_ToPlanetarySystem;
        public struct StarClusterActions
        {
            private Controls m_Wrapper;
            public StarClusterActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Zoom => m_Wrapper.m_StarCluster_Zoom;
            public InputAction @MoveCamera => m_Wrapper.m_StarCluster_MoveCamera;
            public InputAction @AltCamera => m_Wrapper.m_StarCluster_AltCamera;
            public InputAction @BoxSelectionStart => m_Wrapper.m_StarCluster_BoxSelectionStart;
            public InputAction @BoxSelectionRelease => m_Wrapper.m_StarCluster_BoxSelectionRelease;
            public InputAction @Cancel => m_Wrapper.m_StarCluster_Cancel;
            public InputAction @ToPlanetarySystem => m_Wrapper.m_StarCluster_ToPlanetarySystem;
            public InputActionMap Get() { return m_Wrapper.m_StarCluster; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(StarClusterActions set) { return set.Get(); }
            public void SetCallbacks(IStarClusterActions instance)
            {
                if (m_Wrapper.m_StarClusterActionsCallbackInterface != null)
                {
                    Zoom.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnZoom;
                    Zoom.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnZoom;
                    Zoom.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnZoom;
                    MoveCamera.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnMoveCamera;
                    MoveCamera.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnMoveCamera;
                    MoveCamera.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnMoveCamera;
                    AltCamera.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnAltCamera;
                    AltCamera.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnAltCamera;
                    AltCamera.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnAltCamera;
                    BoxSelectionStart.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionStart;
                    BoxSelectionStart.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionStart;
                    BoxSelectionStart.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionStart;
                    BoxSelectionRelease.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionRelease;
                    BoxSelectionRelease.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionRelease;
                    BoxSelectionRelease.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnBoxSelectionRelease;
                    Cancel.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCancel;
                    Cancel.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCancel;
                    Cancel.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCancel;
                    ToPlanetarySystem.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnToPlanetarySystem;
                    ToPlanetarySystem.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnToPlanetarySystem;
                    ToPlanetarySystem.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnToPlanetarySystem;
                }
                m_Wrapper.m_StarClusterActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Zoom.started += instance.OnZoom;
                    Zoom.performed += instance.OnZoom;
                    Zoom.canceled += instance.OnZoom;
                    MoveCamera.started += instance.OnMoveCamera;
                    MoveCamera.performed += instance.OnMoveCamera;
                    MoveCamera.canceled += instance.OnMoveCamera;
                    AltCamera.started += instance.OnAltCamera;
                    AltCamera.performed += instance.OnAltCamera;
                    AltCamera.canceled += instance.OnAltCamera;
                    BoxSelectionStart.started += instance.OnBoxSelectionStart;
                    BoxSelectionStart.performed += instance.OnBoxSelectionStart;
                    BoxSelectionStart.canceled += instance.OnBoxSelectionStart;
                    BoxSelectionRelease.started += instance.OnBoxSelectionRelease;
                    BoxSelectionRelease.performed += instance.OnBoxSelectionRelease;
                    BoxSelectionRelease.canceled += instance.OnBoxSelectionRelease;
                    Cancel.started += instance.OnCancel;
                    Cancel.performed += instance.OnCancel;
                    Cancel.canceled += instance.OnCancel;
                    ToPlanetarySystem.started += instance.OnToPlanetarySystem;
                    ToPlanetarySystem.performed += instance.OnToPlanetarySystem;
                    ToPlanetarySystem.canceled += instance.OnToPlanetarySystem;
                }
            }
        }
        public StarClusterActions @StarCluster => new StarClusterActions(this);

        // Planet
        private readonly InputActionMap m_Planet;
        private IPlanetActions m_PlanetActionsCallbackInterface;
        private readonly InputAction m_Planet_Newaction;
        public struct PlanetActions
        {
            private Controls m_Wrapper;
            public PlanetActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Newaction => m_Wrapper.m_Planet_Newaction;
            public InputActionMap Get() { return m_Wrapper.m_Planet; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlanetActions set) { return set.Get(); }
            public void SetCallbacks(IPlanetActions instance)
            {
                if (m_Wrapper.m_PlanetActionsCallbackInterface != null)
                {
                    Newaction.started -= m_Wrapper.m_PlanetActionsCallbackInterface.OnNewaction;
                    Newaction.performed -= m_Wrapper.m_PlanetActionsCallbackInterface.OnNewaction;
                    Newaction.canceled -= m_Wrapper.m_PlanetActionsCallbackInterface.OnNewaction;
                }
                m_Wrapper.m_PlanetActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Newaction.started += instance.OnNewaction;
                    Newaction.performed += instance.OnNewaction;
                    Newaction.canceled += instance.OnNewaction;
                }
            }
        }
        public PlanetActions @Planet => new PlanetActions(this);

        // PlanetarySystem
        private readonly InputActionMap m_PlanetarySystem;
        private IPlanetarySystemActions m_PlanetarySystemActionsCallbackInterface;
        private readonly InputAction m_PlanetarySystem_Cancel;
        public struct PlanetarySystemActions
        {
            private Controls m_Wrapper;
            public PlanetarySystemActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Cancel => m_Wrapper.m_PlanetarySystem_Cancel;
            public InputActionMap Get() { return m_Wrapper.m_PlanetarySystem; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlanetarySystemActions set) { return set.Get(); }
            public void SetCallbacks(IPlanetarySystemActions instance)
            {
                if (m_Wrapper.m_PlanetarySystemActionsCallbackInterface != null)
                {
                    Cancel.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCancel;
                    Cancel.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCancel;
                    Cancel.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCancel;
                }
                m_Wrapper.m_PlanetarySystemActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Cancel.started += instance.OnCancel;
                    Cancel.performed += instance.OnCancel;
                    Cancel.canceled += instance.OnCancel;
                }
            }
        }
        public PlanetarySystemActions @PlanetarySystem => new PlanetarySystemActions(this);
        public interface IStarClusterActions
        {
            void OnZoom(InputAction.CallbackContext context);
            void OnMoveCamera(InputAction.CallbackContext context);
            void OnAltCamera(InputAction.CallbackContext context);
            void OnBoxSelectionStart(InputAction.CallbackContext context);
            void OnBoxSelectionRelease(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
            void OnToPlanetarySystem(InputAction.CallbackContext context);
        }
        public interface IPlanetActions
        {
            void OnNewaction(InputAction.CallbackContext context);
        }
        public interface IPlanetarySystemActions
        {
            void OnCancel(InputAction.CallbackContext context);
        }
    }
}
