using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerName, enemyName;

    void Start()
    {
        if (photonView.IsMine)
        {
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<Attacks>().enabled = true;
            transform.Find("Player Canvas").gameObject.SetActive(true);

            SetPlayerName("You", Color.blue);
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Attacks>().enabled = false;
            transform.Find("Player Canvas").gameObject.SetActive(false);

            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }

    void SetPlayerName(string name, Color color)
    {
        playerName.text = name;
        playerName.color = color;
    }

    public void SetEnemyName(string name, Color color)
    {
        enemyName.text = name;
        enemyName.color = color;
    }
}
