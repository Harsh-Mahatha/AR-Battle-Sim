using UnityEngine;

public class ExitScript : MonoBehaviour
{
    void Update()
    {
         if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                GoToMainMenu();
            }
        }
    }
    public void GoToMainMenu()
    {
                if (SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.LoadScene("Home Screen");
                    Debug.Log("Loading Main Menu Scene");
                }
                else
                {
                    if (Photon.Pun.PhotonNetwork.InRoom)
                    {
                        Photon.Pun.PhotonNetwork.LeaveRoom();
                        Debug.Log("Left Photon room; client will return to Master server.");
                    }
                }    
    }  
}
