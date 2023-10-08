using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using System;
using UnityEngine.Networking;
/// <summary>
/// 游戏管理
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //玩家数据
    [HideInInspector]
    public int money;
    public int unLockedLevel;
    public int selectLevel;
    public int gunLevel;
    public Player player;
    public bool anthonyIsDead;
    public GameUIManager gameUIManager;
    public bool[] firstEnterLevels;
    public bool showEnd;

    //系统设置
    public float volume;
    public float joystickSize;

    //游戏数据
    public List<WeaponProperties> weaponPropertiesList;
    public string[] stories;
    public Vector2 inputValue;
    public float inputAngle;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
#if UNITY_STANDALONE_WIN
         LoadWeaponProperty();
         LoadStories();
#elif UNITY_ANDROID
        StartCoroutine(LoadWeaponProperty());
        StartCoroutine(LoadStories());        
#endif
        LoadGame();
        //unLockedLevel = 6;
        //SceneManager.LoadScene(selectLevel);
        //测试
        //money=1000;
        //firstEnterLevels = new bool[6] { true, true, true, true, true, true };
    }
    /// <summary>
    /// 读档
    /// </summary>
    private void LoadGame()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            money = PlayerPrefs.GetInt("Money");
        }
        else
        {
            money = 0;
        }
        if (PlayerPrefs.HasKey("Levels"))
        {
            unLockedLevel = PlayerPrefs.GetInt("Levels");
        }
        else
        {
            unLockedLevel = 1;
        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            volume = PlayerPrefs.GetFloat("Sound");
        }
        else
        {
            volume = 1;
        }
        if (PlayerPrefs.HasKey("Joystick"))
        {
            joystickSize = PlayerPrefs.GetFloat("Joystick");
        }
        else
        {
            joystickSize = 0.3f;
        }
        if (PlayerPrefs.HasKey("FirstEnter"))
        {
            firstEnterLevels = GetBoolArray("FirstEnter");
        }
        else
        {
            firstEnterLevels = new bool[6] { true, true, true, true, true, true };
        }
        if (PlayerPrefs.HasKey("AnthonyIsDead"))
        {
            anthonyIsDead =Convert.ToBoolean(PlayerPrefs.GetInt("AnthonyIsDead"));
        }
        else
        {
            anthonyIsDead = false;
        }
        selectLevel = 1;
        gunLevel = 1;
    }

    //存贮Json文件
    private void SaveByJson()
    {
        string filePath = Application.streamingAssetsPath + "/Stories.json";
        string saveJsonStr = JsonMapper.ToJson(stories);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
    }
#if UNITY_STANDALONE_WIN
    //读取武器属性信息
    private void LoadWeaponProperty()
    {
        weaponPropertiesList = new List<WeaponProperties>();
        string filePath = Application.streamingAssetsPath + "/WeaponProperties.json";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            weaponPropertiesList = JsonMapper.ToObject<List<WeaponProperties>>(jsonStr);
        }
        if (weaponPropertiesList.Count==0)
        {
            Debug.Log("读取武器信息失败");
        }
    }
    //读取武器属性信息
    private void LoadStories()
    {
        string filePath = Application.streamingAssetsPath + "/Stories.json";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            stories = JsonMapper.ToObject<string[]>(jsonStr);
        }
        if (stories.Length==0)
        {
            Debug.Log("读取故事情节失败");
        }
    }
#elif UNITY_ANDROID
    //读取武器属性信息
    private IEnumerator LoadWeaponProperty()
    {
        weaponPropertiesList = new List<WeaponProperties>();
        string filePath = Application.streamingAssetsPath + "/WeaponProperties.json";
        //www
        //WWW www = new WWW(filePath);
        //while (!www.isDone)
        //{
        //    yield return null;
        //}
        //string jsonStr = www.text;
        //weaponPropertiesList = JsonMapper.ToObject<List<WeaponProperties>>(jsonStr);
        //if (weaponPropertiesList.Count == 0)
        //{
        //    Debug.Log("读取武器信息失败");
        //}
        //UnityWebRequest
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        string jsonStr = unityWebRequest.downloadHandler.text;
        weaponPropertiesList = JsonMapper.ToObject<List<WeaponProperties>>(jsonStr);
        if (weaponPropertiesList.Count == 0)
        {
            Debug.Log("读取武器信息失败");
        }
    }
    //读取故事情节
    private IEnumerator LoadStories()
    {
        string filePath = Application.streamingAssetsPath + "/Stories.json";
        //www
        //WWW www = new WWW(filePath);
        //while (!www.isDone)
        //{
        //    yield return null;
        //}
        //string jsonStr = www.text;
        //stories = JsonMapper.ToObject<string[]>(jsonStr);
        //if (stories.Length == 0)
        //{
        //    Debug.Log("读取故事情节失败");
        //}
        //UnityWebRequest
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        string jsonStr = unityWebRequest.downloadHandler.text;
        stories = JsonMapper.ToObject<string[]>(jsonStr);
        if (stories.Length == 0)
        {
            Debug.Log("读取故事情节失败");
        }
    }
#endif
    /// <summary>
    /// 加载主场景
    /// </summary>
    public void LoadMainScene()
    {
        Invoke("ReturnToMainScene",2);
    }
    private void ReturnToMainScene()
    {
        PlayerPrefs.SetInt("Money", GameManager.Instance.money);
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// 存档bool数组
    /// </summary>
    /// <param name="key"></param>
    /// <param name="boolArray"></param>
    public void SetBoolArray(string key,params bool[] boolArray)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < boolArray.Length-1; i++)
        {
            sb.Append(boolArray[i]).Append("|");
        }
        sb.Append(boolArray[boolArray.Length-1]);
        PlayerPrefs.SetString(key,sb.ToString());
    }
    /// <summary>
    /// 读档bool数组
    /// </summary>
    /// <param name="key"></param>
    public bool[] GetBoolArray(string key)
    {
        string[] stringArray = PlayerPrefs.GetString(key).Split('|');
        bool[] boolArray = new bool[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
            boolArray[i] = Convert.ToBoolean(stringArray[i]);
        }
        return boolArray;
    }
}


