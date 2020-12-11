using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class statusButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] TextMeshProUGUI _value;
        [SerializeField] TextMeshProUGUI _price;
        [SerializeField] Image _img;

        public statusButton setInit(string name, Sprite img)
        {
            _name.text = name;
            _img.sprite = img;

            return this;
        }


        public void setData(int level, string explain, int price)
        {
            _level.text = $"Lv.{level.ToString()}";
            _value.text = explain;
            _price.text = price.ToString();
        }
    }
}