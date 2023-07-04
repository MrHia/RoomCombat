using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class Test : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI horizontalText;
    [SerializeField] private TextMeshProUGUI verticalText;
    [SerializeField] private TextMeshProUGUI ActionText;
    [SerializeField] private TextMeshProUGUI moveDirectionText;
    [SerializeField] public InputAction playerMove;
    [SerializeField] public InputAction playerAttack;
    [SerializeField] public InputAction playerDash;
    Vector2 moveDirection = Vector2.zero;
    public InputSystem inputSystem;

    private void Awake() {

        inputSystem = new InputSystem();
    }
    private void OnEnable() {
        playerMove = inputSystem.PlayerInput.Move;
        playerMove.Enable();
        playerMove.performed += i => moveDirection = i.ReadValue<Vector2>();
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
    private void Update() {
/*        moveDirection = playerMove.ReadValue<Vector2>();*/
        moveDirectionText.text = $"move Direction: {moveDirection}";

    }
    private void Attack(InputAction.CallbackContext context) {
        ActionText.text = $"Action: attack";
        Debug.Log("Attack");
    }
    private void Dash(InputAction.CallbackContext context) {
        ActionText.text = $"Action: Dash";
        Debug.Log("Dash");
    }
}
