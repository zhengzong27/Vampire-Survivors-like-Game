using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;
    bool isInitialized = false;
    private List<Transform> activeBullets = new List<Transform>();
    private const int MAX_BULLETS = 100; // 设置最大子弹数量限制

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void OnDestroy()
    {
        // 清理所有子弹
        foreach (Transform bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.gameObject.SetActive(false);
            }
        }
        activeBullets.Clear();
    }

    void Update()
    {
        if(!GameManager.instance.isLive || !isInitialized)
            return;
            
        switch (id)
        {
            case 0: // 近战武器
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            case 1: // 远程武器
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    if (activeBullets.Count < MAX_BULLETS)
                    {
                        Fire();
                    }
                }
                break;
        }
    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.count = Mathf.Min(count, 10); // 限制最大数量
        if(id == 0)
            Batch();
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void init(ItemData data)
    {
        if (isInitialized)
            return;

        //Basic set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        
        //Property set
        id = data.itemId;
        damage = data.baseDamage;
        count = Mathf.Min(data.baseCount, 10); // 限制初始数量

        // 检查 PoolManager 是否可用
        if (GameManager.instance.pool == null || GameManager.instance.pool.prefabs == null)
        {
            Debug.LogError("PoolManager 未正确初始化！");
            return;
        }

        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if(data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0: // 近战武器
                speed = 150;
                Batch();
                break;
            case 1: // 远程武器
                speed = 0.5f;
                // 激活右手
                Transform handRight = player.transform.Find("HandRight");
                if (handRight != null)
                {
                    handRight.gameObject.SetActive(true);
                }
                break;
        }

        //Hand set
        if (player.hands != null && player.hands.Length > (int)data.itemType)
        {
            Hand hand = player.hands[(int)data.itemType];
            if (hand != null)
            {
                hand.spriter.sprite = data.hand;
                hand.gameObject.SetActive(true);
            }
        }
        
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        isInitialized = true;
    }

    void Batch()
    {
        if (!isInitialized)
            return;

        // 清理旧的子弹
        foreach (Transform bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.gameObject.SetActive(false);
            }
        }
        activeBullets.Clear();

        // 生成新的子弹
        for(int index = 0; index < count; index++)
        {
            GameObject newBullet = GameManager.instance.pool.Get(prefabId);
            if (newBullet != null)
            {
                Transform bullet = newBullet.transform;
                bullet.parent = transform;
                bullet.localPosition = Vector3.zero;
                bullet.localRotation = Quaternion.identity;
                Vector3 rotVec = Vector3.forward * 360 * index / count;
                bullet.Rotate(rotVec);
                bullet.Translate(bullet.up * 1.5f, Space.World);
                
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.Init(damage, -1, Vector3.zero);
                }
                
                activeBullets.Add(bullet);
            }
        }
    }

    void Fire()
    {
        if (!isInitialized || !player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        GameObject bulletObj = GameManager.instance.pool.Get(prefabId);
        if (bulletObj != null)
        {
            Transform bullet = bulletObj.transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Init(damage, count, dir);
            }
            
            activeBullets.Add(bullet);
        }
    }
}
