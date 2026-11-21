using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using DG.Tweening;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Speed source class")]
        public ISpeedSource SpeedSource;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Aim")]
        [SerializeField] private LayerMask _groundMask = ~0;
        [Tooltip("Dead zone for aiming")]
        [SerializeField] private float _aimDeadZone = 0.2f;
        [Tooltip("Camera for aiming")]
        [SerializeField] private Camera _mainCamera;
        [Tooltip("Rotation speed for aiming")]
        [SerializeField] private float _rotationSpeed = 0f;
        [Tooltip("Crosshair prefabs")]
        [SerializeField] private RectTransform _crosshair;
        [SerializeField] private Canvas _canvas;
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private Vector3 _originalCrosshairRotation;
        private Vector3 _originalCrosshairScale;
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        public PlayerState MCState = PlayerState.Idle;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            _originalCrosshairScale = _crosshair.localScale;
            _originalCrosshairRotation = _crosshair.localRotation.eulerAngles;
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            SpeedSource = GetComponent<ISpeedSource>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            MCState = PlayerState.Idle;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            AimAndAttack();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            //_cinemachineTargetPitch = 50f;
            //_cinemachineTargetYaw = 0f;
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);

            //// Góc cố định kiểu Duckov
            //_cinemachineTargetPitch = 50f;
            //_cinemachineTargetYaw = 0f;

            //CinemachineCameraTarget.transform.rotation =
            //    Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }

        private void Move()
        {
            float effectiveSpeed = SpeedSource.EffectiveSpeed;
            MoveSpeed = effectiveSpeed;
            SprintSpeed = effectiveSpeed * 2.0f;
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Xoay theo con trỏ chuột
            if (TryGetAimDirection(out Vector3 aimDir))
            {
                _targetRotation = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;

                float rotation = _rotationSpeed <= 0f
                    ? _targetRotation
                    : Mathf.MoveTowardsAngle(transform.eulerAngles.y, _targetRotation, _rotationSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }

            // Di chuyển theo hướng camera, độc lập với hướng nhìn
            Vector3 inputDirection = GetCameraRelativeMoveDirection();

            _controller.Move(inputDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        private void AimAndAttack()
        {
            var actions = _playerInput.actions;

            if (actions["Aim"].IsPressed())
                Bus<Aim>.Raise(new Aim(true));

            if(actions["Aim"].WasReleasedThisFrame())
                Bus<Aim>.Raise(new Aim(false));

            if (actions["Attack"].IsPressed())
                Bus<ShootTriggered>.Raise(new ShootTriggered());
        }
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        private bool TryGetMouseWorldPointPhysics(out Vector3 point)
        {
            point = default;
            if (Mouse.current == null || _mainCamera == null) return false;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            UpdateCursorCrosshair(mousePos);
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f, _groundMask, QueryTriggerInteraction.Ignore))
            {
                point = hit.point;
                return true;
            }

            return false;
        }

        // Hướng nhìn (đã bỏ trục Y) từ player tới con trỏ chuột
        private bool TryGetAimDirection(out Vector3 direction)
        {
            direction = default;
            if (!TryGetMouseWorldPointPhysics(out Vector3 point)) return false;

            direction = point - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < _aimDeadZone * _aimDeadZone) return false;

            direction.Normalize();
            return true;
        }
        private Vector3 GetCameraRelativeMoveDirection()
        {
            if (_input.move == Vector2.zero || _mainCamera == null) return Vector3.zero;

            Transform cam = _mainCamera.transform;

            // Chiếu hướng camera xuống mặt phẳng ngang
            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
            Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up);

            // Phòng trường hợp camera nhìn thẳng xuống đất (forward gần như song song trục Y)
            if (camForward.sqrMagnitude < 0.0001f)
                camForward = Vector3.ProjectOnPlane(cam.up, Vector3.up);

            camForward.Normalize();
            camRight.Normalize();

            return (camForward * _input.move.y + camRight * _input.move.x).normalized;
        }
        private void UpdateCursorCrosshair(Vector2 mousePos)
        {
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _crosshair.position = mousePos;
            }
            else
            {
                // Screen Space - Camera / World Space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)_canvas.transform,
                    mousePos,
                    _canvas.worldCamera,
                    out Vector2 localPoint);

                _crosshair.localPosition = localPoint;
            }
        }
        private void HandleSpeedChange(SpeedChange args)
        {
            if (args._character.gameObject == this.gameObject)
            {
                MoveSpeed = args._character.GetCurrentSpeed();
                SprintSpeed = MoveSpeed * 2.0f;
            }
        }
    }
}