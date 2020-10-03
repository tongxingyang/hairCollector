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
        [SerializeField] TextMeshProUGUI _lvl;
        [SerializeField] TextMeshProUGUI _explain;

        SkillKeyList _skType;
        Action<SkillKeyList> _btnFunction;
        Action _close;

        public void selectSkill()
        {
            _btnFunction(_skType);
            _close();
        }

        public void setBtn(SkillKeyList sk, int lvl, Action<SkillKeyList> BtnFunction, Action close)
        {
            _skType = sk;
            _name.text = DataManager.GetTable<string>(DataTable.skill, $"{(int)sk}", "skill");
            _skillImg.sprite = DataManager.Skillicon[sk];

            _lvl.text = $"Lvl.{lvl + 1}";
            string str = DataManager.GetTable<string>(DataTable.skill, $"{(int)sk}", SkillValData.explain.ToString());            
            _explain.text = str.Replace("\\\\n", "\n");
            _btnFunction = BtnFunction;

            _close = close;
        }
    }
}