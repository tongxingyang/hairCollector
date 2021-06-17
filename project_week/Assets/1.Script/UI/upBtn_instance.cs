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
        [SerializeField] TextMeshProUGUI _explain;
        [SerializeField] TextMeshProUGUI _lvl;

        upgradePopup _upGrade;
        SkillKeyList _skType;
        Action<SkillKeyList, NotiType> _instantApply;
        Action<SkillKeyList> _openTree;
        Action _whenClose;

        public void FixedInit(upgradePopup upGrade, Action<SkillKeyList, NotiType> instantApply, Action<SkillKeyList> openTree, Action whenClose)
        {
            _upGrade = upGrade;
            _instantApply = instantApply;
            _openTree = openTree;
            _whenClose = whenClose;

            GetComponent<Button>().onClick.AddListener(selectSkill);
        }
        Action<SkillKeyList> _whenAbil;
        public void setBtn(SkillKeyList sk, int lvl, Action<SkillKeyList> act)
        {
            _skType = sk;

            _skillImg.sprite = DataManager.Skillicon[sk];            

            if (sk < SkillKeyList.SnowBall)
            { 
                _name.text = D_skill.GetEntity($"{sk}").f_skill_name;

                _explain.text = D_skill.GetEntity($"{sk}").f_explain;
                
                _lvl.gameObject.SetActive(true);
                _lvl.text = $"lv.{lvl}";
            }
            else
            {
                switch (sk)
                {
                    case SkillKeyList.SnowBall:
                        _name.text = "발사(눈덩이류)";
                        break;
                    case SkillKeyList.IceBall:
                        _name.text = "광역(얼덩이류)";
                        break;
                    case SkillKeyList.IceBat:
                        _name.text = "돌파(전방류)";
                        break;
                    case SkillKeyList.Shield:
                        _name.text = "실드(실드류)";
                        break;
                    case SkillKeyList.Field:
                        _name.text = "환경(디버프류)";
                        break;
                    case SkillKeyList.Summon:
                        _name.text = "소환(보조류)";
                        break;
                }

                _explain.text = $"{D_skill.GetEntity($"{sk}").f_skill_name} 스킬 습득";
                _lvl.gameObject.SetActive(false);
            }

            _whenAbil = (sk < SkillKeyList.SnowBall) ? act : null;
        }

        public void setFeedback(Action whenThrow)
        {
            _name.text = "경험치 회수";
            _explain.text = "레벨업을 보류하고 경험치 33% 획득";

            _whenClose = whenThrow;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => _whenClose?.Invoke());
        }

        public void selectSkill()
        {
            if (_skType < SkillKeyList.SnowBall)
            {
                _instantApply?.Invoke(_skType, _upGrade.Noti);

                _whenAbil?.Invoke(_skType);
                _whenClose?.Invoke();
            }
            else if (_skType == SkillKeyList.non)
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