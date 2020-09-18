using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform[] aad;
    float runningTime;
    float yPos;
    // Update is called once per frame
    void Update()
    {
        runningTime += Time.deltaTime;
        for (int i = 0; i < 2; i++)
        {
            yPos = Mathf.Sin(runningTime + 0.75f * i);
            Debug.Log(yPos + " / " + runningTime);
            aad[i].transform.position = new Vector2(2 * i, yPos);
        }
    }
}
