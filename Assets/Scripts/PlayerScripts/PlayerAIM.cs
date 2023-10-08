using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 安德鲁的AI管理者，负责搜索敌人以及瞄准敌人
/// </summary>
public class PlayerAIM : MonoBehaviour
{
    public Transform cameraTrans;//摄像机
    public GameObject mark;//用来标记敌人的标记
    public float minDistance;//最近距离
    public float maxDistance;//视野范围
    private Player player;//玩家引用
    private Enemy nearestEnemy;//最近敌人
    private RaycastHit2D hit;
    private int layerValue;
    private LayerMask layerMask;
    private float distance;//怪物跟玩家之间的距离
     
    // Start is called before the first frame update
    void Start()
    {
        cameraTrans = Camera.main.transform;
        player = GameManager.Instance.player;
        mark.SetActive(false);
        minDistance = maxDistance = 10;
        //屏蔽第9层的射线检测
        //layerValue = ~(1 << 9);
        layerMask = ~(1 << 9)&~(1<<2);
        //射线遮罩总结(以第九层为例):
        //1.想要打开某一个层的射线检测:
        //layerMask = 1 << 9;
        //2.打开除某一个层级以外其他所有层级的射线检测
        //layerMask = ~(1 << 9);
        //3.打开所有层检测
        //layerMask = ~(1 << 0);
        //4.打开某几层的检测
        //layerMask = (1 << 10) | (1 << 9);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < player.enemyList.Count; i++)
        {
            if (player.enemyList[i]==null)
            {
                return;
            }
            
            hit = Physics2D.Raycast(transform.position,
                player.enemyList[i].transform.position-transform.position,4, layerMask);
            if (hit.collider!=null)
            {
                if (!hit.collider.gameObject.CompareTag("Wall"))
                {
                    distance = Vector3.Distance(transform.position,player.enemyList[i].transform.position);
                    if (distance<maxDistance&&distance<minDistance)
                    {
                        //在玩家视野里
                        minDistance = distance;
                        nearestEnemy = player.enemyList[i];
                    }
                }
                
            }
        }
        //有目标
        if (nearestEnemy!=null)
        {
            

            //mark.transform.SetParent(nearestEnemy.transform);
            //mark.transform.localPosition = Vector3.zero;
            //mark.transform.rotation = transform.rotation;
            mark.SetActive(true);

            Vector3 moveDirection = nearestEnemy.transform.position - transform.position;
            if (moveDirection!=Vector3.zero)
            {
                //Mathf.Atan2()返回弧度值，表示反三角函数中的arctanx
                //Mathf.Rad2Deg 弧度转度等于360/(PI*2)
                float angle =Mathf.Atan2(moveDirection.x,moveDirection.y)*Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle,-Vector3.forward);
            }
            
        }
        else
        {
            transform.rotation = Quaternion.Euler(0,0,player.moveAngle);
            mark.SetActive(false);
        }
        
    }

    private void LateUpdate()
    {
        cameraTrans.transform.position = new Vector3(transform.position.x, transform.position.y, -7.8f);
        if (nearestEnemy!=null)
        {
            mark.transform.position = nearestEnemy.transform.position;
            if (hit.collider!=null)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    minDistance = maxDistance = 10;
                    nearestEnemy = null;
                }
            }
        }
        else
        {
            minDistance = maxDistance = 10;
            nearestEnemy = null;
        }
    }
}
