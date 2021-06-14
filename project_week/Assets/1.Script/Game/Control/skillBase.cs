using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class skillBase
    {
        protected SkillKeyList type;

        public string name;

        protected int lvl;
        protected int max_lvl;

        public string explain;
        public string information;
        public bool upGrade;
        protected bool val0_mul;

        public abstract void Init(SkillKeyList sk);
        public abstract void skillUp();

        public SkillKeyList Type { get => type; }
        public bool isMax { get { return lvl >= max_lvl; } }
        public bool active { get { return lvl > 0; } }
        public int Lvl { get => lvl; }
        public int MaxLvl { get => max_lvl; }
    }

    /// <summary> 능력치 </summary>
    public class ability : skillBase
    {
        public float val0;
        public float val0_increase { get; private set; }

        public Action<SkillKeyList> skUp { get; set; }

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            lvl = 0;

            max_lvl     = D_skill.GetEntity(sk.ToString()).f_max_level;

            explain     = D_skill.GetEntity(sk.ToString()).f_explain;
            information = D_skill.GetEntity(sk.ToString()).f_info;

            val0            = D_skill.GetEntity(sk.ToString()).f_val0;
            val0_increase   = D_skill.GetEntity(sk.ToString()).f_val0_increase;
            string m        = D_skill.GetEntity(sk.ToString()).f_val0_cal;
            val0_mul        = (m.Equals("m")) ? true : false;
        }

        public override void skillUp()
        {
            //Debug.Log(val0 + " / " + val0_increase);
            if (lvl < max_lvl)
            {
                skUp(type);

                if (val0_mul)
                    val0 *= val0_increase;
                else
                    val0 += val0_increase;

                lvl++;
            }
        }
    }

    /// <summary> 스킬 </summary>
    public class skill : skillBase
    {
        int skillRank;  // 스킬 랭크

        public float val0;  // 값0
        public float val1;  // 값1

        float val0_increase; // 값0 증가량
        float val1_increase; // 값1 증가량
        bool val1_mul;

        public float delay; // 쿨
        public float keep;  // 지속시간
        public int count;   // 개수
        public float size;  // 크기
        public float range; // 사거리

        float delay_reduce;  // 쿨 감소량
        float keep_increase; // 지속시간 증가량
        float size_increase; // 크기 증가량
        int count_increase;  // 개수 증가량

        public SkillKeyList _essential = SkillKeyList.non;      // 선행 스킬 필요 여부
        public List<SkillKeyList> _choice;  // 필요한 선행 스킬
        private bool _overrided;         // 진화형 오버라이드

        public inheritType Inherit { get; private set; }
        public bool IsLock { get => shouldHaveKey > 0; }        
        public int shouldHaveKey { get; private set; }
        public void unLock() { shouldHaveKey = 0; }
        public Action<SkillKeyList> OverChk { get; set; }

        public float _timer;        // 쿨 타이머

        float rangeDelay;

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            _timer = 0;

            lvl = 0;
            max_lvl = D_skill.GetEntity(sk.ToString()).f_max_level;

            skillRank = D_skill.GetEntity(sk.ToString()).f_rank % 10;

            name        = D_skill.GetEntity(sk.ToString()).f_skill_name;
            explain     = D_skill.GetEntity(sk.ToString()).f_explain;
            information = D_skill.GetEntity(sk.ToString()).f_info;

            val0            = D_skill.GetEntity(sk.ToString()).f_val0;
            val0_increase   = D_skill.GetEntity(sk.ToString()).f_val0_increase;
            string m        = D_skill.GetEntity(sk.ToString()).f_val0_cal;
            val0_mul        = (m.Equals("m")) ? true : false;

            val1            = D_skill.GetEntity(sk.ToString()).f_val1;
            val1_increase   = D_skill.GetEntity(sk.ToString()).f_val1_increase;
            m               = D_skill.GetEntity(sk.ToString()).f_val1_cal; ;
            val1_mul        = (m.Equals("m")) ? true : false;

            delay           = D_skill.GetEntity(sk.ToString()).f_delay;
            keep            = D_skill.GetEntity(sk.ToString()).f_keep;
            count           = D_skill.GetEntity(sk.ToString()).f_count;
            size            = 1;

            Inherit = D_skill.GetEntity(sk.ToString()).f_inheritType;

            delay_reduce    = D_skill.GetEntity(sk.ToString()).f_delay_reduce;
            keep_increase   = D_skill.GetEntity(sk.ToString()).f_keep_increase;
            size_increase   = D_skill.GetEntity(sk.ToString()).f_size_increase;
            count_increase  = D_skill.GetEntity(sk.ToString()).f_count_increase;
            range           = D_skill.GetEntity(sk.ToString()).f_range;

            SkillKeyList skl = D_skill.GetEntity(sk.ToString()).f_essential;
            if (skl != SkillKeyList.non)
            {
                _essential = skl;
                _choice = new List<SkillKeyList>();

                for (int i = 0; i < 2; i++)
                {
                    skl = D_skill.GetEntity(sk.ToString()).f_choice0;
                    if (skl != SkillKeyList.non)
                    {
                        _choice.Add(skl);
                        
                        skl = D_skill.GetEntity(sk.ToString()).f_choice1;
                        if (skl != SkillKeyList.non)
                        {
                            _choice.Add(skl);
                        }
                    }                    
                }
            }                   

            _overrided = false;

            // lock 체크
            shouldHaveKey = D_skill.GetEntity(type.ToString()).f_medal;
        }

        public override void skillUp()
        {
            if (lvl == 0)
            {
                switch (Inherit)
                {
                    case inheritType.rebase:
                    case inheritType.non:
                    case inheritType.medal:
                        break;
                    case inheritType.union:
                        OverChk(_essential);
                        for (int i = 0; _choice != null && i < _choice.Count; i++)
                        {
                            OverChk(_choice[i]);
                        }
                        break;
                    case inheritType.over:
                    case inheritType.overmedal:
                        OverChk(_essential);
                        break;
                }
            }
            else
            {
                if (val0_mul) // 값0
                    val0 *= val0_increase;
                else
                    val0 += val0_increase;

                if (val1_mul) // 값1
                    val1 *= val1_increase;
                else
                    val1 += val1_increase;

                delay -= delay_reduce;
                keep += keep_increase;
                count += count_increase;
                size *= size_increase;
            }

            lvl++;
        }

        public void overrideOff()
        {
            _overrided = true;
        }

        public bool chk_Time(float delta, float cool)
        {
            if (_overrided)
                return false;                       

            if (lvl > 0)
            {
                _timer += delta;

                if (_timer > delay * cool)
                {
                    _timer = 0;
                    return true;

                }
                return false;
            }
            return false;
        }

        public bool chk_shotable(float delta, float cool, float dist)
        {
            if (_overrided)
                return false;

            if (lvl > 0)
            {
                _timer += delta;
                // Debug.Log(_timer +" > "+ delay * cool);
                if (_timer > delay * cool)
                {
                    if (dist < range)
                    {
                        _timer = 0;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        //public bool chk_rangeshotable(float delta, float cool, float dist)
        //{
        //    if (lvl > 0)
        //    {
        //        if (_timer <= 0f)
        //        {
        //            rangeDelay = UnityEngine.Random.Range(1.5f, delay);
        //        }

        //        _timer += delta;
        //        if (_timer > rangeDelay * cool)
        //        {
        //            if (dist < range)
        //            {
        //                _timer = 0;
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    return false;
        //}

        public void clear()
        {
            Init(type);
        }
    }
}