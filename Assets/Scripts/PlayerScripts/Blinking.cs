using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{
    public GameObject blink;
    private float time;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer<=0)
        {
            if (blink.activeSelf)
            {
                blink.SetActive(false);
            }
            else
            {
                blink.SetActive(true);
            }
            timer = time;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
