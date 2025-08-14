using UnityEngine;
using Photon.Pun;
using TMPro;
public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerName, enemyName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (photonView.IsMine)
        {
            gameObject.GetComponent<PlayerMovement>().enabled = true;
            transform.Find("Player Canvas").gameObject.SetActive(true); // Activate the child object (e.g., player model)
        }
        else
        {
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            transform.Find("Player Canvas").gameObject.SetActive(false); // Deactivate the child object for other players
        }

    }

    void SetPlayerNames()
    {
        if (photonView.IsMine)
        {
            playerName.text = "You";
            playerName.color = Color.blue;
        }
        else
        {
            enemyName.text = photonView.Owner.NickName;
            enemyName.color = Color.red;
        }
    }
}
