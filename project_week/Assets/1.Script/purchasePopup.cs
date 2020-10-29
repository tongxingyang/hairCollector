using BansheeGz.BGDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class purchasePopup : MonoBehaviour, UIInterface
    {
        [SerializeField] Image _present;

        Action _whenClose = null;
        bool _pushable;

        public Action WhenClose { get => _whenClose; set => _whenClose = value; }

        void Awake()
        {
            close();
        }

        public void setOpen(Sprite sp)
        {
            _present.sprite = sp;
            _present.SetNativeSize();
            _present.transform.localScale = Vector3.one * ((((RectTransform)_present.transform).GetWidth() > 300f) ? 2f : 3f);
            open();
        }

        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            _whenClose?.Invoke();
            gameObject.SetActive(false);
        }


        public void offPanel()
        {
            if (_pushable)
            { 
            
            }
        }
    }
}