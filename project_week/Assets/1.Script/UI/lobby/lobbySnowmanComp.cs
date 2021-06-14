using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace week
{
    public class lobbySnowmanComp : MonoBehaviour
    {
        #region [ snowman value ] ===================================

        [Header("SNOW")]
        [SerializeField] TextMeshProUGUI _nowSkinName;
        [SerializeField] TextMeshProUGUI _nowSkinLevel;
        [SerializeField] Image _snowman;
        [SerializeField] Image _skinRankRabel;
        [SerializeField] TextMeshProUGUI _hpTxt;
        [SerializeField] TextMeshProUGUI _attTxt;
        [SerializeField] TextMeshProUGUI _defTxt;
        [SerializeField] TextMeshProUGUI _hpgenTxt;
        [SerializeField] TextMeshProUGUI _effTxt;
        [Space]
        [SerializeField] Image _enhanceBtn;
        [SerializeField] GameObject[] _enhancePriceTag;
        [SerializeField] TextMeshProUGUI _enhancePrice;
        [Space]
        [SerializeField] Image _applyBtn;
        [SerializeField] TextMeshProUGUI _applyTxt;
        #endregion

        #region [ skin value ] ===================================

        [Header("SKIN")]
        [SerializeField] TextMeshProUGUI _hasSkinTxt;
        [SerializeField] Transform _skinBoxParent;
        [SerializeField] GameObject _skinBox;

        Dictionary<SkinKeyList, skinBox> _skinBoxies;
        public SkinKeyList TempSelectSkin { get; private set; }  // 임시 선택스킨
        float[] _tempStats;             // 임시 능력치
        int _tempSkinLvl;
        int _tempSkinEnhancePrice;

        bool isMaster { set { _enhancePriceTag[0].SetActive(!value); _enhancePriceTag[1].SetActive(value); } }
        public bool hasCheck(SkinKeyList skin) { return ((BaseManager.userGameData.HasSkin & (1 << (int)skin)) > 0); }

        #endregion

        Action _costRefresh; // 뭔가 구매시 재화 새로고침
        Action _skinRefresh; // 준비창 스킨 새로고침

        #region [ snowman ]

        /// <summary> snowman 초기화 </summary>
        public void Init(Action costRefresh, Action skinRefresh)
        {
            _costRefresh = costRefresh;
            _skinRefresh = skinRefresh;

            _enhanceBtn.GetComponent<Button>().onClick.AddListener(skinUpgrade);

            chkSkinCount();

            skinStart();
        }

        /// <summary> 스킨개수 세기 </summary>
        void chkSkinCount()
        {
            int full = 0, has = 0;
            for (SkinKeyList i = 0; i < SkinKeyList.max; i++)
            {
                if (D_skin.GetEntity(i.ToString()).f_enable)
                {
                    full++;
                    if ((BaseManager.userGameData.HasSkin & (1 << (int)i)) > 0)
                        has++;
                }
            }
            _hasSkinTxt.text = $"보유중인 스킨 ({has}/{full})";
        }

        /// <summary> snowman 능력치 보여주기 (새로고침) </summary>
        public void showSnowmanInfo()
        {
            // 눈사람 이미지
            _snowman.sprite = DataManager.SkinSprite[TempSelectSkin];

            _tempSkinLvl = BaseManager.userGameData.SkinLevel[(int)TempSelectSkin];
            _nowSkinLevel.text = $"Lv.{_tempSkinLvl}";
            _nowSkinName.text = D_skin.GetEntity(TempSelectSkin.ToString()).f_skinname;
            
            if (_tempSkinLvl < 10)
            {
                int rank = D_skin.GetEntity(TempSelectSkin.ToString()).f_rank;
                int rate = Convert.ToInt32(Mathf.Pow(2, rank > 3 ? 3 : rank));
                _tempSkinEnhancePrice = gameValues._skinEnhancePrice[_tempSkinLvl] * rate;
                _enhancePrice.text = _tempSkinEnhancePrice.ToString();
            }

            BaseManager.userGameData.applyLevel();

            // 체력
            _hpTxt.color = (_tempStats[0] > 1) ? Color.green : Color.white;            
            _hpTxt.text = $"{BaseManager.userGameData.o_Hp * _tempStats[0]}";
            // 공격력
            _attTxt.color = (_tempStats[1] > 1) ? Color.green : Color.white;
            _attTxt.text = $"{BaseManager.userGameData.o_Att * _tempStats[1]}";
            // 방어력
            _defTxt.color = (_tempStats[2] > 0) ? Color.green : Color.white;
            _defTxt.text = $"{BaseManager.userGameData.o_Def + _tempStats[2]}";
            // 체젠
            _hpgenTxt.color = (_tempStats[3] > 0) ? Color.green : Color.white;
            _hpgenTxt.text = $"{BaseManager.userGameData.o_Hpgen + _tempStats[3]}";
            // 능력
            _effTxt.text = BaseManager.userGameData.getSkinExplain(TempSelectSkin, false);

            _skinRankRabel.color = gameValues._skinRankColor[D_skin.GetEntity(TempSelectSkin.ToString()).f_rank];
            setEnhanceBtn();
        }

        #endregion

        #region [ skin ]

        /// <summary> skin box 설치 및 초기화 </summary>
        public void skinStart()
        {
            TempSelectSkin = BaseManager.userGameData.Skin;

            List<SkinKeyList>[] list = new List<SkinKeyList>[5];            
            for(int i = 0; i < 5; i++) 
            {
                list[i] = new List<SkinKeyList>();
            }
            for (SkinKeyList i = SkinKeyList.snowman; i < SkinKeyList.max; i++)
            {
                if (D_skin.GetEntity(i.ToString()).f_enable)
                {
                    list[D_skin.GetEntity(i.ToString()).f_rank].Add(i);
                }
            }

            _skinBoxies = new Dictionary<SkinKeyList, skinBox>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    skinBox sb = Instantiate(_skinBox).GetComponent<skinBox>();
                    SkinKeyList skin = list[i][j];

                    sb.transform.SetParent(_skinBoxParent);
                    sb.transform.localScale = Vector3.one;
                    sb.setSkinBox(skin, this);

                    _skinBoxies.Add(skin, sb);
                }
            }

            Apply_tempSkin(TempSelectSkin);

            BaseManager.userGameData.applySkin();

            skinBoxRefresh();
            showSnowmanInfo();
            skinApplyButtonRefresh();
        }

        /// <summary> 임시 스킨 장착 능력치 적용 </summary>
        public void Apply_tempSkin(SkinKeyList newSkin)
        {
            // 임시 스킨 스탯 적용
            TempSelectSkin = newSkin;
            string key = TempSelectSkin.ToString();

            _tempStats = new float[(int)UserGameData.defaultStat.max];
            for (UserGameData.defaultStat i = UserGameData.defaultStat.hp; i < UserGameData.defaultStat.max; i++)
            {
                float m = (i == UserGameData.defaultStat.def || i == UserGameData.defaultStat.hpgen) ? 1f : 0.01f;
                _tempStats[(int)i] = D_skin.GetEntity(key.ToString()).Get<float>((SkinValData.d_hp + (int)i).ToString()) * m;

                if (i < UserGameData.defaultStat.speed)
                {
                    _tempStats[(int)i] += D_skin.GetEntity(key.ToString()).Get<float>((SkinValData.ex_hp + (int)i).ToString()) * m * BaseManager.userGameData.SkinLevel[(int)TempSelectSkin];
                }
            }

            showSnowmanInfo();
            skinBoxRefresh();
            skinApplyButtonRefresh();
        }

        /// <summary> 스킨 박스 초기화 </summary>
        void skinBoxRefresh()
        {
            foreach (skinBox sb in _skinBoxies.Values)
            {
                sb.chkState();
            }
        }

        /// <summary> 스킨 구매/적용 버튼 새로고침 </summary>
        void skinApplyButtonRefresh()
        {
            // 보유했는지 확인
            bool result = hasCheck(TempSelectSkin);
            if (result) // 보유
            {
                // 장착했는지 확인
                result = (BaseManager.userGameData.Skin == TempSelectSkin);
                if (result) // 장착 --> 아무일 없음
                {
                    _applyTxt.text = "장착중";
                    _applyBtn.color = new Color(0.6f, 0.6f, 0.6f);
                }
                else // 미장착 --> 장착
                {
                    _applyTxt.text = "장 착";
                    _applyBtn.color = new Color(1f, 0.6f, 0f);
                }
            }
            else // 미보유 --> 구매하기
            {
                _applyTxt.text = "구 매";
                _applyBtn.color = Color.white;

                // 구매 가능여부
                if (_skinBoxies[TempSelectSkin].isPurchasable) // (구매가능) 돈 있음
                {
                    
                }
                else // (구매 불가) 돈 없음
                {
                }
            }
        }

        /// <summary> 스킨 구매/적용 버튼 </summary>
        public void skinApplyButton()
        {
            // 보유했는지 확인
            bool result = hasCheck(TempSelectSkin);
            if (result) // 보유
            {
                // 장착했는지 확인
                result = (BaseManager.userGameData.Skin == TempSelectSkin);
                if (result) // 장착 --> 아무일 없음
                {
                    Debug.Log("this skin is already applied");
                }
                else // 미장착 --> 장착
                {
                    BaseManager.userGameData.Skin = TempSelectSkin;
                    _skinRefresh?.Invoke();

                    skinBoxRefresh();

                    AuthManager.instance.SaveDataServer(true);
                }

                BaseManager.userGameData.applySkin();
            }
            else // 미보유 --> 구매하기
            {
                // 구매 가능여부
                if (_skinBoxies[TempSelectSkin].isPurchasable) // (구매가능) 돈 있음
                {
                    WindowManager.instance.Win_message.showPresentAct(D_skin.GetEntity(TempSelectSkin.ToString()).f_skinname + "을 구매하시겠습눈?",
                        DataManager.SkinSprite[TempSelectSkin], () =>
                        {
                            if (_skinBoxies[TempSelectSkin].Cur == skinBox.cur.gem)
                            {
                                BaseManager.userGameData.Gem -= _skinBoxies[TempSelectSkin].Price;
                            }
                            else if (_skinBoxies[TempSelectSkin].Cur == skinBox.cur.coin)
                            {
                                BaseManager.userGameData.Coin -= _skinBoxies[TempSelectSkin].Price;
                            }

                            // 구매
                            BaseManager.userGameData.HasSkin |= (1 << (int)TempSelectSkin);
                            if (hasCheck(TempSelectSkin))
                            {
                                Debug.Log(TempSelectSkin.ToString() + ": 구매완료");
                                WindowManager.instance.Win_celebrate.whenStatusUp();
                                WindowManager.instance.Win_purchase.setSkinPurchase(TempSelectSkin)
                                    .setImgSize();

                                _skinBoxies[TempSelectSkin].PossibleSelect();
                                _costRefresh?.Invoke();

                                BaseManager.userGameData.Skin = TempSelectSkin;
                                _skinRefresh?.Invoke();
                                setEnhanceBtn();
                                skinBoxRefresh();

                                AuthManager.instance.SaveDataServer(true);
                            }
                            else
                            {
                                Debug.LogError(TempSelectSkin.ToString() + ": 구매에러 : 이미 보유중인데 구매시도");
                            }

                        });
                }
                else // (구매 불가) 돈 없음
                {
                    switch (_skinBoxies[TempSelectSkin].Cur)
                    {
                        case skinBox.cur.gem:
                            WindowManager.instance.showMessage("보석이 부족해요");
                            break;
                        case skinBox.cur.coin:
                            WindowManager.instance.showMessage("코인이 부족해요");
                            break;
                        default:
                            WindowManager.instance.showMessage("아직 얻지 못했어요");
                            break;
                    }
                }

            }

            skinApplyButtonRefresh();
        }

        /// <summary> 스킨 강화버튼 초기화 </summary>
        void setEnhanceBtn()
        {
            if (hasCheck(TempSelectSkin) == false)
            {
                isMaster = false;
                _enhanceBtn.color = Color.gray;
                _enhanceBtn.raycastTarget = false;
                return;
            }

            if (BaseManager.userGameData.SkinLevel[(int)TempSelectSkin] == 10)
            {
                isMaster = true;
                _enhanceBtn.color = new Color(1f, 0.6f, 0f);
                _enhanceBtn.raycastTarget = false;
            }
            else
            {
                isMaster = false;

                if (BaseManager.userGameData.Coin >= _tempSkinEnhancePrice)
                {
                    _enhanceBtn.color = Color.green;
                    _enhanceBtn.raycastTarget = true;
                }
                else
                {
                    _enhanceBtn.color = Color.gray;
                    _enhanceBtn.raycastTarget = false;
                }
            }
        }

        /// <summary> 업그레이드 </summary>
        void skinUpgrade()
        {
            if (BaseManager.userGameData.Coin >= _tempSkinEnhancePrice)
            {
                BaseManager.userGameData.Coin -= _tempSkinEnhancePrice;
                BaseManager.userGameData.SkinLevel[(int)TempSelectSkin]++;

                Apply_tempSkin(TempSelectSkin);

                _costRefresh?.Invoke();

                AuthManager.instance.SaveDataServer(false);
            }
        }

        #endregion

        #region [ skinBox용 ]

        /// <summary> 스킨 구매 버튼 </summary>
        public void Purchase_skinButton(SkinKeyList skin)
        {
            if (TempSelectSkin != skin)
                Debug.LogError("Purchase_skinButton : 스킨 구매 오류");

            // 보유했는지 확인
            bool result = hasCheck(TempSelectSkin);
            if (result == false) // 미보유 --> 구매하기
            {
                // 구매 가능여부
                if (_skinBoxies[TempSelectSkin].isPurchasable) // (구매가능) 돈 있음
                {
                    WindowManager.instance.Win_message.showPresentAct(D_skin.GetEntity(TempSelectSkin.ToString()).f_skinname + "을 구매하시겠습눈?",
                        DataManager.SkinSprite[TempSelectSkin], () =>
                        {
                            if (_skinBoxies[TempSelectSkin].Cur == skinBox.cur.gem)
                            {
                                BaseManager.userGameData.Gem -= _skinBoxies[TempSelectSkin].Price;
                            }
                            else if (_skinBoxies[TempSelectSkin].Cur == skinBox.cur.coin)
                            {
                                BaseManager.userGameData.Coin -= _skinBoxies[TempSelectSkin].Price;
                            }

                            // 구매
                            BaseManager.userGameData.HasSkin |= (1 << (int)TempSelectSkin);
                            if (hasCheck(TempSelectSkin))
                            {
                                Debug.Log(TempSelectSkin.ToString() + ": 구매완료");
                                WindowManager.instance.Win_celebrate.whenStatusUp();
                                WindowManager.instance.Win_purchase.setSkinPurchase(TempSelectSkin)
                                    .setImgSize();

                                _skinBoxies[TempSelectSkin].PossibleSelect();
                                _costRefresh?.Invoke();

                                BaseManager.userGameData.Skin = TempSelectSkin;
                                _skinRefresh?.Invoke();
                                setEnhanceBtn();
                                skinBoxRefresh();
                                chkSkinCount();

                                AuthManager.instance.SaveDataServer(true);
                            }
                            else
                            {
                                Debug.LogError(TempSelectSkin.ToString() + ": 구매에러 : 이미 보유중인데 구매시도");
                            }

                        });

                    skinApplyButtonRefresh(); // 새로고침
                }
                else // (구매 불가) 돈 없음
                {
                    switch (_skinBoxies[TempSelectSkin].Cur)
                    {
                        case skinBox.cur.gem:
                            WindowManager.instance.showMessage("보석이 부족해요");
                            break;
                        case skinBox.cur.coin:
                            WindowManager.instance.showMessage("코인이 부족해요");
                            break;
                        default:
                            WindowManager.instance.showMessage("아직 얻지 못했어요");
                            break;
                    }
                }
            }
        }

        /// <summary> 스킨 적용 버튼 </summary>
        public void Apply_skinButton(SkinKeyList skin)
        {
            if (TempSelectSkin != skin)
                Debug.LogError("Apply_skinButton : 스킨 적용 오류");

            // 보유했는지 확인
            bool result = hasCheck(TempSelectSkin);
            if (result) // 보유
            {
                // 장착했는지 확인
                result = (BaseManager.userGameData.Skin == TempSelectSkin);
                if (result) // 장착 --> 아무일 없음
                {
                    Debug.Log("this skin is already applied");
                }
                else // 미장착 --> 장착
                {
                    BaseManager.userGameData.Skin = TempSelectSkin;
                    _skinRefresh?.Invoke();

                    skinBoxRefresh();

                    AuthManager.instance.SaveDataServer(true);
                }

                BaseManager.userGameData.applySkin();
            }

            skinApplyButtonRefresh();
        }

        #endregion
    }
}