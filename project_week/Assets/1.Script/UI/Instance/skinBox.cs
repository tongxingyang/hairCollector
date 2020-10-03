using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class skinBox : MonoBehaviour
    {
        enum cur { standard, quest, gold, gem }

        [Header("name & Image")]
        [SerializeField] TextMeshProUGUI _skinName;
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
        cur _cur;
        int _price;

        public Action<SkinKeyList> WhenSkinSelect { set => _whenSkinSelect = value; }

        public void setSkinBox(SkinKeyList skin)
        {
            _skin = skin;
            _skinName.text = DataManager.GetTable<string>(DataTable.skin, ((int)skin).ToString(), SkinValData.skinname.ToString());
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
                string str = DataManager.GetTable<string>(DataTable.skin, ((int)skin).ToString(), SkinValData.currency.ToString());
                _cur = EnumHelper.StringToEnum<cur>(str);
                _price = DataManager.GetTable<int>(DataTable.skin, ((int)skin).ToString(), SkinValData.price.ToString());

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
                _whenSkinSelect(_skin);
            }
            else // 없어서 구매
            {
                purchaseSkin();
            }
        }

        void purchaseSkin()
        {
            BaseManager.userGameData.HasSkin |= (1 << (int)_skin);
            if ((BaseManager.userGameData.HasSkin & (1 << (int)_skin)) > 0)
            {
                Debug.Log(_skin.ToString() + ": 구매완료");
                possibleSelect();
            }
            else
            {
                Debug.LogError(_skin.ToString() + ": 구매에러");
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
    }
}