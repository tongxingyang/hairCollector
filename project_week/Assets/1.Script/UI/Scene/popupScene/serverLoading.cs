using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class serverLoading : MonoBehaviour, UIInterface
    {        
        Image _panel;

        Action _whenClose; 

        public bool IsOpen { get; set; }
        bool _isInteract;

        void Awake()
        {
            _panel = GetComponent<Image>();
            _panel.GetComponent<Button>().onClick.AddListener(onClick);
            close();
        }

        public void open()
        {
            IsOpen = true;

            _isInteract = false;
            _panel.color = new Color(0, 0, 0, 0.5f);

            gameObject.SetActive(true);
        }

        public void close()
        {
            IsOpen = false;
            gameObject.SetActive(false);
        }

        public void setButtonClose(Action whenClose)
        {
            _whenClose = whenClose;

            _isInteract = true;
            _panel.color = new Color(0, 0, 0, 0);
        }

        void onClick()
        {
            if (_isInteract)
            {
                _whenClose?.Invoke();
                _whenClose = null;

                close();
            }
        }
    }
}