using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class skillBase
    {
        protected getSkillList type;

        protected int lvl;
        protected int max_lvl;

        public string explain;
        public string information;
        public bool upGrade;

        public abstract void Init(int i);
        public abstract void skillUp();

        public bool chk_lvl { get { return lvl < max_lvl; } }
        public bool active { get { return lvl > 0; } }
        public int Lvl { get => lvl; }
    }
    public class ability : skillBase
    {
        public float val;

        public Action<getSkillList> skUp;

        public override void Init(int i)
        {
            type = (getSkillList)i;

            upGrade = false;
            lvl = 0;

            max_lvl = DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.max_level.ToString());

            explain = DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.information.ToString());

            val = DataManager.GetTable<float>(DataTable.skill, i.ToString(), SkillValData.val.ToString());
        }

        public override void skillUp()
        {
            if (lvl == 0)
            {
            }
            else if (lvl < max_lvl)
            {
                skUp(type);
            }

            lvl++;
        }
    }
    public class skill : skillBase
    {
        public float att;
        public float delay;
        public float keep;
        public int count;
        public float size;
        public float range;

        public float att_increase;
        public float delay_reduce;
        public float keep_increase;
        public float size_increase;
        public int count_increase;

        public skillNote note;

        public float _timer;

        float rangeDelay;

        public override void Init(int i)
        {
            type = (getSkillList)i;
            upGrade = false;

            lvl = 0;
            max_lvl = DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.max_level.ToString());

            explain = DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.explain.ToString());
            information = DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.information.ToString());

            att = DataManager.GetTable<float>(DataTable.skill, i.ToString(), SkillValData.att.ToString());
            delay = DataManager.GetTable<float>(DataTable.skill, i.ToString(), SkillValData.delay.ToString()) * BaseManager.userEntity.Cool;
            keep = DataManager.GetTable<float>(DataTable.skill, i.ToString(), SkillValData.keep.ToString());
            count = DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.count.ToString());
            size = 1;

            att_increase = (100f + DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.att_increase.ToString())) / 100;
            delay_reduce = (100f - DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.delay_reduce.ToString())) / 100;
            keep_increase = (100f - DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.keep_increase.ToString())) / 100;
            size_increase = (100f + DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.size_increase.ToString())) / 100;
            count_increase = DataManager.GetTable<int>(DataTable.skill, i.ToString(), SkillValData.count_increase.ToString());
            range = DataManager.GetTable<float>(DataTable.skill, i.ToString(), SkillValData.range.ToString());

            note = EnumHelper.StringToEnum<skillNote>(DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.note.ToString()));
        }

        public override void skillUp()
        {
            if (lvl == 0)
            {
            }
            else if (lvl < max_lvl)
            {
                att *= att_increase;
                delay *= delay_reduce;
                keep *= keep_increase;
                count += count_increase;
                size *= size_increase;
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
            Init((int)type);
        }
    }
}