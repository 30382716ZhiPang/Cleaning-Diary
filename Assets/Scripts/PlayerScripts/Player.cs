using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 安德鲁主脚本
/// </summary>
public class Player : MonoBehaviour
{
    //资源
    public GameObject blood;//血迹
    public GameObject bloodParticle;//减血特效
    public AudioClip bonus;//奖励音效
    public GameObject explosion;
    public GameObject deadBloodGo;

    //数据
    public bool isMoving;//玩家是否移动
    public float moveAngle;//移动角度
    public int speed;//移动速度
    public int maxHP;//血量上限
    public float HP;//当前血量
    public int regenHpSpeed;//HP回复速度
    public int delayRegen;//受到伤害再次可回复HP时的延迟时间（固定值）
    public float delayTimer;//可回复血量计时器
    public int kills;//通关需要击杀的僵尸数量
    public int curKills;
    public bool bossIsDead;
    private bool decreaseHP;//被感染后掉血

    //引用
    private Rigidbody2D playerRig;
    private AudioSource audioSource;
    public List<Enemy> enemyList;
    public PlayerShooting playerShooting;

    private void OnEnable()
    {
        GameManager.Instance.player = this;
        HP = maxHP = 100;
        speed = 15;
        regenHpSpeed = delayRegen = 1;
        audioSource = GetComponent<AudioSource>();
        playerRig = GetComponent<Rigidbody2D>();
        enemyList = new List<Enemy>();
        if (GameManager.Instance.selectLevel==3
            || GameManager.Instance.selectLevel == 5
            || GameManager.Instance.selectLevel == 6)
        {
            bossIsDead = false;
        }
        if (GameManager.Instance.selectLevel == 6 &&!GameManager.Instance.anthonyIsDead)
        {
            decreaseHP = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        if (decreaseHP)
        {
            HP -= 1 * Time.deltaTime;
            LimitHP();
            Die();
        }
        else
        {
            RegenHP();
        }        
    }
    /// <summary>
    /// 玩家移动
    /// </summary>
    private void PlayerMove()
    {
        isMoving = false;
#if UNITY_STANDALONE_WIN
        if (Input.GetKey(KeyCode.W))
        {
            playerRig.AddForce(new Vector2(0, speed / (1 + 0.1f * playerShooting.weight)) * 70 * Time.deltaTime);
            moveAngle = 0;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            playerRig.AddForce(new Vector2(0, -speed / (1 + 0.1f * playerShooting.weight)) * 70 * Time.deltaTime);
            moveAngle = 180;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerRig.AddForce(new Vector2(-speed / (1 + 0.1f * playerShooting.weight), 0) * 70 * Time.deltaTime);
            moveAngle = 90;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerRig.AddForce(new Vector2(speed / (1 + 0.1f * playerShooting.weight), 0) * 70 * Time.deltaTime);
            moveAngle = -90;
            isMoving = true;
        }
#elif UNITY_ANDROID
        playerRig.AddForce(new Vector2(GameManager.Instance.inputValue.x* speed / (1 + 0.1f * playerShooting.weight)* 70 * Time.deltaTime, 
        GameManager.Instance.inputValue.y* speed / (1 + 0.1f * playerShooting.weight) *70 * Time.deltaTime));
        if (GameManager.Instance.inputValue!=Vector2.zero)
        {
            isMoving = true;
            moveAngle = GameManager.Instance.inputAngle;
        }
        else
        {
            isMoving = false;
            moveAngle = 0;
        }
#endif



    }
    /// <summary>
    /// 血量回复
    /// </summary>
    private void RegenHP()
    {
        if (delayTimer<=0&&HP<maxHP)
        {
            HP += regenHpSpeed * Time.deltaTime;
            LimitHP();
        }
        else if (delayTimer>0)
        {
            delayTimer -= Time.deltaTime;
        }
      
    }
    /// <summary>
    /// 道具
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string colTagStr = collision.tag;
        switch (colTagStr)
        {
            case "EnemyBullet":
                Instantiate(bloodParticle,transform.position,Quaternion.Euler
                    (0,0,collision.transform.rotation.eulerAngles.z+180));
                Instantiate(blood,transform.position, Quaternion.Euler
                    (0, 0, collision.transform.rotation.eulerAngles.z + 180));
                Destroy(collision.gameObject);
                HP -= 10;
                LimitHP();
                Die();
             
                break;
            case "BulletItem":
                if (playerShooting.curBullets<playerShooting.totalBullets)
                {
                    DestoryItem(collision);
                    playerShooting.curBullets += (int)(playerShooting.totalBullets * 0.25f);
                    playerShooting.LimitBullet();
                }
                break;
            case "HealthItem":
                if (HP<maxHP)
                {
                    DestoryItem(collision);
                    HP += 20;
                    LimitHP();
                   
                }
                break;
            case "MineItem":
                if (playerShooting.curMine<3)
                {
                    DestoryItem(collision);
                    playerShooting.curMine += 1;
                    GameManager.Instance.gameUIManager.UpdateMineUI(playerShooting.curMine/playerShooting.mines);
                }
                break;
            case "TurretItem":
                if (!playerShooting.hasTurret)
                {
                    DestoryItem(collision);
                    playerShooting.hasTurret = true;
                    GameManager.Instance.gameUIManager.UpdateTurretUI(playerShooting.hasTurret);
                }
                break;
            case "MoneyItem":
                DestoryItem(collision);
                GameManager.Instance.money += 5;
                break;
            case "Key":
                audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                collision.GetComponent<Key>().OpenDoor();
                Destroy(collision.gameObject);
                break;
            case "EnemyMine":
                Instantiate(explosion, transform.position, Quaternion.identity);
                HP -= 60;
                Destroy(collision.gameObject);
                LimitHP();
                Die();
                break;
            default:
                break;
        }
    }

    private void DestoryItem(Collider2D collision)
    {
        Destroy(collision.gameObject);
        audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
    }
    /// <summary>
    /// 补给箱
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Box")
        {
            if (collision.gameObject.name.Contains("Bullet"))
            {
                if (playerShooting.curBullets<playerShooting.totalBullets)
                {
                    playerShooting.curBullets += (int)(playerShooting.totalBullets * 0.25f);
                    playerShooting.LimitBullet();
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }
               
            }
            else if (collision.gameObject.name.Contains("Health"))
            {
                if (HP < maxHP)
                {
                    HP += 20;
                    LimitHP();
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }               
            }
            else if (collision.gameObject.name.Contains("Turret"))
            {
                if (playerShooting.curMine < 3)
                {
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                    playerShooting.curMine =playerShooting.mines;
                    GameManager.Instance.gameUIManager.UpdateMineUI(1);
                }
                if (!playerShooting.hasTurret)
                {
                    playerShooting.hasTurret = true;
                    GameManager.Instance.gameUIManager.UpdateTurretUI(playerShooting.hasTurret);
                    audioSource.PlayOneShot(bonus, GameManager.Instance.volume * 0.5f);
                }
            }
        }
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public void Die()
    {
        if (HP <= 0)
        {
            GameManager.Instance.LoadMainScene();
            Instantiate(deadBloodGo, transform.position, transform.rotation);
            Destroy(gameObject);                        
        }      
    }
    /// <summary>
    /// 限制以及更新血量
    /// </summary>
    public void LimitHP()
    {
        if (HP >= maxHP)
        {
            HP = maxHP;
        }
        else if (HP<0)
        {
            HP = 0;
        }
        GameManager.Instance.gameUIManager.UpdateHPSlider(HP / maxHP);
    }
}
