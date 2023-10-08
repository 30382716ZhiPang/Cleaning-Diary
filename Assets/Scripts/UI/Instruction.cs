using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    private float viewDistance;

    private TextMesh textMesh;

    private float distance;

    private Transform playerTrans;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        viewDistance = 1.5f;
        playerTrans = GameManager.Instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTrans==null)
        {
            return;
        }
        distance = Vector3.Distance(transform.position,playerTrans.position);
        if (distance<viewDistance)
        {
            textMesh.characterSize = 0.05f * (1f - distance / viewDistance);
        }
        else
        {
            textMesh.characterSize = 0;
        }
    }
}
