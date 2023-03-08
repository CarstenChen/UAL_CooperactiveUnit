using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshDissolve : MonoBehaviour
{
    public float minYOffset;
    public float maxYOffset;
    protected Material dissolveMtl;
    protected float currentYOffset;
    public int completKeyNum;

    protected bool canPress;
    protected float completeRate;
    public float currentKeyNum;
    protected float tick;
    protected float currentCoolDown;
    protected float coolDown = 0.5f;

    protected bool finished;
    // Start is called before the first frame update

    private void OnEnable()
    {
        dissolveMtl = GetComponent<MeshRenderer>().material;
        currentYOffset = minYOffset;

        if (AIDirector.Instance != null)
        {
            if(AIDirector.Instance.isInBodyChange)
                StartCoroutine(DealWithMeshDissolveOnAutomaticWriting());
            else
            {
                StartCoroutine(DealWithMeshDissolve());

            }
        }
        else
        {
            StartCoroutine(DealWithMeshDissolve());
        }

    }

    private void Update()
    {
        if (!finished)
        {
            DealWithAutoWriting();
            completeRate = Mathf.Clamp(currentKeyNum / completKeyNum, 0f, 1f);
        }


    }

    IEnumerator DealWithMeshDissolveOnAutomaticWriting()
    {
        yield return null;

        if (completeRate < 1 || currentYOffset < maxYOffset)
        {
            currentYOffset = Mathf.Lerp(currentYOffset, minYOffset + 2f * maxYOffset * completeRate, 0.02f);
            if (maxYOffset - currentYOffset <= 0.01f)
            {
                currentYOffset = maxYOffset;
            }

            Vector3 offset = new Vector3(0, currentYOffset, 0);
            dissolveMtl.SetVector("_DissolveOffset", offset);

            StartCoroutine(DealWithMeshDissolveOnAutomaticWriting());
        }
        else
        {
            //SoundManager.Instance.PlayBodyChangeSound();
            PlayerChangeBody.playerCompleteAutomaticWriting = true;
            finished = true;
            StopAllCoroutines();
        }
    }

    IEnumerator DealWithMeshDissolve()
    {
        yield return null;

        if (currentYOffset < maxYOffset)
        {
            currentYOffset = Mathf.Lerp(currentYOffset, maxYOffset, 0.02f);

            if (maxYOffset - currentYOffset <= 0.01f)
            {
                currentYOffset = maxYOffset;
            }

            Vector3 offset = new Vector3(0, currentYOffset, 0);
            dissolveMtl.SetVector("_DissolveOffset", offset);

            StartCoroutine(DealWithMeshDissolve());
        }
        else
        {
            StopAllCoroutines();
                        
        }
    }

    void DealWithAutoWriting()
    {
        if (currentCoolDown > 0)
        {
            Debug.Log(canPress);
            canPress = false;
            currentCoolDown -= Time.deltaTime;
        }
        else
        {
            canPress = true;
        }

        if (Keyboard.current.anyKey.isPressed && canPress)
        {
            currentKeyNum++;
            currentCoolDown = coolDown * (1 - completeRate);
            Debug.Log(canPress);
        }
    }
}
