using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class celebrationEffect : MonoBehaviour
    {
        [SerializeField] GameObject _fire;
        [SerializeField] GameObject _feather;
        [SerializeField] GameObject _firecrumb;
        [SerializeField] GameObject _colorPaper;

        public bool Fire { set => _fire.SetActive(value); }
        public bool Feather { set => _feather.SetActive(value); }
        public bool Firecrumb { set => _firecrumb.SetActive(value); }
        public bool colorPaper { set => _colorPaper.SetActive(value); }

        private void Awake()
        {
            allClose();
        }

        /// <summary> 전체 닫기 </summary>
        public void allClose()
        {
            Fire = false;
            Feather = false;
            Firecrumb = false;
            colorPaper = false;
        }

        /// <summary> 상점 구매시 </summary>
        public void whenPurchase()
        {
            Fire = true;
            Feather = true;
            Firecrumb = false;
            colorPaper = false;
        }

        /// <summary> 신기록 </summary>
        public void whenNewResult()
        {
            Fire = false;
            Feather = false;
            Firecrumb = true;
            colorPaper = false;
        }

        /// <summary> 능력치강화 </summary>
        public void whenStatusUp()
        {
            Fire = false;
            Feather = false;
            Firecrumb = false;
            colorPaper = true;
        }
    }
}