using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePool
{
    private int maxNumInPool = int.MaxValue;
    public Queue<GameObject> queue;

    public GamePool()
    {
        queue = new Queue<GameObject>();
    }
    public int MaxNumInPool
    {
        get
        {
            return maxNumInPool;
        }
        set
        {
            //ȷ���Ǵ���0С��int���޵ĸ���
            maxNumInPool = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }
}
