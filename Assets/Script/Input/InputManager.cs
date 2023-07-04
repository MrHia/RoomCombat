using UnityEngine;
using UnityEngine.InputSystem;

namespace RoomCombat {
    public class InputManager : MonoBehaviour {
        [SerializeField] private InputAction playerMove;
        [SerializeField] private InputAction playerAttack;
        [SerializeField] private InputAction playerDash;
        Vector2 movementInput = Vector2.zero;
        public InputSystem inputSystem;
        public float verticalInput;
        public float horizontalInput;

        private void Awake() {

            inputSystem = new InputSystem();
        }
        private void OnEnable() {
            playerMove = inputSystem.PlayerInput.Move;
            playerMove.Enable();
            playerMove.performed += i => movementInput = i.ReadValue<Vector2>();
            playerAttack = inputSystem.PlayerInput.Attack;
            playerAttack.Enable();
            //playerAttack.performed += Attack;
            playerDash = inputSystem.PlayerInput.Dash;
            playerDash.Enable();
            //playerDash.performed += Dash;
        }
        private void OnDisable() {
            playerMove.Disable();
            playerAttack.Disable();
            playerDash.Disable();
        }
        private void Update() {
            //movementInput = playerMove.ReadValue<Vector2>();
            //moveDirectionText.text = $"move Direction: {moveDirection}";
            Vector2 leftStickValue = Gamepad.current.leftStick.ReadValue();
            Debug.Log(": " + leftStickValue);
        }
/*        private void Attack(InputAction.CallbackContext context) {
            //ActionText.text = $"Action: attack";
            Debug.Log("Attack");
        }
        private void Dash(InputAction.CallbackContext context) {
            //ActionText.text = $"Action: Dash";
            Debug.Log("Dash");
        }*/
        public void handleMovementInput() {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
        }

    }
}