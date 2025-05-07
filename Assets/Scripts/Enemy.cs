using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using Timers;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    bool isLive;
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    private float moveTimer = 0f;
    private const float MOVE_UPDATE_INTERVAL = 0.1f; // 移动更新间隔
    private Vector2 currentDirection = Vector2.zero;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();    
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive || !isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        moveTimer += Time.fixedDeltaTime;
        if (moveTimer >= MOVE_UPDATE_INTERVAL)
        {
            moveTimer = 0f;
            UpdateMovement();
        }
        else
        {
            ApplyMovement();
        }
    }

    private void UpdateMovement()
    {
        if (target == null)
            return;

        Vector2 dirVec = target.position - rigid.position;
        currentDirection = dirVec.normalized;
    }

    private void ApplyMovement()
    {
        if (currentDirection == Vector2.zero)
            return;

        Vector2 nextVec = currentDirection * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive)
            return;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
        moveTimer = 0f;
        currentDirection = Vector2.zero;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive || !gameObject.activeInHierarchy)
            return;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            health -= bullet.damage;
            if (health > 0)
            {
                anim.SetTrigger("Hit");
                StartCoroutine(KnockBack());
            }
            else
            {
                Die();
            }
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        if (!gameObject.activeInHierarchy)
            yield break;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirvec = transform.position - playerPos;
        rigid.AddForce(dirvec.normalized * 3, ForceMode2D.Impulse);
    }

    private void Die()
    {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);
        GameManager.instance.kill++;
        GameManager.instance.GetExp();
        TimersManager.SetTimer(this, 3f, Dead);
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }
}
