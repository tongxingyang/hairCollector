using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class getSkillCase : MonoBehaviour
    {
        [SerializeField] Image _img;
        [SerializeField] TextMeshProUGUI _txt;

        public void Init(SkillKeyList skill, int lvl)
        {
            _img.sprite = DataManager.Skillicon[skill];

            string str = DataManager.GetTable<string>(DataTable.skill, ((int)skill).ToString(), SkillValData.skill_name.ToString()) + System.Environment.NewLine + $"lvl.{lvl}";
            _txt.text = str;
        }
    }
}