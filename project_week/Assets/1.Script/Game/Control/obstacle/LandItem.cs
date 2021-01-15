﻿using System;
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
            _tem.SetActive(true);
        }

        IEnumerator respone()
        {
            float t = 0f;

            while (true)
            {
                t += Time.deltaTime;

                if (t > regenTime)
                {
                    t = 0f; 
                    _tem.SetActive(true);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
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
                    float val = _player.MaxHp * gameValues._healpackVal;
                    _player.getHealed(val);
                    StartCoroutine(respone());
                    break;
                case landtem.gem:
                    _gs.getGem();
                    break;
                case landtem.present:
                    _gs.getEquip();
                    getEquip?.Invoke();
                    break;
                case landtem.sward:
                    if (_player.IsHero)
                    {
                        _player.setDeBuff(eBuff.att, 60f, 2);
                        _player.setDeBuff(eBuff.def, 60f, 2);
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