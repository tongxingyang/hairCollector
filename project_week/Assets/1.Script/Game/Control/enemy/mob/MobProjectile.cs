using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class MobProjectile : MobControl
    {
        [SerializeField] Transform[] _shotPos;

        int p;
        float _pAngle;
        protected override void otherWhenFixInit()
        {
            setEvent();
        }

        protected override void otherWhenRepeatInit()
        {
            applyAttack();
            _shotTerm = 0.5f;
        }

        protected void applyAttack()
        {
            _pAngle = 10f;
            //StartCoroutine(mopAttack());
        }

        protected override IEnumerator mopAttack()
        {
            yield return null;
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

        protected override void switchStat(stat st)
        {
            _stat = st;

            switch (st)
            {
                case stat.trace:
                    SetAnimation("trace", true, 1f);
                    break;
                case stat.die:
                    break;
                default:
                    Debug.LogError($"잘못된 상태 : {_stat.ToString()}");
                    break;
            }
        }
        int pp;
        void setEvent()
        {
            _spine.state.Event += delegate (TrackEntry trackEntry, Spine.Event e)
            {
                if (e.Data.Name.Equals("attack"))
                {
                    p++;
                    if (p % 3 == 0)
                    {
                        _proj = (EnSkill_Proj)_enProjMng.makeEnProj(EnShot.fireball);
                        _proj.transform.position = _shotPos[p%2].position;
                        
                        _proj.operation(_player.transform.position, _pAngle);

                        if (p == 6) p = 0;
                    }
                }
            };
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