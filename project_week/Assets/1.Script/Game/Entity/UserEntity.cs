using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace week
{
    public class UserEntity
    {
        #region private

        /// <summary> 닉넴 </summary>
        [SerializeField] string _nickName;
        /// <summary> 코인 </summary>
        [SerializeField] int _coin;
        /// <summary> 보석 </summary>
        [SerializeField] int _gem;
        /// <summary> 강화레벨 </summary>
        [SerializeField] int[] _statusLevel;
        /// <summary> 어빌리티 포인트 </summary>
        [SerializeField] int _ap;

        [SerializeField] int _hp;
        [SerializeField] float _hpgen;
        [SerializeField] int _def;
        [SerializeField] float _attFactor;
        [SerializeField] float _cool;
        [SerializeField] float _expFactor;
        [SerializeField] float _coinFactor;
        [SerializeField] float _skinEnhance;

        [SerializeField] float _bgmVol;
        [SerializeField] float _sfxVol;

        [SerializeField] int _timeRecord;

        [SerializeField] int _doubleCoin;
        [SerializeField] bool _removeAD;

        #endregion

        #region [properties]
        public string NickName { get => _nickName; set => _nickName = value; }
        public int Coin { get => _coin; set => _coin = value; }
        public int Gem { get => _gem; set => _gem = value; }
        public int[] StatusLevel { get => _statusLevel; set => _statusLevel = value; }
        public int Ap { get => _ap; set => _ap = value; }

        public int Hp { get => _hp; set => _hp = value; }
        public float Hpgen { get => _hpgen; set => _hpgen = value; }
        public int Def { get => _def; set => _def = value; }
        public float AttFactor { get => _attFactor; set => _attFactor = value; }
        public float Cool { get => _cool; set => _cool = value; }
        public float ExpFactor { get => _expFactor; set => _expFactor = value; }
        public float CoinFactor { get => _coinFactor; set => _coinFactor = value; }
        public float SkinEnhance { get => _skinEnhance; set => _skinEnhance = value; }

        public float BgmVol { get => _bgmVol; set => _bgmVol = value; }
        public float SfxVol { get => _sfxVol; set => _sfxVol = value; }
        public int TimeRecord { get => _timeRecord; set => _timeRecord = value; }

        public int DoubleCoin { get => _doubleCoin; set => _doubleCoin = value; }
        public bool RemoveAD { get => _removeAD; set => _removeAD = value; }

        #endregion

        /// <summary> 초기화 </summary>
        public UserEntity()
        {
            _nickName = "ready_Player_1";

            _coin = 99999999;

            StatusLevel = new int[(int)StatusData.max];

            _hp             = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString());
            _hpgen          = DataManager.GetTable<float>(DataTable.status, "default", StatusData.hpgen.ToString());
            _def            = DataManager.GetTable<int>(DataTable.status, "default", StatusData.def.ToString());
            _attFactor      = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString());
            _cool           = DataManager.GetTable<float>(DataTable.status, "default", StatusData.cool.ToString());
            _expFactor      = DataManager.GetTable<float>(DataTable.status, "default", StatusData.exp.ToString());
            _coinFactor     = DataManager.GetTable<float>(DataTable.status, "default", StatusData.coin.ToString());
            _skinEnhance    = DataManager.GetTable<float>(DataTable.status, "default", StatusData.skin.ToString());

            _ap = 0;

            _bgmVol = 1f;
            _sfxVol = 1f;

            _timeRecord = 0;

            _doubleCoin = 1;
            _removeAD = false;
        }

        /// <summary> 레벨 -> 스탯에 적용 </summary>
        public void applyLevel()
        {
            _hp             = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                             + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * StatusLevel[(int)StatusData.hp];
            _hpgen          = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * StatusLevel[(int)StatusData.hpgen];
            _def            = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * StatusLevel[(int)StatusData.def];
            _attFactor      = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                             + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * StatusLevel[(int)StatusData.att];
            _cool = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), StatusLevel[(int)StatusData.cool]);
            _expFactor      = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), StatusLevel[(int)StatusData.exp]);
            _coinFactor     = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), StatusLevel[(int)StatusData.coin]);
            _skinEnhance    = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * StatusLevel[(int)StatusData.skin];
        }

        /// <summary> 스탯 레벨 업 </summary>
        public void statusLevelUp(StatusData stat)
        {
            StatusLevel[(int)stat]++;

            switch (stat)
            {
                case StatusData.hp:
                    _hp = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                        + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * StatusLevel[(int)StatusData.hp];
                    break;
                case StatusData.att:
                    _attFactor = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                        + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * StatusLevel[(int)StatusData.att];
                    break;
                case StatusData.def:
                    _def = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * StatusLevel[(int)StatusData.def];
                    break;
                case StatusData.hpgen:
                    _hpgen = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * StatusLevel[(int)StatusData.hpgen];
                    break;
                case StatusData.cool:
                    _cool = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), StatusLevel[(int)StatusData.cool]);
                    break;                
                case StatusData.exp:
                    _expFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), StatusLevel[(int)StatusData.exp]);
                    break;
                case StatusData.coin:
                    _coinFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), StatusLevel[(int)StatusData.coin]);
                    break;
                case StatusData.skin:
                    _skinEnhance = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * StatusLevel[(int)StatusData.skin];
                    break;
            }
        }

        public string saveData()
        {
            return JsonUtility.ToJson(this);
        }

        public string getLifeTime(float time, bool isTwoLine)
        {
            int year;
            int season;
            int day;
            int m;
            int s;

            year = (int)(time / (24 * 60));
            time -= year * 24 * 60;
            season = (int)(time / (6 * 60));
            time -= season * 6 * 60;
            day = (int)(time / (2 * 60));
            time -= day * 2 * 60;
            m = (time > 60) ? 1 : 0;
            time -= m * 60;
            s = (int)time;

            string str = "";
            if (year > 0)
            {
                str += $"{year}년 ";
            }
            if (season > 0)
            {
                str += $"{season}계절 ";
            }
            if (day > 0)
            {
                switch (day)
                {
                    case 0:
                        str += "첫째날 ";
                        break;
                    case 1:
                        str += "둘째날 ";
                        break;
                    case 2:
                        str += "셋째날 ";
                        break;
                }
            }

            if (isTwoLine)
            {
                str += System.Environment.NewLine;
            }

            if (m > 0)
            {
                str += $"{m}분 ";
            }
            str += $"{s}초";

            return str;
        }
    }
}