using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    bool isDead = false;
    private float damageTimer = 0f;
    private const float DAMAGE_INTERVAL = 0.5f; // 伤害间隔时间
    private HashSet<GameObject> collidingEnemies = new HashSet<GameObject>();

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    void Update()
    {
        if(!GameManager.instance.isLive || isDead)
            return;
        inputVec.x=Input.GetAxisRaw("Horizontal");
        inputVec.y=Input.GetAxisRaw("Vertical");

        // 处理伤害计时
        if (collidingEnemies.Count > 0)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= DAMAGE_INTERVAL)
            {
                damageTimer = 0f;
                TakeDamage();
            }
        }
        else
        {
            damageTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if(!GameManager.instance.isLive || isDead)
            return;
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void OnMove(InputValue value)
    {
        if(!GameManager.instance.isLive || isDead)
            return;
        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        if(!GameManager.instance.isLive || isDead)
            return;
        anim.SetFloat("speed", inputVec.magnitude);
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive || isDead)
            return;

        if(collision.gameObject.CompareTag("Enemy"))
        {
            collidingEnemies.Add(collision.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive || isDead)
            return;

        if(collision.gameObject.CompareTag("Enemy"))
        {
            collidingEnemies.Remove(collision.gameObject);
        }
    }

    private void TakeDamage()
    {
        if(!GameManager.instance.isLive || isDead)
            return;

        GameManager.instance.health -= 5f; // 每次造成5点伤害
        if(GameManager.instance.health <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        collidingEnemies.Clear();
        
        // 禁用所有子对象的武器
        for(int index=2; index<transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(false);
        }  
        
        anim.SetTrigger("Dead");
        GameManager.instance.GameOver();
        
        // 停止移动
        inputVec = Vector2.zero;
        if(rigid != null)
        {
            rigid.velocity = Vector2.zero;
        }
    }
}
