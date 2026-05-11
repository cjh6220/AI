using UnityEngine;

namespace DiveCat.Gameplay.Aiming
{
    /// <summary>
    /// Provides aiming input based on the mouse position.
    /// </summary>
    [AddComponentMenu("Gameplay/Aiming/Mouse Aiming Input")]
    public class MouseAimingInput : MonoBehaviour, IAimingInputProvider
    {
        [SerializeField] private Camera _targetCamera;
        [SerializeField] private bool _requireButtonDown = false;
        [SerializeField] private string _aimButtonName = "Fire2"; // Right click by default

        private void Awake()
        {
            // Fallback if camera is not assigned
            if (_targetCamera == null)
            {
                _targetCamera = Camera.main;
            }
        }

        public Vector2 GetAimWorldPosition()
        {
            if (_targetCamera == null) return Vector2.zero;

            Vector3 mousePos = Input.mousePosition;
            // LineRenderer and Raycasts in 2D use World Space.
            // ScreenToWorldPoint requires Z to be distance from camera, but for 2D Z=0 is usually fine if orthographic.
            mousePos.z = -_targetCamera.transform.position.z; 
            return _targetCamera.ScreenToWorldPoint(mousePos);
        }

        public bool IsAiming => !_requireButtonDown || Input.GetButton(_aimButtonName);
    }
}
