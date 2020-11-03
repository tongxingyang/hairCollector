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
            Accountchk,
                        
            Fill,

            pressButton
        }

        protected override Enum GetEnumImage() { return new E_IMAGE(); }

        #endregion

        public bool isDataLoadSuccess { get; private set; }
        float time = 0f;
        float gauge = 0f;
        bool selectAccount;

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
            mImgs[(int)E_IMAGE.Accountchk].gameObject.SetActive(false);
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

#if UNITY_EDITOR
            if (ES3.KeyExists("userEntity")) // 기본 유저 데이터 존재
            {
                ES3.DeleteKey("userEntity");
            }
            AuthManager.instance.firebaseDatabaseEventInit(); // 지워
            StartCoroutine(AuthManager.instance.loadVersionFromFB()); // 지워
            BaseManager.userGameData = new UserGameData(); // 만들고
            BaseManager.userGameData.saveDataToLocal(); // 기기저장
#else
            // 인터넷 - 데이터 체크 
            yield return StartCoroutine(userDataAfterNetChk());            
            gauge += 0.15f;
#endif


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

            gauge += 0.5f;

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

        IEnumerator userDataAfterNetChk()
        {
            if (true || AuthManager.instance.networkCheck()) // 인터넷 연결
            {
                // 구글 관련
                AuthManager.instance.Login();
                AdManager.instance.adStart();

                yield return new WaitUntil(() => AuthManager.instance.isLoginFb == true);
                yield return StartCoroutine(AuthManager.instance.loadVersionFromFB());  // 버전 가져오기
                yield return StartCoroutine(AuthManager.instance.getTimestamp());       // 서버시간 가져오기

                if (ES3.KeyExists("userEntity")) // 기본 유저 데이터 존재
                {
                    //ES3.DeleteKey("userEntity");
                    //BaseManager.userGameData = new UserGameData(); // 만들고
                    //BaseManager.userGameData.saveDataToLocal(); // 기기저장

                    Debug.Log("인터넷 연결 : 기기에 유저 데이터 있음");
                    
                    // 기기에 저장된 데이터 가져오기
                    BaseManager.userGameData.loadDataFromLocal(ES3.Load<UserEntity>("userEntity"));

                    // 서버에 데이터 있는지 체크
                    yield return StartCoroutine(AuthManager.instance.chkExistData());

                    bool result = AuthManager.instance.IsExist;
                    if (result)
                    {
                        Debug.Log("서버에 데이터 있음");
                        // 서버에 저장된 최초가입 날짜 가져옴
                        yield return StartCoroutine(AuthManager.instance.loadFirstJoinDate());

                        result = (BaseManager.userGameData.Join == AuthManager.instance.LoadedFirstJoin);
                        if (result) // 같은 데이터
                        {
                            Debug.Log("같은 데이터임(시작날짜기준)");
                            // 서버에 저장된 마지막 저장 날짜 가져옴
                            yield return StartCoroutine(AuthManager.instance.loadLastSaveDate());

                            if (AuthManager.instance.LoadedLastSave == BaseManager.userGameData.LastSave) // 같음
                            {
                                Debug.Log("정상 작동");
                                // ok.
                            }
                            else // 내용 다르면 기기껄로
                            {
                                Debug.Log("기기꺼");
                                yield return StartCoroutine(AuthManager.instance.saveDataToFB()); // 기기 내용 그대로 서버로
                            }
                        }
                        else // 이거 데이터 다른데??
                        {
                            Debug.Log("서버랑 기기 데이터 상이(시작날짜기준)");
                            selectAccount = false;
                            accountException();

                            yield return new WaitUntil(() => selectAccount == true);
                        }
                    }
                    else // 기기에는 데이터 있는데 서버에는 데이터 없어??
                    {
                        Debug.Log("기기에는 데이터 있는데 서버에 데이터 없음");
                        yield return StartCoroutine(AuthManager.instance.saveDataToFB()); // 서버저장
                    }
                }
                else // 기기에 유저 데이터 없음
                {
                    Debug.Log("인터넷 연결 : 기기에 유저 데이터 없음");
                    // 서버에 데이터 있는지 체크
                    yield return StartCoroutine(AuthManager.instance.chkExistData());

                    bool result = AuthManager.instance.IsExist;
                    if (result)
                    {
                        Debug.Log("기기에는 없으나 서버에는 있음");
                        yield return StartCoroutine(AuthManager.instance.loadDataFromFB());
                        BaseManager.userGameData.saveDataToLocal(); // 서버 -> 기기 저장
                    }
                    else
                    {
                        Debug.Log("저는 이 게임 처음입니다.");
                        BaseManager.userGameData = new UserGameData(); // 만들고
                        AuthManager.instance.AllSaveUserEntity(); // 다 저장
                    }
                }
            }
            else // 인터넷 연결해제
            {
                if (ES3.KeyExists("userEntity")) // 기본 유저 데이터 존재
                {
                    Debug.Log("인터넷 연결해제 : 기기에 유저 데이터 있음");
                    BaseManager.userGameData.loadDataFromLocal(ES3.Load<UserEntity>("userEntity"));
                }
                else // 기본 유저 데이터 없음
                {
                    Debug.Log("인터넷 연결해제 : 기기에 유저 데이터 없음");
                    BaseManager.userGameData = new UserGameData();
                    BaseManager.userGameData.saveDataToLocal(); // 기기저장
                }
            }

            gauge += 0.15f;

            ////저장된 유저 데이터 로드
            //if (ES3.KeyExists("userEntity"))
            //{
            //    ES3.DeleteKey("userEntity");
            //}

            BaseManager.userGameData.flashData();
            Debug.Log("데이터 완료");
        }

        /// <summary> 계정 문제 창 </summary>
        public void accountException()
        {
            mImgs[(int)E_IMAGE.Accountchk].gameObject.SetActive(true);
        }

        /// <summary> 구글 계정선택 </summary>
        public void selectGoogleAccount()
        {
            StartCoroutine(getGoogleAccount());
        }

        IEnumerator getGoogleAccount()
        {
            yield return StartCoroutine(AuthManager.instance.loadDataFromFB());

            mImgs[(int)E_IMAGE.Accountchk].gameObject.SetActive(false);
            selectAccount = true;
        }

        /// <summary> 기기 계정선택 </summary>
        public void selectPhoneAccount()
        {
            StartCoroutine(getPhoneAccount());
        }

        IEnumerator getPhoneAccount()
        {
            yield return StartCoroutine(AuthManager.instance.saveDataToFB());

            mImgs[(int)E_IMAGE.Accountchk].gameObject.SetActive(false);
            selectAccount = true;
        }

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