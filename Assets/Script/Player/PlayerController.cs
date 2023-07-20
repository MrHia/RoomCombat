using UnityEngine;
using UnityEngine.InputSystem;

namespace RoomCombat {
    public class PlayerController : Character {
        //player stats
        Collider m_Collider;
        InputSystem inputActions;
        //State
        enum PlayerState {
            Idle,
            Moving,
            Attacking,
            Dashing,
            Dead,
            Hurt
        }
        [SerializeField] private PlayerState playerState = PlayerState.Idle;
        // Animation
        private Animator animator;
        public Vector3 rotationChildVfx;
        private Transform childVifxPlayer;
        // Moving
        public float horizontal;
        public float vertical;
        private Transform cameraObject;
        private Rigidbody playerRigidbody;
        [SerializeField] private Vector3 moveDirection;
        private float moveAmount;
        public float rotationSpeed;
        public UltimateJoystick joystick;
        //dash
        private float dashDuration = 1f;
        private float dashTimer = 0;
        public float dashSpeed;
        //Attack
        private float attackTimer = 0;
        public Transform attackPos;
        //Dead
        public bool isGround;
        //hurt
        public float cooldowntHurt = 1.1f;
        public float hurtTimer = 0;
        //input
        InputSystem inputSystem;
        [SerializeField] private InputAction playerMove;
        [SerializeField] private InputAction playerAttack;
        [SerializeField] private InputAction playerDash;
        Vector2 movementInput = Vector2.zero;
        public float verticalInput;
        public float horizontalInput;
        private void Awake() {

            inputSystem = new InputSystem();
        }
        private void Start() {
            playerRigidbody = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            childVifxPlayer = transform.GetChild(0);
            cameraObject = Camera.main.transform;
            m_Collider = GetComponent<Collider>();
            if (GameObject.FindGameObjectWithTag("JoyStick") != null) {
                joystick = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<UltimateJoystick>();
            }
        }
        private void OnEnable() {
            playerMove = inputSystem.PlayerInput.Move;
            playerMove.Enable();
            playerMove.performed += i => movementInput = i.ReadValue<Vector2>();
            playerAttack = inputSystem.PlayerInput.Attack;
            playerAttack.Enable();
            playerAttack.performed += Attack;
            playerDash = inputSystem.PlayerInput.Dash;
            playerDash.Enable();
            playerDash.performed += Dash;
        }
        private void OnDisable() {
            playerMove.Disable();
            playerAttack.Disable();
            playerDash.Disable();
        }

        public override void Move() {
            if (playerState == PlayerState.Dead) {
                return;
            }
            if (playerState == PlayerState.Attacking) {
                return;
            }
            moveDirection = cameraObject.forward * vertical;
            moveDirection = moveDirection + cameraObject.right * horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;
            moveDirection = moveDirection * speed;
            Vector3 movementVelocity = moveDirection;
            playerRigidbody.velocity = movementVelocity;
        }
        public void Rotate() {
            if (playerState == PlayerState.Dead) {
                return;
            }
            Vector3 targetDirection = Vector3.zero;
            targetDirection = cameraObject.forward * vertical;
            targetDirection = targetDirection + cameraObject.right * horizontal;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero) {
                targetDirection = transform.forward;
            }
            Quaternion targetRotate = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotate = Quaternion.Slerp(transform.rotation, targetRotate, rotationSpeed * Time.deltaTime);
            transform.rotation = playerRotate;
        }

        private void Update() {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            if (joystick != null && (joystick.HorizontalAxis != 0 || joystick.VerticalAxis != 0)) {
                horizontal = joystick.HorizontalAxis;
                vertical = joystick.VerticalAxis;
            }
            /*            horizontal = joystick.HorizontalAxis;
                        vertical = joystick.VerticalAxis;*/
            if (!isGround && this.transform.position.y < -0.2 || this.transform.position.y < -0.2) {
                playerState = PlayerState.Dead;
            }
            /*            Debug.Log(this.transform.position.y);*/
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            switch (playerState) {
                case PlayerState.Idle:
                    animator.SetBool("isHurt", false); 
                    Rotate();
                    /*rotationChildVfx = new Vector3(0, 0, 0);*/
                    animator.SetBool("isRun", false);
                    if (moveAmount != 0) {
                        playerState = PlayerState.Moving;
                    }
                    break;

                case PlayerState.Moving:
                    animator.SetBool("isHurt", false);
                    Rotate();
                    /*rotationChildVfx = new Vector3(0, 20f, 0);*/
                    Move();
                    animator.SetBool("isRun", true);
                    if (moveAmount == 0) {
                        playerState = PlayerState.Idle;
                    }
                    break;
                case PlayerState.Attacking:
                    animator.SetBool("isHurt", false);
                    /*rotationChildVfx = new Vector3(0, -20, 0);*/
                    if (attackTimer <= 0f) {
                        playerState = PlayerState.Idle;
                    }
                    else {
                        attackTimer -= Time.deltaTime;
                        animator.SetBool("isRun", false);
                    }
                    break;
                case PlayerState.Dashing:
                    animator.SetBool("isHurt", false);
                    /*rotationChildVfx = new Vector3(0, 20f, 0);*/
                    if (dashTimer <= 0.5f) {
                        playerState = PlayerState.Idle;
                    }
                    else {
                        dashTimer -= Time.deltaTime;

                        animator.SetBool("isRun", false);
                    }
                    break;
                case PlayerState.Hurt:
                    Hurt();
                    break;
                case PlayerState.Dead:
                    animator.SetBool("isHurt", false);
                    animator.SetBool("isRun", false);
                    FindObjectOfType<UIManager>().LoseGame();
                    break;
                

            }
        }
        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Ground")) {
                playerRigidbody.mass = 1f;
                isGround = true;
            }

        }
        private void OnCollisionExit(Collision collision) {
            if (collision.gameObject.CompareTag("Ground")) {
                playerRigidbody.mass = 10f;
                isGround = false;

            }
        }
        public void Attack(InputAction.CallbackContext context) {
            if (playerState == PlayerState.Dead) {
                return;
            }
            if (playerState != PlayerState.Attacking && playerState != PlayerState.Dashing) {
                playerRigidbody.velocity = Vector3.zero;
                playerState = PlayerState.Attacking;
                FindObjectOfType<soundManager>().Play("Sword");
                attackTimer = cooldownAttack;
                animator.SetTrigger("Attack");
            }
        }
        public void Dash(InputAction.CallbackContext context) {
            if (playerState == PlayerState.Dead) {
                return;
            }
            Quaternion currentRotation = transform.rotation;
            Vector3 dashDirection = currentRotation * Vector3.forward;
            dashDirection.Normalize();
            dashDirection.y = 0;
            if (playerState != PlayerState.Attacking && playerState != PlayerState.Dashing) {
                playerRigidbody.AddForce(dashDirection.normalized * dashSpeed, ForceMode.Impulse);
                playerState = PlayerState.Dashing;
                dashTimer = dashDuration;
                animator.SetTrigger("Dash");
            }
        }
        public void HurtStart() {
            playerState = PlayerState.Hurt;
        }
        public void Hurt() {
            Debug.Log("Hurt player");
            if (hurtTimer >= cooldowntHurt) {
                hurtTimer = 0f;
                playerState = PlayerState.Idle;
                animator.SetBool("isHurt", false);
            }
            else {
                animator.SetBool("isRun", false);
                animator.SetBool("isHurt", true);
                hurtTimer += Time.deltaTime;
            }
        }
        public override void Dead() {
            FindObjectOfType<UIManager>().LoseGame();
            Destroy(gameObject);
        }

    }
}
