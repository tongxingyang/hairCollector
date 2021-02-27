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
        public bool chk_lvl { get { return lvl < max_lvl; } }
        public bool active { get { return lvl > 0; } }
        public int Lvl { get => lvl; }
        public int MaxLvl { get => max_lvl; }
    }

    /// <summary> 능력치 </summary>
    public class ability : skillBase
    {
        public float val0;
        float val0_increase;

        public Action<SkillKeyList> skUp { get; set; }

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            lvl = 0;

            max_lvl = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.max_level.ToString());

            explain = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.info.ToString());

            val0             = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val0.ToString());
            val0_increase    = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val0_increase.ToString());
            string m = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.val0_cal.ToString());
            val0_mul = (m.Equals("m")) ? true : false;
        }

        public override void skillUp()
        {
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

        public SkillKeyList _essential = SkillKeyList.max;      // 선행 스킬 필요 여부
        public List<SkillKeyList> _choice;  // 필요한 선행 스킬
        private bool _overrided;         // 진화형 오버라이드

        public inheritType _inherit;

        public Action<SkillKeyList> OverChk { get; set; }

        public float _timer;        // 쿨 타이머

        float rangeDelay;

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            _timer = 0;

            lvl = 0;
            max_lvl = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.max_level.ToString());

            skillRank = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.rank.ToString()) % 10;

            name = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.skill_name.ToString());
            explain = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.info.ToString());

            val0 = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val0.ToString()); 
            val0_increase = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val0_increase.ToString());
            string m = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.val0_cal.ToString());
            val0_mul = (m.Equals("m")) ? true : false;
            
            val1 = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val1.ToString());
            val1_increase = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.val1_increase.ToString());
            m = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.val1_cal.ToString());
            val1_mul = (m.Equals("m")) ? true : false;

            delay = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.delay.ToString()); // * BaseManager.userGameData.o_Cool;
            keep = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.keep.ToString());
            count = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.count.ToString());
            size = 1;
            
            m = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.inheritType.ToString());
            _inherit = EnumHelper.StringToEnum<inheritType>(m);

            delay_reduce = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.delay_reduce.ToString());
            keep_increase = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.keep_increase.ToString());
            size_increase = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.size_increase.ToString());
            count_increase = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.count_increase.ToString());
            range = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.range.ToString());

            string str = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.essential.ToString());
            if (str.Equals("n") == false)
            {
                _essential = EnumHelper.StringToEnum<SkillKeyList>(str);
                _choice = new List<SkillKeyList>();

                for (int i = 0; i < 2; i++)
                {
                    str = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), (SkillValData.choice0 + i).ToString());
                    if (str.Equals("n") == false)
                    {
                        SkillKeyList skl = EnumHelper.StringToEnum<SkillKeyList>(str);
                        _choice.Add(skl);
                    }
                }
            }            

            _overrided = false;
        }

        public override void skillUp()
        {
            if (lvl == 0)
            {
                switch (_inherit)
                {
                    case inheritType.non:
                        break;
                    case inheritType.over:
                        Debug.Log("over : " + _essential.ToString());
                        OverChk(_essential);
                        break;
                    case inheritType.overover:
                    case inheritType.overSelect:
                        OverChk(_essential); 
                        
                        for (int i = 0; _choice != null && i < _choice.Count; i++)
                        {
                            OverChk(_choice[i]);
                        }
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
            if (skillRank == 0)
                return;

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