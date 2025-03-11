using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // New Grapple Control Flag
        [Header("Grapple Control")]
        [Tooltip("When true, disables movement input (e.g., while grappling)")]
        public bool isGrappled = false;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        public float _speed;
        private float _rotationVelocity;
        public float _verticalVelocity;
        public float _terminalVelocity = 53.0f;

        [Header("Double Jump")]
        public bool _canDoubleJump;
        public bool hasJumped = false; // Flag to check if the player has normal jumped
        public bool hasDoubleJumped = false;  // Flag to check if the player has double jumped
        private float _doubleJumpDelayTimer = 0.0f;  // Timer for the delay before double jump is allowed
        private const float DoubleJumpDelay = 0.1f;  // The delay (0.1 seconds) before allowing the double jump

        // timeout deltatime
        public float _jumpTimeoutDelta;
        public float _fallTimeoutDelta;

        [Header("Sprint")]
        public bool isSprinting = false;
        private float sprintPressTime = 0f; // Timer to track sprint input duration
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        public float SprintStrafeFactor = 0.5f;
        public float AirSprintSpeed = 0.25f; // Speed multiplier for sprinting mid-air
        [Header("SprintAnimationFix")]
        public Animator animator; // Reference to the Animator
        [Header("SpeedLines")]
        public GameObject speedLines;
        public float speedLinesScaleSprint = 1.2f;
        public float speedLinesScaleDash = 1f;
        public float speedLinesLerpSpeed = 10f;
        private Vector3 originalSpeedLinesScale;
        [Header("Dash")]
        public bool isDashing = false;     // Flag for dashing
        public float DashAmount = 25;
        private float lastDashTime = -Mathf.Infinity; // Stores the time of the last dash
        public float DashCooldown = 1.0f; // Time (in seconds) before the player can dash again




#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.0001f;

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
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            if (speedLines != null)
            {
                originalSpeedLinesScale = speedLines.transform.localScale;
            }
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                // Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                // CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                // (Update to allow camera tilt:
                Vector3 newRotation = CinemachineCameraTarget.transform.localEulerAngles;
                newRotation.x = _cinemachineTargetPitch; // Only update X
                CinemachineCameraTarget.transform.localEulerAngles = newRotation;

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        /*private void Move()
        {
            // Disable movement input if the player is grappled
            if (isGrappled)
            {
                return;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }*/

        //Reworked Sprint with additional functionality:
        private void Move()
        {
            // Disable movement input if the player is grappled
            if (isGrappled)
            {
                return;
            }

            // Detect sprint input duration
            if (_input.sprint)
            {
                sprintPressTime += Time.deltaTime; // Increment timer while sprint is held
            }
            else
            {
                if (sprintPressTime > 0 && sprintPressTime < 0.3f)
                {
                    // Dash occurs if sprint was pressed for less than 0.3 seconds and cooldown is over
                    if (Time.time >= lastDashTime + DashCooldown)
                    {
                        StartDash();
                    }
                }
                sprintPressTime = 0f; // Reset timer when sprint input is released
            }

            // Sprinting requires sprint input to be held long enough AND the player must be pressing forward
            if (!isDashing && _input.sprint && sprintPressTime >= 0.3f && _input.move.y > 0)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false; // Disable sprinting if not meeting conditions
            }

            // Update Animator to reflect sprinting state
            if (animator != null)
            {
                animator.SetBool("currentlySprinting", isSprinting);

                // Adjust animation speed while sprinting in the air
                if (isSprinting)
                {
                    animator.speed = Grounded ? 1f : 0.1f; // Ensures animation plays at 0.1 speed while airborne.
                }
                else
                {
                    animator.speed = 1f; // Reset to default when not sprinting
                }
            }

            // Check if shoot input is pressed and stop sprinting if it is
            if (_input.shoot)
            {
                isSprinting = false;
            }

            // Set target speed based on sprinting, dashing, or normal movement
            float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;

            if (!Grounded && isSprinting)
            {
                targetSpeed *= AirSprintSpeed; // Reduce sprint speed in air
            }

            if (isDashing)
            {
                targetSpeed = DashAmount; // Use dash speed instead
            }

            // If no movement input, set target speed to 0 (EXCEPT WHEN SPRINTING OR DASHING)
            if (_input.move == Vector2.zero && !isSprinting && !isDashing)
            {
                targetSpeed = 0.0f;
            }

            // Current horizontal speed reference
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // Normalized input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            if (isSprinting)
            {
                // Always move forward while sprinting, relative to player's orientation
                inputDirection = transform.forward;

                // Reduce sideways movement while sprinting
                inputDirection += transform.right * (_input.move.x * SprintStrafeFactor);
            }
            else if (isDashing)
            {
                // Move in the direction of input.move with a dash
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // Move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Update speed lines scaling
            UpdateSpeedLines();
        }

        // Start Dash Function
        private void StartDash()
        {
            if (!isDashing) // Prevent repeated dashes
            {
                isDashing = true;
                lastDashTime = Time.time; // Record the time of the dash
                Invoke(nameof(StopDash), 0.15f); // Dash lasts for 0.15 seconds
            }
        }

        // Stop Dash Function
        private void StopDash()
        {
            isDashing = false;
        }

        //Speedlines Stuffz
        private void UpdateSpeedLines()
        {
            if (speedLines != null)
            {
                Vector3 targetScale = originalSpeedLinesScale;

                if (isSprinting)
                {
                    targetScale *= speedLinesScaleSprint;
                }
                else if (isDashing)
                {
                    targetScale *= speedLinesScaleDash;
                }

                // Lerp speed lines to the target scale
                speedLines.transform.localScale = Vector3.Lerp(speedLines.transform.localScale, targetScale, Time.deltaTime * speedLinesLerpSpeed);
            }
        }

        /*private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }*/

        //JumpAndGravity but with Double Jump Functionality:
        private void JumpAndGravity()
        {
            // Reset variables when grounded
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                hasJumped = false;  // Reset jump flag when grounded
                hasDoubleJumped = false;  // Reset double jump flag when grounded
                _doubleJumpDelayTimer = 0.0f;  // Reset the double jump delay timer

                // Reset vertical velocity when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Regular jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && Grounded)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);  // Apply jump force
                    hasJumped = true;  // Set hasJumped to true after a standard jump
                    _doubleJumpDelayTimer = DoubleJumpDelay;  // Start the delay timer before allowing double jump
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // In the air
                if (_doubleJumpDelayTimer <= 0.0f && _input.jump && hasJumped && !hasDoubleJumped)  // If delay has passed and the player has jumped
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);  // Apply double jump force
                    hasDoubleJumped = true;  // Set hasDoubleJumped to true after the second jump
                    _canDoubleJump = false;  // Disable further double jumps
                    hasJumped = false;  // Just to reset the jump flag to avoid loop
                }

                // Decrease the double jump delay timer when in the air
                if (_doubleJumpDelayTimer > 0.0f)
                {
                    _doubleJumpDelayTimer -= Time.deltaTime;
                }

                // Set _jumpTimeoutDelta when in the air for the second jump
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                _input.jump = false;  // Reset jump input to prevent spamming
            }

            // Apply gravity to the player
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
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

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
