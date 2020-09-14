using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class MobOnlyFollow : MobControl
    {
        [SerializeField] SpriteRenderer _render;
        protected override void otherWhenFixInit()
        {
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

                deBuffChk(deltime);
                chkDestroy(deltime);
                chkFrozen(deltime);

                yield return new WaitUntil(() => _gs.Pause == false);
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

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_att);
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