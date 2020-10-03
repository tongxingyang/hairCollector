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
            Deco,
            option
        }

        enum eImg
        {
            optionBtn,

            //hair,
            //eyebrow,
            //beard,
            //cloth,

            stageImage
        }

        enum eTmp
        {
            CoinTxt,
            GemTxt,
            Record
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

        statusComp _status;
        storeComp _store;
        optionComp _option;

        // Start is called before the first frame update
        void Start()
        {
            _status = mGos[(int)eGO.Status].GetComponent<statusComp>();
            _store = mGos[(int)eGO.Store].GetComponent<storeComp>();
            _option = mGos[(int)eGO.option].GetComponent<optionComp>();
            _store.CoinRefresh = refreshCoin;

            MTmps[(int)eTmp.CoinTxt].text = BaseManager.userGameData.Coin.ToString();

            _isLobby = true;
            mGos[(int)eGO.Lobby].SetActive(true);
            mGos[(int)eGO.Store].SetActive(false);
            mGos[(int)eGO.Status].SetActive(false);
            mGos[(int)eGO.Deco].SetActive(false);
            mGos[(int)eGO.option].SetActive(false);
            setStage();

            SoundManager.instance.PlayBGM(BGM.Lobby);
            refreshCoin();

            _snow.OnMasterChanged(0.5f);
            _snow.OnSnowChanged(0.5f);
        }

        public void setEquip(DataTable type)
        {
            int val = (int)type;
        }

        public void refreshCoin()
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
            mGos[(int)eGO.Store].SetActive(true);
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
        }
        public void openStatus()
        {
            _isLobby = false;
            mGos[(int)eGO.Lobby].SetActive(false);
            _status.open();
            mImgs[(int)eImg.optionBtn].sprite = _optionImg[1];
            _status.costRefresh(refreshCoin);
        }
        public void openDeco()
        {
            _isLobby = false;
            mGos[(int)eGO.Lobby].SetActive(false);
            mGos[(int)eGO.Deco].SetActive(true);
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

                mGos[(int)eGO.Store].SetActive(false);
                mGos[(int)eGO.Status].SetActive(false);
                mGos[(int)eGO.Deco].SetActive(false);

                mGos[(int)eGO.Lobby].SetActive(true);
            }
        }

        void setStage()
        {
            MTmps[(int)eTmp.Record].text = $"최고기록 : {BaseManager.instance.convertToTime(BaseManager.userGameData.TimeRecord)}";
            //MTmps[(int)eTmp.st].color = Color.black;
        }
    }
}
