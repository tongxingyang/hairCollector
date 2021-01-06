using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class skillTreeComp : MonoBehaviour
    {
        public enum skillType { launch, range, rush, shield, environment, summon }
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] Image _panel;
        [SerializeField] TextMeshProUGUI _explainName;
        [SerializeField] TextMeshProUGUI _explain;

        [SerializeField] GameObject[] _taps;

        void Start()
        {
            for (int i = 0; i < _taps.Length; i++)
            {
                _taps[i].SetActive(false);
            }
        }

        public void openTap(skillType type)
        {
            _taps[(int)type].SetActive(true); ;
        }
    }
}