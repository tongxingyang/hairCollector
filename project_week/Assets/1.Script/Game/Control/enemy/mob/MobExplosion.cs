using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MobExplosion : MobControl
    {
        protected override void otherWhenFixInit()
        {
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

                deBuffChk(deltime);
                chkDestroy(deltime);
                chkFrozen(deltime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_att);
                enemyDie();
            }
            else if (collision.gameObject.tag.Equals("Finish"))
            {
                Destroy();
            }
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
                    break;
                case stat.die:
                    break;
                default:
                    Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                    break;
            }
        }
    }
}