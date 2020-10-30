using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace week
{
    public class optionComp : MonoBehaviour
    {
        [Header("테스트")]
        [SerializeField] LobbyScene _lobby;
        [Header("windows")]
        [SerializeField] Transform _win;
        [SerializeField] GameObject _cloud;
        [SerializeField] GameObject _credit;
        [Header("volume")]
        [SerializeField] Slider _bgmVol;
        [SerializeField] Slider _sfxVol;

        [SerializeField] TextMeshProUGUI _LoginTxt;

        private void Awake()
        {
            _bgmVol.value = SoundManager.instance.masterVolumeBGM;
            _sfxVol.value = SoundManager.instance.masterVolumeSFX;

            _cloud.SetActive(false);
            _credit.SetActive(false);

            if (AuthManager.instance.isLogin)
            {
                _LoginTxt.text = "구글 계정 로그아웃";
            }
            else
            {
                _LoginTxt.text = "구글 계정 로그인";
            }
        }

        #region [ windows ]

        /// <summary> 옵션창 오픈 </summary>
        public void open()
        {
            gameObject.SetActive(true);

            _win.localScale = new Vector3(1f, 0f);
            _win.DOScaleY(1f, 0.3f).SetEase(Ease.OutBounce);
        }

        /// <summary> 옵션창 닫기 </summary>
        public void close()
        {
            Debug.Log(BaseManager.userGameData.BgmVol + " // " + _bgmVol.value);
            BaseManager.userGameData.saveDataToLocal();
            _win.localScale = new Vector3(1f, 0f);

            _cloud.SetActive(false);
            _credit.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary> 클라우드 저장 열기 </summary>
        public void openCloud()
        {
            if (AuthManager.instance.isLogin)
            {
                _cloud.SetActive(true);
            }
        }
        
        /// <summary> 클라우드 저장 닫기 </summary>
        public void closeCloud()
        {
            _cloud.SetActive(false);
        }

        /// <summary> 크래딧 열기 </summary>
        public void openCredit()
        {
            _credit.SetActive(true);
        }

        /// <summary> 크래딧 닫기 </summary>
        public void closeCredit()
        {
            _credit.SetActive(false);
            close();
        }

        #endregion

        public void manualLogin()
        {
            //AuthManager.instance.loginSwitch(()=> {
            //    _LoginTxt.text = "구글 계정 로그아웃";
            //}, 
            //() => {
            //    _LoginTxt.text = "구글 계정 로그인";
            //});
        }

        #region [ 볼륨 ]

        public void bgmCtrl(float val)
        {
            SoundManager.instance.SetVolumeBGM(val);
        }
        public void sfxCtrl(float val)
        {
            SoundManager.instance.SetVolumeSFX(val);
        }

        #endregion

        public void statInit()
        {
            Debug.Log("초기화");

            BaseManager.userGameData = new UserGameData();

            //_lobby.refreshCost();

            BaseManager.userGameData.saveDataToLocal();
        }
    }
}