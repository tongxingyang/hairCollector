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

        Dictionary<SkinKeyList, skinBox> _skinBoxies;
        SkinKeyList _selectSkin;

        #region [초기화]

        public void Init()
        {
            _selectSkin = (SkinKeyList)BaseManager.userGameData.Skin;
            showSkinInfo();

            _skinBoxies = new Dictionary<SkinKeyList, skinBox>();
            for (SkinKeyList i = SkinKeyList.snowman; i < SkinKeyList.max; i++)
            {
                if (DataManager.GetTable<bool>(DataTable.skin, i.ToString(), SkinValData.enable.ToString()))
                {
                    skinBox sb = Instantiate(_skinBox).GetComponent<skinBox>();

                    sb.transform.SetParent(_skinBoxParent);
                    sb.transform.localScale = Vector3.one;
                    sb.setSkinBox((SkinKeyList)i);

                    _skinBoxies.Add(i, sb);
                }
            }

            gameObject.SetActive(false);
        }

        /// <summary> 창 오픈시 </summary>
        public void open()
        {
            showSkinInfo();
            foreach (skinBox sb in _skinBoxies.Values)
            {
                sb.chkState();
            }
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

            foreach (skinBox sb in _skinBoxies.Values)
            {
                sb.chkState();
            }
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