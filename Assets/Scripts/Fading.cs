using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 血迹褪色
/// </summary>
public class Fading : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float speed;
    public float waitTime;
    private bool StartFading; 
    public bool isEnemyBlood;
    public AudioSource audioSource;
    public AudioClip enemyDieClip;
    public AudioClip playerDieClip;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource!=null)
        {
            if (isEnemyBlood)
            {
                audioSource.PlayOneShot(enemyDieClip, GameManager.Instance.volume);
            }
            else
            {
                audioSource.PlayOneShot(playerDieClip, GameManager.Instance.volume);
            }
        }       
        Invoke("FadeBlood",waitTime); 
    }

    // Update is called once per frame
    void Update()
    {
        if (StartFading)
        {
            //血迹消失褪色
            Color color = spriteRenderer.color;
            color.a -= speed * Time.deltaTime;
            color.a = Mathf.Clamp(color.a, 0, 1);
            spriteRenderer.color = color;
            if (color.a<=0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FadeBlood()
    {
        StartFading = true;
    }
}
