using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

namespace week
{
    public class skinBox : MonoBehaviour
    {
        enum cur { standard, quest, gold, gem }

        [Header("name & Image")]
        [SerializeField] TextMeshProUGUI _skinNameTxt;
        [SerializeField] Image _skinImg;
        [SerializeField] GameObject _block;
        [Header("price")]
        [SerializeField] Image _priceBox;
        [SerializeField] Image _curIcon;
        [SerializeField] RectTransform _priceSize;
        [SerializeField] TextMeshProUGUI _priceTxt;

        [SerializeField] Sprite[] _curImg;

        Action<SkinKeyList> _whenSkinSelect;

        SkinKeyList _skin;
        string _skinName;

        cur _cur;
        int _price;

        public void setAction(Action<SkinKeyList> wss)
        {
            _whenSkinSelect = wss;
        }

        public void setSkinBox(SkinKeyList skin)
        {
            _skin = skin;
            _skinName = DataManager.GetTable<string>(DataTable.skin, skin.ToString(), SkinValData.skinname.ToString());
            _skinNameTxt.text = _skinName;
            _skinImg.sprite = DataManager.SkinSprite[skin];

            bool result = (BaseManager.userGameData.Skin == skin);
            if (result)
            {
                _block.SetActive(false);
                _curIcon.gameObject.SetActive(false);
                _priceSize.anchoredPosition = new Vector2(0, 0);
                _priceSize.sizeDelta = new Vector2(260, 80);
                _priceBox.color = Color.yellow;
                _priceTxt.text = "장착중";
                return;
            }

            result = ((BaseManager.userGameData.HasSkin & (1 << (int)skin)) > 0);

            if (result == false)
            {
                string str = DataManager.GetTable<string>(DataTable.skin, skin.ToString(), SkinValData.currency.ToString());
                _cur = EnumHelper.StringToEnum<cur>(str);
                _price = DataManager.GetTable<int>(DataTable.skin, skin.ToString(), SkinValData.price.ToString());

                switch (_cur)
                {
                    case cur.quest:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(false);
                        _priceSize.anchoredPosition = new Vector2(0, 0);
                        _priceSize.sizeDelta = new Vector2(260, 80);
                        _priceBox.color = Color.gray;
                        _priceTxt.text = "구매불가";
                        break;
                    case cur.gold:
                    case cur.gem:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(true);
                        _priceSize.anchoredPosition = new Vector2(30, 0);
                        _priceSize.sizeDelta = new Vector2(200, 80);
                        _curIcon.sprite = _curImg[(int)_cur - 2];
                        _priceBox.color = Color.white;
                        _priceTxt.text = _price.ToString();
                        break;
                    case cur.standard:
                        possibleSelect();
                        break;
                }
            }
            else
            {
                possibleSelect();
            }
        }

        public void selectSkin()
        {
            if ((BaseManager.userGameData.HasSkin & (1 << (int)_skin)) > 0) // 있어서 선택
            {
                _whenSkinSelect?.Invoke(_skin);
            }
            else // 없어서 구매
            {
                if ((_cur == cur.gem && _price <= BaseManager.userGameData.Gem) || (_cur == cur.gold && _price <= BaseManager.userGameData.Coin)) // 돈 있음
                {
                    WindowManager.instance.Win_message.showPresentAct(_skinName + "을 구매하시겠습눈?",
                        DataManager.SkinSprite[_skin], ()=> 
                    {
                        if (_cur == cur.gem)
                        {
                            BaseManager.userGameData.Gem -= _price;
                        }
                        else if (_cur == cur.gold)
                        {
                            BaseManager.userGameData.Coin -= _price;
                        }

                        purchaseSkin();
                    });
                }
                else // 돈 없음
                {
                    if (_cur == cur.gem || _cur == cur.gold)
                    {
                        string str = "";
                        if (_cur == cur.gem) { str = "보석"; }
                        else if (_cur == cur.gold) { str = "코인"; }

                        WindowManager.instance.showMessage(str + "이 부족해요");
                    }
                    else
                    {
                        WindowManager.instance.showMessage("아직 얻지 못했어요");
                    }
                }
            }
        }

        /// <summary> 스킨 구매 </summary>
        void purchaseSkin()
        {
            BaseManager.userGameData.HasSkin |= (1 << (int)_skin);
            if ((BaseManager.userGameData.HasSkin & (1 << (int)_skin)) > 0)
            {
                Debug.Log(_skin.ToString() + ": 구매완료");
                WindowManager.instance.Win_celebrate.whenPurchase();
                WindowManager.instance.Win_purchase.setOpen(DataManager.SkinSprite[_skin]);

                possibleSelect();

                AuthManager.instance.AllSaveUserEntity();
            }
            else
            {
                Debug.LogError(_skin.ToString() + ": 구매에러 : 이미 보유중인데 구매시도");
            }
        }

        void possibleSelect()
        {
            _block.SetActive(false);
            _curIcon.gameObject.SetActive(false);
            _priceSize.anchoredPosition = new Vector2(0, 0);
            _priceSize.sizeDelta = new Vector2(260, 80);
            _priceBox.color = Color.yellow;
            _priceTxt.text = "선택가능";
        }

        public void chkState()
        {
            bool result = (BaseManager.userGameData.Skin == _skin);
            if (result)
            {
                _block.SetActive(false);
                _curIcon.gameObject.SetActive(false);
                _priceSize.anchoredPosition = new Vector2(0, 0);
                _priceSize.sizeDelta = new Vector2(260, 80);
                _priceBox.color = Color.yellow;
                _priceTxt.text = "장착중";
                return;
            }

            if ((BaseManager.userGameData.HasSkin & (1 << (int)_skin)) == 0)
            {
                string str = DataManager.GetTable<string>(DataTable.skin, _skin.ToString(), SkinValData.currency.ToString());
                _cur = EnumHelper.StringToEnum<cur>(str);
                _price = DataManager.GetTable<int>(DataTable.skin, _skin.ToString(), SkinValData.price.ToString());

                switch (_cur)
                {
                    case cur.quest:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(false);
                        _priceSize.anchoredPosition = new Vector2(0, 0);
                        _priceSize.sizeDelta = new Vector2(260, 80);
                        _priceBox.color = Color.gray;
                        _priceTxt.text = "구매불가";
                        break;
                    case cur.gold:
                    case cur.gem:
                        _block.SetActive(true);
                        _curIcon.gameObject.SetActive(true);
                        _priceSize.anchoredPosition = new Vector2(30, 0);
                        _priceSize.sizeDelta = new Vector2(200, 80);
                        _curIcon.sprite = _curImg[(int)_cur - 2];
                        _priceBox.color = Color.white;
                        _priceTxt.text = _price.ToString();
                        break;
                    case cur.standard:
                        possibleSelect();
                        break;
                }
            }
            else
            {
                possibleSelect();
            }
        }
    }
}