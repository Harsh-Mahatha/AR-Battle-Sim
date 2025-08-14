using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public PlayerInputActions inputActions;
    //public Transform enemyTransform; 

    public Animator anim;

    private void OnEnable()
    {
        if (inputActions == null)   
            inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        //transform.LookAt(enemyTransform.position);

        anim.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);

    }
}
