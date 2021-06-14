﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class upgradePopup : MonoBehaviour
    {
        [Header("upgrade")]
        [SerializeField] GameObject _suggestPanel;
        [SerializeField] GameObject _treePanel;
        [SerializeField] upBtn_instance[] upBtns;

        TextMeshProUGUI setBtnTxt;

        PlayerCtrl _player;
        skillTreeComp _tree;

        SkillKeyList _newEquip;
        int _slotCnt = 2;
        int _selectNum;

        public NotiType Noti { get; private set; }
        // 창 종료시 gameScene 행동
        Action _whenCloseUpgradePanel;

        Color _blue = new Color(0.54f, 0.68f, 0.98f);
        Color _brown = new Color(0.91f, 0.8f, 0.64f);

        // Start is called before the first frame update
        void Awake()
        {
            _suggestPanel.SetActive(false);
            _treePanel.SetActive(false);
            close();
        }

        public void Init(GameScene gs, Action whenCloseUpgradePanel)
        {
            _player = gs.Player;
            _whenCloseUpgradePanel = whenCloseUpgradePanel;

            _tree = _treePanel.GetComponent<skillTreeComp>();
            _tree.Init(gs, close);

            for (int i = 0; i < 3; i++)
            {
                upBtns[i].FixedInit(this, _player.getSkill, (type)=> {
                    _treePanel.SetActive(true);
                    _suggestPanel.SetActive(false);

                    _tree.openTap(type);                    
                }, close);
            }

            upBtns[3].setFeedback(pressThrowBtn);
        }

        public void open(NotiType noti)
        {
            Noti = noti;
            gameObject.SetActive(true);
        }

        public void close()
        {
            _whenCloseUpgradePanel?.Invoke();

            gameObject.SetActive(false);
        }

        #region upgrade

        /// <summary> 스킬습득 - 제안 -> 트리 </summary>
        public void getSkillTreeOpen(NotiType noti)
        {
            open(noti);

            _suggestPanel.SetActive(true);
            _treePanel.SetActive(false);

            suggest(noti);
        }

        /// <summary> 업그레이드 제안 </summary>
        void suggest(NotiType noti)
        {
            List<SkillKeyList> slct = new List<SkillKeyList>();
            List<SkillKeyList> _skillList 
                = new List<SkillKeyList> { SkillKeyList.SnowBall, SkillKeyList.IceBall, SkillKeyList.IceBat, SkillKeyList.Shield, SkillKeyList.Field, SkillKeyList.Summon };

            int[] rate = new int[] { 9, 7, 1 };
            for (int i = 0; i < 3; i++)
            {
                float num = UnityEngine.Random.Range(0, 10);

                if (num < rate[i]) // 스킬
                {
                    int s = UnityEngine.Random.Range(0, _skillList.Count);
                    slct.Add(_skillList[s]);
                    _skillList.Remove(_skillList[s]);
                    
                }
                else // 능력치
                {
                    SkillKeyList ab;
                    bool bl = false;

                    do
                    {
                        ab = (SkillKeyList)UnityEngine.Random.Range(0, (int)SkillKeyList.SnowBall);
                        
                        if (_player.Abils[ab].isMax == false && slct.Contains(ab) == false)
                        {
                            slct.Add(ab);
                            bl = false;
                        }
                        else
                            bl = true;

                    } while (bl);
                }
            }

            // slct[0] = SkillKeyList.Field; // 지우기

            for (int i = 0; i < 3; i++)
            {
                int lvl = (slct[i] < SkillKeyList.SnowBall) ? _player.Abils[slct[i]].Lvl : 0;

                upBtns[i].setBtn(slct[i], lvl);
            }
        }

        public void pressThrowBtn()
        {
            _player.expFeedback();

            close();
        }

        /// <summary> 오픈 -> 바로 트리 </summary>
        public void confirm_skillTree(SkillKeyList sk, NotiType noti = NotiType.non)
        {
            open(noti);
            _suggestPanel.SetActive(false);
            _treePanel.SetActive(true);

            _tree.openTap(sk);
        }

        #endregion
    }
}