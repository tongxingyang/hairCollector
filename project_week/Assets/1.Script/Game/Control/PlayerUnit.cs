using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using week;

public class PlayerUnit : MonoBehaviour
{
    /// <summary> 조이스틱 </summary>
    [SerializeField] Joystick _joyStick = null;

    [Space(20)]

    [SerializeField] float _speed = 1f;

    [Space(20)]

    [SerializeField] SpriteRenderer[] renders;

    Vector2 _compVec;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            if (BaseManager.userEntity.Item[i] > -1)
            {
                renders[i].sprite = DataManager.GetTable<Sprite>((DataTable)i, BaseManager.userEntity.Item[i].ToString(), "sprite");
            }
            else
            {
                renders[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {        
        _compVec = (Vector2)transform.position + (_joyStick.Direction * _speed * Time.deltaTime);

        if (_compVec.x > 2.1f)
        {
            _compVec.x = 2.1f;
        }
        else if (_compVec.x < -2.1f)
        {
            _compVec.x = -2.1f;
        }

        if (_compVec.y > 3.5f)
        {
            _compVec.y = 3.5f;
        }
        else if (_compVec.y < -4.4f)
        {
            _compVec.y = -4.4f;
        }

        transform.position = _compVec;
    }
}
