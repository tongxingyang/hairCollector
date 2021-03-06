using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MobExplosion : MobControl
    {
        protected override void otherWhenFixInit()
        {
            _dotDmg = new dotDmg();
        }

        protected override void otherWhenRepeatInit()
        {
            _dotDmg.reset();
        }

        protected override void otherWhenDie()
        {
            switchStat(stat.die);
        }

        protected override IEnumerator lifeCycle()
        {
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

                check_mobStair(deltime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        public void selfEnemyDie()
        {
            if (_isDie == false)
            {
                // SoundManager.instance.PlaySFX(SFX.endie);
                _efMng.makeEff("selfEx", transform.position);
                otherWhenDie();
                Destroy();
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(this, Att);

                selfEnemyDie();
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
    }
}