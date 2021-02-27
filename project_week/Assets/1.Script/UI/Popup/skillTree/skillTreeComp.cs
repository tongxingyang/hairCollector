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
        public void copy(skillBtnData d)
        {
            _type = d._type;
            _btnType = d._btnType;
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
        Action _whenCloseUpgradePanel;

        PlayerCtrl _player;
        public PlayerCtrl player { get => _player; }
        public skillBtnData _nowData { get; set; }

        public void Init(PlayerCtrl player, Action whenCloseUpgradePanel)
        {
            _player = player;
            _whenCloseUpgradePanel = whenCloseUpgradePanel;
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
            for (int i = 0; i < _taps.Length; i++)
            {
                _tapObjs[i].SetActive(false);
            }

            int rank = DataManager.GetTable<int>(DataTable.skill, type.ToString(), SkillValData.rank.ToString());
            skillType n = (skillType)(rank / 10);
            Color col = Color.white;

            switch (n)
            {
                case skillType.launch:
                    _title.text = "발 사";                    
                    ColorUtility.TryParseHtmlString("#3197DDFF", out col);                    
                    break;
                case skillType.range:
                    _title.text = "광 역";
                    ColorUtility.TryParseHtmlString("#468B96FF", out col);
                    break;
                case skillType.rush:
                    _title.text = "돌 파";
                    ColorUtility.TryParseHtmlString("#474D96FF", out col);
                    break;
                case skillType.shield:
                    _title.text = "실 드";
                    ColorUtility.TryParseHtmlString("#874696FF", out col);
                    break;
                case skillType.environment:
                    _title.text = "환 경";
                    ColorUtility.TryParseHtmlString("#B76C27FF", out col);
                    break;
                case skillType.summon:
                    _title.text = "소 환";
                    ColorUtility.TryParseHtmlString("#F8DB58FF", out col);
                    break;
            }
            _panel.color = col;

            _nowData.reset();
            chkOpenApply(false);
            _explainName.text = "--";
            _explain.text = "--";

            _taps[(int)n].OnOpen();
            _refresh = _taps[(int)n].refreshSkill;
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
            Debug.Log(_nowData._type);
            _player.getSkill(_nowData._type);
            gameObject.SetActive(false);
            _whenCloseUpgradePanel?.Invoke();
        }

        public void chkOpenApply(bool bl)
        {
            _apply.raycastTarget = bl;
            _apply.color = (bl) ? Color.red : Color.grey;
        }
    }
}