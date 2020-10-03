using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class LandItem : MonoBehaviour
    {
        [SerializeField] landtem _temtype;
        [SerializeField] GameObject _tem;

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
            switch (_temtype)
            {
                case landtem.heal:
                    _player.getHealed(20);
                    StartCoroutine(respone());
                    break;
                case landtem.gem:
                    //_player.getGem();
                    break;
                case landtem.present:
                    _gs.getEquip();
                    getEquip();
                    break;
                case landtem.sward:
                    if (_player.IsHero)
                    {
                        Debug.Log("칼먹음");
                        _player.setDeBuff(snowStt.att, 60f, 2);
                        _player.setDeBuff(snowStt.def, 60f, 2);
                    }
                    else
                    {
                        return;
                    }
                    break;
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