using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public GameObject[] pictures;
    public Animator animator;
    public string[] animationStateNames;
    public PictureStateSpawner pictureStateSpawner;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < pictures.Length; i++)
        {
            if (pictureStateSpawner.GetIsActive(i))
            {
                animator.Play(animationStateNames[i]);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < pictures.Length; i++)
        {
            pictureStateSpawner.SaveData(i,pictures[i].activeSelf);
        }
    }
}
