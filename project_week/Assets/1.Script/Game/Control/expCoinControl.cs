using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class expCoinControl : MonoBehaviour
    {
        [SerializeField] Sprite[] _sprites;
        SpriteRenderer _renderer;
        public bool isExp { get; set; }
        float _speed;
        float value;

        public Action<int> expFunc { private get; set; }
        public Action<cointype> coinFunc { private get; set; }
        public bool isUse { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            isUse = true;

            if (UnityEngine.Random.Range(0, 11) < 2) // 코인
            {
                isExp = false;
                _renderer.sprite = _sprites[1];

                transform.localScale = Vector3.one * 0.75f;
                value = 10;
                _speed = 4;
            }
            else // 경치
            {
                isExp = true;
                _renderer.sprite = _sprites[0];

                int val = UnityEngine.Random.Range(0, 10);
                //if (val == 9)
                //{
                //    transform.localScale = Vector3.one * 0.5f;
                //    value = 10;
                //    _speed = 6;
                //}
                if (val > 6)
                {
                    transform.localScale = Vector3.one * 0.75f;
                    value = 8;
                    _speed = 2;
                }
                else
                {
                    transform.localScale = Vector3.one * 1f;
                    value = 3;
                    _speed = 2;
                }
            }

            gameObject.SetActive(true);
        }

        public void Destroy()
        {
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
                if (isExp)
                {
                    expFunc((int)value);
                    SoundManager.instance.PlaySFX(SFX.exp);
                }
                else
                {
                    coinFunc(cointype.extraCoin);
                    SoundManager.instance.PlaySFX(SFX.coin);
                }

                Destroy();
            }
            else if (collision.tag.Equals("Finish"))
            {
                Destroy();
            }

        }
    }
}