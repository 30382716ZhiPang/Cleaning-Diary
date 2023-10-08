using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 加载游戏
/// </summary>
public class LoadGame : MonoBehaviour
{
    public Text text;
    private AsyncOperation op;
    public GameObject diaryPanelGo;

    // Start is called before the first frame update
    void Start()
    {
        diaryPanelGo.SetActive(false);
        if (GameManager.Instance.showEnd)
        {
            op = SceneManager.LoadSceneAsync(0);
        }
        else
        {
            op = SceneManager.LoadSceneAsync(GameManager.Instance.selectLevel);
        }        
        op.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (op.progress>=0.9f)
        {
            text.text = "按下任意键继续";
            if (Input.anyKeyDown)
            {
                if (GameManager.Instance.firstEnterLevels[GameManager.Instance.selectLevel-1])
                {
                    diaryPanelGo.SetActive(true);
                    gameObject.SetActive(false);
                    GameManager.Instance.firstEnterLevels[GameManager.Instance.selectLevel - 1] = false;
                    GameManager.Instance.SetBoolArray("FirstEnter", GameManager.Instance.firstEnterLevels);
                }
                else
                {
                    LoadNextScene();
                }
            }
        }
    }
    /// <summary>
    /// 加载游戏场景
    /// </summary>
    public void LoadNextScene()
    {
        op.allowSceneActivation = true;
    }
}
