using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    string sceneTobeLoaded;

    private void Awake()
    {
        if (Instance == null) // Singleton pattern to ensure only one instance exists
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        sceneTobeLoaded = sceneName;
        StartCoroutine(BeginLoadScene());

    }

    private IEnumerator BeginLoadScene()
    {
        yield return SceneManager.LoadSceneAsync("Loading Screen");
        StartCoroutine(LoadActualScene());
    }
    
    private IEnumerator LoadActualScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTobeLoaded);
        asyncLoad.allowSceneActivation = false; 
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // Check if the scene is almost loaded
            {
                asyncLoad.allowSceneActivation = true; 
            }
            yield return null;
        }
    }
}
