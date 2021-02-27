using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class upBtn_instance : MonoBehaviour
    {
        [SerializeField] Image _skillImg;
        [SerializeField] TextMeshProUGUI _name;

        SkillKeyList _skType;
        Action<SkillKeyList> _instantApply;
        Action<SkillKeyList> _openTree;
        Action _whenClose;

        public void FixedInit(Action<SkillKeyList> instantApply, Action<SkillKeyList> openTree, Action whenClose)
        {
            _instantApply = instantApply;
            _openTree = openTree;
            _whenClose = whenClose;
        }

        public void setBtn(SkillKeyList sk, int lvl)
        {
            _skType = sk;
            _name.text = DataManager.GetTable<string>(DataTable.skill, $"{sk}", SkillValData.skill_name.ToString());
            _skillImg.sprite = DataManager.Skillicon[sk];

            //_lvl.text = $"Lvl.{lvl + 1}";
            string str = DataManager.GetTable<string>(DataTable.skill, $"{sk}", SkillValData.explain.ToString());            
            //_explain.text = str.Replace("\\\\n", "\n");
        }

        public void setFeedback(Action whenClose)
        {
            _name.text = "환류";
            _whenClose = whenClose;
        }

        public void selectSkill()
        {
            if (_skType < SkillKeyList.Snowball)
            {
                _instantApply?.Invoke(_skType);

                _whenClose?.Invoke();
            }
            else if (_skType == SkillKeyList.max)
            {
                _whenClose?.Invoke();
            }
            else
            {
                _openTree?.Invoke(_skType);
            }
        }
    }
}