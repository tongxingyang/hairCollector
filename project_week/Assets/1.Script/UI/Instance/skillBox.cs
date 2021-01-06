using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class skillBox : MonoBehaviour
    {
        [SerializeField] Image _skillImg;
        [SerializeField] Image _caseColor;
        [SerializeField] TextMeshProUGUI _num;

        public void Init()
        {
            

        }

        void colorSetting(int point, int end)
        {
            if (point < end)
                _caseColor.color = Color.grey;
            else if (point == end)
                _caseColor.color = Color.yellow;
            else
                _caseColor.color = Color.red;
        }
    }
}