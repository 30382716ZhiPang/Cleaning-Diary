using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 通关点脚本
/// </summary>
public class Finish : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.curKills>=player.kills&&player.bossIsDead)
            {
                PlayerPrefs.SetInt("Money",GameManager.Instance.money);
                GameManager.Instance.unLockedLevel += 1;
                PlayerPrefs.SetInt("Levels", GameManager.Instance.unLockedLevel);
                SceneManager.LoadScene(0);
            }
        }
    }
}
