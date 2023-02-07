using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmittedObject : MonoBehaviour
{
    public AnimationCurve moveCurve;

    public float speed;
    private float x;
    public float time = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (AIDirector.Instance.goEnding)
        //    return;

        Move();

        if (x >= 1)
        {
            ObjectPool.instance.SetGameObject(this.gameObject, 0);
        }
    }

    private void Move()
    {
        x += Time.deltaTime / time;

        transform.position = Vector3.LerpUnclamped(transform.position, transform.position+new Vector3(0, speed*Time.deltaTime, 0), moveCurve.Evaluate(x));
    }
}
