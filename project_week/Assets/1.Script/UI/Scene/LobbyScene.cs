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
            Lobby,
            Store,
            Snowman,
            Quest,
            Rank,
            Post,
            option,
            nicChangePanel,
            updatePanel,

            postExcla,
            questExcla
        }

        enum eImg
        {
            optionBtn,

            snowmanImg,

            stageImage,
            Coin,
            Gem
        }

        enum eTmp
        {
            CoinTxt,
            GemTxt,
            RecordTxt,
            nickName,
            upVersion
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
        Vector2 _offset;
        readonly Vector2 _rect = new Vector2(1f, 2f);

        bool _isLobby;

        storeComp _store;
        snowmanComp _snowman;
        questComp _quest;
        rankComp _rank;
        postComp _post;

        optionComp _option;
        nickChangePopup _nickPanel;

        public Transform CoinTxt { get { return mImgs[(int)eImg.Coin].transform; } }
        public Transform GemTxt { get { return mImgs[(int)eImg.Gem].transform; } }

        bool _refreshCoinChk;
        bool _refreshGemChk;
        bool _chkTomorrow = false;

        // Start is called before the first frame update
        void Start()
        {
            // 재화 텍스트 이벤트를 위한 초기화
            if (BaseManager.userGameData.followCoin == 0)
            {
                BaseManager.userGameData.followCoin = BaseManager.userGameData.Coin;
                BaseManager.userGameData.followGem = BaseManager.userGameData.Gem;
            }
            WindowManager.instance.Win_coinGenerator.RefreshFollowCost = refreshFollowCost;

            // 로비에서 연결되는 창 초기화            
            _quest = mGos[(int)eGO.Quest].GetComponent<questComp>();
            _quest.Init((chk) => { mGos[(int)eGO.questExcla].SetActive(chk); });
            _rank = mGos[(int)eGO.Rank].GetComponent<rankComp>();
            _rank.Init();
            _post = mGos[(int)eGO.Post].GetComponent<postComp>();
            _post.Init(this, (chk) => { mGos[(int)eGO.postExcla].SetActive(chk); });

            _store = mGos[(int)eGO.Store].GetComponent<storeComp>();
            _store.Init(_post.notOpenRefreshCheck);
            _snowman = mGos[(int)eGO.Snowman].GetComponent<snowmanComp>();
            _snowman.Init(refreshCost, _quest.refreshCheckQuest);

            _option = mGos[(int)eGO.option].GetComponent<optionComp>();
            _nickPanel = mGos[(int)eGO.nicChangePanel].GetComponent<nickChangePopup>();
            _option.NickChange = () =>
            {
                mGos[(int)eGO.option].SetActive(false);
                _nickPanel.open();
            };
            _nickPanel.completeChange = () => {
                setStage(); 
                refreshCost(); 
            };

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.followCoin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.followGem.ToString();

            _isLobby = true;
            mGos[(int)eGO.Quest].SetActive(false);
            mGos[(int)eGO.Rank].SetActive(false);
            mGos[(int)eGO.option].SetActive(false);
            mGos[(int)eGO.nicChangePanel].SetActive(false);
            mGos[(int)eGO.updatePanel].SetActive(false);
            setStage();

            // 날짜체크 초기화
            //AuthManager.instance.WhenTomorrow = null;
            AuthManager.instance.WhenTomorrow = _quest.setNextDay;
            StartCoroutine(AuthManager.instance.checkNextDay());
            _chkTomorrow = true;

            // bgm
            SoundManager.instance.PlayBGM(BGM.Lobby);

            // 로비 눈사람 이미지
            refreshSnowImg();

            // 로비 눈 이펙트
            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);

            // 게임 종료후 코인 이펙트 관련
            if (BaseManager.userGameData.GameReward != null)
            {
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, CoinTxt.position, currency.coin, BaseManager.userGameData.GameReward[0], 1);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, GemTxt.position, currency.gem, BaseManager.userGameData.GameReward[1], 2);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, CoinTxt.position, currency.ap, BaseManager.userGameData.GameReward[2], 3);

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

            refreshCost();

            AuthManager.instance.checkNextDay();
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

            if (DateTime.Now.Minute % 10 == 0)
            {
                if (_chkTomorrow == false)
                {
                    _chkTomorrow = true;
                    StartCoroutine(AuthManager.instance.checkNextDay());
                }
            }
            else 
            {
                _chkTomorrow = false;
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
        }

        public void refreshCost()
        {
            MTmps[(int)eTmp.CoinTxt].text = ((int)BaseManager.userGameData.Coin).ToString();
            MTmps[(int)eTmp.GemTxt].text = ((int)BaseManager.userGameData.Gem).ToString();
        }

        public void PlayGame()
        {
            SoundManager.instance.StopBGM();
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
        }
      
        public void openQuest()
        {
            _isLobby = false;
            allClose(true);
            _quest.open();
        }

        public void openRank()
        {
            _isLobby = false;
            allClose(false);

            _rank.open();
        }

        public void openPost()
        {
            _isLobby = false;
            allClose(false);

            _post.open();
        }

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
                refreshSnowImg();
            }
        }

        public void openUpdate()
        {
            MTmps[(int)eTmp.upVersion].text = $"{AuthManager.instance.Version} 버전이 출시되었습니다." + System.Environment.NewLine + "업데이트 해도 되고 안해도 되고요.";
            mGos[(int)eGO.updatePanel].SetActive(true);
        }

        public void connectDownLoadURL()
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.munzi.snowAdventure");
        }

        void allClose(bool lobby)
        {
            mGos[(int)eGO.Quest].SetActive(false);
            mGos[(int)eGO.Rank].SetActive(false);
            mGos[(int)eGO.option].SetActive(false);
            mGos[(int)eGO.nicChangePanel].SetActive(false);
        }

        void setStage()
        {
            MTmps[(int)eTmp.nickName].text = BaseManager.userGameData.NickName;

            if (BaseManager.userGameData.TimeRecord == 0)
            {
                MTmps[(int)eTmp.RecordTxt].text = "응애 나 아기눈사람";
            }
            else
            {
                MTmps[(int)eTmp.RecordTxt].text = $"{BaseManager.userGameData.getLifeTime(BaseManager.userGameData.TimeRecord, false)}";
            }
        }

        void refreshSnowImg()
        {
            mImgs[(int)eImg.snowmanImg].sprite = DataManager.SkinSprite[BaseManager.userGameData.Skin];
        }

        [Button]
        public void getMoney()
        {
            BaseManager.userGameData.Gem += 2000;
            BaseManager.userGameData.Coin += 500000;

            refreshCost();
        }

        [Button]
        public void reset()
        {
            BaseManager.userGameData = new UserGameData();
            AuthManager.instance.SaveDataServer();
            refreshCost();
            setStage();
        }
    }
}
