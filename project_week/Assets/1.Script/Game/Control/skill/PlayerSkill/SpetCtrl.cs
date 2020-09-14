using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class SpetCtrl : MonoBehaviour, IPause
    {
        [SerializeField] Animator _ani;
        [SerializeField] SpriteRenderer _render;
        [SerializeField] Transform _pos;

        bool _isUse;

        GameScene _gs;
        PlayerCtrl _player;

        Action _maxup;

        Vector3 closedMob;

        public Transform Pos { get => _pos; }

        public void Init(GameScene gs, Action up)
        {
            _gs = gs;
            _player = gs.Player;
            _maxup = up;
        }

        public void appear(int lvl)
        {
            if (lvl == 3)
            {
                if (UnityEngine.Random.Range(0, 10) == 0)
                {
                    _ani.SetTrigger("up1");
                    _maxup();
                }
                else
                {
                    _ani.SetTrigger("up0");
                }
            }
            else if(lvl == 1)
            {
                _isUse = true;
                transform.position = _player.transform.position;
                _ani.SetTrigger("standard");
                StartCoroutine(play());
            }
        }

        IEnumerator play()
        {
            while (_isUse)
            {
                if (Vector3.Distance(transform.position, _player.transform.position) > 1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, 3f * Time.deltaTime);
                    _render.flipX = (_player.transform.position.x - transform.position.x > 0) ? true : false;
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        public void onPause(bool bl)
        {
            _ani.speed = (bl) ? 0f : 1f;
        }
    }
}