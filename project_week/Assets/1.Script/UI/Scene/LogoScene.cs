using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class LogoScene : UIBase
    {
        #region

        enum E_IMAGE
        {
            TeamLogo,
            GameLogo,
                        
            Fill,

            pressButton
        }

        protected override Enum GetEnumImage() { return new E_IMAGE(); }

        #endregion

        public bool isDataLoadSuccess { get; private set; }
        float time = 0f;
        float gauge = 0f;

        // Start is called before the first frame update
        void Start()
        {
            isDataLoadSuccess = false;
            mImgs[(int)E_IMAGE.Fill].fillAmount = 0f;
            mImgs[(int)E_IMAGE.pressButton].gameObject.SetActive(false);
            mImgs[(int)E_IMAGE.Fill].gameObject.SetActive(true);
            StartCoroutine(showLogo());
        }

        private void Update()
        {            
            if (gauge > time)
            {
                time += Time.deltaTime;
                mImgs[(int)E_IMAGE.Fill].fillAmount += Time.deltaTime;
            }
        }

        #region 로고 오프닝

        /// <summary> 로고 오프닝 (로그인) </summary>
        IEnumerator showLogo()
        {
            // 로고 보여주는 부분
            mImgs[(int)E_IMAGE.TeamLogo].gameObject.SetActive(true);
            mImgs[(int)E_IMAGE.GameLogo].gameObject.SetActive(false);
            
            yield return new WaitForSeconds(1.5f);

            // 오프닝 화면과 데이터 로드
            mImgs[(int)E_IMAGE.TeamLogo].gameObject.SetActive(false);
            mImgs[(int)E_IMAGE.GameLogo].gameObject.SetActive(true);

            yield return StartCoroutine(dataLoad());

            yield return new WaitUntil(()=> mImgs[(int)E_IMAGE.Fill].fillAmount == 1f);
            LoadComplete();
        }

        IEnumerator dataLoad()
        {
            // BG 로드
            bool result = DataManager.LoadBGdata();
            if (result)
            {
                isDataLoadSuccess = true;
            }
            else
                Debug.LogError("앱 데이터 로드 에러");

            gauge += 0.15f;

            yield return new WaitForEndOfFrame();

            // 사전 게임 데이터 제작 [bg필요?]
            BaseManager.PreGameData = new PreGameData();
            gauge += 0.2f;

            // 구글 관련
            AuthManager.instance.Login();
            AdManager.instance.adStart();
            gauge += 0.15f;
#if UNITY_EDITOR

#else
            yield return new WaitUntil(() => AuthManager.instance.isLoginFb == true);
#endif

            //저장된 유저 데이터 로드
            if (ES3.KeyExists("userEntity"))
            {
                ES3.DeleteKey("userEntity");
            }

            if (ES3.KeyExists("userEntity"))
            {
                Debug.Log("기본 유저 데이터 존재");
                BaseManager.userGameData.LoadUserEntity(ES3.Load<UserEntity>("userEntity"));
            }
            else
            {
                Debug.Log("기본 유저 데이터가 없으므로 제작함");
                BaseManager.userGameData = new UserGameData();
            }

            BaseManager.userGameData.flashData();
            gauge += 0.15f;

            // 사운드매니저 로드
            BaseManager.instance.GetComponent<SoundManager>().startSound();
            BaseManager.instance.GetComponent<touchManager>().Init();
            SoundManager.instance.PlayBGM(BGM.Lobby);
            gauge += 0.05f;

            // 리소스 폴더에서 프리팹 로드
            DataManager.loadPrefabs();
            gauge += 0.15f;

            // 윈도우 로드
            WindowManager.instance.LoadWin();
            BaseManager.instance.SceneLoadStart = WindowManager.instance.Win_loading.open;
            BaseManager.instance.SceneLoadComplete = WindowManager.instance.Win_loading.close;

            gauge += 0.15f;

            // 여기 끝난거 체크해서 다음 씬 진행
        }

#endregion

        void LoadingBar()
        {
            mImgs[(int)E_IMAGE.Fill].fillAmount = gauge;
        }

        void LoadComplete()
        {
            mImgs[(int)E_IMAGE.pressButton].gameObject.SetActive(true);
            mImgs[(int)E_IMAGE.Fill].gameObject.SetActive(false);
        }

        public void StartGame()
        {
            BaseManager.instance.convertScene(SceneNum.LogoScene.ToString(), SceneNum.LobbyScene);
        }

#region 네트워크 안내판

        ///// <summary> 네트워크 연결 안내문 표시 </summary>
        //public void popNetException()
        //{
        //    m_pkImages[(int)E_IMAGE.networkExcep].gameObject.SetActive(true);
        //}

        ///// <summary> 접속 재시도 </summary>
        //public void reConnect()
        //{
        //    m_pkImages[(int)E_IMAGE.networkExcep].gameObject.SetActive(false);

        //    StartCoroutine(showLogo());
        //}

        /// <summary> 종료 </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

#endregion
    }
}