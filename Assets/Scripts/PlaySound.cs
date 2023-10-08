using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 音频播放
/// </summary>
public class PlaySound : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip,GameManager.Instance.volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
