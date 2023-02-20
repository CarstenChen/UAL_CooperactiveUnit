using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGuideTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AIDirector.Instance.canTriggerMonsterGuide = true;
            Destroy(this.gameObject);
        }
    }
}
