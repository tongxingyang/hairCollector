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
            int leng = _field.text.Length;
            // 글자수 체크
            if (leng < 2)
            {
                WindowManager.instance.Win_message.showMessage("닉네임이 너무 짧다리~");
                return;
            }
            else if (leng > 12)
            {
                WindowManager.instance.Win_message.showMessage("닉네임이 너무 길다눈~");
                return;
            }
            else if (string.IsNullOrWhiteSpace(_field.text))
            {
                WindowManager.instance.Win_message.showMessage("공백만으로는 닉네임을 만들수 없지");
                return;
            }
            else if (_field.text[0].Equals(' ') || _field.text[leng - 1].Equals(' '))
            {
                WindowManager.instance.Win_message.showMessage("양끝단은 공백으로 두지 말아줭");
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
            //bool result = false;
            //yield return StartCoroutine(AuthManager.instance.searchNickName(_field.text, (chk) =>
            //{
            //    result = chk;

            //    _closable = true;
            //}));

            // yield return new WaitUntil(() => _closable == true);

            //if (result) // 중복 있음
            //{
            //    WindowManager.instance.Win_message.showMessage("중복된 닉네임이눈!");
            //}
            //else // 중복 없음 생성 가능
            //{
            
            yield return null;

            BaseManager.userGameData.NickName = _field.text;
            if (BaseManager.userGameData.FreeNichkChange)
            {
                BaseManager.userGameData.Gem -= gameValues._nickPrice;
            }
            else
            {
                BaseManager.userGameData.FreeNichkChange = true;
            }

            completeChange?.Invoke();
            AuthManager.instance.SaveDataServer();

            NanooManager.instance.setUid(AuthManager.instance.Uid);
            NanooManager.instance.setRankingRecord();

            WindowManager.instance.Win_message.showActMessage("새로운 닉네임" + System.Environment.NewLine +
                $"[{BaseManager.userGameData.NickName}]" + System.Environment.NewLine + "변경 완료!", null);

            _closable = true;
            close();
            //}
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