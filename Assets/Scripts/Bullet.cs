using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;
    Rigidbody2D rigid;
    private float spawnTimer = 0f;
    private float spawnInterval = 0.5f;
    private Transform nearestEnemy;
    private float moveSpeed = 15f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();    
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 每0.5秒生成一个子弹
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnBullet();
        }

        // 如果子弹已经生成，寻找最近的敌人并移动
        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void SpawnBullet()
    {
        // 寻找最近的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        if (nearestEnemy != null)
        {
            // 生成子弹
            GameObject bullet = Instantiate(gameObject, transform.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.damage = this.damage;
                bulletComponent.per = this.per;
            }
        }
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        if (per > -1)
        {
            rigid.velocity = dir * moveSpeed;
        }
    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.per = count;
        // 通知玩家更新装备效果
        GameManager.instance.player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        // 对敌人造成伤害
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.health -= damage;
            if (enemy.health <= 0)
            {
                enemy.Dead();
            }
        }

        // 销毁子弹
        gameObject.SetActive(false);
    }
}
