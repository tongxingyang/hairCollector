using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    [Serializable]
    public class Option
    {
        [SerializeField] public float BgmVol;
        [SerializeField] public float SfxVol;
        public Option()
        {
            BgmVol = SfxVol = 1f;
        }
    }

    public class UserGameData
    {
        public enum defaultStat { hp, att, def, hpgen, cool, exp, coin, speed, max }

        /// <summary> 저장되는 정보 </summary>
        UserEntity _userEntity;

        /// <summary> 랭킹최저점 </summary>
        public ObscuredFloat _minRank { get; set; }
        /// <summary> 가장 최근 랭킹 최신화 시간 </summary>
        public DateTime _rankRefreshTime { get; set; }

        /// <summary> 전투결과 </summary>
        public ObscuredInt[] GameReward { get; set; }

        #region -------------------------[skin value]-------------------------

        /// <summary> 상시적용 스킨 능력치 </summary>
        ObscuredFloat[] _addStats;

        /// <summary> 적용할 계절 </summary>
        season? _applySeason = null;

        bool[] _skinBval;
        ObscuredFloat[] _skinFval;
        ObscuredInt[] _skinIval;
        snowballType _ballType;

        #region [skin properties]

        public ObscuredFloat[] AddStats { get => _addStats; set => _addStats = value; }
        public season? ApplySeason { get => _applySeason; set => _applySeason = value; }
        public bool[] SkinBval { get => _skinBval; }
        public ObscuredFloat[] SkinFval { get => _skinFval; }
        public ObscuredInt[] SkinIval { get => _skinIval; }
        public snowballType BallType { get => _ballType; }

        #endregion

        #endregion

        public UserEntity.property Property { get => _userEntity._property; set => _userEntity._property = value; }
        public UserEntity.status Status { get => _userEntity._status; set => _userEntity._status = value; }
        public UserEntity.quest Quest { get => _userEntity._quest; set => _userEntity._quest = value; }
        public UserEntity.payment Payment { get => _userEntity._payment; set => _userEntity._payment = value; }
        public UserEntity.gameUtility Util { get => _userEntity._util; set => _userEntity._util = value; }

        #region [properties]

        // 기본 정보 ==============================================================
        // - 닉 / 재화 
        public string NickName { get => _userEntity._property._nickName; set => _userEntity._property._nickName = value; }
        public ObscuredInt Coin { get => _userEntity._property._currency[(int)currency.coin]; set => _userEntity._property._currency[(int)currency.coin] = value; }
        public ObscuredInt Gem { get => _userEntity._property._currency[(int)currency.gem]; set => _userEntity._property._currency[(int)currency.gem] = value; }
        public ObscuredInt Ap { get => _userEntity._property._currency[(int)currency.ap]; set => _userEntity._property._currency[(int)currency.ap] = value; }

        public ObscuredInt followCoin { get; set; }
        public ObscuredInt followGem { get; set; }

        // - 스킨
        public ObscuredInt HasSkin { get => _userEntity._property._hasSkin; set => _userEntity._property._hasSkin = value; }
        public SkinKeyList Skin { get => (SkinKeyList)((int)_userEntity._property._skin); set => _userEntity._property._skin = (int)value; }

        // 기록 ==============================================================
        public ObscuredInt TimeRecord { get => _userEntity._record._timeRecord; }
        public ObscuredInt RecordSkin { get => _userEntity._record._recordSkin; }
        public ObscuredInt BossRecord { get => _userEntity._record._bossRecord; set => _userEntity._record._bossRecord = value; }
        public ObscuredInt ArtifactRecord { get => _userEntity._record._artifactRecord; set => _userEntity._record._artifactRecord = value; }
        public ObscuredInt AdRecord { get => _userEntity._record._adRecord; set => _userEntity._record._adRecord = value; }
        public ObscuredInt ReinRecord { get => _userEntity._record._reinRecord; set => _userEntity._record._reinRecord = value; }

        // 퀘스트 ==============================================================
        // - 일일
        public ObscuredInt[] DayQuest { get => _userEntity._quest._dayQuest; set => _userEntity._quest._dayQuest = value; }
        public ObscuredInt QuestSkin { get => _userEntity._quest._questSkin; set => _userEntity._quest._questSkin = value; }
        // - 전체
        public ObscuredInt GetTimeReward { get => _userEntity._quest._getTimeReward; set => _userEntity._quest._getTimeReward = value; }
        public ObscuredInt GetBossReward { get => _userEntity._quest._getBossReward; set => _userEntity._quest._getBossReward = value; }
        public ObscuredInt GetArtifactReward { get => _userEntity._quest._getArtifactReward; set => _userEntity._quest._getArtifactReward = value; }

        // 스탯 ==============================================================
        public ObscuredInt[] StatusLevel { get => _userEntity._status._statusLevel; set => _userEntity._status._statusLevel = value; }

        public ObscuredInt o_Hp { get => _userEntity._status._hp; }
        public ObscuredFloat o_Hpgen { get => _userEntity._status._hpgen; }
        public ObscuredInt o_Def { get => _userEntity._status._def; }
        public ObscuredFloat o_Att { get => _userEntity._status._att; }
        public ObscuredFloat o_Cool { get => _userEntity._status._cool; }
        public ObscuredFloat o_ExpFactor { get => _userEntity._status._expFactor; }
        public ObscuredFloat o_CoinFactor { get => _userEntity._status._coinFactor; }
        public ObscuredFloat SkinEnhance { get => _userEntity._status._skinEnhance; }

        // 인앱결제 ==============================================================
        public mulCoinChkList AddMulCoinList { set => _userEntity._payment._mulCoinList |= (1 << (int)value); }
        public bool chkMulCoinList(mulCoinChkList index)
        {
            return (_userEntity._payment._mulCoinList & (1 << (int)index)) > 0;
        }
        public ObscuredInt PaymentChkList { get => _userEntity._payment._chkList; set => _userEntity._payment._chkList = value; }
        public bool RemoveAd { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.removeAD)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.removeAD) : 0; }
        public bool MulCoin { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.mulCoins)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.mulCoins) : 0; }
        public bool StartPack { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.startPack)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.startPack) : 0; }
        public bool SkinPack { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.skinPack)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.skinPack) : 0; }
                

        // 유틸 ==============================================================
        public string UniqueNumber { get => _userEntity._util._uniqueNumber; }
        public long LastSave { get => _userEntity._util._lastSave;
            set
            {
                if (AuthManager.instance.networkCheck()) 
                { _userEntity._util._lastSave = value; };
            }
        }
        public ObscuredInt UtilChkList { get => _userEntity._util._chkList; set => _userEntity._util._chkList = value; }
        public bool FreeNichkChange { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.freeNickChange)) > 0;
                                        set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.freeNickChange) : 0; }
        public bool IsSavedServer   { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.isSavedServer)) > 0;
                                        set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.isSavedServer) : 0; }
        

        #endregion

        #region [ES3 저장/로드]

        /// <summary> 저장 </summary>
        public void saveDataToLocal()
        {            
            ES3.Save(AuthManager.instance.Uid, _userEntity.saveData());
        }

        /// <summary>  </summary>
        public void loadDataFromLocal(UserEntity userEntity)
        {
            _userEntity = userEntity;
        }

        #endregion

        #region [초기화]

        public UserGameData()
        {
            _userEntity = new UserEntity(AuthManager.instance.networkCheck());
            _userEntity.applyLevel();

            _skinBval = new bool[(int)skinBvalue.max];
            _skinFval = new ObscuredFloat[(int)skinFvalue.max];
            _skinIval = new ObscuredInt[(int)skinIvalue.max];
            _ballType = snowballType.standard;

            applySkin();
        }

        public void flashData()
        {
        }

        #endregion

        #region [강화/스킨/적용]

        public void statusLevelUp(StatusData stat)
        {
            _userEntity.statusLevelUp(stat);
        }

        /// <summary>  </summary>
        public void applySkin()
        {
            SkinKeyList skin = (SkinKeyList)((int)_userEntity._property._skin);
            string key = skin.ToString();

            string ss = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.season.ToString());

            ObscuredInt j;
            ObscuredFloat chk;
            ObscuredFloat add = 0;

            _addStats = new ObscuredFloat[(int)defaultStat.max];
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;
                chk = DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.d_hp + j).ToString());
                _addStats[(int)i] = chk;

                if (chk == 1)
                    continue;

                if (i < defaultStat.speed)
                {
                    add = _userEntity._status._statusLevel[(int)StatusData.skin] * DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.ex_hp + j).ToString());
                }

                _addStats[(int)i] += add;
            }

            if (ss.Equals("n") == false)
            {
                _applySeason = EnumHelper.StringToEnum<season>(ss);
            }

            string t_B = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeB.ToString());
            if (t_B.Equals("n") == false) 
            {
                skinBvalue sbv = EnumHelper.StringToEnum<skinBvalue>(t_B);
                _skinBval[(int)sbv] = true;
            }

            string t_F = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeF.ToString());
            if (t_F.Equals("n") == false)
            {
                skinFvalue sfv = EnumHelper.StringToEnum<skinFvalue>(t_F);
                
                _skinFval[(int)sfv] = DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval0.ToString());
                _skinFval[(int)sfv] += _userEntity._status._statusLevel[(int)StatusData.skin] * DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval1.ToString());
            }

            string t_I = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeI.ToString());
            if (t_I.Equals("n") == false)
            {
                skinIvalue siv = EnumHelper.StringToEnum<skinIvalue>(t_I);

                _skinIval[(int)siv] = DataManager.GetTable<int>(DataTable.skin, key, SkinValData.Ival.ToString());
            }

            string sb = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.snowball.ToString());
            if (sb.Equals("n") == false)
            {
                _ballType = EnumHelper.StringToEnum<snowballType>(sb);
            }
        }

        /// <summary> 스킨 설명 </summary>
        public string getSkinExplain(SkinKeyList skin)
        {
            string str = "";
            ObscuredInt statVal = 0;
            switch (skin)
            {
                case SkinKeyList.snowman:
                    str = "겨울에 만들어진 눈사람";
                    break;
                case SkinKeyList.fireman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100;
                    str = "여름한정!" + System.Environment.NewLine + $"공격력 {statVal}% 증가";
                    break;
                case SkinKeyList.grassman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.hpgen] * 100) - 100;
                    str = "봄 한정!" + System.Environment.NewLine + $"체력재생량 {statVal}% 증가";
                    break;
                case SkinKeyList.rockman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.hp] * 100) - 100;
                    str = "눈 대신 돌을 던진다." + System.Environment.NewLine + $"체력 {statVal}% 증가";
                    break;
                case SkinKeyList.citrusman:
                    ObscuredFloat val = _addStats[(int)defaultStat.coin] * 100 - 100;
                    str = "눈 대신 귤을 던진다." + System.Environment.NewLine + "겨울한정!" + System.Environment.NewLine + string.Format("코인획득량 {0:0.0}% 증가", val);
                    break;
                case SkinKeyList.bulbman:
                    str = "머리의 전구로 밤을 밝힌다.";
                    break;
                case SkinKeyList.wildman:
                    str = $"잃은 체력 1%당 공격력 {_skinFval[(int)skinFvalue.wild]}% 증가";
                    break;
                case SkinKeyList.mineman:
                    str = $"지뢰 개수 +{_skinIval[(int)skinIvalue.mine]}" + System.Environment.NewLine + $"지뢰 공격력 {_skinFval[(int)skinFvalue.mine]}% 증가";
                    break;
                case SkinKeyList.robotman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100;
                    str = "늪지 무효" + System.Environment.NewLine + $"방어력 {statVal}% 증가";
                    break;
                case SkinKeyList.icecreamman:
                    str = "눈덩이가 빙결 적용" + System.Environment.NewLine + $"블리자드, 아이스에이지 발동시 체력 {_skinFval[(int)skinFvalue.iceHeal]}% 회복";
                    break;
                case SkinKeyList.goldenarmorman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100;
                    str = $"30초에 한번씩 {_skinFval[(int)skinFvalue.invincible]}초간 무적" + System.Environment.NewLine + $"방어력 {statVal}% 증가";
                    break;
                case SkinKeyList.angelman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.speed] * 100) - 100;
                    str = $"눈사람이 죽을때 최대체력의 {_skinFval[(int)skinFvalue.rebirth]}%로 부활" + System.Environment.NewLine +"(유적과 버프 상실)" + System.Environment.NewLine + $"이동속도 {statVal}% 증가";
                    break;
                case SkinKeyList.squareman:
                    str = "네모난 눈을 던진다." + System.Environment.NewLine + $"30% 확률로 {_skinFval[(int)skinFvalue.criticDmg]}%의 추가데미지를 준다.";
                    break;
                case SkinKeyList.spiderman:
                    str = "추가로 달린 다리로 더 많은 눈덩이를 던진다." + System.Environment.NewLine + $"눈덩이 공격력 {_skinFval[(int)skinFvalue.snowball]}% 증가";
                    break;
                case SkinKeyList.vampireman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100;
                    str = $"눈덩이로 준 피해의 {_skinFval[(int)skinFvalue.blood]}%만큼 체력을 회복한다." + System.Environment.NewLine + $"공격력 {statVal}% 증가";
                    break;
                case SkinKeyList.heroman:
                    ObscuredInt[] stt = new ObscuredInt[3] { Convert.ToInt32(_addStats[(int)defaultStat.hp] * 100) - 100,
                        Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100, 
                        Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100 };

                    str = "용사의 검을 찾으면 일시적으로 공격력/방어력이 2배가 된다."
                        + System.Environment.NewLine + $"체력 {stt[0]}% 증가/공격력 {stt[1]}% 증가/방어력 {stt[2]}% 증가";
                    break;
                default:
                    break;
            }
            return str;
        }

        #endregion

        #region [ 기타 설정 함수 ]

        public void setNewRecord(ObscuredInt newRecord)
        {
            _userEntity._record._timeRecord = newRecord;
            _userEntity._record._recordSkin = _userEntity._property._skin;
        }

        #endregion

        /// <summary> ObscuredFloat 기록 --> 전부 string으로 변환 </summary>
        public string getLifeTime(ObscuredFloat time, bool isTwoLine)
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

        /// <summary> ObscuredFloat 기록 --> 날짜까지만 string으로 변환 </summary>
        public string getTimeRecordToString(ObscuredFloat time)
        {
            ObscuredInt year;
            ObscuredInt season;
            ObscuredInt day;

            year = (int)(time / (24 * 60));
            time -= year * 24 * 60;
            season = (int)(time / (6 * 60));
            time -= season * 6 * 60;
            day = (int)(time / (2 * 60));
            time -= day * 2 * 60;

            string str = "";
            if (year > 0)
            {
                str += $"{year}년 ";
            }

            switch (season)
            {
                case 0:
                    str += "봄 ";
                    break;
                case 1:
                    str += "여름 ";
                    break;
                case 2:
                    str += "가을 ";
                    break;
                case 3:
                    str += "겨울 ";
                    break;
            }

            if (day > 0)
            {
                switch (day)
                {
                    case 0:
                        str += "첫째날";
                        break;
                    case 1:
                        str += "둘째날";
                        break;
                    case 2:
                        str += "셋째날";
                        break;
                }
            }

            return str;
        }

        /// <summary> 데이터 저장 </summary>
        public string getUserData()
        {
            return _userEntity.saveData();
        }

        public Dictionary<string, object> getRankData(string uid)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["_uid"] = uid;
            data["_nick"] = NickName;
            data["_time"] = TimeRecord;
            data["_boss"] = BossRecord;
            data["_skin"] = RecordSkin;
            data["_version"] = gameValues._version;

            return data;
        }

        public DateTime getEpochDate()
        {
            DateTime utcCreated = gameValues.epoch.AddMilliseconds(AuthManager.instance.LastLogin);
            return utcCreated;
        }
    }
}