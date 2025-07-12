using JetBrains.Annotations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
<<<<<<< Updated upstream
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
=======
    public float moveSpeed = 5f;
    public PlayerInputActions inputActions;
    public GameObject Enemy;

    public Animator anim;

    private void OnEnable()
>>>>>>> Stashed changes
    {
        
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        
=======
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        transform.LookAt(Enemy.transform.position);

        anim.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);

>>>>>>> Stashed changes
    }
}
