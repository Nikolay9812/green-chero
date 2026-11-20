using UnityEngine;
using UnityEngine.InputSystem;

// Archero-style movement: move in world directions (W = up the screen),
// turn smoothly to face the direction of travel, and stay on the ground.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Movement tuning (editable in Inspector)
    public float speed = 5.0f;
    public float turnSpeed = 720f; // degrees per second

    public float gravity = -20f;

    // Drag the "Player/Move" action from GameInputAction here in the Inspector
    public InputActionReference moveAction;

    // On-screen joystick for phones. If left empty, it is found automaticly
    public FloatingJoystick joystick;

    // True while the player is walking. Later the auto-shoot will only fire when this is false.
    public bool IsMoving { get; private set; }

    // How hard the stick is pushed: 0 = standing, 1 = full speed. The animation script reads this.
    public float MoveAmount { get; private set; }

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (joystick == null) joystick = FindAnyObjectByType<FloatingJoystick>();

        /* speed += Upgrades.BonusMoveSpeed; // permanent talent from the Collectible page
        speed += Inventory.Bonus(GearStat.MoveSpeed); // worn gear
        speed *= Artifacts.Multiplier(ArtifactStat.MoveSpeedPercent); // found artifacts */
    }

    void OnEnable()
    {
        // Start reading input
        if (moveAction != null) moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();

        // When the character dies this script is turned off, so make shure he dosen't keep "walking"
        IsMoving = false;
        MoveAmount= 0f;
    }

    void Update()
    {
        // x = left/right, y = forward/back
        Vector2 input = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        // the joystick wins while a finger (or the mouse) is draggin it
        if (joystick != null && joystick.Direction != Vector2.zero) input = joystick.Direction;

        // World-space direction on the ground (XZ plane). Clam so diagonals aren't faster.
        Vector3 direction = Vector3.ClampMagnitude(new Vector3(input.x, 0f, input.y), 1f);
        IsMoving = direction.sqrMagnitude > 0.01f;
        MoveAmount = IsMoving ? direction.magnitude : 0f;

        // turn to face where we're walking
        if (IsMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Simple gravity so the player sticks to the flor
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    // Puts the player somewhere else instantly (used when a new room is loaded).
    // A CharacterController ignores position changes while it's on, so switch it off for a moment.
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
        verticalVelocity = 0f;
    }
}