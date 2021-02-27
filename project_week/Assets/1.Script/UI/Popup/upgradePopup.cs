using System;
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
        [SerializeField] GameObject _presentPanel;
        [SerializeField] GameObject _treePanel;
        [SerializeField] upBtn_instance[] upBtns;

        [Header("Present")]
        [SerializeField] Transform _light;
        [Space]
        [SerializeField] Image presentAni;
        [SerializeField] Image present;
        [SerializeField] TextMeshProUGUI presentNames;
        [SerializeField] TextMeshProUGUI presentExs;
        [SerializeField] Image blind;        
        [Space]
        [SerializeField] Image setBtn;
        [SerializeField] Image throwBtn;
        [Space]
        [SerializeField] Sprite _empty;
        [Header("Equip Addon")]
        [SerializeField] Image[] equips;
        [SerializeField] Image[] equipCases;
        [SerializeField] TextMeshProUGUI[] equipLvls;
        [SerializeField] TextMeshProUGUI[] equipNames;
        [SerializeField] TextMeshProUGUI[] equipExs;

        TextMeshProUGUI setBtnTxt;

        PlayerCtrl _player;
        skillTreeComp _tree;

        SkillKeyList _newEquip;
        int _slotCnt = 2;
        int _selectNum;

        // 창 종료시 gameScene 행동
        Action _whenCloseUpgradePanel;

        Color _blue = new Color(0.54f, 0.68f, 0.98f);
        Color _brown = new Color(0.91f, 0.8f, 0.64f);

        // Start is called before the first frame update
        void Awake()
        {
            _treePanel.SetActive(false);
            gameObject.SetActive(false);
            setBtnTxt = setBtn.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            _light.Rotate(Vector3.forward * 2);
        }

        public void setting(PlayerCtrl player, Action whenCloseUpgradePanel)
        {
            _player = player;
            _whenCloseUpgradePanel = whenCloseUpgradePanel;

            _tree = _treePanel.GetComponent<skillTreeComp>();
            _tree.Init(_player, ()=> { gameObject.SetActive(false); _whenCloseUpgradePanel(); });

            for (int i = 0; i < 3; i++)
            {
                upBtns[i].FixedInit(_player.getSkill, (type)=> {
                    _treePanel.SetActive(true);
                    _suggestPanel.SetActive(false);
                    _presentPanel.SetActive(false);

                    _tree.openTap(type);                    
                }, pressThrowBtn);
            }

            upBtns[3].setFeedback(()=> {
                
                pressThrowBtn();
            });
        }

        #region upgrade

        /// <summary> 레벨업할때 오픈 </summary>
        public void levelUpOpen()
        {
            gameObject.SetActive(true);
            _suggestPanel.SetActive(true);
            _treePanel.SetActive(false);
            _presentPanel.SetActive(false);

            suggest();
        }

        /// <summary> 업그레이드 제안 </summary>
        void suggest()
        {
            List<SkillKeyList> slct = new List<SkillKeyList>();
            List<SkillKeyList> _skillList 
                = new List<SkillKeyList> { SkillKeyList.Snowball, SkillKeyList.Iceball, SkillKeyList.IceBat, SkillKeyList.Shield, SkillKeyList.Field, SkillKeyList.Pet };

            for (int i = 0; i < 3; i++)
            {
                float num = UnityEngine.Random.Range(0, 10);

                if (num < 9 - (i * 4)) // 스킬
                {
                    int s = UnityEngine.Random.Range(0, _skillList.Count);
                    slct.Add(_skillList[s]);
                    // slct.Add(SkillKeyList.Field); // 테스트용
                    _skillList.Remove(_skillList[s]);
                    
                }
                else // 능력치
                {
                    SkillKeyList ab;
                    bool bl = false;

                    do
                    {
                        ab = (SkillKeyList)UnityEngine.Random.Range(0, (int)SkillKeyList.Snowball);
                        
                        if (_player.Abils[ab].chk_lvl && slct.Contains(ab) == false)
                        {
                            slct.Add(ab);
                            bl = false;
                        }
                        else
                            bl = true;

                    } while (bl);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int lvl = (slct[i] < SkillKeyList.Snowball) ? _player.Abils[slct[i]].Lvl : 0;

                upBtns[i].setBtn(slct[i], lvl);
            }
        }

        public void pressThrowBtn()
        {
            _whenCloseUpgradePanel();

            gameObject.SetActive(false);
        }

        #endregion

        #region [equip 유물]

        /// <summary> 유물 오픈 </summary>
        //public void presentOpen()
        //{
        //    upgradePanel.SetActive(false);
        //    presentPanel.SetActive(true);

        //    presentAni.gameObject.SetActive(true);
        //    presentAni.raycastTarget = true;
        //    present.gameObject.SetActive(false);
        //    blind.gameObject.SetActive(false);

        //    setBtnActive(false);
        //    throwBtnActive(false);

        //    mySlotRefresh();

        //    _selectNum = -1;

        //    setBtnTxt.text = "장착";

        //    getNewEquip();

        //    for (int i = 0; i < _slotCnt; i++)
        //    {
        //        equips[i].color = Color.white;
        //        equipCases[i].color = Color.white * 0;
        //        equipCases[i].raycastTarget = false;
        //    }

        //    SkillKeyList eq;
        //    for (int i = 0; i < _slotCnt; i++)
        //    {
        //        eq = _player.selectEquips[i];
        //        if (eq == SkillKeyList.max)
        //        {
        //            equips[i].sprite = _empty;
        //            equipLvls[i].text = "";
        //            equipNames[i].text = "";
        //            equipExs[i].text = "";
        //            continue;
        //        }

        //        equips[i].sprite = DataManager.Skillicon[_player.selectEquips[i]];
        //        // equipLvls[i].text = $"+{_player.Equips[eq].Lvl.ToString()}";
        //        equipNames[i].text = DataManager.GetTable<string>(DataTable.skill, ((int)eq).ToString(), SkillValData.skill.ToString());
        //        equipExs[i].text = DataManager.GetTable<string>(DataTable.skill, ((int)eq).ToString(), SkillValData.explain.ToString());
        //    }

        //    present.sprite = DataManager.Skillicon[_newEquip];
        //    presentNames.text = "???";
        //    presentExs.text = "???";

        //    gameObject.SetActive(true);
        //}

        /// <summary> 새로운 장비 고르기 </summary>
        void getNewEquip()
        {
            List<SkillKeyList> eqlist = new List<SkillKeyList>();
            //for (SkillKeyList eq = SkillKeyList.poison; eq < SkillKeyList.max; eq++)
            //{
            //    if (_player.Equips[eq].chk_lvl)
            //    {
            //        eqlist.Add(eq);
            //    }
            //}

            int num = UnityEngine.Random.Range(0, eqlist.Count);
            
            _newEquip = eqlist[num];
        }

        /// <summary> 같은거 있나 확인 </summary>
        //bool chkSameEquip()
        //{
        //    for (int i = 0; i < _slotCnt; i++)
        //    {
        //        if (_player.selectEquips[i] < SkillKeyList.poison)
        //        {
        //            Debug.LogError("잘못된 장비 요청");
        //        }

        //        if (_newEquip == _player.selectEquips[i])
        //        {
        //            bool bl;
        //            for (int j = 0; j < _slotCnt; j++)
        //            {
        //                bl = (_newEquip != _player.selectEquips[j]);

        //                equips[j].color = (bl) ? Color.gray : Color.white;
        //                equipCases[j].raycastTarget = !bl;
        //            }

        //            setBtnTxt.text = "강화";
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary> 슬롯 개수 새로고침 </summary>
        //void mySlotRefresh()
        //{
        //    if (_player.isGetSlot)
        //    {
        //        _slotCnt = 3;
        //        equipCases[0].transform.localPosition = new Vector3(-350f, 105f);
        //        equipCases[1].transform.localPosition = new Vector3(0f, 105f);
        //        equipCases[2].gameObject.SetActive(true);
        //        equipCases[2].transform.localPosition = new Vector3(350f, 105f);
        //    }
        //    else
        //    {
        //        _slotCnt = 2;
        //        equipCases[0].transform.localPosition = new Vector3(-200f, 105f);
        //        equipCases[1].transform.localPosition = new Vector3(200f, 105f);
        //        equipCases[2].gameObject.SetActive(false);
        //    }
        //}

        /// <summary> 선물 누르기 </summary>
        public void pressBox()
        {
            presentAni.raycastTarget = false;
            // StartCoroutine(boxOpen());
        }

        //IEnumerator boxOpen()
        //{
        //    Color col = Color.white;
        //    col.a = 0;
        //    blind.gameObject.SetActive(true);

        //    while (col.a < 1f)
        //    {
        //        col.a += Time.deltaTime * 2;
        //        blind.color = col;

        //        yield return new WaitForEndOfFrame();
        //    }

        //    presentAni.gameObject.SetActive(false);
        //    present.gameObject.SetActive(true);

        //    bool result = chkSameEquip();
        //    if (result == false)
        //    {
        //        for (int i = 0; i < _slotCnt; i++)
        //        {
        //            equips[i].color = Color.white;
        //            equipCases[i].color = Color.white * 0;
        //            equipCases[i].raycastTarget = true;
        //        }
        //    }

        //    yield return new WaitForSeconds(0.5f);
            
        //    while (col.a > 0f)
        //    {
        //        col.a -= Time.deltaTime*2;
        //        blind.color = col;

        //        yield return new WaitForEndOfFrame();
        //    }

        //    presentNames.text = DataManager.GetTable<string>(DataTable.skill, ((int)_newEquip).ToString(), SkillValData.skill.ToString());
        //    presentExs.text = DataManager.GetTable<string>(DataTable.skill, ((int)_newEquip).ToString(), SkillValData.explain.ToString());

        //    blind.gameObject.SetActive(false);
        //    setBtnActive(true);
        //    throwBtnActive(true);

        //    yield return null;
        //}

        /// <summary> 장착할 슬롯 고르기 </summary>
        public void selectEquipBtn(int i)
        {
            //_selectNum = i;
            //setBtnTxt.text = (_player.selectEquips[i] == SkillKeyList.max) ? "장착" : (_player.selectEquips[i] == _newEquip) ? "강화" : "교체";

            //for (int n = 0; n < _slotCnt; n++)
            //{
            //    equipCases[n].color = (i == n) ? Color.green : Color.white * 0;
            //}
        }

        /// <summary> 장착 버튼 활성화 </summary>
        void setBtnActive(bool bl)
        {
            setBtn.raycastTarget = bl;
            setBtn.color = (bl) ? _blue : _blue * 0.6f;
        }

        /// <summary> 버리기 버튼 활성화 </summary>
        void throwBtnActive(bool bl)
        {
            throwBtn.raycastTarget = bl;
            throwBtn.color = (bl) ? _brown : _brown * 0.6f;
        }

        /// <summary> 장착 버튼 </summary>
        public void pressSetBtn()
        {
            if(_selectNum == -1)
            {
                return;
            }

            // _player.setEquip(_selectNum, _newEquip);

            List<SkillKeyList> eqlist = new List<SkillKeyList>();

            //for (SkillKeyList eq = SkillKeyList.poison; eq < SkillKeyList.max; eq++)
            //{
            //    if (eq == SkillKeyList.slot)
            //        continue;

            //    if (_player.Equips[eq].active)
            //    {
            //        bool has = false;
            //        for (int i = 0; i < _slotCnt; i++)
            //        {
            //            if(eq ==_player.selectEquips[i])
            //            {
            //                has = true;
            //                break;    
            //            }
            //        }

            //        if (has == false)
            //        {
            //            _player.Equips[eq].clear();
            //            _player.chkSkillObj();
            //        }
            //    }
            //}

            _whenCloseUpgradePanel();
            gameObject.SetActive(false);
        }

        

        #endregion
    }
}