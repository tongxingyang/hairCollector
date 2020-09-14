using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    public class messagePopup : MonoBehaviour, UIInterface
    {
        [SerializeField] TextMeshProUGUI _msg;

        void Awake()
        {
            close();
        }

        public void showMessage(string msg)
        {
            open();
            _msg.text = msg;
        }

        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }
    }
}