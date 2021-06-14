using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;

namespace week
{
    public class LobbyScene : UIBase
    {
        #region [UIBase]
        enum eGO
        {
            Store,
            Snowman,
            Lobby,
            Status,

            wins,
            updatePanel,

            postExcla,
            questExcla
        }

        enum eImg
        {
            optionBtn,

            Coin,
            Gem
        }

        enum eTmp
        {
            CoinTxt,
            GemTxt,
            RecordTxt,
            nickName,
            upVersion,
            levelTxt
        }

        protected override Enum GetEnumGameObject() { return new eGO(); }
        protected override Enum GetEnumImage() { return new eImg(); }
        public TextMeshProUGUI[] MTmps;
        protected Enum GetEnumTmp() { return new eTmp(); }

        protected override void OtherSetContent()
        {
            if (GetEnumTmp() != null)
            {
                MTmps = SetComponent<TextMeshProUGUI>(GetEnumTmp());
            }
        }

        #endregion

        [SerializeField] SnowController _snow;
        [SerializeField] Transform _angle;
        [SerializeField] RawImage _pattern;
        [Space]
        [SerializeField] Canvas _canvas;

        Vector2 _offset;
        readonly Vector2 _rect = new Vector2(1f, 2f);

        bool _isLobby;

        storeComp _storeComp;
        lobbySnowmanComp _snowComp;
        lobbyComp _lobbyComp;
        lobbyStatusComp _statusComp;

        questComp _quest;
        rankComp _rank;
        postComp _post;

        optionComp _option;
        nickChangePopup _nickPanel;
        couponComp _coupon;
        levelPopup _levelPanel;
        recommendPopup _recommend;

        public Transform CoinTxt { get { return mImgs[(int)eImg.Coin].transform; } }
        public Transform GemTxt { get { return mImgs[(int)eImg.Gem].transform; } }

        bool _refreshCoinChk;
        bool _refreshGemChk;

        float time = 0;
        Action refresh_SnowImg;

        // Start is called before the first frame update
        void Start()
        {
            _canvas.worldCamera = Camera.main;
            _canvas.planeDistance = 1400f;

            // 재화 텍스트 이벤트를 위한 초기화
            if (BaseManager.userGameData.followCoin == 0)
            {
                BaseManager.userGameData.followCoin = BaseManager.userGameData.Coin;
                BaseManager.userGameData.followGem = BaseManager.userGameData.Gem;
            }
            WindowManager.instance.Win_coinGenerator.RefreshFollowCost = refreshFollowCost;
            getMyPreRanking();

            // 로비에서 연결되는 창 초기화            
            // ========== [ 각 탭 ] ========================================

            _storeComp = mGos[(int)eGO.Store].GetComponent<storeComp>();
            _snowComp = mGos[(int)eGO.Snowman].GetComponent<lobbySnowmanComp>();
            _lobbyComp = mGos[(int)eGO.Lobby].GetComponent<lobbyComp>();
            _statusComp = mGos[(int)eGO.Status].GetComponent<lobbyStatusComp>();

            Debug.Log(mGos[(int)eGO.Status].gameObject);         
            
            // ========== [ 서브 창 ] ======================================

            _quest = Instantiate(DataManager.PanelFabs[lobbyPanel.questPanel], mGos[(int)eGO.wins].transform).GetComponent<questComp>();
            _rank = Instantiate(DataManager.PanelFabs[lobbyPanel.rankPanel], mGos[(int)eGO.wins].transform).GetComponent<rankComp>();
            _post = Instantiate(DataManager.PanelFabs[lobbyPanel.postPanel], mGos[(int)eGO.wins].transform).GetComponent<postComp>();
            _option = Instantiate(DataManager.PanelFabs[lobbyPanel.optionPanel], mGos[(int)eGO.wins].transform).GetComponent<optionComp>(); 
            _nickPanel = Instantiate(DataManager.PanelFabs[lobbyPanel.nicPanel], mGos[(int)eGO.wins].transform).GetComponent<nickChangePopup>();
            _levelPanel = Instantiate(DataManager.PanelFabs[lobbyPanel.levelPanel], mGos[(int)eGO.wins].transform).GetComponent<levelPopup>();
            _recommend = Instantiate(DataManager.PanelFabs[lobbyPanel.recommendPanel], mGos[(int)eGO.wins].transform).GetComponent<recommendPopup>();

            // ========== [ 각 탭 초기화 ] ========================================
            BaseManager.userGameData.Skin = SkinKeyList.snowman;
            BaseManager.userGameData.HasSkin |= 1;
            _storeComp.Init((chk) => { mGos[(int)eGO.postExcla].SetActive(chk); });
            _snowComp.Init(refresh_Cost, _lobbyComp.refresh_SnowImg);
            _lobbyComp.Init(this);
            _statusComp.Init(refresh_Cost, _quest.refresh_CheckQuest, _snowComp.showSnowmanInfo);

            // ========== [ 서브 창 초기화 ] ========================================

            _quest.Init(this, (chk) => { mGos[(int)eGO.questExcla].SetActive(chk); });
            _rank.Init();
            _post.Init(this, (chk) => { mGos[(int)eGO.postExcla].SetActive(chk); });
            _option.Init(this, () =>
             {
                 _option.gameObject.SetActive(false);
                 _nickPanel.open();
             });
            _nickPanel.completeChange = () => {
                _lobbyComp.setStage();
                refresh_Cost(); 
            };
            _levelPanel.Init(_lobbyComp.show_levelRecord);

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.followCoin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.followGem.ToString();

            _isLobby = true;
            _quest.close();
            _rank.close();
            _option.close();
            _nickPanel.close();
            mGos[(int)eGO.updatePanel].SetActive(false);

            // 날짜체크 초기화
            AuthManager.instance.WhenTomorrow = BaseManager.userGameData.setNextDay;
            AuthManager.instance.WhenTomorrow += _quest.setNextDay;
            AuthManager.instance.WhenTomorrow += (i) => AuthManager.instance.SaveDataServer(true);

            StartCoroutine(AuthManager.instance.checkNextDay());

            // bgm
            SoundManager.instance.PlayBGM(BGM.Lobby);

            // 로비 눈 이펙트
            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);

