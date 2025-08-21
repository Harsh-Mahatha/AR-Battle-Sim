using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public PlayerInputActions inputActions;
    public Transform enemyTransform;
    public Animator anim;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            if (inputActions == null)
                inputActions = new PlayerInputActions();
            inputActions.Enable();
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine && inputActions != null)
            inputActions.Disable();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (var player in FindObjectsOfType<PlayerMovement>())
            {
                if (player != this) 
                {
                    enemyTransform = player.transform;
                    player.enemyTransform = this.transform; // also assign me as enemy for them
                }
            }
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return; 

        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        if (enemyTransform != null)
        {
            transform.LookAt(enemyTransform.position);
        }

        anim.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);
    }
}
