using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 打字特效脚本
/// </summary>
public class TextWriteEffect : MonoBehaviour
{
    public float charPerSecond;//打字时间间隔
    private string words;//整个需要打出的日记内容
    private bool startWrite;//是否开始打字特效
    private float timer;//计时器
    private Text text;
    private int currentPos;
    private AudioSource audioSource;
    public LoadGame loadGame;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        startWrite = false;
        charPerSecond = 0.2f;
        text = GetComponent<Text>();
        int storyIndex = GameManager.Instance.selectLevel - 1;
        if (storyIndex==5)
        {
            if (GameManager.Instance.anthonyIsDead)
            {
                storyIndex = 5;
            }
            else
            {
                storyIndex = 7;
            }
            if (GameManager.Instance.showEnd)
            {
                storyIndex++;
            }
        }
        words = GameManager.Instance.stories[storyIndex];
        text.text = "";
        audioSource = GetComponent<AudioSource>();
        StartEffect();
    }

    // Update is called once per frame
    void Update()
    {
        StartWriting();
    }
    /// <summary>
    /// 开始书写日记
    /// </summary>
    public void StartEffect()
    {
        startWrite = true;
    }
    /// <summary>
    /// 书写日记
    /// </summary>
    private void StartWriting()
    {
        if (startWrite)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            timer += Time.deltaTime;
            if (timer>=charPerSecond)
            {
                timer = 0;
                currentPos++;
                text.text = words.Substring(0,currentPos);
                if (currentPos>=words.Length)
                {
                    FinishWriting();
                }
            }
        }
    }
    /// <summary>
    /// 完成书写
    /// </summary>
    private void FinishWriting()
    {
        startWrite = false;
        timer = 0;
        currentPos = 0;
        text.text = words;
        audioSource.Stop();
        Invoke("LoadNextScene",2);
    }

    private void LoadNextScene()
    {
        GameManager.Instance.showEnd = false;
        loadGame.LoadNextScene();
    }
}
