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
        [Header("windows")]
        //[SerializeField] Transform _win;
        //[SerializeField] Image _offChange;
        [Header("volume")]
        [SerializeField] Slider _bgmVol;
        [SerializeField] Slider _sfxVol;

        [Space]
        LobbyScene _lobby;
        [SerializeField] couponComp _coupon;

        Action _nickChange;

        public void Init(LobbyScene lobby, Action nickChange)
        {
            _bgmVol.value = SoundManager.instance.masterVolumeBGM;
            _sfxVol.value = SoundManager.instance.masterVolumeSFX;

            //_offChange.color = (AuthManager.instance.networkCheck()) ? Color.grey : Color.white;
            //_offChange.raycastTarget = (AuthManager.instance.networkCheck() == false);

            _lobby = lobby;
            _nickChange = nickChange;

            _coupon.Init(lobby, close);
        }

        #region [ windows ]

        /// <summary> 옵션창 오픈 </summary>
        public void open()
        {
            gameObject.SetActive(true);

            //_win.localScale = new Vector3(1f, 0f);
            //_win.DOScaleY(1f, 0.3f).SetEase(Ease.OutBounce);
        }

        /// <summary> 옵션창 닫기 </summary>
        public void close()
        {
            //Debug.Log(BaseManager.option.BgmVol + " // " + _bgmVol.value);
            BaseManager.instance.saveOption();
            //_win.localScale = new Vector3(1f, 0f);

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
            Application.OpenURL("https://cafe.naver.com/snowadventure");
        }

        public void openCredit()
        {
            _coupon.gameObject.SetActive(true);
            // close();
        }

        /// <summary> 오프라인 계정 전환 </summary>
        public void offLineAccount()
        {
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.LobbyScene);
        }

        public void ExitGame()
        {
            StartCoroutine(whenQuit());
        }

        IEnumerator whenQuit()
        {
            // BaseManager.userGameData.saveDataToLocal();

#if UNITY_EDITOR

#else
            yield return StartCoroutine(AuthManager.instance.saveDataToFB());
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
            _lobby.getMoney();

            AuthManager.instance.queryTest_allCheck();

        }
    }
}