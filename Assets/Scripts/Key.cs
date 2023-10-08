using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制机关门打开的钥匙
/// </summary>
public class Key : MonoBehaviour
{
    public GameObject locked;
    public GameObject unLocked;

    public void OpenDoor()
    {
        locked.SetActive(false);
        unLocked.SetActive(true);
    }
}
