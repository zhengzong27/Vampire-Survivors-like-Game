using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public float health=3;
    public float maxHealth=3;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        health = 3;
    }

    void OnEnable()
    {
        Debug.Log("GameManager 被启用");
    }

    void OnDisable()
    {
        Debug.Log("GameManager 被禁用");
    }

    private void Update()
    {
        if (instance != this)
            return;
            
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
        Debug.Log($"GameManager Update - gameTime: {gameTime:F2}, maxGameTime: {maxGameTime}, 剩余时间: {maxGameTime - gameTime:F2}");
    }
    public void GetExp() 
    {
        exp++;
        if (exp == nextExp[level]) 
        {   level++;
            exp = 0;
        
        }
    }
}
 