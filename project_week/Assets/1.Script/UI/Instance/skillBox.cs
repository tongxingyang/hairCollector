using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using NaughtyAttributes;
using UnityEngine.EventSystems;

namespace week
{
    public class skillBox : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        RectTransform _rect;
        [Header("skill")]
        [SerializeField] Image _back;                   // 배경
        [SerializeField] Image _skillImg;               // 스킬
        [SerializeField] Image _case;                   // 케이스
        [Space]
        [SerializeField] TextMeshProUGUI _num;          // 스킬 레벨
        [Header("lock")]
        [SerializeField] GameObject _medal;             // 메달
        [SerializeField] TextMeshProUGUI _medalMount;   // 메달양
        [Space]
        [SerializeField] Image _enable;                 // 활성화

        Button _button;

        skillTreeComp _tree;
        Action<string, string> _infoSet; // 정보 보여주기

        skill _refer;       // 스킬 정보
        skillBtnData _data; // 버튼 정보

        #region [ line ] 

        [Header("Line")]
        [SerializeField] UILineRenderer[] _take;
        SkillKeyList[] prev; // (or)and(or)and(or) 구조
        UILineRenderer[] _takeLine;

        [SerializeField] UILineRenderer[] _go;
        List<SkillKeyList>[] _goLineData; // 리스트 : 브라더스킬 / 배열 : 가짓수

        SkillKeyList[] _BrotherArray;

        Func<bool> _temChk;
        bool hasMedal { 
            get
            {
                if (_tree.player.Inventory.ContainsKey(gainableTem.questKey))
                {
                    return _refer.shouldHaveKey >= _tree.player.Inventory[gainableTem.questKey];
                }

                return false;
            } 
        }

        Color orange = new Color(1f, 0.7f, 0f);

        #endregion

        enum skillState { not, have, master, over };
        void skillLvlColor(skillState state)
        {
            Color sk = Color.gray, txt = Color.gray;
            switch (state)
            {
                case skillState.not:
                    sk = txt = Color.gray;
                    break;
                case skillState.have:
                    sk = txt = Color.white;
                    break;
                case skillState.master:
                    sk =  Color.white;
                    txt = orange;
                    break;
                case skillState.over:
                    sk = Color.white;
                    txt = Color.red;
                    break;
            }
            _back.color = sk;
            _skillImg.color = sk;
            _case.color = sk;
            _num.color = txt;
        }


        /// <summary> 초기화 </summary>
        public skillBox Init(skillTreeComp tree, skill refer)
        {
            // 초기화
            _tree = tree;
            _refer = refer;
            _infoSet = _tree.setInfo;
            _data = new skillBtnData(_refer, btnType.standardType);

            _enable.gameObject.SetActive(false);
            setBox(_refer.Lvl);

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickSkillBtn);

            _rect = GetComponent<RectTransform>();

