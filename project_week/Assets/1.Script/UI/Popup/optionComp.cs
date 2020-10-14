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
        [SerializeField] LobbyScene _lobby;
        [SerializeField] Transform _win;

        [SerializeField] Slider _bgmVol;
        [SerializeField] Slider _sfxVol;

        [SerializeField] GameObject cloudWin;
        [SerializeField] GameObject developWin;
        [SerializeField] TextMeshProUGUI _LoginTxt;

        private void Awake()
        {
            _bgmVol.value = SoundManager.instance.masterVolumeBGM;
            _sfxVol.value = SoundManager.instance.masterVolumeSFX;
            cloudWin.SetActive(false);

            if (googleManager.instance.isLogin)
            {
                _LoginTxt.text = "구글 계정 로그아웃";
            }
            else
            {
                _LoginTxt.text = "구글 계정 로그인";
            }
        }

        public void statInit()
        {
            Debug.Log("초기화");

            BaseManager.userGameData = new UserGameData();

            _lobby.refreshCost();

            BaseManager.userGameData.saveUserEntity();
        }

        public void developers()
        {
            developWin.SetActive(true);
        }

        public void manualLogin()
        {
            googleManager.instance.loginSwitch(()=> {
                _LoginTxt.text = "구글 계정 로그아웃";
            }, 
            () => {
                _LoginTxt.text = "구글 계정 로그인";
            });
        }

        public void bgmCtrl(float val)
        {
            SoundManager.instance.SetVolumeBGM(val);
        }
        public void sfxCtrl(float val)
        {
            SoundManager.instance.SetVolumeSFX(val);
        }

        public void openCloud()
        {
            if (googleManager.instance.isLogin)
            {
                cloudWin.SetActive(true);
            }
        }
        public void closeCloud()
        {
            cloudWin.SetActive(false);
        }

        public void open()
        {
            gameObject.SetActive(true);

            _win.localScale = new Vector3(1f, 0f);
            _win.DOScaleY(1f, 0.3f).SetEase(Ease.OutBounce);
        }

        public void close()
        {
            Debug.Log(BaseManager.userGameData.BgmVol + " // " + _bgmVol.value);
            BaseManager.userGameData.saveUserEntity();
            _win.localScale = new Vector3(1f, 0f);

            developWin.SetActive(false);
            cloudWin.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}