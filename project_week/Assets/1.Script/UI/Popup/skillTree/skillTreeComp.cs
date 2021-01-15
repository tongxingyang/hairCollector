using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public enum btnType { defaultType, reinType, standardType, selectType }
    public class skillBtnData
    {
        public SkillKeyList _type;
        public btnType _btnType;
        public skillBtnData() { reset(); }
        public skillBtnData(SkillKeyList type, btnType btnType)
        {
            _type = type;
            _btnType = btnType;
        }
        public void reset()
        {
            _type = SkillKeyList.max;
            _btnType = btnType.standardType;
        }
        public static bool operator ==(skillBtnData a, skillBtnData b)
        {
            return (a._type == b._type) && (a._btnType == b._btnType);
        }
        public static bool operator !=(skillBtnData a, skillBtnData b)
        {
            return !((a._type == b._type) && (a._btnType == b._btnType));
        }
    }

    public class skillTreeComp : MonoBehaviour
    {
        public enum skillType { launch, range, rush, shield, environment, summon }
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] Image _panel;
        [SerializeField] TextMeshProUGUI _explainName;
        [SerializeField] TextMeshProUGUI _explain;

        [SerializeField] GameObject[] _tapObjs;
        [SerializeField] Image _apply;

        ISkillTap[] _taps;
        Action _refresh;

        PlayerCtrl _player;
        public PlayerCtrl player { get => _player; }
        public skillBtnData _nowData { get; set; }

        public void Init(PlayerCtrl player)
        {
            _player = player;
            _nowData = new skillBtnData();

            _taps = new ISkillTap[_tapObjs.Length];
            for (int i = 0; i < _taps.Length; i++)
            {
                _tapObjs[i].SetActive(false);
                _taps[i] = _tapObjs[i].GetComponent<ISkillTap>();
                _taps[i].Init(player, this);
            }
        }

        public void openTap(SkillKeyList type)
        {
            string str = DataManager.GetTable<string>(DataTable.skill, type.ToString(), SkillValData.type.ToString());

            int n = (int)EnumHelper.StringToEnum<skillType>(str);

            _nowData.reset();
            _taps[n].OnOpen();
            _refresh = _taps[n].refreshSkill;
        }

        public void setTitle(string str) { _title.text = str; }
        public void setInfo(string name, string expl) 
        {
            _explainName.text = name;
            _explain.text = expl;

            _refresh?.Invoke();
        }

        public void SelectSKill()
        {
            _player.getSkill(_nowData._type);
        }

        public void chkOpenApply(bool bl)
        {
            _apply.raycastTarget = bl;
            _apply.color = (bl) ? Color.red : Color.grey;
        }
    }
}