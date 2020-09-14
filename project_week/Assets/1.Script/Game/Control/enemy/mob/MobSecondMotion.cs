using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using GooglePlayGames.BasicApi.Multiplayer;

namespace week
{
    public class MobSecondMotion : MobControl
    {
        [SerializeField] MobWeapon[] _weapon;

        Vector3 _targetPos;
        float t_2nd;
        float dmg;

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
        }

        protected override void otherWhenDie()
        {
            switchStat(stat.die);
        }

        protected override IEnumerator lifeCycle()
        {
            float deltime = 0;
            while (IsUse)
            {
                deltime = Time.deltaTime; 
                PlayerDist = Vector3.Distance(_player.transform.position, transform.position);

                if (_stat != stat.attack2)
                {
                    transform.localScale = new Vector3((transform.position.x - _player.transform.position.x > 0) ? 1 : -1, 1);
                }

                switch (_stat)
                {
                    case stat.trace:
                        switch2ndAttMove();
                        break;
                    case stat.attack:
                        applyAttack();
                        break;
                    case stat.attack2:
                        apply2ndAttack();
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

                yield return new WaitUntil(()=>_gs.Pause == false);
            }
        }

        protected void applyAttack()
        {
            _isAtting = true;
        }
        
        protected void apply2ndAttack()
        {
            if (_isFrozen == false)
            {
                transform.position += _targetPos * _pspeed * _speedFactor * Time.deltaTime;
            }

            t_2nd += Time.deltaTime;

            if (t_2nd > 1f)
            {
                switchStat(stat.trace);
                t_2nd = 0;
                _isCool = true;
            }
        }

        void setEvent()
        {
            _spine.state.Event += delegate (TrackEntry trackEntry, Spine.Event e)
            {
                if (e.Data.Name.Equals("endMotion"))
                {
                    switchStat(stat.trace);
                    
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
                    SetAnimation("trace", true, 1f);
                    dmg = _patt;
                    foreach (MobWeapon mw in _weapon)
                    {
                        mw.active(false);
                    }
                    _isCool = true;
                    _isAtting = false;
                    break;
                case stat.attack:
                    dmg = _att;
                    SetAnimation("attack", false, 1f);
                    foreach (MobWeapon mw in _weapon)
                    {
                        mw.active(true);
                    }
                    _staticCool = 1;
                    break;
                case stat.attack2:
                    dmg = _att;
                    SetAnimation("rush", true, 1f);
                    _targetPos = (_player.transform.position - transform.position).normalized;
                    _staticCool = 3;
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
                _player.getDamaged(dmg);

                if (_stat == stat.attack2)
                {
                    switchStat(stat.trace);
                }
            }
        }
    }
}