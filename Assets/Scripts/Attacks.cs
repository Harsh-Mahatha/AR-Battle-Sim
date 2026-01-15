using UnityEngine;
using Photon.Pun;

public class Attacks : MonoBehaviourPunCallbacks
{
    public PlayerInputActions inputActions;
    public Animator anim;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

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
        if (inputActions.Player.Attack.triggered)
        {
            OnAttack();
        }
        if (inputActions.Player.Super.triggered)
        {
            OnSuper();
        }
        if (inputActions.Player.TakeDamage.triggered)
        {
            OnDamage();
        }
    }

    private void OnAttack()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCOnAttack", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPCOnAttack()
    {
        anim.SetTrigger("Attacking");
        GetComponent<PunchSpawner>().SpawnPunch();
        Debug.Log("Attack synchronized");
    }

    private void OnSuper()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCOnSuper", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPCOnSuper()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        anim.SetTrigger("Super");
        Debug.Log("Super synchronized");
    }

    private void OnDamage()
    {
        if (photonView.IsMine)
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            HealthManager healthManager = GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(20);
                Debug.Log("Took Damage");
            }
        }
    }

    public void OnSuperEnd()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        Debug.Log("Super ended");
    }   

     public void OnDamageEnd()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        Debug.Log("Damage Taken");
    }   
}
