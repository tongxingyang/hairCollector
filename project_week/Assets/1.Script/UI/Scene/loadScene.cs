using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace week
{
    public class loadScene : MonoBehaviour, UIInterface
    {
        Image _panel;

        void Start()
        {
            _panel = GetComponentInChildren<Image>(); 
            gameObject.SetActive(false);
        }

        public void open()
        {
            gameObject.SetActive(true);
            //_panel.DOFade(1, 0.2f);
        }

        public void close()
        {
            //_panel.DOFade(0, 0.2f).OnComplete(() => gameObject.SetActive(false));
            gameObject.SetActive(false);
        }
    }
}