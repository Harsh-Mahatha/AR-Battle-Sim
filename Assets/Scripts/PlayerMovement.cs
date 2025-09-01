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

   public void AssignEnemy()
    {
        if (photonView.IsMine)
        {
            foreach (var player in FindObjectsOfType<PlayerMovement>())
            {
                if (player != this)
                {
                    enemyTransform = player.transform;
                    player.enemyTransform = this.transform;

                    var mySetup = GetComponent<PlayerSetup>();
                    var enemySetup = player.GetComponent<PlayerSetup>();

                    if (mySetup != null && enemySetup != null)
                    {
                        mySetup.SetEnemyName(player.photonView.Owner.NickName, Color.red);
                        enemySetup.SetEnemyName(photonView.Owner.NickName, Color.blue);
                    }
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
