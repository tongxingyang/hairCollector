using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class statusButton : MonoBehaviour
    {
        Color[] _colorList = new Color[4] {
            new Color(0.67f, 0.23f, 0f),
            new Color(0.84f, 0.84f, 0.84f),
            new Color(1f, 0.66f, 0f),
            new Color(0f, 1f, 0.87f) };

        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _level;
        
        [SerializeField] TextMeshProUGUI _price;
        [SerializeField] Image[] _cases;
        [SerializeField] Image _img;

        statusKeyList _stat;
        int cost;

        /// <summary> 초기화 (변경 없는 값) </summary>
        public statusButton setInit(statusKeyList stat, Sprite img)
        {
            _stat = stat;
            _name.text = DataManager.GetTable<string>(DataTable.status, _stat.ToString(), StatusData.sName.ToString());
            _img.sprite = img;

            cost = DataManager.GetTable<int>(DataTable.status, _stat.ToString(), StatusData.cost.ToString());

            return this;
        }

        /// <summary> 데이터 설정 (변경 있는 값) </summary>
        public int setData(int level, ref int addlvl)
        {
            Color _color;
            int sum = level + addlvl;
            int rate = 1;
            int rankLvl = 0;

            if (level > 85-1) 
            {
                _color = _colorList[3];
                rate = 4;
                rankLvl = level - 85;
            }
            else if (level > 55-1)
            {
                _color = _colorList[2];
                rate = 3;
                rankLvl = level - 55;
                if (sum > 85)
                    addlvl = 85 - level;
            }
            else if (level > 20-1)
            {
                _color = _colorList[1];
                rate = 2;
                rankLvl = level - 20;
                if (sum > 55)
                    addlvl = 55 - level;
            }
            else
            {
                _color = _colorList[0];

                if (sum > 20)                
                    addlvl = 20 - level;                
            }

            _level.text = $"Lv.{level + addlvl} <size=35>/ 100</size>";
            _level.color = (addlvl > 0) ? Color.green : Color.white;

            _cases[0].color = _color;
            _cases[1].color = _color * 0.75f;

            int cost = DataManager.GetTable<int>(DataTable.status, _stat.ToString(), StatusData.cost.ToString());
            int price = ((rankLvl + addlvl) / 5 + 1) * cost * rate;

            _price.text = price.ToString();
            _price.color = (addlvl > 0) ? Color.green : Color.white;

            return cal_usedAp(level, addlvl) * rate;
        }

        int cal_usedAp(int lvl, int add)
        {
            if (add == 0)
                return 0;

            int v, m, cal;

            lvl = (lvl > 85) ? lvl - 85 : (lvl > 55) ? lvl - 55 : (lvl > 20) ? lvl - 20 : 0;

            v = (lvl + add) / 5;
            m = (lvl + add) % 5;
            cal = (int)((1 + v) * (m + 5 * v * 0.5f)) * cost;

            v = lvl / 5;
            m = lvl % 5;

            cal -= (int)((1 + v) * (m + 5 * v * 0.5f)) * cost;

            return cal;
        }

        ///// <summary> 데이터 설정 (변경 없는 값) </summary>
        //public void reset(int level)
        //{
        //    _level.text = $"Lv.{level} <size=35>/ 100</size>";
        //    _level.color = Color.white;

        //    if (level < 20)
        //        _color = _colorList[0];
        //    else if (level < 55)
        //        _color = _colorList[1];
        //    else if (level < 85)
        //        _color = _colorList[2];
        //    else
        //        _color = _colorList[3];

        //    _cases[0].color = _color;
        //    _cases[1].color = _color * 0.75f;

        //    int cost = DataManager.GetTable<int>(DataTable.status, _stat.ToString(), StatusData.cost.ToString());
        //    int price = (level / 5 + 1) * cost;

        //    _price.text = price.ToString();
        //    _price.color = Color.white;
        //}
    }
}