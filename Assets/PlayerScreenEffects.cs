using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class PlayerScreenEffects : MonoBehaviour
{
    public Volume volume;
    public float monsterDetectRange;
    [NonSerialized] public Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = monsterDetectRange;
        volume.profile.TryGet<Vignette>(out vignette);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag =="Monster")
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            float value = 1 - distance / monsterDetectRange;
            vignette.intensity.value = Mathf.Clamp(value, 0, 1);
          //ClampedFloatParameter vignetteIntensity = new ClampedFloatParameter(value, 0, 1,true);
          //vignette.intensity = vignetteIntensity;
        }
    }
}
