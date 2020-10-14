using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

            stageImage
        }

        enum eTmp
        {
            CoinTxt,
            GemTxt,
            RecordTxt
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

            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);
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
    }
}
