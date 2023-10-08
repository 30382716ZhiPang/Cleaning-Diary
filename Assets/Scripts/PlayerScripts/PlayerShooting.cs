using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 安德鲁的射击控制（切枪，射击,装弹）
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    //资源
    public GameObject[] bullets;//0.Pistol/Ak/MG 1.Snipe 2. ShotGun 3.CrossBow
    public GameObject[] turrets;//炮塔
    public AudioClip[] shootClips;//射击音频
    public AudioClip reloadClip;
    public GameObject mineGo;//地雷

    //引用
    public GameObject[] weapons;
    public AudioSource audioSource;
    public GameObject lineLeft;
    public GameObject lineRight;
    private Player player;
    private WeaponProperties weaponProperties;
    public ParticleSystem flash;

    //属性
    public float maxInaccuracy;//最大不精确度
    public float recoilForce;//后坐力(攻击中，影响射击精度)
    public float destabilization;//不稳定性（在所有情况下都会影响精度）
    public float aimingDeSpeed;//瞄准不精度减少速度
    public float minInaccuracy;//最小不精确度
    public float attackCD;//攻击CD
    public int magazine;//弹夹中子弹数量
    public int totalBullets;//全部弹药
    public float reload;//装弹时间
    public float weight;//枪重
    public int repulsion;//子弹击退力
    private int gunLevel;
    private int turretLevel;
    //当前属性
    private float curInaccuracy;//当前不精确度
    private float curDestabilization;//当前不稳定性
    private float curAttackCD;//攻击CD
    public int curMagazine;//当前弹夹里的子弹数量
    public int curBullets;//当前全部的子弹数量(背包+弹夹)
    public float curReload;//装弹CD
    public bool hasTurret;//有炮塔
    public int curMine;//当前地雷数量
    public int mines;//当前可持有的最大地雷数量
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        gunLevel = GameManager.Instance.gunLevel;
        weapons[gunLevel - 1].SetActive(true);
        hasTurret = true;
        curMine = mines = 3;
        weaponProperties = GameManager.Instance.weaponPropertiesList[GameManager.Instance.gunLevel-1];
        maxInaccuracy = (float)weaponProperties.maxInaccuracy;
        recoilForce = (float)weaponProperties.recoilForce;
        destabilization = (float)weaponProperties.destabilization;
        aimingDeSpeed = (float)weaponProperties.aimingDeSpeed;
        minInaccuracy = (float)weaponProperties.minInaccuracy;
        attackCD = (float)weaponProperties.attackCD;
        magazine = weaponProperties.magazine;
        totalBullets = weaponProperties.totalBullets;
        reload = (float)weaponProperties.reload;
        weight = (float)weaponProperties.weight;
        repulsion = weaponProperties.repulsion;
        audioSource = GetComponent<AudioSource>();
        curBullets = totalBullets;
        curMagazine = magazine;
        curReload = curAttackCD = 0;
        turretLevel = GameManager.Instance.gunLevel / 2;
        if (turretLevel == 0)
        {
            turretLevel = 1;
        }
        player.playerShooting = this;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDestabilization();
        CalculateInaccuracy();
        CalcultateCD();
#if UNITY_STANDALONE_WIN
        MonitorInput();
