using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

namespace week
{
    public class skillBox : MonoBehaviour
    {
        [SerializeField] Image _skillImg;
        [SerializeField] Image _caseColor;
        [SerializeField] TextMeshProUGUI _num;

        skillTreeComp _tree;
        Action<string, string> _infoSet;

        skill _refer;
        skillBtnData _data;

        #region [ line ] 

        [Header("Line")]
        [SerializeField]  UILineRenderer[] _take;
        List<SkillKeyList> prev;
        Func<bool> _temChk;
        Image[] _temImg;

        [SerializeField] UILineRenderer[] _go;
        [SerializeField] UILineRenderer _rein;

        List<SkillKeyList>[] _chkGoLine;

        Color orange = new Color(1f, 0.7f, 0f);

        #endregion

        Color boxColor
        {
            set
            {
                _skillImg.color = (value == Color.grey) ? value : Color.white;
                _caseColor.color = value;
            }
        }

        public skillBox Init(skillTreeComp tree, skill refer)
        {            
            _tree = tree;
            _refer = refer;

            _infoSet = tree.setInfo;

            _data = new skillBtnData(_refer.Type, btnType.standardType);

            return this;
        }

        public skillBox setFrom(SkillKeyList p0, SkillKeyList p1 = SkillKeyList.max)
        {
            prev = new List<SkillKeyList>();

            prev.Add(p0);

            if (p1 != SkillKeyList.max)
                prev.Add(p1);

            return this;
        }

        public void setDefault(List<SkillKeyList>[] chkGoLine)
        {
            _chkGoLine = chkGoLine;

            _data._btnType = btnType.defaultType;
        }

        public void setRein()
        {
            _data._btnType = btnType.reinType;
        }

        public void setTem(GameObject ob, Func<bool> temChk)
        {
            _temImg = ob.GetComponentsInChildren<Image>();
            _temChk = temChk;
        }

        /// <summary> 창 열릴때 </summary>
        public void OnOpen()
        {
            switch (_data._btnType)
            {
                case btnType.defaultType:
                    {
                        int lvl = (_refer.Lvl > _refer.MaxLvl) ? _refer.MaxLvl : _refer.Lvl;
                        _num.text = $"{lvl} / {_refer.MaxLvl}";

                        if (lvl == _refer.MaxLvl)
                            boxColor = orange;
                        else if (lvl == 0)
                            boxColor = Color.grey;
                        else
                            boxColor = Color.white;
                    }
                    break;
                case btnType.reinType:
                    {
                        int lvl = (_refer.Lvl >= _refer.MaxLvl) ? _refer.Lvl - _refer.MaxLvl : 0;
                        _num.text = $"{lvl} / 99";
                        boxColor = Color.white;

                        if (_refer.Lvl > _refer.MaxLvl)
                            boxColor = Color.red;
                        else
                        {
                            boxColor = Color.grey;
                        }
                    }
                    break;
                case btnType.standardType:
                    {
                        _num.text = $"{_refer.Lvl} / {_refer.MaxLvl}";
                        boxColor = Color.white;

                        if (_refer.Lvl == 0)
                        {
                            boxColor = Color.grey;
                        }
                        else if (_refer.Lvl < _refer.MaxLvl)
                        {
                            boxColor = Color.white;
                        }
                        else if (_refer.Lvl == _refer.MaxLvl)
                            boxColor = orange;
                        else
                            boxColor = Color.red;
                    }
                    break;
            }

            setLine();
            //_tree.chkOpenApply(chkGetable());
            if (_temChk != null)
            {
                bool bl = _temChk();
                _temImg[0].color = (bl) ? Color.white : Color.grey;
                _temImg[1].color = (bl) ? orange : Color.grey;
            }
        }

        /// <summary> 선택 </summary>
        public void OnClickSkillBtn()
        {
            _tree._nowData = _data;
            _infoSet?.Invoke(_refer.name, _refer.explain);
            _tree.chkOpenApply(chkGetable());
        }

