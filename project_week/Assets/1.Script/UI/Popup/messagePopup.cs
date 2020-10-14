using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace week
{
    public class messagePopup : MonoBehaviour
    {
        [Header("only_Msg")]
        [SerializeField] GameObject _msgBox;
        [SerializeField] CanvasGroup _msgGroup;
        [SerializeField] Transform _pos;
        [SerializeField] TextMeshProUGUI _onlyMsgTxt;
        bool _isPlay;
        float time = 0;

        [Header("act_Msg")]
        [SerializeField] GameObject _actBox;
        [SerializeField] TextMeshProUGUI _actMsgTxt;
        Action _act;

        void Awake()
        {
            _isPlay = false;
            gameObject.SetActive(false);
        }

        #region [only Message]
        public void showMessage(string msg)
        {
            gameObject.SetActive(true);
            _msgBox.SetActive(true);
            _actBox.SetActive(false);

            _pos.localPosition = Vector3.zero;

            _onlyMsgTxt.text = msg;

            if (_isPlay == false)
            {
                StartCoroutine(winMove());
            }
            else
            {
                time = 0;
            }
        }
        IEnumerator winMove()
        {
            _isPlay = true;
            _msgGroup.alpha = 0;

            while (time < 2f)
            {
                time += Time.deltaTime;

                _pos.localPosition += Vector3.up * 20f * Time.deltaTime;
                _msgGroup.alpha = (1f - (time * 0.5f));

                yield return new WaitForEndOfFrame();
            }

            time = 0;
            _isPlay = false;
            gameObject.SetActive(false);
        }

        #endregion

        #region [act Message]

        public void showActMessage(string msg, Action act)
        {
            _act = act;

            gameObject.SetActive(true);
            _msgBox.SetActive(false);
            _actBox.SetActive(true);

            _actMsgTxt.text = msg;
        }

        public void okButton()
        {
            _act(); 
            gameObject.SetActive(false);
        }

        public void cancel()
        {
            gameObject.SetActive(false);
        }

        #endregion





        
    }
}