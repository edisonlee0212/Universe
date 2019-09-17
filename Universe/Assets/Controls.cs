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
                },
                {
                    ""name"": ""RotateCamera"",
                    ""type"": ""Value"",
                    ""id"": ""49fecd58-d45e-45eb-b3fd-1d26da85cce9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraRotateStart"",
                    ""type"": ""Button"",
                    ""id"": ""c06439bf-d792-43ec-8c18-92d6755c82c7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""CameraRotateRelease"",
                    ""type"": ""Button"",
                    ""id"": ""b34966a6-8136-4b7c-949e-487f8fdaaa99"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FollowStar"",
                    ""type"": ""Button"",
                    ""id"": ""06a745d5-d3fc-4567-bd89-47ede661429e"",
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
                    ""name"": """",
                    ""id"": ""f0faf8a0-79c2-4879-aebc-2a8b3e1b3984"",
                    ""path"": ""<Mouse>/leftButton"",
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
                    ""path"": ""<Mouse>/leftButton"",
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
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToPlanetarySystem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0f5d948d-6c32-4dd6-ac8e-babb619520a5"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""611f53e4-1ff8-45e8-b1fe-55b74653a2be"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotateStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc1b0e0e-2b50-49e9-bc6d-b8c56336b6c4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotateRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
                    ""id"": ""8f03bcc6-0ef3-44f6-9dc5-2203e3a366ee"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FollowStar"",
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
                },
                {
                    ""name"": ""CameraRotateRelease"",
                    ""type"": ""Button"",
                    ""id"": ""21ccc294-1869-44e9-866b-076ed8ef7b99"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraRotateStart"",
                    ""type"": ""Button"",
                    ""id"": ""6280865e-b4f2-4f2a-b0f1-0896e905a769"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""54ee392d-61dd-41d1-859d-c5fe7d47c39b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateCamera"",
                    ""type"": ""Value"",
                    ""id"": ""d474e21b-e5db-4d37-89f6-523ac0fe6643"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AltCamera"",
                    ""type"": ""Button"",
                    ""id"": ""ac492e1a-85f5-47c9-80c7-1843b52f68a4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
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
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""4adee7b0-16fe-40b7-bb7e-7904dd3ca5d6"",
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
                    ""id"": ""3874a0f7-1de4-454a-8960-684580bc8195"",
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
                    ""id"": ""713ce5c9-5171-4b0a-9b91-81fa47f4d38a"",
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
                    ""id"": ""0386bf1a-b95f-484d-83f8-ae30d182e20e"",
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
                    ""id"": ""779ff189-38ca-4816-97a9-9d18d0be3b04"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1229adf6-b05a-4e74-a0ef-126aaf17cce0"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotateRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f770e9bc-7200-4289-a888-792a3e60aa18"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotateStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7162a7d6-74fe-4207-90bb-72d42d93de66"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ed2ad197-0132-40c7-bbc1-9a598c1885e9"",
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
                    ""id"": ""de85d3ab-4e17-411b-af80-7442c910215e"",
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
                    ""id"": ""fc15d3b0-6709-4cf5-b324-91e182bbfddd"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
            m_StarCluster_RotateCamera = m_StarCluster.GetAction("RotateCamera");
            m_StarCluster_CameraRotateStart = m_StarCluster.GetAction("CameraRotateStart");
            m_StarCluster_CameraRotateRelease = m_StarCluster.GetAction("CameraRotateRelease");
            m_StarCluster_FollowStar = m_StarCluster.GetAction("FollowStar");
            // Planet
            m_Planet = asset.GetActionMap("Planet");
            m_Planet_Newaction = m_Planet.GetAction("New action");
            // PlanetarySystem
            m_PlanetarySystem = asset.GetActionMap("PlanetarySystem");
            m_PlanetarySystem_Cancel = m_PlanetarySystem.GetAction("Cancel");
            m_PlanetarySystem_CameraRotateRelease = m_PlanetarySystem.GetAction("CameraRotateRelease");
            m_PlanetarySystem_CameraRotateStart = m_PlanetarySystem.GetAction("CameraRotateStart");
            m_PlanetarySystem_MoveCamera = m_PlanetarySystem.GetAction("MoveCamera");
            m_PlanetarySystem_RotateCamera = m_PlanetarySystem.GetAction("RotateCamera");
            m_PlanetarySystem_AltCamera = m_PlanetarySystem.GetAction("AltCamera");
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
        private readonly InputAction m_StarCluster_RotateCamera;
        private readonly InputAction m_StarCluster_CameraRotateStart;
        private readonly InputAction m_StarCluster_CameraRotateRelease;
        private readonly InputAction m_StarCluster_FollowStar;
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
            public InputAction @RotateCamera => m_Wrapper.m_StarCluster_RotateCamera;
            public InputAction @CameraRotateStart => m_Wrapper.m_StarCluster_CameraRotateStart;
            public InputAction @CameraRotateRelease => m_Wrapper.m_StarCluster_CameraRotateRelease;
            public InputAction @FollowStar => m_Wrapper.m_StarCluster_FollowStar;
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
                    RotateCamera.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnRotateCamera;
                    RotateCamera.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnRotateCamera;
                    RotateCamera.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnRotateCamera;
                    CameraRotateStart.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateStart;
                    CameraRotateStart.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateStart;
                    CameraRotateStart.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateStart;
                    CameraRotateRelease.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateRelease;
                    CameraRotateRelease.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateRelease;
                    CameraRotateRelease.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnCameraRotateRelease;
                    FollowStar.started -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnFollowStar;
                    FollowStar.performed -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnFollowStar;
                    FollowStar.canceled -= m_Wrapper.m_StarClusterActionsCallbackInterface.OnFollowStar;
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
                    RotateCamera.started += instance.OnRotateCamera;
                    RotateCamera.performed += instance.OnRotateCamera;
                    RotateCamera.canceled += instance.OnRotateCamera;
                    CameraRotateStart.started += instance.OnCameraRotateStart;
                    CameraRotateStart.performed += instance.OnCameraRotateStart;
                    CameraRotateStart.canceled += instance.OnCameraRotateStart;
                    CameraRotateRelease.started += instance.OnCameraRotateRelease;
                    CameraRotateRelease.performed += instance.OnCameraRotateRelease;
                    CameraRotateRelease.canceled += instance.OnCameraRotateRelease;
                    FollowStar.started += instance.OnFollowStar;
                    FollowStar.performed += instance.OnFollowStar;
                    FollowStar.canceled += instance.OnFollowStar;
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
        private readonly InputAction m_PlanetarySystem_CameraRotateRelease;
        private readonly InputAction m_PlanetarySystem_CameraRotateStart;
        private readonly InputAction m_PlanetarySystem_MoveCamera;
        private readonly InputAction m_PlanetarySystem_RotateCamera;
        private readonly InputAction m_PlanetarySystem_AltCamera;
        public struct PlanetarySystemActions
        {
            private Controls m_Wrapper;
            public PlanetarySystemActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Cancel => m_Wrapper.m_PlanetarySystem_Cancel;
            public InputAction @CameraRotateRelease => m_Wrapper.m_PlanetarySystem_CameraRotateRelease;
            public InputAction @CameraRotateStart => m_Wrapper.m_PlanetarySystem_CameraRotateStart;
            public InputAction @MoveCamera => m_Wrapper.m_PlanetarySystem_MoveCamera;
            public InputAction @RotateCamera => m_Wrapper.m_PlanetarySystem_RotateCamera;
            public InputAction @AltCamera => m_Wrapper.m_PlanetarySystem_AltCamera;
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
                    CameraRotateRelease.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateRelease;
                    CameraRotateRelease.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateRelease;
                    CameraRotateRelease.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateRelease;
                    CameraRotateStart.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateStart;
                    CameraRotateStart.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateStart;
                    CameraRotateStart.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnCameraRotateStart;
                    MoveCamera.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnMoveCamera;
                    MoveCamera.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnMoveCamera;
                    MoveCamera.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnMoveCamera;
                    RotateCamera.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnRotateCamera;
                    RotateCamera.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnRotateCamera;
                    RotateCamera.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnRotateCamera;
                    AltCamera.started -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnAltCamera;
                    AltCamera.performed -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnAltCamera;
                    AltCamera.canceled -= m_Wrapper.m_PlanetarySystemActionsCallbackInterface.OnAltCamera;
                }
                m_Wrapper.m_PlanetarySystemActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Cancel.started += instance.OnCancel;
                    Cancel.performed += instance.OnCancel;
                    Cancel.canceled += instance.OnCancel;
                    CameraRotateRelease.started += instance.OnCameraRotateRelease;
                    CameraRotateRelease.performed += instance.OnCameraRotateRelease;
                    CameraRotateRelease.canceled += instance.OnCameraRotateRelease;
                    CameraRotateStart.started += instance.OnCameraRotateStart;
                    CameraRotateStart.performed += instance.OnCameraRotateStart;
                    CameraRotateStart.canceled += instance.OnCameraRotateStart;
                    MoveCamera.started += instance.OnMoveCamera;
                    MoveCamera.performed += instance.OnMoveCamera;
                    MoveCamera.canceled += instance.OnMoveCamera;
                    RotateCamera.started += instance.OnRotateCamera;
                    RotateCamera.performed += instance.OnRotateCamera;
                    RotateCamera.canceled += instance.OnRotateCamera;
                    AltCamera.started += instance.OnAltCamera;
                    AltCamera.performed += instance.OnAltCamera;
                    AltCamera.canceled += instance.OnAltCamera;
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
            void OnRotateCamera(InputAction.CallbackContext context);
            void OnCameraRotateStart(InputAction.CallbackContext context);
            void OnCameraRotateRelease(InputAction.CallbackContext context);
            void OnFollowStar(InputAction.CallbackContext context);
        }
        public interface IPlanetActions
        {
            void OnNewaction(InputAction.CallbackContext context);
        }
        public interface IPlanetarySystemActions
        {
            void OnCancel(InputAction.CallbackContext context);
            void OnCameraRotateRelease(InputAction.CallbackContext context);
            void OnCameraRotateStart(InputAction.CallbackContext context);
            void OnMoveCamera(InputAction.CallbackContext context);
            void OnRotateCamera(InputAction.CallbackContext context);
            void OnAltCamera(InputAction.CallbackContext context);
        }
    }
}
