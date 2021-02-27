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

        Vector3 closedMob;

        public Transform Pos { get => _pos; }

        public void Init(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;
        }

        public void setPet(SkillKeyList sk)
        {
            if (sk == SkillKeyList.Pet)
            {
                _isUse = true;
                transform.position = _player.transform.position;

                _ani.SetTrigger("standard");                
            }
            else if (sk == SkillKeyList.Pet2)
            {
                _ani.SetTrigger("up0");
            }
            else if (sk == SkillKeyList.BPet)
            {
                _ani.SetTrigger("up1");
            }
            else
            {
                Debug.LogError("wrong : " + sk.ToString());
            }

            StartCoroutine(play());
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