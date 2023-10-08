using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : ScrollRect
{
    private float radius;

    private Transform imgArrowTrans;



    protected override void Start()
    {
        base.Start();
        radius = content.sizeDelta.x * 0.5f;
        imgArrowTrans = transform.Find("Img_Arrow");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        Vector2 contentPosition = content.anchoredPosition;
        //判断摇杆的位置是否大于半径
        if (contentPosition.magnitude>radius)
        {
            //摇杆位置设置到边界处
            contentPosition = contentPosition.normalized * radius;
            SetContentAnchoredPosition(contentPosition);
        }
        Vector2 inputVector = content.anchoredPosition.normalized;
        GameManager.Instance.inputValue = inputVector;
        float inputAngle = -Mathf.Atan2(GameManager.Instance.inputValue.x, GameManager.Instance.inputValue.y) * Mathf.Rad2Deg;
        GameManager.Instance.inputAngle = inputAngle;
        imgArrowTrans.localRotation = Quaternion.Euler(new Vector3(0,0,inputAngle));
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        SetContentAnchoredPosition(Vector2.zero);
        GameManager.Instance.inputValue = Vector2.zero;
        imgArrowTrans.localRotation = Quaternion.identity;
    }
}
