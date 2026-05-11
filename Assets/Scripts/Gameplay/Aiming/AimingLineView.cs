using UnityEngine;

namespace DiveCat.Gameplay.Aiming
{
    /// <summary>
    /// Handles the visual representation of the aiming line using a LineRenderer.
    /// Includes smooth transitions for showing/hiding.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("Gameplay/Aiming/Aiming Line View")]
    public class AimingLineView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AimingSystem _aimingSystem;
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("Settings")]
        [SerializeField] private float _baseWidth = 0.05f;
        [SerializeField] private float _fadeSpeed = 10f;
        
        private float _currentAlpha = 0f;
        private Vector3[] _linePoints = new Vector3[2];

        private void Reset()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _aimingSystem = GetComponentInParent<AimingSystem>();
            
            // Default setup for 2D line
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = _baseWidth;
            _lineRenderer.endWidth = _baseWidth;
        }

        private void Awake()
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
            
            // Ensure LineRenderer is initialized for 2D
            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }

        private void LateUpdate()
        {
            if (_aimingSystem == null) return;

            bool targetActive = _aimingSystem.IsActive;
            
            // Smooth visibility handling
            float targetAlpha = targetActive ? 1f : 0f;
            _currentAlpha = Mathf.MoveTowards(_currentAlpha, targetAlpha, Time.deltaTime * _fadeSpeed);

            if (_currentAlpha <= 0f)
            {
                if (_lineRenderer.enabled) _lineRenderer.enabled = false;
                return;
            }

            if (!_lineRenderer.enabled) _lineRenderer.enabled = true;

            // Update width based on current alpha for a "growing" effect or just use it for alpha if the material supports it.
            // Production tip: Modifying Width is more compatible than material properties which might cause draw call spikes if not handled.
            float width = _baseWidth * _currentAlpha;
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;

            // Update positions
            _linePoints[0] = _aimingSystem.StartPoint;
            _linePoints[1] = _aimingSystem.EndPoint;
            
            _lineRenderer.SetPositions(_linePoints);
        }
    }
}
