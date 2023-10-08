using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Game场景中的UI管理
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public Slider hpSlider;
    public Slider bulletSlider;
    public CanvasGroup canvasGroup;
    public GameObject joyStick;
    public Image imgWeapon;
    public Sprite[] weaponSprite;
    private GameManager gameManager;
    public Image imgAttackCD;
    public Image imgReloadCD;
    public Image imgLeftMine;
    public Image imgTurret;
    public RenderTexture renderTexture;
    public RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.gameUIManager = this;
#if UNITY_STANDALONE_WIN
        canvasGroup.interactable = false;
        imgWeapon.transform.parent.gameObject.SetActive(true);
        imgWeapon.sprite = weaponSprite[gameManager.gunLevel - 1];
        joyStick.SetActive(false);
#elif UNITY_ANDROID
        canvasGroup.interactable = true;
        imgWeapon.transform.parent.gameObject.SetActive(false);
        joyStick.SetActive(true);
        gameManager.inputValue = Vector2.zero;
        gameManager.inputAngle = 0;
#endif
    }

    #region UI更新方法
    /// <summary>
    /// 更新血条
    /// </summary>
    public void UpdateHPSlider(float value)
    {
        hpSlider.value = value;
    }
    /// <summary>
    /// 更新子弹条
    /// </summary>
    /// <param name="value"></param>
    public void UpdateBulletSlider(float value)
    {
        bulletSlider.value = value;
    }
    /// <summary>
    /// 炮塔是否使用UI显示
    /// </summary>
    public void UpdateTurretUI(bool hasTurret)
    {
        if (hasTurret)
        {
            imgTurret.fillAmount = 1;
        }
        else
        {
            imgTurret.fillAmount = 0;
        }
    }
    /// <summary>
    /// 剩余地雷个数的UI显示
    /// </summary>
    /// <param name="value"></param>
    public void UpdateMineUI(float value)
    {
        imgLeftMine.fillAmount = value;
    }
    /// <summary>
    /// 攻击CD
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAttackCDUI(float value)
    {
        imgAttackCD.fillAmount = value;
    }
    /// <summary>
    /// 装弹时显示装弹完成剩余时间，装弹完毕后显示弹夹中子弹剩余数量
    /// </summary>
    public void UpdateReloadAndMagazineBulletUI(float value)
    {
        imgReloadCD.fillAmount = value;
    }
    #endregion
    #region 按钮方法
    public void SetMine()
    {
        gameManager.player.playerShooting.SetMine();
    }
    public void SetTurret()
    {
        gameManager.player.playerShooting.SetTurret();
    }
    public void Reload()
    {
        gameManager.player.playerShooting.Reload();
    }
    public void Attack()
    {
        gameManager.player.playerShooting.Shoot();
    }
    #endregion

}
