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

        public bool Fire { set => _fire.SetActive(value); }
        public bool Feather { set => _feather.SetActive(value); }
        public bool Firecrumb { set => _firecrumb.SetActive(value); }

        private void Awake()
        {
            allClose();
        }

        public void whenPurchase()
        {
            Fire = true;
            Feather = true;
            Firecrumb = false;
        }

        public void whenNewResult()
        {
            Fire = false;
            Feather = false;
            Firecrumb = true;
        }

        public void allClose()
        {
            Fire = false;
            Feather = false;
            Firecrumb = false;
        }
    }
}