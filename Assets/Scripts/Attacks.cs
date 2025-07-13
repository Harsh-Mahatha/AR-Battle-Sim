using UnityEngine;

public class Attacks : MonoBehaviour
{
    public PlayerInputActions inputActions;
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
        if (inputActions.Player.Attack.triggered)
        {
            OnAttack();
        }
        if (inputActions.Player.Super.triggered)
        {
            OnSuper();
        }
    }

    private void OnAttack()
    {
        anim.SetTrigger("Attacking");
        //GetComponent<PunchSpawner>().SpawnPunch();
        Debug.Log("Attack button pressed");
    }

    private void OnSuper()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        anim.SetTrigger("Super");
        Debug.Log("Super button pressed");
    }

    private void OnSuperEnd()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        Debug.Log("Super ended");
    }   
}
