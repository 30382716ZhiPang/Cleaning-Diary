using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 安东尼僵尸形态
/// </summary>
public class AnthonyZombie : EnemySoldier
{
    //资源
    public GameObject turret;
    public GameObject mine;
    //数据
    public int mineCD;
    public int turretCD;
    private float curMineCD;
    private float curTurretCD;

    protected override void Start()
    {
        base.Start();
        if (GameManager.Instance.anthonyIsDead)
        {
            GameManager.Instance.player.enemyList.Remove(this);
            Destroy(gameObject);
        }
        curTurretCD = 10;
    }

    protected override void Update()
    {
        base.Update();
        UseItem();       
    }
    /// <summary>
    /// 安东尼用雷和炮塔
    /// </summary>
    private void UseItem()
    {
        if (hit.collider==null)
        {
            return;
        }
        if (!hit.collider.CompareTag("Player"))
        {
            return;
        }
        if (curMineCD<=0)
        {
            curMineCD = mineCD;
            Instantiate(mine, transform.position, Quaternion.identity);
        }
        else
        {
            curMineCD -= Time.deltaTime;
        }
        if (curTurretCD<=0)
        {
            curTurretCD = turretCD;
            Instantiate(turret,transform.position,Quaternion.identity);
        }
        else
        {
            curTurretCD -= Time.deltaTime;
        }
    }
    
    protected override void LookAtPlayerAndAttack()
    {
        Vector3 moveDirection = playerTrans.transform.position - transform.position;
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            follow = true;
            playerLastPos = playerTrans.position;
        }
        if (curCD <= 0 && curMagazine > 0)
        {
            audioSource.PlayOneShot(shootClip, GameManager.Instance.volume);
            Instantiate(bulletGo, transform.position,
                              Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - inaccuracy));
            Instantiate(bulletGo, transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - inaccuracy * 0.5f));
            Instantiate(bulletGo, transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
            Instantiate(bulletGo, transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + inaccuracy * 0.5f));
            Instantiate(bulletGo, transform.position,
                               Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + inaccuracy));
            curCD = attackCD;
            curMagazine -= 1;
            if (curMagazine <= 0)
            {
                curReload = reload;
            }
        }
    }
}
