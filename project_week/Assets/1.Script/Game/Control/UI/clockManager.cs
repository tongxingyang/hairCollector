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
        [SerializeField] nightDarkCtrl _night;

        GameScene _gs;

        readonly int _dateSecond = 120; // 하루 -> 초
        float _nightSecond = 30f;       // 밤 -> 초
        float _degree;
        bool _isNight;
        float _startDark = 0.6f;

        List<KeyValuePair<season, int>> _seasonData;

        /// <summary> 진정한 기록 </summary>
        public float RecordSecond { get; private set; }   // 전체 시간
        /// <summary> 전체 일수 </summary>
        public int RecordDay { get; private set; }
        /// <summary> 전체 달수 </summary>
        public int RecordMonth { get; private set; }
        /// <summary> 전체 년수 </summary>
        public int RecordYear { get; private set; }

        public int _dayInSeason { get; private set; }
        /// <summary> 이번달이 몇일인지 </summary>
        public int NowDayinSeason { get => _seasonData[RecordDay % 4].Value; }

        float _dateTime;
        float _monthTime;

        public season NowSeason { get; private set; }

        bool _firstDay = true;
        public bool chk1Wave { get => _monthTime > ((_firstDay) ? 60f : 0f); }
        public bool chk2Wave { get => _monthTime > ((_seasonData[RecordMonth % 4].Value == 2) ? 60f : 120f); }
        public bool chk3Wave { get => _monthTime > ((_seasonData[RecordMonth % 4].Value == 2) ? 120f : 240f); }

        public Action changeDay { get; set; }
        public Action<season> changeSS { get; set; }
        
        public void Init(GameScene gs)
        {            
            _gs = gs;
            _isNight = false;
            _degree = -360f / _dateSecond;

            // 계절 설정
            _seasonData = new List<KeyValuePair<season, int>>();

            for (UserGameData.sson ss = 0; ss < UserGameData.sson.max; ss++)
            {
                string[] str = D_level.GetEntity(_gs.StageLevel.ToString()).Get<string>(ss.ToString()).Split(',');
                _seasonData.Add(new KeyValuePair<season, int>(EnumHelper.StringToEnum<season>(str[0]), int.Parse(str[1])));
                //_seasonData.Add(new KeyValuePair<season, int>(season.dark, int.Parse(str[1])));
            }

            NowSeason = _seasonData[0].Key;
            _seasonImg.sprite = seasons[(int)NowSeason];

            _night.Init(gs);
        }

        /// <summary> gameScene 코루틴에서 시간계산 </summary>
        public void accTime(float deltime)
        {
            RecordSecond += deltime;
            _dateTime += deltime;
            _monthTime += deltime;

            _time.text = BaseManager.userGameData.getLifeTime(_gs.StageLevel, RecordSecond);
                        
            _chim.rotation = Quaternion.Euler(0, 0, _dateTime * _degree);

            if (_dateTime > _dateSecond - _nightSecond && _isNight == false) // 전구사람체크
            {
                float calDark = (BaseManager.userGameData.Skin == SkinKeyList.bulbman) ? 0.2f : _startDark;

                _night.startNight(calDark);
                _isNight = true;
            }

            
            if (_dateTime > _dateSecond) // 하루 지남
            {
                _dateTime = 0;  // 초기화
                
                // 아침
                {
                    _isNight = false;
                    _night.endNight();
                }

                _dayInSeason++;
                RecordDay++;

                changeDay?.Invoke();

                if (_seasonData[RecordMonth % 4].Value <= _dayInSeason) // 계절 변동
                {
                    RecordMonth++;
                    NowSeason = _seasonData[RecordMonth % 4].Key;
                    _monthTime = 0;

                    _dayInSeason = 0;
                    RecordYear = RecordDay / 12;
                    _startDark += 0.05f;

                    _nightSecond = D_season.GetEntity(NowSeason.ToString()).f_night;

                    changeSS?.Invoke(NowSeason); // += 추가로 day 기반 몹 강화율 계산
                    _seasonImg.sprite = seasons[(int)NowSeason];

                }
            }

            if (chk1Wave)
                _firstDay = false;
        }
    }
}