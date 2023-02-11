using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAntInvokeManager : MonoBehaviour
{
    public float invokeInterval;
    protected RockAntAIController[] ants;
    protected int index=0;
    // Start is called before the first frame update

    private void Awake()
    {
        ants = GetComponentsInChildren<RockAntAIController>();

        for(int i = 0; i < ants.Length; i++)
        {
            ants[i].gameObject.SetActive(false);
        }
    }
    void Start()
    {
        InvokeRepeating("InvokeAnt", 0f, invokeInterval);
    }

    void InvokeAnt()
    {
        if(index>= ants.Length)
        {
            CancelInvoke();
        }
        else
        {
            ants[index++].gameObject.SetActive(true);
        }

    }
}
