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

            // 닉변 비용체크
            if (BaseManager.userGameData.FreeNichkChange == false)
            {
            }
            else if (BaseManager.userGameData.Gem < gameValues._nickPrice)
            {
                WindowManager.instance.Win_message.showMessage("사실 닉변하는데 보석 듬ㅋㅋ" + System.Environment.NewLine +
                    $"(딱- {gameValues._nickPrice - BaseManager.userGameData.Gem}개만 더모으면 바꿀 수 있어요!)");
                return;
            }

            _closable = false;
            Debug.Log("-----------");

            StartCoroutine(chkNickName());
        }

        IEnumerator chkNickName()
        {
            yield return StartCoroutine(AuthManager.instance.searchNickName(_field.text, (chk) =>
            {
                if (chk) // 중복 없음 생성 가능
                {
                    BaseManager.userGameData.NickName = _field.text;
                    if (BaseManager.userGameData.FreeNichkChange)
                    {
                        BaseManager.userGameData.Gem -= gameValues._nickPrice;
                    }
                    else
                    {
                        BaseManager.userGameData.FreeNichkChange = true;
                    }
                }
                else // 중복 있음
                {
                    WindowManager.instance.Win_message.showMessage("중복된 닉네임이눈!");
                }

                _closable = true;
            }));

            yield return new WaitUntil(() => _closable == true);

            completeChange?.Invoke();
            AuthManager.instance.SaveUserEntity();
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