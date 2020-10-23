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
            option
        }

        enum eImg
        {
            optionBtn,

            snowmanImg
        }

        enum eTmp
        {
            CoinTxt,
            GemTxt,
            RecordTxt,
            nickName
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

        optionComp _option;

        // Start is called before the first frame update
        void Start()
        {
            _store = mGos[(int)eGO.Store].GetComponent<storeComp>();
            _status = mGos[(int)eGO.Status].GetComponent<statusComp>();
            _skin = mGos[(int)eGO.Skin].GetComponent<skinComp>();
            _skin.Init(refreshCost);
            _quest = mGos[(int)eGO.Quest].GetComponent<questComp>();

            _option = mGos[(int)eGO.option].GetComponent<optionComp>();

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.Coin.ToString();

            _isLobby = true;
            mGos[(int)eGO.Lobby].SetActive(true);
            mGos[(int)eGO.Store].SetActive(false);
            mGos[(int)eGO.Status].SetActive(false);
            mGos[(int)eGO.Skin].SetActive(false);
            mGos[(int)eGO.Quest].SetActive(false);
            mGos[(int)eGO.option].SetActive(false);
            setStage();

            SoundManager.instance.PlayBGM(BGM.Lobby);
            refreshCost();

            refreshSnowImg();

            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);
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

        public void refreshCost()
        {
            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.Coin.ToString();
            MTmps[(int)eTmp.GemTxt].text = BaseManager.userGameData.Gem.ToString();
        }

        public void PlayGame()
        {
            SoundManager.instance.StopBGM();
            BaseManager.userGameData.saveUserEntity();
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
        }

        public void openStore()
        {
            _isLobby = false;
            mGos[(int)eGO.Lobby].SetActive(false);

            _store.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];

            _store.costRefresh(refreshCost);
        }
        public void openStatus()
        {
            _isLobby = false;
            mGos[(int)eGO.Lobby].SetActive(false);

            _status.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];

            _status.costRefresh(refreshCost);
        }
        public void openSkin()
        {
            _isLobby = false;
            mGos[(int)eGO.Lobby].SetActive(false);

            _skin.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
        }
        public void openQuest()
        {
            _isLobby = false;

            _quest.open();

            _quest.costRefresh(refreshCost);
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

                mGos[(int)eGO.Store].SetActive(false);
                mGos[(int)eGO.Status].SetActive(false);
                mGos[(int)eGO.Skin].SetActive(false);
                mGos[(int)eGO.Quest].SetActive(false);

                mGos[(int)eGO.Lobby].SetActive(true);
                refreshSnowImg();
            }
        }

        void setStage()
        {
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
