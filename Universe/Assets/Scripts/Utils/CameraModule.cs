using UnityEngine;

namespace Universe {
    [CreateAssetMenu]
    public class CameraModule : ScriptableObject
    {
        #region Private
        [SerializeField]
        private GameObject m_CameraPrefab = null;
        private static float _X, _Y;
        #endregion

        #region Public
        private static GameObject m_MainCamera;
        private static Transform m_MainCameraTransform;

        public static GameObject Camera { get => m_MainCamera; set => m_MainCamera = value; }
        public static Transform MainCameraTransform { get => m_MainCameraTransform; set => m_MainCameraTransform = value; }
        #endregion

        #region Managers
        public void Init()
        {
            m_MainCamera = Instantiate(m_CameraPrefab);
            m_MainCameraTransform = m_MainCamera.transform;
        }

        public void ShutDown()
        {
            m_MainCameraTransform = null;
            Destroy(m_MainCamera);
        }
        #endregion

        #region Methods
        public static void RotateCamera(Vector2 angle)
        {
            _X += angle.x / 40;             _Y -= angle.y / 40;
            _Y = ClampAngle(_Y, -90, 90);
            Quaternion rotation = Quaternion.Euler(_Y, _X, 0);
            m_MainCameraTransform.localRotation = rotation;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
        #endregion

        public void Update()
        {
            switch (ControlSystem.ControlMode)
            {
                case ControlMode.StarCluster:
                    RotateCamera(ControlSystem.InputSystem.StarCluster.RotateCamera.ReadValue<Vector2>());
                    break;
                case ControlMode.PlanetarySystem:
                    RotateCamera(ControlSystem.InputSystem.PlanetarySystem.RotateCamera.ReadValue<Vector2>());
                    break;
                default:
                    break;
            }
        }
    }
}
