using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace week
{
    public class LobbyScene : UIBase
    {
        #region [UIBase]
        enum eGO
        {
            Lobby,
            Store,
            Status,
            Skin,
            Quest,
            Rank,
            option,
            nicChangePanel,
            updatePanel
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

        [SerializeField] Sprite[] _optionImg;
        [SerializeField] SnowController _snow;
        [SerializeField] Transform _angle;
        [SerializeField] RawImage _pattern;
        Vector2 _offset;
        readonly Vector2 _rect = new Vector2(1f, 2f);

        bool _isLobby;

        storeComp _store;
        statusComp _status;
        skinComp _skin;
        questComp _quest;
        rankComp _rank;

        optionComp _option;
        nickChangePopup _nickPanel;

        public Transform CoinTxt { get { return mImgs[(int)eImg.Coin].transform; } }
        public Transform GemTxt { get { return mImgs[(int)eImg.Gem].transform; } }

        bool _refreshCoinChk;
        bool _refreshGemChk;

        // Start is called before the first frame update
        void Start()
        {
            if (BaseManager.userGameData.followCoin == 0)
            {
                BaseManager.userGameData.followCoin = BaseManager.userGameData.Coin;
                BaseManager.userGameData.followGem = BaseManager.userGameData.Gem;
            }

            _store = mGos[(int)eGO.Store].GetComponent<storeComp>();
            _status = mGos[(int)eGO.Status].GetComponent<statusComp>();
            _status.CostRefresh = refreshCost;
            _skin = mGos[(int)eGO.Skin].GetComponent<skinComp>();
            _skin.Init();
            _quest = mGos[(int)eGO.Quest].GetComponent<questComp>();
            _rank = mGos[(int)eGO.Rank].GetComponent<rankComp>();
            _rank.Init();

            _option = mGos[(int)eGO.option].GetComponent<optionComp>();
            _nickPanel = mGos[(int)eGO.nicChangePanel].GetComponent<nickChangePopup>();
            _nickPanel.completeChange = () => { setStage(); refreshCost(); };

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.followCoin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.followGem.ToString();

            _isLobby = true;
            mGos[(int)eGO.Lobby].SetActive(true);
            mGos[(int)eGO.Store].SetActive(false);
            mGos[(int)eGO.Status].SetActive(false);
            mGos[(int)eGO.Skin].SetActive(false);
            mGos[(int)eGO.Quest].SetActive(false);
            mGos[(int)eGO.Rank].SetActive(false);
            mGos[(int)eGO.option].SetActive(false);
            mGos[(int)eGO.nicChangePanel].SetActive(false);
            mGos[(int)eGO.updatePanel].SetActive(false);
            setStage();

            SoundManager.instance.PlayBGM(BGM.Lobby);

            refreshSnowImg();

            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);

            WindowManager.instance.Win_coinGenerator.RefreshFollowCost = refreshFollowCost;

            if (BaseManager.userGameData.GameReward != null)
            {
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, CoinTxt.position, currency.coin, BaseManager.userGameData.GameReward[0], 1);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, GemTxt.position, currency.gem, BaseManager.userGameData.GameReward[1], 2);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.stageImage].transform.position, CoinTxt.position, currency.ap, BaseManager.userGameData.GameReward[2], 3);

                BaseManager.userGameData.GameReward = null;
            }

            if (BaseManager.userGameData.FreeNichkChange == false) 
            {
                _nickPanel.open();
            }
            if (BaseManager.userGameData.IsSavedServer == false)
            {
                AuthManager.instance.AllSaveUserEntity();
            }
            if(AuthManager.instance.LastVersion > gameValues._version)
            {
                openUpdate();    
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
            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.Coin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.Gem.ToString();
        }

        public void PlayGame()
        {
            SoundManager.instance.StopBGM();
            //BaseManager.userGameData.saveDataToLocal();
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
        }

        public void openStore()
        {
            _isLobby = false;
            allClose(false);

            _store.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
        }
        public void openStatus()
        {
            _isLobby = false;
            allClose(false);

            _status.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
        }
        public void openSkin()
        {
            _isLobby = false;
            allClose(false);

            _skin.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
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
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
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
                mImgs[(int)eImg.optionBtn].sprite = _optionImg[0];

                allClose(true);
                refreshSnowImg();
            }
        }

        public void openNick()
        {
            _nickPanel.open();
        }

        public void openUpdate()
        {
            MTmps[(int)eTmp.RecordTxt].text = $"{AuthManager.instance.LastVersion}.버전이 출시되었습니다." + System.Environment.NewLine + "업데이트 해주세요.";
            mGos[(int)eGO.updatePanel].SetActive(true);
        }

        public void connectDownLoadURL()
        { 
        
        }

        void allClose(bool lobby)
        {
            mGos[(int)eGO.Lobby].SetActive(lobby);

            mGos[(int)eGO.Store].SetActive(false);
            mGos[(int)eGO.Status].SetActive(false);
            mGos[(int)eGO.Skin].SetActive(false);
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
            //MTmps[(int)eTmp.st].color = Color.black;
        }

        void refreshSnowImg()
        {
            mImgs[(int)eImg.snowmanImg].sprite = DataManager.SkinSprite[BaseManager.userGameData.Skin];
        }
    }
}
