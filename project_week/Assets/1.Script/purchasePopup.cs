using BansheeGz.BGDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class purchasePopup : MonoBehaviour, UIInterface
    {
        [SerializeField] RectTransform _panel;
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] Image _present;

        Action _whenClose = null;
        Action _after = null;
        bool _pushable;

        public Action WhenClose { get => _whenClose; set => _whenClose = value; }

        void Awake()
        {
            close();
        }

        public purchasePopup setOpen(Sprite sp, string str, Action after = null)
        {            
            _title.text = str + System.Environment.NewLine + "구매 완료!";
            _present.sprite = sp;
            _present.SetNativeSize();
            _present.transform.localScale = Vector3.one * ((((RectTransform)_present.transform).GetWidth() > 300f) ? 2f : 3f);

            _after = after;
            open();

            return this; ;
        }

        public void setImgSize(bool isBig=true)
        {
            if (isBig)
            { 
                _panel.sizeDelta = new Vector2(950f, 1350f);
                _present.transform.localPosition = Vector3.zero;
            }
            else
            {
                _panel.sizeDelta = new Vector2(850f, 1150f);
                _present.transform.localPosition = new Vector3(0, -75f);
            }
        }

        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            _whenClose?.Invoke();
            _after?.Invoke();
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