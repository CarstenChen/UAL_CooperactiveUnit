using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAnimStateChange : MonoBehaviour
{
    public GameObject[] pictures;
    public static PortalAnimStateChange Instance;
    public int animCount;
    public bool isPlay;
    Animator anim;
    public PictureStateSpawner pictureStateSpawner;
    private void Awake()
    {

        Instance = this;

        anim = GetComponent<Animator>();
    }

    void Start()
    {
        for (int i = 0; i < pictures.Length; i++)
        {
            if (pictureStateSpawner.GetIsActive(i))
            {
                animCount++;
            }
        }
    }

    void Update()
    {
        if (isPlay)
            StartCoroutine(PortalChangeEffect(4.833f));
    }


    IEnumerator PortalChangeEffect(float delay)
    {
        isPlay = false;
        yield return new WaitForSeconds(delay);
        switch (animCount)
        {
            case 1:
                anim.Play("Portal_PictureAppear");
                break;
            case 2:
                anim.Play("Portal_Transfer0-1");
                break;
            case 3:
                anim.Play("Portal_Transfer1-2");
                break;
            case 4:
                anim.Play("Portal_Transfer2-3");
                break;
            default:
                break;
        }

    }
}
