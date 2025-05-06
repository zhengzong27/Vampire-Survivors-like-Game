using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levelup : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        gameObject.SetActive(false);  // 直接使用 SetActive 而不是调用 Hide
    }
    public void Show()
    {
        Next();
        gameObject.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.Stop();
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        if (GameManager.instance != null)
        {
            GameManager.instance.Resume();
        }
    }
    public void Select(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            items[index].OnClick();
        }
    }
    void Next()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogError("Levelup: items 数组未初始化或为空！");
            return;
        }

        //1.禁用所有项目
        foreach(Item item in items)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
            }
        }
        
        //2.随机激活三个升级词条
        int[] ran = new int[3];
        int maxAttempts = 100; // 防止无限循环
        int attempts = 0;
        
        while(attempts < maxAttempts)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);
            
            if(ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
            {
                break;
            }
            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Levelup: 无法找到三个不同的随机选项！");
            return;
        }
        
        //3.激活选中的选项
        for(int index = 0; index < ran.Length; index++)
        {
            if (ran[index] >= 0 && ran[index] < items.Length)
            {
                Item ranItem = items[ran[index]];
                if (ranItem != null && ranItem.data != null)
                {
                    //3.满级词条处理方法
                    if(ranItem.level >= ranItem.data.damages.Length)
                    {
                        // 确保索引4存在
                        if (items.Length > 4)
                        {
                            items[4].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        ranItem.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
