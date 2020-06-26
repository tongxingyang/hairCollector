using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class hairCoinControl : MonoBehaviour
    {
        int value;
        float _speed;

        GameScene _gs;
        public bool isUse { get; set; }

        public void setting(GameScene gs)
        {
            _gs = gs;
        }

        public void Init()
        {
            int val = Random.Range(0, 10);
            if (val == 9)
            {
                transform.localScale = Vector3.one * 2;
                value = 1;
                _speed = 1;
            }
            else if (val >= 6)
            {
                transform.localScale = Vector3.one * 1.5f;
                value = 3;
                _speed = 2;
            }
            else
            {
                transform.localScale = Vector3.one;
                value = 5;
                _speed = 3;
            }
            isUse = true;
            gameObject.SetActive(true);
        }

        public void Destroy()
        {
            Debug.Log("파괴");
            isUse = false;
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isUse)
            {
                transform.position += Vector3.down * _speed * Time.deltaTime;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _gs.getHair(value);
            }

            Destroy();
        }
    }
}