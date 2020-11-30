using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class statusButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] TextMeshProUGUI _explain;
        [SerializeField] TextMeshProUGUI _price;

        public void setData(int level, string explain, int price)
        {
            _level.text = $"Lv.{level.ToString()}";
            _explain.text = explain;
            _price.text = price.ToString();
        }
    }
}