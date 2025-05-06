using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public GameObject[] prefabs;
    List<GameObject>[] pools;
    public bool isInitialized { get; private set; } = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("PoolManager: prefabs 数组未设置！请在 Inspector 中设置预制体。");
            return;
        }

        pools = new List<GameObject>[prefabs.Length];
        for(int index=0;index<pools.Length;index++)
        {
            pools[index] = new List<GameObject>();
        }
        isInitialized = true;
        Debug.Log("PoolManager 初始化完成");
    }

    public GameObject Get(int index)
    {
        if (!isInitialized)
        {
            Debug.LogError("PoolManager.Get: PoolManager 尚未初始化完成！");
            return null;
        }

        if (index < 0 || index >= prefabs.Length)
        {
            Debug.LogError($"PoolManager.Get: 无效的索引 {index}，prefabs 数组长度为 {prefabs.Length}");
            return null;
        }

        if (pools == null || pools[index] == null)
        {
            Debug.LogError("PoolManager.Get: pools 未正确初始化！");
            return null;
        }

        GameObject select = null;

        foreach(GameObject item in pools[index]) 
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(prefabs[index],transform);
            pools[index].Add(select);
        }

        return select;
    }
}