#endif
    }
    /// <summary>
    /// 计算CD时间
    /// </summary>
    private void CalcultateCD()
    {
        if (curAttackCD>0)
        {
            curAttackCD -= Time.deltaTime;
        }
        GameManager.Instance.gameUIManager.UpdateAttackCDUI((attackCD-curAttackCD)/attackCD);
        
        if (curReload>0)
        {
            //装弹中
            curReload -= Time.deltaTime;
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((reload - curReload) / reload);
        }
        else
        {
            if (curMagazine<=0)
            {
                curMagazine = magazine;
            }
        }
    }

    /// <summary>
    /// 计算不稳定性
    /// </summary>
    private void CalculateDestabilization()
    {
        if (player.isMoving)
        {
            //移动后不稳定性逐渐增加(扛枪走)
            if (curDestabilization < destabilization)
            {
                curDestabilization += destabilization * Time.deltaTime * 1.5f;
            }
            else
            {
                curDestabilization = destabilization;
            }
        }
        else
        {
            curDestabilization = 0;
        }
    }
    /// <summary>
    /// 计算不精确度
    /// </summary>
    private void CalculateInaccuracy()
    {
        //当前不精度>=最小不精确度+当前不稳定性(下限值)
        if (curInaccuracy>=minInaccuracy+curDestabilization)
        {
            //表示当前不精确度很大，那么安德鲁随着时间的推移慢慢集中精神
            curInaccuracy -= Time.deltaTime * aimingDeSpeed;
        }
        else
        {
            curInaccuracy = minInaccuracy + curDestabilization;
        }
        lineLeft.transform.localRotation = Quaternion.AngleAxis(curInaccuracy,Vector3.forward);
        lineRight.transform.localRotation = Quaternion.AngleAxis(-curInaccuracy,Vector3.forward);
    }
    /// <summary>
    /// 攻击射击
    /// </summary>
    public void Shoot()
    {
        //射击需要满足的条件
        if (curBullets>0&&curMagazine>0&&curAttackCD<=0)
        {
            audioSource.PlayOneShot(shootClips[gunLevel - 1], GameManager.Instance.volume);
            switch (gunLevel)
            {
                case 1:
                case 2:
                case 3:
                    CreateBullect(0);
                    break;
                case 4:
                    CreateBullect(1);
                    break;
                case 5:
                    CreateBullect(2);
                    break;
                default:
                    CreateBullect(3);
                    break;
            }
            if (gunLevel != 6&& gunLevel != 1)
            {
                flash.Play();
            }
            curMagazine -= 1;
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((float)curMagazine/magazine);
            curBullets -= 1;
            LimitBullet();
            curAttackCD = attackCD;
            curInaccuracy += recoilForce;
            if (curInaccuracy>maxInaccuracy)
            {
                curInaccuracy = maxInaccuracy;
            }
            if (curMagazine<=0&&curReload<=0)
            {
                Reload();
            }
        }

    }
    /// <summary>
    /// 装弹方法
    /// </summary>
    public void Reload()
    {
        if (curMagazine<magazine)
        {
            GameManager.Instance.gameUIManager.UpdateReloadAndMagazineBulletUI((float)curMagazine / magazine);
            audioSource.PlayOneShot(reloadClip,GameManager.Instance.volume);
            curReload = reload;
            curMagazine = 0;
        }
    }
    /// <summary>
    /// 产生子弹
    /// </summary>
    /// <param name="bullectIndex"></param>
    private void CreateBullect(int bullectIndex)
    {
        if (gunLevel == 5)
        {
            //散弹枪
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z-curInaccuracy));
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - curInaccuracy*0.5f));
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + curInaccuracy * 0.5f));
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z+curInaccuracy));
        }
        else
        {
            Instantiate(bullets[bullectIndex], transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z +
                               UnityEngine.Random.Range(-curInaccuracy, curInaccuracy)));
        }
      
    }
    /// <summary>
    /// 监听玩家输入
    /// </summary>
    private void MonitorInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {

            SetTurret();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {

            SetMine();
        }
    }
    /// <summary>
    /// 放置地雷
    /// </summary>
    public void SetMine()
    {
        if (curMine > 0)
        {
            curMine -= 1;
            GameManager.Instance.gameUIManager.UpdateMineUI((float)curMine / mines);
            Instantiate(mineGo, transform.position, Quaternion.identity);
        }
    }
    /// <summary>
    /// 设置炮塔
    /// </summary>
    public void SetTurret()
    {
        if (hasTurret)
        {
            hasTurret = false;
            Instantiate(turrets[turretLevel - 1], transform.position, Quaternion.identity);
            GameManager.Instance.gameUIManager.UpdateTurretUI(hasTurret);
        }
    }

    /// <summary>
    /// 限制子弹以及更新子弹条
    /// </summary>
    public void LimitBullet()
    {
        if (curBullets >=totalBullets )
        {
            curBullets = totalBullets;
        }
        else if (curBullets < 0)
        {
            curBullets = 0;
        }
        GameManager.Instance.gameUIManager.UpdateBulletSlider((float)curBullets / totalBullets);
    }

}
/// <summary>
/// 武器属性结构体
/// </summary>
public struct WeaponProperties
{
    //属性
    public double maxInaccuracy;//最大不精确度
    public double recoilForce;//后坐力(攻击中，影响射击精度)
    public double destabilization;//不稳定性（在所有情况下都会影响精度）
    public double aimingDeSpeed;//瞄准精度减少速度
    public double minInaccuracy;//最小不精确度
    public double attackCD;//攻击CD
    public int magazine;//弹夹中子弹数量
    public int totalBullets;//全部弹药
    public double reload;//装弹时间
    public double weight;//枪重
    public int repulsion;//子弹击退力
}
