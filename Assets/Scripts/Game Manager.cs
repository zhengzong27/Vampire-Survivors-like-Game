using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public int winCount = 0;
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
    public Result uiResult;
    public GameObject enemyCleaner;
    public bool isWin = false;  // 添加胜利标志
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
        Resume();
    }
    public void GameOver()
    {
        isWin = false;  // 设置失败标志
        StartCoroutine(GameOverCoroutine());
    }
    IEnumerator GameOverCoroutine()
    {
        isLive=false;
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
    }
    public void GameWin()
    {
        isWin = true;  // 设置胜利标志
        StartCoroutine(GameWinCoroutine());
    }
    IEnumerator GameWinCoroutine()
    {
        isLive=false;
        CleanAllEnemies();
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        if (winCount < 5)
        {
            uiResult.ShowContinue();
        }
        else
        {
            uiResult.ShowRetry();
        }
        Stop();
    }
    public void Continue()
    {
        winCount++;
        gameTime = 0;
        isLive = true;
        uiResult.gameObject.SetActive(false);
        enemyCleaner.SetActive(false);
        Resume();
    }
    public void Retry()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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
        if (!isLive || !isInitialized || instance != this)
            return;
            
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime && health > 0)
        {
            gameTime = maxGameTime;
            GameWin();
        }
    }

    public void GetExp() 
    {
        if(!isLive)
            return;
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

    public void CleanAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.Dead();
            }
        }
    }
}
 