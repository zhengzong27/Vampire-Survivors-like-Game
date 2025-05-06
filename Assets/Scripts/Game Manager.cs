using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public float health;
    public float maxHealth=100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public Levelup uilevelup;
    public bool isInitialized { get; private set; } = false;

    void Awake()
    {

            instance = this;

    }

    public void GameStart()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitializeCoroutine());
        health=maxHealth;
        isLive=true;
        uilevelup.Select(0);
    }

    IEnumerator InitializeCoroutine()
    {
        // 等待一帧，确保 PoolManager 已经初始化
        yield return null;

        // 检查必要的组件
        if (pool == null)
        {
            Debug.LogError("GameManager: PoolManager 未分配！请在 Inspector 中分配 PoolManager 组件。");
            yield break;
        }

        // 等待 PoolManager 初始化完成
        while (!pool.isInitialized)
        {
            yield return null;
        }

        if (player == null)
        {
            Debug.LogError("GameManager: Player 未分配！请在 Inspector 中分配 Player 组件。");
            yield break;
        }

        if (uilevelup == null)
        {
            Debug.LogError("GameManager: Levelup 未分配！请在 Inspector 中分配 Levelup 组件。");
            yield break;
        }

        health = maxHealth;
        isInitialized = true;
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
        if (!isLive)
            return; 
        if (instance != this || !isInitialized)
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
        if (!isInitialized)
            return;

        exp++;
        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)]) 
        {   
            level++;
            exp = 0;
            uilevelup.Show();
        }
    }
    public void Stop()
    {
        isLive=false;
        Time.timeScale=0;
    }
    public void Resume()
    {
        isLive=true;
        Time.timeScale=1;
    }
}
 