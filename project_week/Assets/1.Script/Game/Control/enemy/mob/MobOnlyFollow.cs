using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class MobOnlyFollow : MobControl
    {
        protected override void otherWhenFixInit()
        {
            _dotDmg = new dotDmg();
        }

        protected override void otherWhenRepeatInit()
        {
            _dotDmg.reset();
            //_hp *= 10000;
        }

        protected override void otherWhenDie()
        {
        }

        protected override IEnumerator lifeCycle()
        {
            while (IsUse)
            {
                deltime = Time.deltaTime;
                PlayerDist = Vector3.Distance(_player.transform.position, transform.position);
                _render.flipX = (_player.transform.position.x - transform.position.x > 0) ? false : true;

                switch (_stat)
                {
                    case stat.trace:
                        mopTraceMove();
                        break;
                    case stat.die:
                        break;
                    default:
                        Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                        break;
                }

                chkDotDmg(deltime);
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
                _player.getDamaged(Att);
            }
        }

        protected override void setColor()
        {
            _render.color = _originColor;
        }

        public override void onPause(bool bl)
        {
            _ani.speed = (bl) ? 0 : 1f;
        }
    }
}