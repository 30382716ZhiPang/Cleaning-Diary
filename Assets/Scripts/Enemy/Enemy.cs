using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    //引用
    protected Player player;
    protected Transform playerTrans;
    protected Vector3 playerLastPos;
    protected Rigidbody2D rigid;

    //属性
    protected bool follow;
    public float speed;
    public float HP;
    public int reward;
    protected RaycastHit2D hit;
    protected float curHP;
    protected LayerMask layerMask;
    public bool isBoss;

    //资源
    public GameObject[] bloodGos;//血迹
    public GameObject bloodDeadGo;
    public GameObject bloodParticle;
    public GameObject explosion;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameManager.Instance.player;
        if (player==null)
        {
            return;
        }
        playerTrans = player.transform;
        player.enemyList.Add(this);
        curHP = HP;
        rigid = GetComponent<Rigidbody2D>();
        layerMask = ~(1 << 10) & ~(1 << 2);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (playerTrans==null)
        {
            return;
        }
        hit = Physics2D.Raycast(transform.position,playerTrans.position-transform.position
            ,4,layerMask);
        SearchAndFollowPlayer();
        Move();
    }
    /// <summary>
    /// 搜索跟随玩家
    /// </summary>
    protected void SearchAndFollowPlayer()
    {
        if (follow&&Vector3.Distance(playerLastPos,transform.position)<=0.1f)
        {
            follow = false;
        }
        if (hit.collider!=null)
        {
            if (!hit.collider.gameObject.CompareTag("Wall"))
            {
                LookAtPlayerAndAttack();
            }
            else if (follow)
            {
                Vector3 moveDirection = playerLastPos - transform.position;
                if (moveDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }

        }
        else if (follow)
        {
            Vector3 moveDirection = playerLastPos - transform.position;
            if (moveDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
    /// <summary>
    /// 移动
    /// </summary>
    protected void Move()
    {
        if (hit.collider!=null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                rigid.AddRelativeForce(new Vector2(0,speed));
            }
        }
        if (follow)
        {
            rigid.AddRelativeForce(new Vector2(0,speed*0.5f));
        }
    }
    /// <summary>
    /// 看向玩家并攻击
    /// </summary>
    protected virtual void LookAtPlayerAndAttack()
    {
        Vector3 moveDirection = playerTrans.transform.position - transform.position;
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            follow = true;
            playerLastPos = playerTrans.position;
        }
    }

    /// <summary>
    /// 触发检测
    /// </summary>
    /// <param name="collision"></param>
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Instantiate(bloodParticle,transform.position,Quaternion.Euler(0,0,
                collision.transform.rotation.eulerAngles.z+180));
            if (collision.gameObject.name.Contains("0"))
            {
                curHP -= 8 * GameManager.Instance.gunLevel * 0.5f;
                Instantiate(bloodGos[0], transform.position, Quaternion.Euler(0, 0,
                    collision.transform.rotation.eulerAngles.z + Random.Range(-15, 15)));
            }
            if (collision.gameObject.name.Contains("1"))
            {
                curHP -= 70;
                Instantiate(bloodGos[1], transform.position, Quaternion.Euler(0, 0,
                    collision.transform.rotation.eulerAngles.z));
            }
            if (collision.gameObject.name.Contains("2"))
            {
                curHP -= 15;
                Instantiate(bloodGos[0], transform.position, Quaternion.Euler(0, 0,
                  collision.transform.rotation.eulerAngles.z + Random.Range(-20, 20)));
            }
            if (collision.gameObject.name.Contains("3"))
            {
                curHP -= 30;
                Instantiate(bloodGos[2], transform.position, Quaternion.Euler(0, 0,
                   collision.transform.rotation.eulerAngles.z));
            }
            rigid.AddRelativeForce(new Vector2(0,-player.playerShooting.repulsion));
           
        }
        if (collision.CompareTag("Mine"))
        {
            Instantiate(explosion,transform.position,Quaternion.identity);
            curHP -= 60;
            Destroy(collision.gameObject);
        }       
        Die();
    }
    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Die()
    {
        if (curHP<=0)
        {
            if (isBoss)
            {
                player.bossIsDead = true;
            }
            Instantiate(bloodDeadGo, transform.position, transform.rotation);
            GameManager.Instance.player.curKills += 1;
            GameManager.Instance.money += reward;
            player.enemyList.Remove(this);
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 僵尸攻击方法
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.HP -= 0.5f;
            player.LimitHP();
            player.delayTimer = player.delayRegen;
            player.Die();
        }
    }
}
