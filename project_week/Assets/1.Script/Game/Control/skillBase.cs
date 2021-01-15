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
        public float val;

        public Action<SkillKeyList> skUp;

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            lvl = 0;

            max_lvl = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.max_level.ToString());

            explain = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.info.ToString());

            val = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.stat_val.ToString());
        }

        public override void skillUp()
        {
            if (lvl < max_lvl)
            {
                skUp(type);
            }

            lvl++;
        }
    }

    /// <summary> 스킬 </summary>
    public class skill : skillBase
    {
        public float att;   // 공격력
        public float delay; // 쿨
        public float keep;  // 지속시간
        public int count;   // 개수
        public float size;  // 크기
        public float range; // 사거리

        //public float att_increase;  // 공격력 증가량
        //public float delay_reduce;  // 쿨 감소량
        //public float keep_increase; // 지속시간 증가량
        //public float size_increase; // 크기 증가량
        //public int count_increase;  // 개수 증가량

        public bool _preCondition;      // 선행 스킬 필요 여부
        public SkillKeyList _preSkill;  // 필요한 선행 스킬

        public float _timer;        // 쿨 타이머

        float rangeDelay;

        public override void Init(SkillKeyList sk)
        {
            type = sk;

            upGrade = false;
            _timer = 0;

            lvl = 0;
            max_lvl = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.max_level.ToString());

            name = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.skill_name.ToString());
            explain = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, sk.ToString(), SkillValData.info.ToString());

            att = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.att.ToString());
            delay = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.delay.ToString()); // * BaseManager.userGameData.o_Cool;
            keep = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.keep.ToString());
            count = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.count.ToString());
            size = 1;

            //att_increase = (100f + DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.att_increase.ToString())) / 100;
            //delay_reduce = (100f - DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.delay_reduce.ToString())) / 100;
            //keep_increase = (100f - DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.keep_increase.ToString())) / 100;
            //size_increase = (100f + DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.size_increase.ToString())) / 100;
            //count_increase = DataManager.GetTable<int>(DataTable.skill, sk.ToString(), SkillValData.count_increase.ToString());
            range = DataManager.GetTable<float>(DataTable.skill, sk.ToString(), SkillValData.range.ToString());
        }

        public override void skillUp()
        {
            if (lvl == 0)
            {
            }
            else if (lvl < max_lvl)
            {
                //att     *= att_increase;
                //delay   *= delay_reduce;
                //keep    *= keep_increase;
                //count   += count_increase;
                //size    *= size_increase;
            }

            lvl++;
        }

        public bool chk_Time(float delta, float cool)
        {
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
            if (lvl > 0)
            {
                _timer += delta;
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

        public bool chk_rangeshotable(float delta, float cool, float dist)
        {
            if (lvl > 0)
            {
                if (_timer <= 0f)
                {
                    rangeDelay = UnityEngine.Random.Range(1.5f, delay);
                }

                _timer += delta;
                if (_timer > rangeDelay * cool)
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

        public void clear()
        {
            Init(type);
        }
    }
}