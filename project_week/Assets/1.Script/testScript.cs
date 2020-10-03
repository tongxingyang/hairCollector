using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform[] aad;
    float runningTime;
    float yPos;
    [SerializeField] Vector3 _target = Vector3.right;

    // Update is called once per frame
    void Update()
    {


        transform.Translate(_target * 0.32f * 0.8f * Time.deltaTime, Space.Self);

        var quaterion = Quaternion.Euler(0, 0, 1f);// time * 60f);
        _target = quaterion * _target;

    }

    void updown()
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
