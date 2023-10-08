using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 子弹
/// </summary>
public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public float speed;
    public GameObject destroyParticle;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.up*Time.deltaTime*speed,Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
            Instantiate(destroyParticle,transform.position,Quaternion.identity);
        }
    }
}
