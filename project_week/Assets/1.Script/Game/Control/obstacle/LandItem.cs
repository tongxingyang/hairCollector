using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class LandItem : MonoBehaviour
    {
        [SerializeField] GameObject _tem;
        [SerializeField] Animator _ani;
        [SerializeField] bool _responable;

        GameScene _gs;
        PlayerCtrl _player;

        float regenTime = 15f;

        Action getEquip;

        public void Init(GameScene gs, Action _getEquip)
        {
            _gs = gs;
            _player = gs.Player;
            getEquip = _getEquip;
        }

        IEnumerator respone()
        {
            yield return new WaitForSeconds(regenTime);
            _tem.SetActive(true);
        }

        public void presentRespone()
        {
            _tem.SetActive(true);
        }

        void chkRespone()
        {
            if (_responable) // 리스폰 가능 - 힐팩
            {
                _player.getHealed(20);
                StartCoroutine(respone());
            }
            else // 전설 장비
            {
                _gs.getEquip();
                getEquip();
            }

            _tem.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag.Equals("Player"))
            {
                if (collision.GetComponent<PlayerCtrl>() == null)
                {
                    Debug.Log("누구냐 : " + collision.name);
                }
                chkRespone();
            }
        }
    }
}