            // 게임 종료후 코인 이펙트 관련
            if (BaseManager.userGameData.GameReward != null)
            {
                WindowManager.instance.Win_coinGenerator.getWealth2Point(Vector3.zero, CoinTxt.position, currency.coin, BaseManager.userGameData.GameReward[0], 1);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(Vector3.zero, GemTxt.position, currency.gem, BaseManager.userGameData.GameReward[1], 2);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(Vector3.zero, CoinTxt.position, currency.ap, BaseManager.userGameData.GameReward[2], 3);

                BaseManager.userGameData.GameReward = null;
            }

            // 기타
            if (BaseManager.userGameData.FreeNichkChange == false) // 닉네임
            {
                _nickPanel.open();
            }

            if (BaseManager.NeedPatch) // 버전창
            {
                openUpdate();
            }

            refresh_Cost();

            modify(); // 특별히 체크해주거나 적용해줘야 할 값

            AuthManager.instance.checkNextDay();
            AuthManager.instance.SaveDataServer(true);
        }

        void modify()
        {
            if (BaseManager.userGameData.RemoveAd)
            {
                BaseManager.userGameData.AddMulCoinList(mulCoinChkList.removeAD);
            }

            // BaseManager.userGameData.UtilChkList &= ~(1 << (int)utilityChkList.change_SecondStatus);

            if (BaseManager.userGameData.Change_SecondStatus == false || BaseManager.userGameData.Ap > 0)
            {
                int leng = BaseManager.userGameData.StatusLevel.Length;
                int exist_ap = 0;

                int cost, stair;
                int v, m;

                for (statusKeyList i = 0; i < statusKeyList.max; i++)
                {
                    int lvl = BaseManager.userGameData.StatusLevel[(int)i];

                    cost = D_status.GetEntity(i.ToString()).f_cost;
                    stair = D_status.GetEntity(i.ToString()).f_stair;

                    v = lvl / stair;
                    m = lvl % stair;

                    int cc = (int)((1 + v) * (m + stair * v * 0.5f)) * cost;

                    exist_ap += cc;
                    BaseManager.userGameData.StatusLevel[(int)i] = 0;
                }

                BaseManager.userGameData.Ap += exist_ap;
                BaseManager.userGameData.Coin += BaseManager.userGameData.Ap * 1000;
                BaseManager.userGameData.Ap = 0;

                BaseManager.userGameData.Change_SecondStatus = true;

                _statusComp.refresh_Status();
            }

            if (BaseManager._innerData.showRecommend)
            {
                Debug.Log("신기록 열려?");
                _recommend.open();
                BaseManager._innerData.showRecommend = false;
            }
        }

