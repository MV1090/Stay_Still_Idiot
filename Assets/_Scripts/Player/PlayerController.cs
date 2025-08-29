using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerController : NetworkBehaviour
{
    PlayerInput playerInput;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isSprinting;
    
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

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
    private bool _canJump = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Tooltip("Transform attached to the player for the camera to face")]
    public Transform CameraFollow;

    private GameObject _mainCamera;

    // player
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;


    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;


    private Rigidbody rb;
    private CharacterController cc;
    private PlayerRole pr;

    private const float _threshold = 0.01f;

    GameObject playerObj;

    void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        pr = GetComponent<PlayerRole>();
        Debug.Log($"PlayerController Awake - IsOwner: {IsOwner}, OwnerClientId: {NetworkObject.OwnerClientId}");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //enabled = IsClient;
        Init();
    }
    private void Init()
    {
        if (!IsOwner)
            return;

        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        AddCameraToPlayer();
    }
    private void Start()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnStopMove;
        playerInput.Player.Jump.performed += OnJump;
        playerInput.Player.Sprint.performed += OnSprint;
        playerInput.Player.FirstAction.performed += OnFirstActionInput;
        playerInput.Player.SecondAction.performed += OnSecondActionInput;
        playerInput.Player.ThirdAction.performed += OnThirdActionInput;
        playerInput.Player.FourthAction.performed += OnFourthActionInput;
    }
    void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnStopMove;
        playerInput.Player.Jump.performed -= OnJump;
        playerInput.Player.Sprint.performed -= OnSprint;
        playerInput.Player.FirstAction.performed -= OnFirstActionInput;
        playerInput.Player.SecondAction.performed -= OnSecondActionInput;
        playerInput.Player.ThirdAction.performed -= OnThirdActionInput;
        playerInput.Player.FourthAction.performed -= OnFourthActionInput;
    }
    private void OnJump(InputAction.CallbackContext ctx)
    {
        _canJump = true;
    }
    private void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
               transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
        QueryTriggerInteraction.Ignore);
    }
    private void Jump()
    {
        if (Grounded)
        {
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_canJump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                Debug.Log("Player Jump");
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

            // if we are not grounded, do not jump
            _canJump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private void AddCameraToPlayer()
    {
        var virtualCam = Object.FindAnyObjectByType<CinemachineCamera>();

        if (virtualCam != null && CameraFollow != null)
        {
            virtualCam.Follow = CameraFollow;
            virtualCam.LookAt = CameraFollow;
        }
        else
        {
            Debug.LogWarning("Virtual Camera or Follow Target is missing.");
        }
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _isSprinting ? SprintSpeed : MoveSpeed;

        if (_moveInput == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(cc.velocity.x, 0.0f, cc.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * _moveInput.magnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        cc.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
    public void OnStopMove(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }
    private void OnSprint(InputAction.CallbackContext ctx)
    {
        _isSprinting = ctx.ReadValueAsButton();

        if (_isSprinting)
            Debug.Log("pressed");
        else
            Debug.Log("Released");
    }
    private void OnFirstActionInput(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        bool _IsPressed = ctx.ReadValueAsButton();

        pr.PlayFirstActionLocalFeedback(_IsPressed);
        pr.RequestFirstActionServerRpc(_IsPressed);        
    }
    private void OnSecondActionInput(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        bool _IsPressed = ctx.ReadValueAsButton();

        pr.PlaySecondActionLocalFeedback(_IsPressed);
        pr.RequestSecondActionServerRpc(_IsPressed);        
    }
    private void OnThirdActionInput(InputAction.CallbackContext ctx)
    {       
        if (!IsOwner) return;

        bool _IsPressed = ctx.ReadValueAsButton();
                
        pr.PlayThirdActionLocalFeedback(_IsPressed);
        pr.RequestThirdActionServerRpc(_IsPressed);
    }

    private void OnFourthActionInput(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        bool _IsPressed = ctx.ReadValueAsButton();

        pr.PlayFourthActionLocalFeedback(_IsPressed);
        pr.RequestFourthActionServerRpc(_IsPressed);        
    }   
    
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Move();
        GroundCheck();
        Jump();
    }      
    //private void OnDrawGizmosSelected()
    //{
    //    Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
    //    Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

    //    if (Grounded) Gizmos.color = transparentGreen;
    //    else Gizmos.color = transparentRed;

    //    // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
    //    Gizmos.DrawSphere(
    //        new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
    //        GroundedRadius);
    //}
}
