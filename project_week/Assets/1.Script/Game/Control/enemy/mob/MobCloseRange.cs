using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class MobCloseRange : MobControl
    {
        [SerializeField] Collider2D[] col;
        [SerializeField] MobWeapon[] _weapon;

        protected override void otherWhenFixInit()
        {
            setEvent();
            foreach (MobWeapon mw in _weapon)
            {
                mw.setting(_player, _att);
            }
        }

        protected override void otherWhenRepeatInit()
        {
            //_hp *= 10000;
        }

        protected override void otherWhenDie()
        {
        }

        protected override IEnumerator lifeCycle()
        { 
            float deltime = 0;
            while (IsUse)
            {
                deltime = Time.deltaTime;
                PlayerDist = Vector3.Distance(_player.transform.position, transform.position);
                transform.localScale = new Vector3((transform.position.x - _player.transform.position.x > 0) ? 1 : -1, 1);

                switch (_stat)
                {
                    case stat.trace:
                        switchAttckMove();
                        break;
                    case stat.attack:
                        applyAttack();
                        break;
                    case stat.die:
                        break;
                    default:
                        Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                        break;
                }

                if (_isCool)
                {
                    _coolTime += deltime;
                    if (_coolTime > _staticCool)
                    {
                        _isCool = false;
                        _coolTime = 0;
                    }
                }

                deBuffChk(deltime);
                chkDestroy(deltime);
                chkFrozen(deltime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        protected void applyAttack()
        {
            if (_isAtting)
            {
                return;
            }

            _isAtting = true;

            SetAnimation("attack", false, 1f);
        }

        void setEvent()
        {
            _spine.AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
            {
                if (e.Data.Name.Equals("endMotion"))
                {
                    switchStat(stat.trace);
                    SetAnimation("trace", true, 1f);
                    _isCool = true;
                    _isAtting = false;
                }
            };
        }

        protected override IEnumerator mopAttack()
        {
            yield return null;
        }

        protected override void switchStat(stat st)
        {
            _stat = st;
            switch (st)
            {
                case stat.trace:
                    foreach (Collider2D co in col)
                    {
                        co.gameObject.SetActive(false);
                    }
                    break;
                case stat.attack:
                    foreach (Collider2D co in col)
                    {
                        co.gameObject.SetActive(true);
                    }
                    break;
                case stat.die:
                    break;
                default:
                    Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                    break;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_patt);
            }
        }
    }
}