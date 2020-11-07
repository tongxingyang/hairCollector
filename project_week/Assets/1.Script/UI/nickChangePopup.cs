using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class nickChangePopup : MonoBehaviour, UIInterface
    {
        [SerializeField] InputField _field;
        [SerializeField] TextMeshProUGUI _price;

        public Action completeChange { get; set; }

        bool _closable;

        public void changeNick()
        {
            // 글자수 체크
            if (_field.text.Length < 2)
            {
                WindowManager.instance.Win_message.showMessage("닉네임이 너무 짧다리~");
                return;
            }
            else if (_field.text.Length > 12)
            {
                WindowManager.instance.Win_message.showMessage("닉네임이 너무 길다눈~");
                return;
            }

            // 인터넷 체크
            if (AuthManager.instance.networkCheck())
            {
                // 닉변 비용체크
                if (BaseManager.userGameData.FreeNichkChange == false)
                {
                }
                else if (BaseManager.userGameData.Gem < gameValues._nickPrice)
                {
                    WindowManager.instance.Win_message.showMessage("사실 닉변하는데 보석 듬ㅋㅋ" + System.Environment.NewLine +
                        $"(딱- {gameValues._nickPrice - BaseManager.userGameData.Gem}개만 더모으면 가능!)");
                    return;
                }
            }
            else 
            { 
                // 인터넷 끊겼을땐 + 닉변 한번밖에 못함
                if (BaseManager.userGameData.FreeNichkChange)
                {
                    WindowManager.instance.Win_message.showMessage("닉네임 변경을 위해서는 인터넷을 연결해주세눈!");
                    return;
                }
            }

            _closable = false;
            StartCoroutine(changeNickName());
        }
        IEnumerator changeNickName()
        {
            // 인터넷 체크
            if (AuthManager.instance.networkCheck())
            {
                // 최신화 한지 5분 지났으면 한번 로드
                if ((DateTime.Now - BaseManager.userGameData._rankRefreshTime).Minutes > 5)
                {
                    yield return StartCoroutine(AuthManager.instance.loadRankDataFromFB());
                }

                for (int i = 0; i < AuthManager.instance.Leaders.Count; i++)
                {
                    if (_field.text.Equals(AuthManager.instance.Leaders[i]._nick))
                    {
                        WindowManager.instance.Win_message.showMessage("랭킹에 등록된 닉네임으로는 변경할 수 없눈");

                        _closable = true;
                        yield break;
                    }
                }
            }

            BaseManager.userGameData.NickName = _field.text;
            if (BaseManager.userGameData.FreeNichkChange)
            {
                BaseManager.userGameData.Gem -= gameValues._nickPrice;
            }
            else
            {
                BaseManager.userGameData.FreeNichkChange = true;
            }

            AuthManager.instance.AllSaveUserEntity();

            completeChange?.Invoke();
            _closable = true;
            close();
        }

        public void open()
        {
            _closable = true;
            _field.text = "";

            if (BaseManager.userGameData.FreeNichkChange == false)
            {
                _price.text = "무료";
            }
            else
            {
                _price.text = gameValues._nickPrice.ToString();
            }

            gameObject.SetActive(true);
        }

        public void close()
        {
            Debug.Log(_closable + "/" + BaseManager.userGameData.FreeNichkChange);
            if (_closable && BaseManager.userGameData.FreeNichkChange)
            {
                gameObject.SetActive(false);
            }
        }
    }
}