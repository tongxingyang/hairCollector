using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

namespace week
{
    public class optionComp : MonoBehaviour
    {
        [Header("테스트")]
        [SerializeField] LobbyScene _lobby;
        [Header("windows")]
        [SerializeField] Transform _win;
        [SerializeField] GameObject _developWin;
        [SerializeField] Image _offChange;
        [Header("volume")]
        [SerializeField] Slider _bgmVol;
        [SerializeField] Slider _sfxVol;

        Action _nickChange;
        public Action NickChange { set => _nickChange = value; }

        private void Awake()
        {
            _bgmVol.value = SoundManager.instance.masterVolumeBGM;
            _sfxVol.value = SoundManager.instance.masterVolumeSFX;

            _offChange.color = (AuthManager.instance.networkCheck()) ? Color.grey : Color.white;
            _offChange.raycastTarget = (AuthManager.instance.networkCheck() == false);
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
            Debug.Log(BaseManager.option.BgmVol + " // " + _bgmVol.value);
            BaseManager.instance.saveOption();
            _win.localScale = new Vector3(1f, 0f);

            gameObject.SetActive(false);
        }

        /// <summary> 닉변 창 열기 </summary>
        public void openNickChange()
        {
            _nickChange?.Invoke();
        }
        
        /// <summary> 까페 열기 </summary>
        public void openCafe()
        {
            Application.OpenURL("https://heroesofthestorm.com/ko-kr/");
        }

        public void openCredit()
        {
            _developWin.SetActive(true);
        }

        public void closeCredit()
        {
            _developWin.SetActive(false);
            close();
        }

        public void offLineAccount()
        {
            WindowManager.instance.Win_accountList.open((account) =>
            {
                BaseManager.userGameData.saveDataToLocal();

                AuthManager.instance.Uid = account;
                BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.LobbyScene);
            }, true, true);
        }

        public void ExitGame()
        {
            StartCoroutine(whenQuit());
        }

        IEnumerator whenQuit()
        {
            BaseManager.userGameData.saveDataToLocal();

#if UNITY_EDITOR

#else
            if(AuthManager.instance.networkCheck())
            {            
                yield return StartCoroutine(AuthManager.instance.saveDataToFB());
            }
#endif
            yield return null;
            Application.Quit();
        }

        #endregion

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
        }

        public void getMoney()
        {
            Debug.Log("초기화");
            BaseManager.userGameData.Coin += 10000;
            BaseManager.userGameData.Gem += 500;
            BaseManager.userGameData.Ap += 10;

            BaseManager.userGameData.saveDataToLocal();
        }
    }
}