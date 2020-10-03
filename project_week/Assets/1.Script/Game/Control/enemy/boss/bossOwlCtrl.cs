using DG.Tweening;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class bossOwlCtrl : spineBossControl
    {
        #region [skill value]

        [SerializeField] Transform _body;
        [SerializeField] GameObject[] _shadows;

        [Header("Skill")]
        [SerializeField] Transform skillA;
        EnSkill_Proj epc;
        int skillA_fireCount = 4;

        [SerializeField] MobWeapon[] _skillBWeapon;
        Vector3 skillB_Dir;
        bool _Bmove;
        
        bool _skill_finished;
        float skillCoolTime = 0;
        Vector2 _scale;

        #endregion

        #region [unit value]

        #endregion

        #region [util value]

        [Space]
        [Header("Util")]
        [SerializeField] Transform foot;

        Vector3 _dirVector;

        dir _prevDir;
        stat _prevStat;

        #endregion

        #region [Init - Destroy]

        protected override void otherWhenFixInit()
        {
            setEvent();
            foreach (MobWeapon mw in _skillBWeapon)
            {
                mw.setting(_player, _skill1);
            }
            _scale = _body.localScale;
        }

        protected override void otherWhenRepeatInit()
        {
            base.otherWhenRepeatInit();
            //_hp *= 10000;
            SetAnimation("idle", true, 1f);
        }

        /// <summary> 스파인 이벤트 세팅 </summary>
        void setEvent()
        {
            _spine.AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
            {
                if (e.Data.Name.Equals("endMotion"))
                {
                    Debug.Log("앤드모션");
                    _skill_finished = true;
                    switchStat(stat.Trace);
                    //_isCool = true;
                    //_isAtting = false;
                }
                else if (e.Data.Name.Equals("attack"))
                {
                    if (_stats == stat.skill0)
                    {
                        Debug.Log("어택0");
                        skillAShot();
                    }
                }
                else if (e.Data.Name.Equals("stompEnd"))
                {
                    if (_stats == stat.skill1)
                    {
                        _Bmove = false;
                    }
                }
            };
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
                        // skillAShot();
                        yield return new WaitUntil(() => _skill_finished == true);
                        break;
                    case stat.skill1:
                        yield return StartCoroutine(skillBShot());
                        break;
                    default:
                        break;
                }

                setDetailAnimation();
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
                Debug.Log(transform.position + " / " + _player.transform.position);
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

        /// <summary> 1번스킬 - 불회오리 발사 </summary>
        void skillAShot()
        {
            for (int i = 0; i < skillA_fireCount; i++)
            {
                epc = (EnSkill_Proj)_enProjMng.makeEnProj(EnShot.owl_shot);
                epc.transform.position = skillA.position;
                epc.transform.localScale = Vector3.one;
                epc.operation(_shotDir[i]);
            }
        }

        /// <summary> 2번스킬 - 발구르기 </summary>
        IEnumerator skillBShot()
        {
            _Bmove = true;

            while (_skill_finished == false)
            {
                if (_Bmove)
                {
                    transform.Translate(skillB_Dir * Speed * 1.75f * Time.deltaTime);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        #endregion

        #region [util]

        public override void whenEnemyEnter()
        {
        }

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

            _body.transform.localScale = _scale;
            if (Mathf.Abs(_dirVector.x) > Mathf.Abs(_dirVector.y))
            {
                if (_dirVector.x > 0)
                {
                    _body.transform.localScale = new Vector3(-_scale.x, _scale.y);
                    _dir = dir.right;
                }
                else
                {
                    _dir = dir.left;
                }
            }
            else
            {
                _dir = (_dirVector.y > 0) ? dir.back : dir.front;
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
                    Debug.Log("what");
                    break;
                case stat.skill0:
                    _skill_finished = false;
                    break;
                case stat.skill1:
                    _skill_finished = false;
                    skillB_Dir = (_player.transform.position - transform.position).normalized;
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
                    _shadows[0].SetActive(true);
                    _shadows[1].SetActive(false);
                    break;
                case dir.right:
                case dir.left:
                    d_str = "side-";
                    _shadows[1].SetActive(true);
                    _shadows[0].SetActive(false);
                    break;
            }

            string s_str = "";
            switch (_stats)
            {
                case stat.Idle:
                    s_str = "idle";
                    loop = true;
                    SetAnimation(s_str, loop, spd);
                    return;
                case stat.back:
                    s_str = "walk";
                    loop = true;
                    break;
                case stat.Trace:
                    s_str = "walk";
                    loop = true;
                    break;
                case stat.skill0:
                    s_str = "wingAttack"; 
                    loop = false;
                    break;
                case stat.skill1:
                    s_str = "stomp"; 
                    loop = false;
                    spd = 0.75f;
                    break;
                default:
                    break;
            }

            SetAnimation(d_str + s_str, loop, spd);
        }

        #endregion
    }
}