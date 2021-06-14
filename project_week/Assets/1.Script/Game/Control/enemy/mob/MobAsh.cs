using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MobAsh : MobControl
    {
        protected override float Att { get { return _standardStt[(int)enemyStt.ATT]; } }
        Vector3 movePos;
        protected override void otherWhenFixInit()
        {
        }

        protected override void otherWhenRepeatInit()
        {
            movePos = (_player.transform.position - transform.position).normalized;
        }

        protected override void otherWhenDie()
        {
        }
        protected override IEnumerator lifeCycle()
        {
            _render.flipX = (_player.transform.position.x - transform.position.x > 0) ? false : true;

            while (IsUse)
            {
                deltime = Time.deltaTime;
                PlayerDist = Vector3.Distance(_player.transform.position, transform.position);

                switch (_stat)
                {
                    case stat.trace:
                        TraceMove();
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

        protected override void check_mobStair(float deltime)
        {
            chkDestroy(deltime);

            if (_isFrozen) 
                selfEnemyDie();

            if (!_closed)
            {
                if (PlayerDist < 6f)
                {
                    _outOfViewSpeed = 1f;
                    _closed = true;
                }
                else
                {
                    _outOfViewSpeed = 3f;
                }
            }
        }

        protected void TraceMove()
        {
            transform.position += movePos * Speed * Time.deltaTime;
        }

        protected override void switchStat(stat st)
        {
        }

        /// <summary> 독뎀 적용 안함 </summary>
        protected override void chkDotDmg(float del)
        {

        }
        /// <summary> 받은 데미지~방어력 계산해서 </summary>
        public override float getDamaged(attackData data)
        {            
            return 0;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                float dmg = _player.MaxHp * 0.01f * Att;
                // Debug.Log(dmg + " = " + _player.MaxHp + " ~ " + Att);
                _player.getDamaged(this, dmg);

                selfEnemyDie();
            }
        }
    }
}