using UnityEngine;

namespace DiveCat.Gameplay.Aiming
{
    /// <summary>
    /// Core logic for calculating the aiming line path.
    /// handles collision detection, distance limits, and angle clamping.
    /// </summary>
    [AddComponentMenu("Gameplay/Aiming/Aiming System")]
    public class AimingSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _shotPoint;
        [Tooltip("The transform used as the 'forward' reference for angle limits. Usually the character root or weapon parent.")]
        [SerializeField] private Transform _referenceTransform;

        [Header("Configuration")]
        [SerializeField] private float _maxDistance = 20f;
        [SerializeField] private bool _useCollision = true;
        [SerializeField] private LayerMask _collisionMask;

        [Header("Angle Limits")]
        [SerializeField] private bool _useAngleLimit = false;
        [Range(-180f, 180f)]
        [SerializeField] private float _minAngle = -45f;
        [Range(-180f, 180f)]
        [SerializeField] private float _maxAngle = 45f;

        [Header("Runtime")]
        [SerializeField] private bool _isActive = true;

        private IAimingInputProvider _inputProvider;
        
        private Vector2 _currentStartPoint;
        private Vector2 _currentEndPoint;
        private bool _isCurrentlyAiming;

        public Vector2 StartPoint => _currentStartPoint;
        public Vector2 EndPoint => _currentEndPoint;
        public bool IsActive => _isActive && _isCurrentlyAiming;

        private void Awake()
        {
            _inputProvider = GetComponent<IAimingInputProvider>();
            
            if (_shotPoint == null)
            {
                Debug.LogWarning($"[AimingSystem] ShotPoint is not assigned on {gameObject.name}. Using transform instead.", this);
                _shotPoint = transform;
            }

            if (_referenceTransform == null)
            {
                _referenceTransform = transform;
            }
        }

        private void Update()
        {
            if (!_isActive || _inputProvider == null)
            {
                _isCurrentlyAiming = false;
                return;
            }

            _isCurrentlyAiming = _inputProvider.IsAiming;
            if (!_isCurrentlyAiming) return;

            UpdateAimPoints();
        }

        private void UpdateAimPoints()
        {
            _currentStartPoint = _shotPoint.position;
            Vector2 targetPos = _inputProvider.GetAimWorldPosition();
            
            Vector2 direction = targetPos - _currentStartPoint;
            float distance = direction.magnitude;

            // 1. Handle Angle Clamping
            if (_useAngleLimit)
            {
                // Convert direction to local space of the reference transform
                // This automatically handles character flipping (scale.x = -1) and rotations
                Vector2 localDir = _referenceTransform.InverseTransformDirection(direction);
                
                // Calculate angle in degrees (0 is right/forward)
                float currentAngle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
                
                // Clamp the angle
                float clampedAngle = Mathf.Clamp(currentAngle, _minAngle, _maxAngle);
                
                // Reconstruct direction from clamped angle
                float rad = clampedAngle * Mathf.Deg2Rad;
                localDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                
                // Back to world space
                direction = _referenceTransform.TransformDirection(localDir);
            }

            // 2. Handle Distance Limit
            if (distance > _maxDistance)
            {
                distance = _maxDistance;
            }

            // 3. Handle Collision
            if (_useCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(_currentStartPoint, direction.normalized, distance, _collisionMask);
                
                if (hit.collider != null)
                {
                    _currentEndPoint = hit.point;
                }
                else
                {
                    _currentEndPoint = _currentStartPoint + (direction.normalized * distance);
                }
            }
            else
            {
                _currentEndPoint = _currentStartPoint + (direction.normalized * distance);
            }
        }

        public void SetVisibility(bool visible)
        {
            _isActive = visible;
        }

        // Helper for debugging limits in Scene View
        private void OnDrawGizmosSelected()
        {
            if (!_useAngleLimit || _referenceTransform == null || _shotPoint == null) return;

            Gizmos.color = Color.yellow;
            Vector3 start = _shotPoint.position;
            
            Vector3 minDir = _referenceTransform.TransformDirection(new Vector3(Mathf.Cos(_minAngle * Mathf.Deg2Rad), Mathf.Sin(_minAngle * Mathf.Deg2Rad), 0));
            Vector3 maxDir = _referenceTransform.TransformDirection(new Vector3(Mathf.Cos(_maxAngle * Mathf.Deg2Rad), Mathf.Sin(_maxAngle * Mathf.Deg2Rad), 0));

            Gizmos.DrawLine(start, start + minDir * 2f);
            Gizmos.DrawLine(start, start + maxDir * 2f);
        }
    }
}
