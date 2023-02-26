using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinMeshDissolve : MonoBehaviour
{
    public float minYOffset;
    public float maxYOffset;
    protected Material dissolveMtl;
    public float currentYOffset;
    // Start is called before the first frame update

    private void OnEnable()
    {
        dissolveMtl = GetComponent<SkinnedMeshRenderer>().material;

        currentYOffset = minYOffset;
        StartCoroutine(DealWithMeshDissolve());
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
}
