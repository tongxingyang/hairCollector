using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;

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
        
        /// <summary> 가장 최근 랭킹 최신화 시간 </summary>
        public DateTime _rankRefreshTime { get; set; }

        /// <summary> 전투결과 </summary>
        public ObscuredInt[] GameReward { get; set; }

        /// <summary> 이전 랭킹 </summary>
        public int preRank { get; set; } = -1;

        bool autoRecivePost;

        /// <summary> 타임스탬프 간격 </summary>
        public float TimeCheck { get; set; }

        /// <summary> 레이더 대여 가능? </summary>
        public bool RaderRentalable { get; set; }

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
        
        // - 템
        public ObscuredBool IsSetRader { get => _userEntity._property._isSetRader; set => _userEntity._property._isSetRader = value; }
        public ObscuredLong LastRaderTime { get => _userEntity._property._lastRaderTime; set => _userEntity._property._lastRaderTime = value; }
        
        // 플레이어 통계 데이터 ==============================================================
        public ObscuredInt WholeAccessTime { get => _userEntity._statistics._wholeAccessTime; set => _userEntity._statistics._wholeAccessTime = value; }
        public ObscuredInt PlayCount { get => _userEntity._statistics._playCount; set => _userEntity._statistics._playCount = value; }
        public ObscuredInt StoreUseCount { get => _userEntity._statistics._storeUseCount; set => _userEntity._statistics._storeUseCount = value; }

        // 기록 ==============================================================
        public ObscuredString NowSeasonRankKey { get => _userEntity._record._nowSeasonRankKey; }
        public ObscuredInt SeasonTimeRecord { get => _userEntity._record._seasonTimeRecord; }
        public ObscuredInt RecordSeasonSkin { get => _userEntity._record._recordSeasonSkin; }
        public ObscuredInt AllTimeRecord { get => _userEntity._record._allTimeRecord; }
        public ObscuredInt RecordAllSkin { get => _userEntity._record._recordAllSkin; }

        public ObscuredInt WholeTimeRecord { get => _userEntity._record._wholeTimeRecord; set => _userEntity._record._wholeTimeRecord = value; }
        public ObscuredInt BossRecord { get => _userEntity._record._bossRecord; set => _userEntity._record._bossRecord = value; }
        public ObscuredInt ArtifactRecord { get => _userEntity._record._artifactRecord; set => _userEntity._record._artifactRecord = value; }
        public ObscuredInt AdRecord { get => _userEntity._record._adRecord; set => _userEntity._record._adRecord = value; }
        public ObscuredInt ReinRecord { get => _userEntity._record._reinRecord; set => _userEntity._record._reinRecord = value; }

        // 퀘스트 ==============================================================
        // - 일일
        public ObscuredInt[] DayQuest { get => _userEntity._quest._dayQuest; set => _userEntity._quest._dayQuest = value; }
        public ObscuredInt DayQuestRein { get => _userEntity._quest._dayQuest[0]; set => _userEntity._quest._dayQuest[0] = value; }
        public ObscuredInt DayQuestSkin { get => _userEntity._quest._dayQuest[1]; set => _userEntity._quest._dayQuest[1] = value; }
        public ObscuredInt DayQuestAd { get => _userEntity._quest._dayQuest[2]; set => _userEntity._quest._dayQuest[2] = value; }
        /// <summary> 오늘의 스킨 퀘스트 </summary>
        public ObscuredInt QuestSkin { get => _userEntity._quest._questSkin; set => _userEntity._quest._questSkin = value; }
        // - 전체
        public ObscuredInt GetTimeReward { get => _userEntity._quest._getTimeReward; set => _userEntity._quest._getTimeReward = value; }
        public ObscuredInt GetBossReward { get => _userEntity._quest._getBossReward; set => _userEntity._quest._getBossReward = value; }
        // public ObscuredInt GetArtifactReward { get => _userEntity._quest._getArtifactReward; set => _userEntity._quest._getArtifactReward = value; }

        // 스탯 ==============================================================
        public ObscuredInt[] StatusLevel { get => _userEntity._status._statusLevel; set => _userEntity._status._statusLevel = value; }

        public ObscuredFloat o_Hp           { get; set; }
        public ObscuredFloat o_Att          { get; set; }
        public ObscuredFloat o_Hpgen        { get; set; }
        public ObscuredFloat o_Def          { get; set; }
        public ObscuredFloat o_Cool         { get; set; }
        public ObscuredFloat o_ExpFactor    { get; set; }
        public ObscuredFloat o_CoinFactor   { get; set; }
        public ObscuredFloat SkinEnhance    { get; set; }

        // 인앱결제 ==============================================================
        public int amcl { get => _userEntity._payment._mulCoinList; }
        public void AddMulCoinList(mulCoinChkList index) 
        { 
            _userEntity._payment._mulCoinList |= (1 << (int)index); 
        }
        public bool chkMulCoinList(mulCoinChkList index)
        {
            return (_userEntity._payment._mulCoinList & (1 << (int)index)) > 0;
        }
        public ObscuredInt PaymentChkList { get => _userEntity._payment._chkList; set => _userEntity._payment._chkList = value; }
        public bool RemoveAd { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.removeAD)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.removeAD) : 0; }
        public bool MulCoin3p { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.mul_1st_3p)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.mul_1st_3p) : 0; }
        public bool StartPack { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.startPack)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.startPack) : 0; }
        public bool SkinPack { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.skinPack)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.skinPack) : 0; }        

        public long NextAdGemTime { get => _userEntity._payment._nextAdGemTime; set => _userEntity._payment._nextAdGemTime = value; }

        // 유틸 ==============================================================
        public ObscuredLong LastSave { get => _userEntity._util._lastSave;
            set
            {
                if (AuthManager.instance.networkCheck()) 
                { _userEntity._util._lastSave = value; };
            }
        }
        public ObscuredInt UtilChkList { get => _userEntity._util._chkList; set => _userEntity._util._chkList = value; }
        public bool FreeNichkChange { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.freeNickChange)) > 0;
                                        set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.freeNickChange) : 0; }        

        #endregion

        #region [ES3 저장/로드]

        /// <summary> 데이터 기기 저장(오프라인) </summary>
        public void saveOffLineData()
        {            
            ES3.Save(gameValues._offlineKey, _userEntity.saveData());
        }

        /// <summary> 데이터 기기 로드(오프라인) </summary>
        public void loadOffLineData()
        {
            ObscuredString _offlineDataJson = ES3.Load<string>(gameValues._offlineKey);
            UserEntity _offlineData = JsonConvert.DeserializeObject<UserEntity>(_offlineDataJson, new ObscuredValueConverter());

            _userEntity = _offlineData;
        }

        public void setUserEntity(UserEntity entity)
        {
            //_userEntity = new UserEntity(entity);
            _userEntity = entity;
            applySkin();
        }

        #endregion

        #region [초기화]

        public UserGameData()
        {
            _userEntity = new UserEntity();
            applyLevel();

            applySkin();
        }

        public void flashData()
        {
        }

        /// <summary> obscured 키 Randomize </summary>
        public IEnumerator RandomizeKey_Coroutine()
        {
            var oneSecondWait = new WaitForSecondsRealtime(1f);
            while(true)
            {
                yield return oneSecondWait;

                // 재화 gBrtJ0L0MzMadc5SrEWT3qE2y3B2
                for (int i = 0; i < 3; i++)
                {
                    _userEntity._property._currency[i].RandomizeCryptoKey();
                }
                Coin.RandomizeCryptoKey();
                Gem.RandomizeCryptoKey();
                Ap.RandomizeCryptoKey();

                // 강화창
                for (int i = 0; i < (int)StatusData.max; i++)
                {
                    _userEntity._status._statusLevel[i].RandomizeCryptoKey();
                    StatusLevel[i].RandomizeCryptoKey();
                }                
            }
        }

        /// <summary> 스탯 레벨 -> 스탯에 적용 </summary>
        public void applyLevel()
        {
            o_Hp            = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.hp.ToString())
                                        + (getAddit(StatusData.hp) * StatusLevel[(int)StatusData.hp]);
            o_Att           = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.att.ToString())
                                + (getAddit(StatusData.att) * StatusLevel[(int)StatusData.att]);
            o_Def           = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.def.ToString())
                                + (getAddit(StatusData.def) * StatusLevel[(int)StatusData.def]);
            o_Hpgen         = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.hpgen.ToString())
                                + (getAddit(StatusData.hpgen) * StatusLevel[(int)StatusData.hpgen]);
            o_Cool          = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.cool.ToString())
                                + (getAddit(StatusData.cool) * StatusLevel[(int)StatusData.cool]);
            o_ExpFactor     = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.exp.ToString())
                                + (getAddit(StatusData.exp) * StatusLevel[(int)StatusData.exp]);
            o_CoinFactor    = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.coin.ToString())
                                + (getAddit(StatusData.coin) * StatusLevel[(int)StatusData.coin]);
            SkinEnhance     = StatusLevel[(int)StatusData.skin];
        }

        public float getAddit(StatusData type)
        {
            float add = (float)DataManager.GetTable<int>(DataTable.status, statusKeyList.addition.ToString(), type.ToString());
            float rate = (float)DataManager.GetTable<int>(DataTable.status, statusKeyList.additrate.ToString(), type.ToString());
            return (add / rate);
        }

        #endregion

        #region [강화/스킨/적용]

        /// <summary> 스탯 레벨 업 </summary>
        public void statusLevelUp(StatusData stat)
        {
            StatusLevel[(int)stat]++;

            switch (stat)
            {
                case StatusData.hp:
                    o_Hp = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.hp.ToString())
                                        + (getAddit(StatusData.hp) * StatusLevel[(int)StatusData.hp]);
                    break;
                case StatusData.att:
                    o_Att = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.att.ToString())
                                        + (getAddit(StatusData.att) * StatusLevel[(int)StatusData.att]);
                    break;
                case StatusData.def:
                    o_Def = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.def.ToString())
                                        + (getAddit(StatusData.def) * StatusLevel[(int)StatusData.def]);
                    break;
                case StatusData.hpgen:
                    o_Hpgen = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.hpgen.ToString())
                                        + (getAddit(StatusData.hpgen) * StatusLevel[(int)StatusData.hpgen]);
                    break;
                case StatusData.cool:
                    o_Cool = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.cool.ToString())
                                        + (getAddit(StatusData.cool) * StatusLevel[(int)StatusData.cool]);
                    break;
                case StatusData.exp:
                    o_ExpFactor = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.exp.ToString())
                                        + (getAddit(StatusData.exp) * StatusLevel[(int)StatusData.exp]);
                    break;
                case StatusData.coin:
                    o_CoinFactor = DataManager.GetTable<int>(DataTable.status, statusKeyList.origin.ToString(), StatusData.coin.ToString())
                                        + (getAddit(StatusData.coin) * StatusLevel[(int)StatusData.coin]);
                    break;
                case StatusData.skin:
                    SkinEnhance = StatusLevel[(int)StatusData.skin];
                    break;
            }
        }

        /// <summary> 스킨과 스킨 능력치 적용 </summary>
        public void applySkin()
        {
            SkinKeyList skin = (SkinKeyList)((int)_userEntity._property._skin);
            string key = skin.ToString();

            string ss = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.season.ToString());

            ObscuredInt j;

            _skinBval = new bool[(int)skinBvalue.max];
            _skinFval = new ObscuredFloat[(int)skinFvalue.max];
            _skinIval = new ObscuredInt[(int)skinIvalue.max];
            _ballType = snowballType.standard;

            _addStats = new ObscuredFloat[(int)defaultStat.max];
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;

                _addStats[(int)i] = DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.d_hp + j).ToString()) * 0.01f;

                if (i < defaultStat.speed)
                {
                    _addStats[(int)i] += DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.ex_hp + j).ToString()) * 0.01f * SkinEnhance;
                }
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

        /// <summary> 스킨구매전 데이터 제공용 </summary>
        public void getSkinInfo(SkinKeyList skin, ref float[] fval, ref int[] ival, ref float[] stats)
        {
            string key = skin.ToString();
            int j;

            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;

                stats[(int)i] = DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.d_hp + j).ToString()) * 0.01f;

                if (i < defaultStat.speed)
                {
                    stats[(int)i] += DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.ex_hp + j).ToString()) * 0.01f * SkinEnhance; ;
                }
            }

            string t_F = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeF.ToString());
            if (t_F.Equals("n") == false)
            {
                skinFvalue sfv = EnumHelper.StringToEnum<skinFvalue>(t_F);

                fval[(int)sfv] = DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval0.ToString());
                fval[(int)sfv] += _userEntity._status._statusLevel[(int)StatusData.skin] * DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval1.ToString());
            }

            string t_I = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeI.ToString());

            if (t_I.Equals("n") == false)
            {
                skinIvalue siv = EnumHelper.StringToEnum<skinIvalue>(t_I);

                ival[(int)siv] = DataManager.GetTable<int>(DataTable.skin, key, SkinValData.Ival.ToString());
                Debug.Log(ival[(int)siv]);
            }
        }

        /// <summary> 스킨 설명 </summary>
        public string getSkinExplain(SkinKeyList skin, bool ApplyData = true)
        {
            float[] fval = new float[(int)skinFvalue.max];
            int[] ival = new int[(int)skinIvalue.max];
            float[] stats = new float[(int)defaultStat.max];

            if (ApplyData)
            {
                for (int i = 0; i < (int)skinFvalue.max; i++)
                    fval[i] = _skinFval[i];

                for (int i = 0; i < (int)skinIvalue.max; i++)
                    ival[i] = _skinIval[i];

                for (int i = 0; i < (int)defaultStat.max; i++)
                    stats[i] = _addStats[i];
            }
            else
            {
                getSkinInfo(skin, ref fval, ref ival, ref stats);
            }

            string str = "";
            ObscuredInt statIVal = 0;
            ObscuredFloat statFVal = 0f;

            switch (skin)
            {
                case SkinKeyList.snowman:
                    str = "겨울에 만들어진 눈사람";
                    break;
                case SkinKeyList.fireman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.att] * 100) - 100;
                    str = "여름한정!" + System.Environment.NewLine + $"공격력 {statIVal}% 증가";
                    break;
                case SkinKeyList.grassman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.hpgen] * 100) - 100;
                    str = "봄 한정!" + System.Environment.NewLine + $"체력재생량 {statIVal}% 증가";
                    break;
                case SkinKeyList.rockman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.hp] * 100) - 100;
                    str = "눈 대신 돌을 던진다." + System.Environment.NewLine + $"체력 {statIVal}% 증가";
                    break;
                case SkinKeyList.citrusman:
                    // 미정
                    str = "눈 대신 귤을 던진다.";
                    break;
                case SkinKeyList.bulbman:
                    str = "머리의 전구로 밤을 밝힌다.";
                    break;
                case SkinKeyList.presentman:
                    statFVal = stats[(int)defaultStat.coin] * 100 - 100;
                    str = "눈 대신 선물 받아라!" + System.Environment.NewLine + "겨울한정!" + System.Environment.NewLine + string.Format("코인획득량 {0:0.0}% 증가", statFVal);
                    break;
                case SkinKeyList.wildman:
                    str = $"잃은 체력 1%당 공격력 {fval[(int)skinFvalue.wild]}% 증가";
                    break;
                case SkinKeyList.mineman:
                    str = $"지뢰 개수 +{ival[(int)skinIvalue.mine]}" + System.Environment.NewLine + $"지뢰 공격력 {fval[(int)skinFvalue.mine]}% 증가";
                    break;
                case SkinKeyList.robotman:
                    str = "늪지 무효" + System.Environment.NewLine + string.Format("추가 방어 {0:0.00}% ", stats[(int)defaultStat.def] * 100f);
                    break;
                case SkinKeyList.icecreamman:
                    str = "눈덩이가 빙결 적용" + System.Environment.NewLine + $"블리자드, 아이스에이지 발동시 체력 {fval[(int)skinFvalue.iceHeal]}% 회복";
                    break;
                case SkinKeyList.goldman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.def]);
                    str = $"30초에 한번씩 {fval[(int)skinFvalue.invincible]}초간 무적" + System.Environment.NewLine + $"방어력 {statIVal}% 증가";
                    break;
                case SkinKeyList.angelman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.speed] * 100) - 100;
                    str = $"눈사람이 죽을때 최대체력의 {fval[(int)skinFvalue.rebirth]}%로 부활" + System.Environment.NewLine +"(유적과 버프 상실)" + System.Environment.NewLine + $"이동속도 {statIVal}% 증가";
                    break;
                case SkinKeyList.squareman:
                    str = "네모난 눈을 던진다." + System.Environment.NewLine + $"30% 확률로 {fval[(int)skinFvalue.criticDmg]}%의 추가데미지를 준다.";
                    break;
                case SkinKeyList.spiderman:
                    str = "추가로 달린 다리로 더 많은 눈덩이를 던진다." + System.Environment.NewLine + $"눈덩이 공격력 {fval[(int)skinFvalue.snowball]}% 증가";
                    break;
                case SkinKeyList.vampireman:
                    statIVal = Convert.ToInt32(stats[(int)defaultStat.att] * 100) - 100;
                    str = $"눈덩이로 준 피해의 {fval[(int)skinFvalue.blood]}%만큼 체력을 회복한다." + System.Environment.NewLine + $"공격력 {statIVal}% 증가";
                    break;
                case SkinKeyList.heroman:
                    ObscuredInt[] stt = new ObscuredInt[3] { Convert.ToInt32(stats[(int)defaultStat.hp] * 100) - 100,
                        Convert.ToInt32(stats[(int)defaultStat.att] * 100) - 100, 
                        Convert.ToInt32(stats[(int)defaultStat.def])};

                    str = "용사의 검을 찾으면 일시적으로 공격력/방어력이 2배가 된다."
                        + System.Environment.NewLine + $"체력 {stt[0]}% 증가/공격력 {stt[1]}% 증가/방어력 {stt[2]}% 증가";
                    break;
                case SkinKeyList.santaman:
                    str = $"넌 선물 1개! 난 선물 {ival[(int)skinIvalue.present]}개!" + System.Environment.NewLine
                        + $"선물 먹으면 체력 {fval[(int)skinFvalue.present]}% 회복" + System.Environment.NewLine + "낮은 확률로 공격력 or 방어력 증가";
                    break;
                case SkinKeyList.dragonman:
                    // 미정
                    break;
                default:
                    break;
            }
            return str;
        }

        #endregion

        #region [ 기타 설정 함수 ]

        public void setNewSeasonRecord(ObscuredInt ssRecord)
        {
            _userEntity._record._seasonTimeRecord = ssRecord;
            _userEntity._record._recordSeasonSkin = _userEntity._property._skin;
        }

        public void setNewAllRecord(ObscuredInt allRecord)
        {
            _userEntity._record._allTimeRecord = allRecord;
            _userEntity._record._recordAllSkin = _userEntity._property._skin;
        }

        public void whenRecordNewSeason()
        {
            _userEntity._record._seasonTimeRecord = 0;
            _userEntity._record._recordSeasonSkin = 1;
            _userEntity._record._nowSeasonRankKey = NanooManager.instance.getRANK_CODE;
        }

        public void newNickSetssRecord()
        {
            if (_userEntity._record._seasonTimeRecord > 0)
                _userEntity._record._seasonTimeRecord++;
        }
        public void newNickSetallRecord()
        {
            if (_userEntity._record._allTimeRecord > 0)
                _userEntity._record._allTimeRecord++;
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

        public Dictionary<string, object> getSeasonRankData(string uid)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["_uid"] = uid;
            data["_nick"] = NickName;
            data["_time"] = (int)SeasonTimeRecord;
            data["_boss"] = (int)BossRecord;
            data["_skin"] = (int)RecordSeasonSkin;
            data["_version"] = 11;

            return data;
        }

        /// <summary>  </summary>
        public string getRankData(int skin)
        {
            rankSubData data = new rankSubData(skin);

            return JsonUtility.ToJson(data);
        }

        //public DateTime getEpochDate()
        //{
        //    DateTime utcCreated = gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave);
        //    return utcCreated;
        //}
    }
}