            return this;
        }

        /// <summary> 루트스킬로 세팅 </summary>
        public skillBox setRoot(List<SkillKeyList>[] goLineData)
        {
            setGoLine(goLineData);

            _data.BtnType = btnType.rootType;

            return this;
        }

        /// <summary> 필수스킬 설정 </summary>
        public skillBox setFrom()
        {
            prev = new SkillKeyList[3] {                
                    D_skill.GetEntity(_refer.Type.ToString()).f_essential,
                    D_skill.GetEntity(_refer.Type.ToString()).f_choice0,
                    D_skill.GetEntity(_refer.Type.ToString()).f_choice1 };

            return this;
        }
        
        /// <summary>  </summary>
        public void setGoLine(List<SkillKeyList>[] goLineData)
        {
            _goLineData = goLineData;

            if (_go.Length > 0 && _go.Length != _goLineData.Length)
            {
                Debug.LogWarning(_refer.Type + " : go line 데이터 비일치");
            }
        }

        public skillBox setChoiceType(SkillKeyList[] skl)
        {
            _data.BtnType = btnType.choiceType;
            _BrotherArray = skl;

            return this;
        }

        /// <summary> 창 열릴때 </summary>
        public void OnOpen()
        {
            // 박스
            setBox(_refer.Lvl);

            // 라인
            setLine();

            OnSelect(SkillKeyList.non);
        }

        /// <summary> 선택 </summary>
        public void OnClickSkillBtn()
        {
            _tree._nowData.copy(_data);

            _infoSet?.Invoke(_refer.name, _refer.explain);
            _tree.chkOpenApply(_refer.IsLock, chkGetable());
        }

        public void OnSelect(SkillKeyList data)
        {
            bool chk = chkGetable();

            if (_data.Refer.Type != data)
            {
                setBox(_refer.Lvl);
                setLine();

                if (chk)
                {
                    _enable.gameObject.SetActive(true);
                    _enable.color = Color.white;
                }
                else
                    _enable.gameObject.SetActive(false);
            }
            else
            {                
                int lvl = (chk) ? _refer.Lvl + 1 : _refer.Lvl;
                _enable.gameObject.SetActive(chk);
                _enable.color = Color.green;
                setBox(lvl);
                setLine();
            }
        }

        /// <summary> 박스 색 설정 </summary>
        public void setBox(int lvl)
        {
            switch (_data.BtnType)
            {
                case btnType.rootType:
                    {
                        _num.text = $"{lvl}/{_refer.MaxLvl}";

                        if (lvl > _refer.MaxLvl)
                            skillLvlColor(skillState.over);
                        else if (lvl == _refer.MaxLvl)
                            skillLvlColor(skillState.master);
                        else if (lvl == 0)
                            skillLvlColor(skillState.not);
                        else
                            skillLvlColor(skillState.have);
                    }
                    break;
                case btnType.standardType:
                case btnType.choiceType:
                case btnType.lockType:
                    {
                        _num.text = $"{lvl}/{_refer.MaxLvl}";

                        if (lvl == 0)
                        {
                            skillLvlColor(skillState.not);
                        }
                        else if (lvl < _refer.MaxLvl)
                        {
                            skillLvlColor(skillState.have);
                        }
                        else if (lvl == _refer.MaxLvl)
                            skillLvlColor(skillState.master);
                        else
                            skillLvlColor(skillState.over);
                    }
                    break;
            }

            // lock 체크
            if (_refer.IsLock)
            {
                _case.sprite = _tree._lockCase;
                _enable.sprite = _tree._wlockCase;

                _medalMount.text = $"x {(int)_refer.shouldHaveKey}";
                _medal.SetActive(true);
            }
            else
            {
                // 스킬 랭크
                int rank = D_skill.GetEntity(_refer.Type.ToString()).f_rank;

                // 스킬&버튼 이미지 관련 
                _back.sprite = _tree.getRankBacks(rank);
                _case.sprite = _tree.getRankCases(rank);
                _enable.sprite = _tree.getWRankCases(rank);

                _medal.SetActive(false);
            }

            _skillImg.sprite = DataManager.Skillicon[_refer.Type];
        }

        /// <summary> 선 색 설정 </summary>
        public void setLine()
        {
            if (_take.Length > 0) // 관리할 라인이 있어야함
            {
                // 이미 습득시
                if (_refer.Lvl > 0)
                {
                    for (int i = 0; i < _take.Length; i++)
                    {
                        _take[i].color = orange;
                    }
                }
                else
                {
                    // 아직 미습득
                    bool chk = isPreSkillsMaster();

                    chk &= !_refer.IsLock;

                    for (int i = 0; i < _take.Length; i++)
                    {
                        _take[i].color = (chk) ? Color.white : Color.grey;
                    }

                }
            }

            if (_go.Length > 0)
            {
                for (int i = 0; i < _go.Length; i++)
                { 
                    // 가지친 스킬중 하나라도 습득했는지 체크
                    bool result = false;
                    for (int j = 0; j < _goLineData[i].Count; j++)
                    {
                        if (_tree.player.Skills[_goLineData[i][j]].Lvl > 0)
                        {
                            result = true;
                            break;
                        }
                    }

                    if (_refer.isMax)
                    {
                        if (result)
                            _go[i].color = orange;
                        else
                            _go[i].color = Color.white;
                    }
                    else
                    {
                        _go[i].color = Color.grey;
                    }
                }                
            }
        }

        /// <summary> 이전 스킬, 템등을 다 맞춰서 이 버튼을 눌러 스킬을 얻을수 있는지 여부 </summary>
        bool chkGetable()
        {
            bool chk = false;

            if (_refer.Lvl >= _refer.MaxLvl)
            {
                if (_data.BtnType != btnType.rootType || _refer.MaxLvl == 0)
                {
                    return false;
                }
            }

            // 버튼에 따라서
            switch (_data.BtnType)
            {
                case btnType.rootType:
                    if ((_refer.Inherit == inheritType.rebase && _refer.Lvl < 99) || _refer.Lvl < _refer.MaxLvl)
                    { }
                    else
                    {
                        return false;
                    }
                    break;
                case btnType.standardType:
                    if (_refer.Lvl >= _refer.MaxLvl)
                    {
                        Debug.Log("ㅇㅇ");
                        return false;
                    }
                    break;
                case btnType.choiceType:
                    if (_refer.Lvl >= _refer.MaxLvl)
                    {
                        return false;
                    }
                    if (_refer.Lvl == 0)
                    {
                        for (int i = 0; i < _BrotherArray.Length; i++)
                        {
                            if (_tree.player.Skills[_BrotherArray[i]].Lvl > 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case btnType.lockType:
                    if (_refer.Lvl >= _refer.MaxLvl)
                    {
                        return false;
                    }
                    if (_refer.Lvl == 0)
                    {
                        return hasMedal;
                    }
                    break;
            }

            // 이전 스킬 체크
            if (prev != null)
            {
                chk = !isPreSkillsMaster();
            }            

            return !chk;
        }

        /// <summary> 이전 스킬 마스터 했는지 (필수/선택 구분완료) </summary>
        bool isPreSkillsMaster()
        {
            if (prev[0] != SkillKeyList.non)
            {
                if (_tree.player.Skills[prev[0]].isMax == false) // 필수 마스터 못하면 false                
                    return false;
            }

            bool chk = true;
            for (int i = 1; i < 3; i++)
            {
                if (prev[i] != SkillKeyList.non)
                {
                    chk = false;
                    if (_tree.player.Skills[prev[i]].isMax) //                 
                    {
                        chk = true;
                        break;
                    }
                }
            }

            return chk;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tree.openInfo(_rect, _refer.Type);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _tree.closeInfo();
        }

        [Button]
        public void ctrlLine()
        { 
            if(_go.Length >0)
            {
                for (int i = 0; i < _go.Length; i++)
                {
                    _go[i].Points[0] = transform.localPosition;
                }
            }

            if (_take.Length > 0)
            {
                for (int i = 0; i < _take.Length; i++)
                {
                    _take[i].Points[0] = transform.localPosition;
                }
            }
        }
    }
}