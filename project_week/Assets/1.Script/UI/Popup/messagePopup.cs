using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace week
{
    public class messagePopup : MonoBehaviour, UIInterface
    {
        [SerializeField] CanvasGroup _MsgGroup;
        [SerializeField] GameObject _Panel;
        [SerializeField] RectTransform _MsgPos;
        [Header("info")]
        [SerializeField] TextMeshProUGUI _MsgTxt;
        [SerializeField] GameObject _present;
        [SerializeField] Image _presentImg;
        [SerializeField] GameObject _actBtn;
        bool _isPlay;
        float time = 0;

        Coroutine _onlyMsg;
        Action _act;

        void Awake()
        {
            _isPlay = false;
            gameObject.SetActive(false);
        }

        #region [only Message]
        public void showMessage(string msg)
        {
            open();

            _MsgTxt.text = msg;
            _MsgPos.anchoredPosition = Vector3.zero;

            _Panel.SetActive(false);
            _present.SetActive(false);
            _actBtn.SetActive(false);

            if (_isPlay == false)
            {
                _onlyMsg = StartCoroutine(winMove());
            }
            else
            {
                time = 0;
            }
        }

        IEnumerator winMove()
        {
            _isPlay = true;
            _MsgGroup.alpha = 0;
            time = 0;

            while (time < 2f)
            {
                time += Time.deltaTime;

                _MsgPos.anchoredPosition += Vector2.up * 20f * Time.deltaTime;
                _MsgGroup.alpha = (1f - (time * 0.5f));

                yield return new WaitForEndOfFrame();
            }

            time = 0;
            _isPlay = false;

            close();
            _onlyMsg = null;
        }

        #endregion

        #region [act Message]

        public void showActMessage(string msg, Action act)
        {
            if (_onlyMsg != null)
            {
                StopCoroutine(_onlyMsg);
                _isPlay = false;
            }

            _act = act;
            _MsgPos.anchoredPosition = Vector3.zero;

            _MsgGroup.alpha = 1f;

            _Panel.SetActive(true);
            _present.SetActive(false);
            _actBtn.SetActive(true);

            _MsgTxt.text = msg;

            open();
        }

        public void okButton()
        {
            _act();
            close();
        }

        #endregion

        #region [present act Message]

        public void showPresentAct(string msg, Sprite sp, Action act)
        {
            if (_onlyMsg != null)
            {
                StopCoroutine(_onlyMsg);
                _isPlay = false;
            }

            _act = act;
            _MsgPos.anchoredPosition = Vector3.zero;
            
            _MsgGroup.alpha = 1f;

            _Panel.SetActive(true);
            _present.SetActive(true);
            _actBtn.SetActive(true);

            _presentImg.sprite = sp;
            _MsgTxt.text = msg;

            open();
        }

        #endregion

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