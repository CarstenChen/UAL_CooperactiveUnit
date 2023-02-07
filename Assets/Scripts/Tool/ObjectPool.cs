using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///�����
///<summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance { get; private set; }

    private Dictionary<string, GamePool> pool;
    public Transform poolRoot;

    public delegate void ActiveEvent();
    public static event ActiveEvent OnActiveEvent;
    private void Awake()
    {
        instance = this;
        pool = new Dictionary<string, GamePool>();
    }

    public GameObject GetGameObject(GameObject go, Vector3 position, Quaternion rotation, Transform parent)
    {
        //����û�г��Ӵ���һ������
        if (!pool.ContainsKey(go.name))
        {
            pool.Add(go.name, new GamePool());
        }
        //�����г���û���󴴽�һ������
        if(pool[go.name].queue.Count == 0)
        {
            GameObject newGo = Instantiate(go, position, rotation, parent);
            newGo.name = go.name;//���⣨Clone��

            return newGo;
        }
        //������һ������
        GameObject returnObject = pool[go.name].queue.Dequeue();


        returnObject.transform.SetParent(parent);
        returnObject.transform.position = position;  
        returnObject.transform.rotation = rotation;
        returnObject.SetActive(true);//agent����ǰ������λ�ã���Ȼ����������
        return returnObject;
    }

    public void SetGameObject(GameObject go, float delay)
    {
        if (!pool.ContainsKey(go.name))
        {
            pool.Add(go.name, new GamePool());
            StartCoroutine(ExcueteSetGameObject(go, delay));
        }
        else if(pool[go.name].queue.Count >= pool[go.name].MaxNumInPool)
        {
            Destroy(go, delay);
        }
         
        else
        {
            //�ӳ�ɾ������
            StartCoroutine(ExcueteSetGameObject(go, delay));
        }
    }

    public IEnumerator ExcueteSetGameObject(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
        go.transform.SetParent(poolRoot);
        go.transform.localPosition = Vector3.zero;
        pool[go.name].queue.Enqueue(go);
    }

    public void PreLoadGameObject(GameObject go, int num)
    {
        //����û�г��Ӵ���һ������
        if (!pool.ContainsKey(go.name))
        {
            pool.Add(go.name, new GamePool());
        }

        for(int i = 0; i < num; i++)
        {
            GameObject newGo = Instantiate(go, poolRoot);
            newGo.name = go.name;//���⣨Clone��
            newGo.SetActive(false);
            pool[go.name].queue.Enqueue(newGo);
        }
    }
}