        public void OnSelect(skillBtnData data)
        {
            if (_data != data)
                OnOpen();
            else
            {
                int lvl = (chkGetable()) ? _refer.Lvl + 1 : _refer.Lvl;

                switch (_data._btnType)
                {
                    case btnType.defaultType:
                        {
                            lvl = (lvl > _refer.MaxLvl) ? _refer.MaxLvl : lvl;
                            _num.text = $"{lvl} / {_refer.MaxLvl}";

                            if (lvl == _refer.MaxLvl)
                                boxColor = orange;
                            else if (lvl == 0)
                                boxColor = Color.grey;
                            else
                                boxColor = Color.white;
                        }
                        break;
                    case btnType.reinType:
                        {
                            lvl = (_refer.Lvl >= _refer.MaxLvl) ? _refer.Lvl - _refer.MaxLvl + 1 : 0;
                            _num.text = $"{lvl} / 99";

                            boxColor = Color.white;

                            if (lvl > 0)
                                boxColor = Color.red;
                            else
                            {
                                boxColor = Color.grey;
                            }
                        }
                        break;
                    case btnType.standardType:
                        {
                            _num.text = $"{lvl} / {_refer.MaxLvl}";

                            if (lvl == 0)
                            {
                                boxColor = Color.grey;
                            }
                            else if (lvl < _refer.MaxLvl)
                            {
                                boxColor = Color.white;
                            }
                            else if (lvl == _refer.MaxLvl)
                                boxColor = orange;
                            else
                                boxColor = Color.red;
                        }
                        break;
                }

                setLine();                 

                if (_temChk != null)
                {
                    bool bl = _temChk();
                    _temImg[0].color = (bl) ? Color.white : Color.grey;
                    _temImg[1].color = (bl) ? orange : Color.grey;
                }
            }
        }

        public void setLine()
        {
            if (_take.Length > 0 && prev != null)
            {
                if (_take.Length == prev.Count)
                {
                    for (int i = 0; i < prev.Count; i++)
                    {
                        if (_tree.player.Skills[prev[i]].chk_lvl == false)
                        {
                            if (_refer.Lvl > 0)
                                _take[i].color = orange;
                            else
                                _take[i].color = Color.white;
                        }
                        else
                        {
                            _take[i].color = Color.grey;
                        }
                    }
                }
                else if (_take.Length < prev.Count)
                {
                    bool chk = false;
                    for (int i = 0; i < prev.Count; i++)
                    {
                        chk = _tree.player.Skills[prev[i]].chk_lvl || chk;
                    }

                    if (chk) 
                        _take[0].color = Color.grey;
                    else
                    {
                        _take[0].color = (_refer.chk_lvl == false) ? orange : Color.white;
                    }                        
                }
            }

            if (_go.Length > 0)
            {
                for (int i = 0; i < _go.Length; i++)
                {
                    // 가지친 스킬중 하나라도 습득했는지 체크
                    bool result = false;
                    for (int j = 0; j < _chkGoLine.Length; j++)
                    {
                        if (_tree.player.Skills[_chkGoLine[i][j]].Lvl > 0)
                        {
                            result = true;
                            break;
                        }
                    }

                    if (result)                         // 하나라도 습득시 
                    {
                        _go[i].color = orange;
                    }
                    else if (_refer.chk_lvl == false)   // 습득한거 없지만 이제부터 습득 가능 
                    {
                        _go[i].color = Color.white;
                    }
                    else                                // 습득 불가
                    {
                        _go[i].color = Color.grey;
                    }
                }
            }

            if (_rein != null)
            {
                if (_refer.chk_lvl == false)
                {
                    _rein.color = orange;
                }
                else if (_refer.Lvl == _refer.MaxLvl)
                {
                    _rein.color = Color.white;
                }
                else
                {
                    _rein.color = Color.grey;
                }
            }
        }

        /// <summary> 이전 스킬, 템등을 다 맞춰서 이 버튼을 눌러 스킬을 얻을수 있는지 여부 </summary>
        bool chkGetable()
        {
            bool chk = false;

            if (_refer.Lvl >= ((_data._btnType == btnType.reinType) ? 99 : _refer.MaxLvl))
            {
                return false;
            }

            // 이전 스킬 체크
            if (prev != null)
            {
                for (int i = 0; i < prev.Count; i++)
                {
                    chk = _tree.player.Skills[prev[i]].chk_lvl || chk;
                }
            }

            // 아이템 체크
            if (_temChk != null)
            {
                return _temChk() && !chk;
            }
            else
                return !chk;
        }
    }
}