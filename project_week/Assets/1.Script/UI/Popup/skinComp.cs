using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class skinComp : MonoBehaviour, UIInterface
    {
        [Header("info")]
        [SerializeField] Image _snowman;
        [SerializeField] TextMeshProUGUI _hpTxt;
        [SerializeField] TextMeshProUGUI _attTxt;
        [SerializeField] TextMeshProUGUI _defTxt;
        [SerializeField] TextMeshProUGUI _hpgenTxt;
        [SerializeField] TextMeshProUGUI _effTxt;
        [Header("under")]
        [SerializeField] Transform _skinBoxParent;
        [SerializeField] GameObject _skinBox;

        skinBox[] _skinBoxies;
        SkinKeyList _selectSkin;

        #region [초기화]

        void Awake()
        {
            _selectSkin = (SkinKeyList)BaseManager.userGameData.Skin;
            showSkinInfo();

            _skinBoxies = new skinBox[(int)SkinKeyList.max];
            for (int i = 0; i < (int)SkinKeyList.max; i++)
            {
                _skinBoxies[i] = Instantiate(_skinBox).GetComponent<skinBox>();
                _skinBoxies[i].transform.SetParent(_skinBoxParent);
                _skinBoxies[i].setSkinBox((SkinKeyList)i);
                _skinBoxies[i].WhenSkinSelect = changeSkin;
            }

            gameObject.SetActive(false);
        }

        /// <summary> 창 오픈시 </summary>
        public void open()
        {
            showSkinInfo();
            gameObject.SetActive(true);
        }

        /// <summary> 창 닫기 </summary>
        public void close()
        {
            gameObject.SetActive(false);
        }

        void changeSkin(SkinKeyList newSkin)
        {
            _selectSkin = newSkin;
            BaseManager.userGameData.Skin = newSkin;
            BaseManager.userGameData.applySkin();
            showSkinInfo();
        }

        void showSkinInfo()
        {
            _snowman.sprite = DataManager.SkinSprite[_selectSkin];

            string str;
            float f;

            str = BaseManager.userGameData.o_Hp.ToString();
            f = (BaseManager.userGameData.AddStats[0] > 0) ? (BaseManager.userGameData.AddStats[0] - 1) : 0;
            str += $" (<color=green>+ {Convert.ToInt32(BaseManager.userGameData.o_Hp * f)}</color>)";
            _hpTxt.text = str;

            str = BaseManager.userGameData.o_Att.ToString();
            f = (BaseManager.userGameData.AddStats[1] > 0) ? (BaseManager.userGameData.AddStats[1] - 1) : 0;
            str += $" (<color=green>+ {Convert.ToInt32(BaseManager.userGameData.o_Att * f)}</color>)";
            _attTxt.text = str;

            str = BaseManager.userGameData.o_Def.ToString();
            f = (BaseManager.userGameData.AddStats[2] > 0) ? (BaseManager.userGameData.AddStats[2] - 1) : 0;
            str += $" (<color=green>+ {Convert.ToInt32(BaseManager.userGameData.o_Def * f)}</color>)";
            _defTxt.text = str;

            str = string.Format("{0:0.00}", BaseManager.userGameData.o_Hpgen);
            f = (BaseManager.userGameData.AddStats[3] > 0) ? (BaseManager.userGameData.AddStats[3] - 1) : 0;
            str += string.Format(" (<color=green>+ {0:0.00}</color>)", BaseManager.userGameData.o_Hpgen * f);
            _hpgenTxt.text = str;

            _effTxt.text = BaseManager.userGameData.getSkinExplain(_selectSkin);
        }

        #endregion

    }
}