        private void Update()
        {
            _offset.x -= Time.deltaTime * 0.01f;
            _offset.y -= Time.deltaTime * 0.02f;

            if (_offset.x < -1f)
            {
                _offset.x = 0;
            }

            if (_offset.y < -1f)
            {
                _offset.y = 0;
            }

            _pattern.uvRect = new Rect(_offset, _rect);

            time += Time.deltaTime;
            if (time > 600f)
            {
                time = 0f;
                StartCoroutine(AuthManager.instance.checkNextDay());
            }
        }

        public void setEquip(DataTable type)
        {
            int val = (int)type;
        }

        public void refreshFollowCost()
        {
            if (BaseManager.userGameData.followCoin > BaseManager.userGameData.Coin)
            {
                BaseManager.userGameData.followCoin = BaseManager.userGameData.Coin;
            }
            if (BaseManager.userGameData.followGem > BaseManager.userGameData.Gem)
            {
                BaseManager.userGameData.followGem = BaseManager.userGameData.Gem;
            }

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.followCoin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.followGem.ToString();
            //_statusComp.refresh_AboutAp();
        }

        public void refresh_Cost()
        {
            MTmps[(int)eTmp.CoinTxt].text = ((int)BaseManager.userGameData.Coin).ToString();
            MTmps[(int)eTmp.GemTxt].text = ((int)BaseManager.userGameData.Gem).ToString();
            //_statusComp.refresh_AboutAp();
        }

        //public void refreshAp()
        //{
        //    _statusComp.refresh_AboutAp();
        //}


        #region [ lobbyComp ] =================================================

        /// <summary> (로비탭에서 시행) 일퀘창 열기 </summary>
        public void openQuest()
        {
            _isLobby = false;
            allClose(true);
            _quest.open();
        }

        /// <summary> (로비탭에서 시행) 랭킹창 열기 </summary>
        public void openRank()
        {
            _isLobby = false;
            allClose(false);

            _rank.open();
        }

        /// <summary> (로비탭에서 시행) 우편함 열기 </summary>
        public void openPost()
        {
            _isLobby = false;
            allClose(false);

            _post.open();
        }

        /// <summary> (로비탭에서 시행) 난이도창 열기 </summary>
        public void openLevel()
        {
            _isLobby = false;
            allClose(false);

            _levelPanel.open();
        }

        /// <summary> (로비탭에서 시행) 게임시작 </summary>
        public void PlayGame()
        {
            BaseManager.userGameData.applyLevel();
            SoundManager.instance.StopBGM();
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
        }

        #endregion

        public void openOption()
        {
            if (_isLobby)
            {
                _option.open();
            }
            else
            {
                _isLobby = true;

                allClose(true);
                //refresh_SnowImg();
            }
        }

        public void openUpdate()
        {
            MTmps[(int)eTmp.upVersion].text = $"{AuthManager.instance.Version} 서브" + System.Environment.NewLine + "업데이트";
            mGos[(int)eGO.updatePanel].SetActive(true);
        }

        public void connectDownLoadURL()
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.munzi.snowAdventure");
        }

        void allClose(bool lobby)
        {
            _quest.close();
            _rank.close();
            _option.close();
            _nickPanel.close();
            _levelPanel.close();
        }

        void getMyPreRanking()
        {
            NanooManager.instance.getPreSeasonRankingPersonal((dictionary) =>
            {
                if (dictionary == null)
                {
                    Debug.LogError("랭킹데이터 없음");
                    return;
                }

                BaseManager.userGameData.preRank = int.Parse((string)dictionary["ranking"]);
                // int totalPlayer = int.Parse((string)dictionary["total_player"]);
            });
        }

        [Button]
        public void getMoney()
        {
            BaseManager.userGameData.Gem += 2000;
            BaseManager.userGameData.Coin += 500000;

            refresh_Cost();
        }

        [Button]
        public void reset()
        {
            BaseManager.userGameData = new UserGameData();
            AuthManager.instance.SaveDataServer(false);
            refresh_Cost();
            _lobbyComp.setStage();
        }
    }
}
