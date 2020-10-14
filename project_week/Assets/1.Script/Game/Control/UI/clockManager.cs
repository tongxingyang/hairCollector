using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class clockManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _time;
        [SerializeField] Transform _chim;
        [SerializeField] Image _seasonImg;
        [SerializeField] Sprite[] seasons;
        [SerializeField] nightDarkCtrl _dark;

        GameScene _gs;
        int _day;
        readonly int _date = 120;
        float _night = 30f;
        float _degree;
        bool _isNight;
        float _startDark = 0.6f;

        season _season = season.winter;
        float _recordTime;
        float _dateTime;
        float _monthTime;
        int _seasonNum;
        public float RecordTime { get => _recordTime; set => _recordTime = value; }
        public season Season { get => _season; set => _season = value; }

        float _chk1w = 30f;
        public bool chk1Wave { get => _monthTime > _chk1w; }
        public bool chk2Wave { get => _monthTime > 60f; }
        public bool chk3Wave { get => _monthTime > 120f; }
        public int Day { get => _day; set => _day = value; }

        public Action<season> changeSS;
        public void Init(GameScene gs)
        {
            _gs = gs;
            _isNight = false;
            _degree = -360f / _date;
            _night = _date / 4;
            _dark.Init(gs);
        }

        public void accTime(float deltime)
        {
            _recordTime += deltime;
            _time.text = BaseManager.userGameData.getLifeTime(_recordTime,false);//.convertToTime((int)_recordTime)}({})";

            chkDate(deltime);

            //int ss = (int)RecordTime / 360;
            //_season = (season)(ss % 4);
        }

        void chkDate(float deltime)
        {
            _dateTime += deltime;
            _monthTime += deltime;
            _chim.rotation = Quaternion.Euler(0, 0, _dateTime * _degree);

            if (_dateTime > _date - _night && _isNight == false)
            {
                float calDark = (BaseManager.userGameData.SkinBval[(int)skinBvalue.light]) ? 0.2f : _startDark;

                _dark.startNight(calDark);
                _isNight = true;
            }
            else if (_dateTime > _date)
            {
                _dateTime = 0;
                _day++;
                _isNight = false;

                _dark.endNight();
                chkSeason();
            }

            if (chk1Wave)
                _chk1w = 0f;
        }

        void chkSeason()
        {
            _seasonNum++;
            _startDark += 0.05f;

            if (_seasonNum > 2)
            {
                _seasonNum = 0;
                _monthTime = 0;
                if (_season == season.winter)
                {
                    _season = season.spring;
                }
                else
                {
                    _season++;
                }

                _night = (_season == season.winter) ? (_date/4) : (_season == season.summer) ? (_date / 12) : (_date / 6);

                changeSS(_season);
                _seasonImg.sprite = seasons[(int)_season];
            }
        }
    }
}