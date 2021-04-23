#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using Photon.Pun;
using UnityEngine;

namespace UnityTemplateProjects
{
    
    public class SimpleCameraController : MonoBehaviour
    {
        private new Camera camera;
        
        class CameraState
        {
            public float yaw;
            public float pitch;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
            }

            public void LerpTowards(CameraState target, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            }

            public void UpdateTransform(Transform t, Transform c)
            {
                c.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                t.eulerAngles = new Vector3(0f, yaw, 0f);
            }
        }
        
        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            //m_TargetCameraState.SetFromTransform(transform);
           // m_InterpolatingCameraState.SetFromTransform(transform);
        }

        private void Awake()
        {
            this.RequireComponentInChildren(out camera);

            if (TryGetComponent(out PhotonView photonView))
            {
                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    Destroy(this);
                    foreach(Camera camera in GetComponentsInChildren<Camera>())
                    {
                        Destroy(camera.gameObject);
                    }
                    
                    return;
                }
            }
        }

        void Update()
        {
            if (Time.timeScale == 0) return;
#if ENABLE_LEGACY_INPUT_MANAGER

            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
                
                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            boost += Input.mouseScrollDelta.y * 0.2f;

#elif USE_INPUT_SYSTEM 
            // TODO: make the new input system work
#endif


            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform, camera.transform);
        }
    }

}