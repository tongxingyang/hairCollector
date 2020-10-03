using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MobShooter : MobControl
    {
        [SerializeField] Transform[] _shotPos;
        [SerializeField] bool _isCurved;
        float _shootTime;
        float _pAngle;

        EnShot _shotType = EnShot.fireball;

        bool isInRange
        {
            get 
            {
                if (_isCurved)
                {
                    return PlayerDist < 5f;
                }
                else
                {
                    return true;
                }
                
            }
        }

        protected override void otherWhenFixInit()
        {
            if (getType == Mob.mob_monkey)
            {
                _shotType = EnShot.banana; 
            }

            _dotDmg = new dotDmg();
        }

        protected override void otherWhenRepeatInit()
        {
            applyAttack();
            _shotTerm = 0.5f;
        }

        protected void applyAttack()
        {
            _pAngle = 10f;
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
                _render.flipX = (_player.transform.position.x - transform.position.x > 0) ? false : true;

                switch (_stat)
                {
                    case stat.trace:
                        mopTraceLongMove();
                        break;
                    case stat.die:
                        break;
                    default:
                        Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                        break;
                }

                _shootTime += deltime;

                if (_shootTime > _pspeed && isInRange)
                {
                    _shootTime = 0;

                    _esc = (EnSkillControl)_enProjMng.makeEnProj(_shotType);
                    _esc.transform.position = _shotPos[0].position;

                    _esc.operation(_player.transform.position, 0);
                }

                _dotDmg.dotDmging(deltime);
                chkDestroy(deltime);
                chkFrozen(deltime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        protected override void switchStat(stat st)
        {
            _stat = st;

            switch (st)
            {
                case stat.trace:                    
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

        protected override void setColor()
        {
            _render.color = _originColor;
        }

        protected override IEnumerator damageAni()
        {
            _isDmgAction = true;
            _render.color = new Color(1, 0.4f, 0.4f);

            yield return new WaitForSeconds(0.1f);

            _render.color = _originColor;

            yield return new WaitForSeconds(0.1f);

            _render.color = new Color(1, 0.4f, 0.4f);

            yield return new WaitForSeconds(0.1f);

            _render.color = _originColor;
            _isDmgAction = false;
        }

        public override void onPause(bool bl)
        {
            _ani.speed = (bl) ? 0 : 1f;
        }
    }
}