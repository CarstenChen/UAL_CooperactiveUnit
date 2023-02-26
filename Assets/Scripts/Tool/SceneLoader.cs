using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { get; private set; }
    public Animator sceneFadeAnimator;
    public PlayerController player;
    public GameObject eventSystem;
    public static bool isLoadingScene;

    private void Awake()
    {
        if(instance ==null)
        instance = this;
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject e = Instantiate(eventSystem);
            e.name = "EventSystem";
        }

    }
    // Start is called before the first frame update
    void Start()
    {




        GameObject.DontDestroyOnLoad(GameObject.Find("SceneLoaderCanvas"));
        GameObject.DontDestroyOnLoad(GameObject.Find("EventSystem"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadScene(string name, Color fadeColor)
    {
        isLoadingScene = true;

        GameObject.Find("SceneLoaderCanvas").GetComponentInChildren<Image>().color = fadeColor;

        sceneFadeAnimator.ResetTrigger("FadeIn");

        if (sceneFadeAnimator != null)
        {
            sceneFadeAnimator.SetTrigger("FadeIn");
        }

        yield return new WaitForSeconds(1f);
        

        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.completed += OnLoadedScene;
    }

    public void LoadFirstScene(int levelID)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(string.Format("Level{0}", levelID));
        async.completed += OnLoadedScene;
    }


    public void OnLoadedScene(AsyncOperation obj)
    {
        sceneFadeAnimator.ResetTrigger("FadeOut");

        if (sceneFadeAnimator != null)
        {
            sceneFadeAnimator.SetTrigger("FadeOut");
        }

        isLoadingScene = false;
    }

    public IEnumerator QuitLevel()
    {
        sceneFadeAnimator.ResetTrigger("FadeIn");

        if (sceneFadeAnimator != null)
        {
            sceneFadeAnimator.SetTrigger("FadeIn");
            //sceneFadeAnimator.SetBool("FadeOut", false);
        }


        yield return new WaitForSeconds(1f);

        AsyncOperation async = SceneManager.LoadSceneAsync(string.Format("StartScene"));
        async.completed += OnLoadedScene;
    }
}
