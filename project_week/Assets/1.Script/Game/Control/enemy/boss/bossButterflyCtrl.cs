using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class bossButterflyCtrl : spineBossControl
    {
        #region [skill value]
        [SerializeField] Transform _body;
        EnPoisonAreaCtrl _area;
        float _scale;

        [Space]
        [Header("Skill")]
        [SerializeField] Transform _shotPos;
        EnSkill_Proj esp;
        int sk_A_shotCnt = 12;
        int sk_B_cnt = 4;
        int sk_B_shotCnt = 3;

        float skillCoolTime = 0;

        #endregion

        #region [util value]

        Vector3 _dirVector;

        dir _prevDir;
        stat _prevStat;

        #endregion

        #region [Init - Destroy]

        protected override void otherWhenFixInit()
        {
            _scale = _body.localScale.x;
            _area = GetComponentInChildren<EnPoisonAreaCtrl>();
            _area.Init(_gs);
        }

        protected override void otherWhenRepeatInit()
        {
            base.otherWhenRepeatInit();

            switchStat(stat.Idle);

            //_hp *= 10000;
            SetAnimation("front-trace", true, 1f);
        }

        protected override void otherWhenDie()
        {
        }

        #endregion        

        protected override IEnumerator lifeCycle()
        {
            //StartCoroutine(chkPlayer());

            while (_isDie == false)
            {
                deltime = Time.deltaTime;

                switch (_stats)
                {
                    case stat.Idle:
                        idle();
                        break;
                    case stat.back:
                        back();
                        break;
                    case stat.Trace:
                        trace();
                        break;
                    case stat.skill0:
                        yield return StartCoroutine(skillAShot());
                        break;
                    case stat.skill1:
                        yield return StartCoroutine(skillBShot());
                        break;
                    default:
                        break;
                }

                setDetailAnimation(); 
                chkDotDmg();

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return null;
        }

        #region [haviour]

        /// <summary> 대기 </summary>
        void idle()
        {
            if (Vector3.Distance(transform.position, _player.transform.position) < _bossRange)
            {
                skillCoolTime = 0;
                switchStat(stat.Trace);
            }
        }

        /// <summary> 귀환 </summary>
        void back()
        {
            transform.position = Vector3.MoveTowards(transform.position, _homePos, Speed * 1.6f * Time.deltaTime);

            if (Vector3.Distance(transform.position, _homePos) < 0.5f)
            {
                switchStat(stat.Idle);
            }
        }

        /// <summary> 추적 </summary>
        void trace()
        {
            if (Vector3.Distance(transform.position, _homePos) < 10f)
            {
                skillCoolTime += Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Speed * Time.deltaTime);

                if (skillCoolTime > _bossSkillCool)
                {
                    skillCoolTime = 0;
                    if (Random.Range(0, 2) == 0)
                    {
                        switchStat(stat.skill0);
                    }
                    else
                    {
                        switchStat(stat.skill1);
                    }
                }
            }
            else
            {
                switchStat(stat.back);
            }
        }

        /// <summary> 1번스킬 - 독뿌리기 </summary>
        IEnumerator skillAShot()
        {
            for (int i = 0; i < sk_A_shotCnt; i++)
            {
                esp = (EnSkill_Proj)_enProjMng.makeEnProj(EnShot.bfly_bgPoison, Skill0);
                esp.transform.position = _shotPos.position;
                esp.operation(Random.Range(0f, 360f) * i);

                yield return new WaitForSeconds(0.15f);
            }

            switchStat(stat.Trace);
        }

        /// <summary> 2번스킬 - 순동 </summary>
        IEnumerator skillBShot()
        {
            for (int i = 0; i < sk_B_cnt; i++)
            {
                yield return new WaitForSeconds(0.25f);

                _efMng.makeEff("tel", transform.position);
                yield return new WaitForSeconds(0.16f);

                SoundManager.instance.PlaySFX(SFX.bossTelpo);
                transform.position = _homePos + (Vector3)(Random.insideUnitCircle * 5.5f);

                for (int j = 0; j < sk_B_shotCnt; j++)
                {
                    esp = (EnSkill_Proj)_enProjMng.makeEnProj(EnShot.bfly_smPoison, Skill1);
                    esp.transform.position = _shotPos.position;
                    esp.operation(120f * j - 10f);
                    esp = (EnSkill_Proj)_enProjMng.makeEnProj(EnShot.bfly_smPoison, Skill1);
                    esp.transform.position = _shotPos.position;
                    esp.operation(120f * j + 10f);
                }

                yield return new WaitForSeconds(0.25f);
            }

            switchStat(stat.Trace);
        }

        #endregion

        #region [util]

        public override void whenEnemyEnter()
        { }

        /// <summary> 목적을 향한 방향 체크 </summary>
        void checkDir()
        {
            if (_stats == stat.back)
            {
                _dirVector = (_homePos - transform.position).normalized;
            }
            else
            {
                _dirVector = (_player.transform.position - transform.position).normalized;
            }

            if (_dirVector.y > 0 && Mathf.Abs(_dirVector.x) < Mathf.Abs(_dirVector.y))
            {
                _dir = dir.back;
                _body.localScale = new Vector3((_dirVector.x > 0) ? -_scale : _scale, _scale);
            }
            else
            {
                _dir = dir.front;
                _body.localScale = new Vector3((_dirVector.x > 0) ? _scale : -_scale, _scale);
            }
        }

        /// <summary> 상태변화 </summary>
        protected override void switchStat(stat st)
        {
            _stats = st;
            switch (st)
            {
                case stat.Idle:
                    break;
                case stat.back:
                    break;
                case stat.Trace:
                    break;
                case stat.skill0:
                    break;
                case stat.skill1:
                    break;
                default:
                    break;
            }
        }

        /// <summary> 알맞은 애니메이션 변경 </summary>
        void setDetailAnimation()
        {
            checkDir();

            if (_prevDir == _dir && _stats == _prevStat)
            {
                return;
            }

            _prevDir = _dir;
            _prevStat = _stats;

            string d_str = "";
            bool loop = false;
            float spd = 1f;

            switch (_dir)
            {
                case dir.back:
                case dir.front:
                    d_str = $"{_dir.ToString()}-";
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + _dir.ToString());
                    break;
            }

            string s_str = "trace";
            loop = true;
            switch (_stats)
            {
                case stat.Idle:
                case stat.back:
                case stat.Trace:
                    spd = 1f;
                    break;
                case stat.skill0:
                case stat.skill1:
                    spd = 2f;
                    break;
                default:
                    break;
            }

            SetAnimation(d_str + s_str, loop, spd);
        }

        #endregion
    }
}