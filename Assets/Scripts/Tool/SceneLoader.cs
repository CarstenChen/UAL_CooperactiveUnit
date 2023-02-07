using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { get; private set; }
    public Animator sceneFadeAnimator;
    public PlayerController player;

    private void Awake()
    {
        if(instance ==null)
        instance = this;
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

    public IEnumerator LoadScene(string name)
    {
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
