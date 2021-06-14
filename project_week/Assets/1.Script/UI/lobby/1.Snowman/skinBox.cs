using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class skinBox : MonoBehaviour
    {
        public enum cur { standard, quest, coin, gem }

        [Header("snowman")]
        [SerializeField] Image _skinImg;
        [SerializeField] TextMeshProUGUI _skinNameTxt;
        [Header("rank")]
        [SerializeField] Image _rankCase;
        [SerializeField] Image _rankBack;
        [Space]
        [SerializeField] GameObject _block;
        [Header("price")]
        [SerializeField] Image _curIcon;
        [SerializeField] RectTransform _priceSize;
        [SerializeField] TextMeshProUGUI _priceTxt;
        [Space]
        [SerializeField] Image _btnCurIcon;
        [SerializeField] TextMeshProUGUI _btnPriceTxt;
        [Space]
        [SerializeField] Sprite[] _curImg;
        [Header("etc")]
        [SerializeField] Image _selectBlock;
        [Header("btn")]
        [SerializeField] Button _skinBtn;
        [SerializeField] Button _purchaseBtn;
        [SerializeField] Button _equipBtn;

        Action _refreshCost;

        // 스킨 정보
        SkinKeyList _skin;
        public cur Cur { get; private set; }
        public int Price { get; private set; }
        int _rank;

        lobbySnowmanComp _lsc;

        /// <summary> (가격확인후) 구매가능 여부 </summary>
        public bool isPurchasable
        {
            get
            {
                return (Cur == cur.gem && Price <= BaseManager.userGameData.Gem) || (Cur == cur.coin && Price <= BaseManager.userGameData.Coin);
            }
        }

        /// <summary> 초기화 </summary>
        public void setSkinBox(SkinKeyList skin, lobbySnowmanComp lsc)
        {
            _lsc = lsc;

            // 정보 추출
            _skin = skin;
            _skinNameTxt.text = D_skin.GetEntity(_skin.ToString()).f_skinname;
            _skinImg.sprite = DataManager.SkinSprite[_skin];
            _rank = D_skin.GetEntity(_skin.ToString()).f_rank;
            // 가격
            string str = D_skin.GetEntity(_skin.ToString()).f_currency;
            Cur = EnumHelper.StringToEnum<cur>(str);

            Price = D_skin.GetEntity(_skin.ToString()).f_price;  
            if(Cur >= cur.coin)
            {
                _curIcon.sprite = _curImg[(int)Cur - 2];
                _priceTxt.text = Price.ToString();
                _btnCurIcon.sprite = _curImg[(int)Cur - 2];
                _btnPriceTxt.text = Price.ToString();
            }

            // 랭크 -> 색
            _rankCase.color = gameValues._skinRankColor[_rank]; // new Color(0f, 0.13f, 0.47f);
            _rankBack.color = gameValues._skinRankColor[_rank]; // new Color(0.1f, 0.15f, 0.4f);

            // 버튼 세팅
            _skinBtn.onClick.AddListener(Select_TempSkin);
            _purchaseBtn.onClick.AddListener(Purchase_SelectSkin);
            _equipBtn.onClick.AddListener(Confirm_SelectSkin);

            chkState();
        }

        /// <summary> 현재 스킨 상태(장착, 미장착, 미보유)확인 </summary>
        public void chkState()
        {
            // 보유했는지 확인
            bool result = ((BaseManager.userGameData.HasSkin & (1 << (int)_skin)) > 0);

            if (result == false) // 미보유
            {
                switch (Cur)
                {
                    case cur.quest:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(false);
                        _priceSize.anchoredPosition = new Vector2(0, 50);
                        _priceSize.sizeDelta = new Vector2(300, 80);

                        _priceTxt.text = "구매불가";
                        _priceTxt.color = Color.white;
                        break;
                    case cur.coin:
                    case cur.gem:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(true);
                        _priceSize.anchoredPosition = new Vector2(30, 50);
                        _priceSize.sizeDelta = new Vector2(300, 80);
                        break;
                    case cur.standard:
                        BaseManager.userGameData.HasSkin |= (1 << (int)_skin);
                        PossibleSelect();
                        break;
                }
            }
            else // 보유
            {
                // 장착했는지 확인
                result = (BaseManager.userGameData.Skin == _skin);
                if (result) // 장착
                {
                    _block.SetActive(false);
                    _curIcon.gameObject.SetActive(false);
                    _priceSize.anchoredPosition = new Vector2(0, 50);
                    _priceSize.sizeDelta = new Vector2(300, 80);

                    _priceTxt.text = "장착중";
                    _priceTxt.color = Color.green;
                }
                else // 미장착
                {
                    PossibleSelect();
                }
            }

            _selectBlock.color = (_lsc.TempSelectSkin == _skin) ? Color.green : Color.black;

            ButtonState();
        }

        /// <summary> 스킨 장착 가능 상태 </summary>
        public void PossibleSelect()
        {
            _block.SetActive(false);
            _curIcon.gameObject.SetActive(false);
            _priceSize.anchoredPosition = new Vector2(0, 50);
            _priceSize.sizeDelta = new Vector2(300, 80);
            _priceTxt.text = "선택가능";
            _priceTxt.color = Color.white;
        }

        /// <summary> 부속 버튼 상태 </summary>
        public void ButtonState()
        {
            if (_lsc.TempSelectSkin != _skin)
            {
                _purchaseBtn.gameObject.SetActive(false);
                _equipBtn.gameObject.SetActive(false);
            }
            else // 현재 선택중
            {
                if (_lsc.hasCheck(_skin)) // 보유
                {
                    if (BaseManager.userGameData.Skin == _skin) // 장착중
                    {
                        _purchaseBtn.gameObject.SetActive(false);
                        _equipBtn.gameObject.SetActive(false);
                    }
                    else // 미장착
                    {
                        _purchaseBtn.gameObject.SetActive(false);
                        _equipBtn.gameObject.SetActive(true);
                    }
                }
                else // 미보유
                {
                    _purchaseBtn.gameObject.SetActive(Cur != cur.quest);
                    _equipBtn.gameObject.SetActive(false);
                }
            }
        }

        #region [ button ] 

        /// <summary> 스킨 가선택 (정보 관람용) </summary>
        public void Select_TempSkin()
        {
            _lsc.Apply_tempSkin(_skin);
            ButtonState();
            //_tempSelect(_skin);
        }

        /// <summary> 스킨 찐선택 (스킨 선택) </summary>
        public void Confirm_SelectSkin()
        {
            _lsc.Apply_skinButton(_skin);
            //_confirmSelect(_skin);
        }

        /// <summary> 스킨 구매 </summary>
        public void Purchase_SelectSkin()
        {
            _lsc.Purchase_skinButton(_skin);
        }

        #endregion
    }
}