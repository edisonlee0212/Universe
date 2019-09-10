using UnityEngine;

namespace Universe {
    [CreateAssetMenu]
    public class CameraModule : ScriptableObject
    {
        #region Private
        [SerializeField]
        private Camera m_MainCameraPrefab = null;
        private static float _X, _Y;
        #endregion

        #region Public
        private static Camera m_MainCamera;
        private static Transform m_MainCameraTransform;

        public static Camera MainCamera { get => m_MainCamera; set => m_MainCamera = value; }
        public static Transform MainCameraTransform { get => m_MainCameraTransform; set => m_MainCameraTransform = value; }
        #endregion

        #region Managers
        public void Init()
        {
            m_MainCamera = Instantiate(m_MainCameraPrefab);
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
            _X += angle.x / 40;
            _Y -= angle.y / 40;
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
    }
}
