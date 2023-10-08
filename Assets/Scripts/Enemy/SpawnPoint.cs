using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 僵尸洞（僵尸孵化点）
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    private float delayTimer;
    public float spawnTime;
    public int limit;
    public GameObject enemyGo;
    private bool startSpawn;

    // Start is called before the first frame update
    void Start()
    {
        delayTimer = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawn)
        {
            if (delayTimer>0)
            {
                delayTimer -= Time.deltaTime;
            }
            else
            {
                if (limit>0)
                {
                    Instantiate(enemyGo,transform.position+
                        new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f, 0.1f),0)
                        ,transform.rotation);
                    delayTimer = spawnTime;
                    limit -= 1;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!startSpawn)
            {
                startSpawn = true;
            }
        }
    }
}
