using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using NaughtyAttributes;

namespace week
{
    public class accountListWin : MonoBehaviour
    {
        public enum btnType { select, change }
        
        [SerializeField] TextMeshProUGUI _title;

        [SerializeField] Image[] _btn;
        [SerializeField] Image[] _snowman;
        [SerializeField] TextMeshProUGUI[] _nick;
        [SerializeField] TextMeshProUGUI[] _record;
        [SerializeField] Image _backPanel;

        Action<string> _act;
        btnType _type;

        ObscuredString _selectData;

        public void Awake()
        {
            BtnInit();
            gameObject.SetActive(false);
        }

        void BtnInit()
        {
            _type = btnType.select;
            _selectData = null;
            for (int i = 0; i < 3; i++)
            {
                _btn[i].color = new Color(0.98f, 0.88f, 0.61f);
                _btn[i].transform.localScale = Vector3.one * 0.95f;
            }
        }

        //public void open(Action<string> act, bool select = true, bool cancelable = false)
        //{
        //    _act = act;
        //    _backPanel.raycastTarget = cancelable;

        //    BtnInit();

        //    if (select)
        //    {
        //        _type = btnType.select;
        //        _title.text = "기기에 저장된 데이터(로드)";
        //    }
        //    else
        //    {
        //        _type = btnType.change;
        //        _title.text = "기기에 저장된 데이터(교체)"
        //            + System.Environment.NewLine + "<size=35>데이터를 저장할 공간이 필요합니다.</size>"
        //            + System.Environment.NewLine + "<size=35>교체된 데이터는 삭제됩니다.</size>";

        //        //for (int i = 0; i < 3; i++)
        //        //{
        //        //    _btn[i].raycastTarget = true;
        //        //}
        //    }

        //    string load = (string)ES3.Load(_deviceList[i]);
        //    UserEntity _entity = JsonConvert.DeserializeObject<UserEntity>(load, new ObscuredValueConverter());

        //    _snowman[i].sprite = DataManager.SkinSprite[(SkinKeyList)(int)_entity._property._skin];
        //    _snowman[i].color = Color.white;
        //    _nick[i].text = _entity._property._nickName;
        //    _record[i].text = BaseManager.userGameData.getLifeTime(_entity._record._timeRecord, false);

        //    gameObject.SetActive(true);
        //}

        //public void DataList(int n)
        //{
        //    if (_type == btnType.select && string.IsNullOrEmpty(_deviceList[n]))
        //        return;

        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (i == n)
        //        {
        //            _selectData = _deviceList[n];
        //            _btn[i].color = new Color(1f, 0.73f, 1f);
        //            _btn[i].transform.localScale = Vector3.one;
        //        }
        //        else
        //        {
        //            _btn[i].color = new Color(0.98f, 0.88f, 0.61f);
        //            _btn[i].transform.localScale = Vector3.one * 0.95f;
        //        }
        //    }
        //}

        public void select()
        {
            _act?.Invoke(_selectData);
            gameObject.SetActive(false);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

        //[Button]
        //public void Onclick()
        //{
        //    open((string str) => { }, false);
        //}
    }
}