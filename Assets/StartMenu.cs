using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class StartMenu : MonoBehaviour
{
    public GameObject eventSystem;

    protected float tick = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameObject.Find("EventSystem") == null)
        {
            GameObject e = Instantiate(eventSystem);
            e.name = "EventSystem";
        }
    }

    // Update is called once per frame
    void Update()
    {
        tick -= Time.deltaTime;
        if (Keyboard.current.anyKey.isPressed && !SceneLoader.isLoadingScene && tick<=0f)
        {
            if (Keyboard.current.escapeKey.isPressed)
            {
                PlayerPrefs.SetInt("GamePassed", 0);
                Application.Quit();
            }
            else
            {
                StartCoroutine(SceneLoader.instance.LoadScene("MainScene", Color.black));
            }
        }
    }
}
