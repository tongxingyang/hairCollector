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
        List<SkillKeyList[]> prev; // (or)and(or)and(or) 구조
        List<UILineRenderer[]> _takes;

        Func<bool> _temChk;
        Image[] _temImg;

        [SerializeField] UILineRenderer[] _go;
        [SerializeField] UILineRenderer _rein;

        List<SkillKeyList>[] _chkGoLine;

        SkillKeyList[] _selectBrother;

        bool _onlyChk = false;

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

        public skillBox setFrom(SkillKeyList[] p0, SkillKeyList[] p1 = null)
        {
            prev = new List<SkillKeyList[]>();

            prev.Add(p0);

            if (p1 != null)
                prev.Add(p1);

            if (_take.Length > 0)
            {
                _takes = new List<UILineRenderer[]>();
                int n = 0;
                for (int i = 0; i < prev.Count; i++)
                {
                    UILineRenderer[] ulr = new UILineRenderer[prev[i].Length];
                    for (int j = 0; j < prev[i].Length; j++)
                    {
                        ulr[j] = _take[n++];
                    }
                    _takes.Add(ulr);
                }
            }

            return this;
        }

        public skillBox setDefault(List<SkillKeyList>[] chkGoLine)
        {
            setGoLine(chkGoLine);

            _data._btnType = btnType.defaultType;

            return this;
        }

        public void setGoLine(List<SkillKeyList>[] chkGoLine)
        {
            _chkGoLine = chkGoLine;
        }

        public skillBox setSelectType(SkillKeyList[] skl)
        {
            _data._btnType = btnType.selectType;
            _selectBrother = skl;

            return this;
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

        public void setChecker()
        {
            _onlyChk = true;
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
                case btnType.selectType:
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
            _tree._nowData.copy(_data);            

            Debug.Log(_data._type + " / " + _tree._nowData._type);
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
                    case btnType.selectType:
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
                for (int i = 0; i < prev.Count; i++)
                {
                    for (int j = 0; j < prev[i].Length; j++)
                    {                        
                        if (_tree.player.Skills[prev[i][j]].chk_lvl == false)
                        {                            
                            if (_refer.Lvl > 0)
                                _takes[i][j].color = orange;
                            else
                                _takes[i][j].color = Color.white;
                        }
                        else
                        {
                            // Debug.Log(_data._type + "의 " + prev[i][j]);
                            _takes[i][j].color = Color.grey;
                        }
                    }
                }
            }

            if (_go.Length > 0)
            {
                for (int i = 0; i < _go.Length; i++)
                {
                    // 가지친 스킬중 하나라도 습득했는지 체크
                    bool result = false;
                    for (int j = 0; j < _chkGoLine[i].Count; j++)
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

            // 애초에 강화 불가 오로지 체크용 (ex: 서클)
            if (_onlyChk)
                return false;

            // 버튼에 따라서
            switch (_data._btnType)
            {
                case btnType.defaultType:
                case btnType.standardType:
                    if (_refer.Lvl >= _refer.MaxLvl)
                    {
                        return false;
                    }
                    break;
                case btnType.selectType:
                    if (_refer.Lvl >= _refer.MaxLvl)
                    {
                        return false;
                    }
                    else if (_refer.Lvl == 0)
                    {
                        for (int i = 0; i < _selectBrother.Length; i++)
                        {
                            if (_tree.player.Skills[_selectBrother[i]].Lvl > 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case btnType.reinType:
                    if (_refer.Lvl < _refer.MaxLvl || _refer.Lvl >= 99)
                    {
                        return false;
                    }
                    break;
            }            

            // 이전 스킬 체크
            if (prev != null)
            {
                for (int i = 0; i < prev.Count; i++)
                {
                    chk = !preSkillOrChk(prev[i]) || chk;
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

        bool preSkillOrChk(SkillKeyList[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (_tree.player.Skills[arr[i]].chk_lvl == false)
                {
                    return true;
                }
            }

            return false;
        }
    }
}