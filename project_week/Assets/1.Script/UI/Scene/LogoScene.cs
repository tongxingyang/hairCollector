using Newtonsoft.Json;
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

            // 윈도우 로드
            WindowManager.instance.LoadWin();
            BaseManager.instance.SceneLoadStart = WindowManager.instance.Win_loading.open;
            BaseManager.instance.SceneLoadComplete = WindowManager.instance.Win_loading.close;
            gauge += 0.5f;

            // 리소스 폴더에서 프리팹 로드
            DataManager.loadPrefabs();
            gauge += 0.15f;

            // 사운드매니저 로드
            BaseManager.instance.loadOption();

            BaseManager.instance.GetComponent<SoundManager>().startSound();
            BaseManager.instance.GetComponent<touchManager>().Init();
            SoundManager.instance.PlayBGM(BGM.Lobby);
            gauge += 0.05f;

            BaseManager.userGameData = new UserGameData();
#if UNITY_EDITOR
            if (ES3.KeyExists(AuthManager.instance.Uid)) // 기본 유저 데이터 존재
            {
                // ES3.DeleteKey(AuthManager.instance.Uid);
            }
            if (ES3.KeyExists(gameValues._deviceListKey))
            {
                // ES3.DeleteKey(gameValues._deviceListKey);
            }

            // yield return StartCoroutine(userDataAfterNetChk());
            //BaseManager.userGameData.saveDataToLocal(); // 기기저장
#else
            // 인터넷 - 데이터 체크 
            yield return StartCoroutine(userDataAfterNetChk());
            gauge += 0.15f;
#endif  

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
            if (AuthManager.instance.networkCheck()) // 인터넷 연결
            {                
                AuthManager.instance.Login(); // 구글 로그인 ~> 파베 로그인
                AdManager.instance.adStart(); // 광고 초기화
                yield return new WaitUntil(() => AuthManager.instance.isLoginFb == true); // [대기] 파베 로그인
                
                yield return StartCoroutine(AuthManager.instance.loadVersionFromFB());  // [대기] 버전

                if (AuthManager.instance.loadMatchingData()) // 기기 (uid)매칭 데이터 로드
                {
                    Debug.Log("인터넷 연결 : 기기에 유저 데이터 있음");

                    yield return StartCoroutine(AuthManager.instance.chkExistData()); // [대기] 서버 데이터 유무 체크

                    bool result = AuthManager.instance.IsExist;
                    if (result)
                    {
                        Debug.Log("서버에 데이터 있음");
                        
                        yield return StartCoroutine(AuthManager.instance.loadUniqueNumDate()); // [대기] 계정 고유번호

                        result = (BaseManager.userGameData.UniqueNumber == AuthManager.instance.UniqueNum);
                        if (result) // 같은 데이터
                        {
                            Debug.Log("같은 데이터임(시작날짜기준)");
                            
                            yield return StartCoroutine(AuthManager.instance.loadLastSaveDate()); // [대기] 서버에 저장된 마지막 저장 날짜

                            if (AuthManager.instance.LoadedLastSave == BaseManager.userGameData.LastSave) // 일치
                            {
                                Debug.Log("정상 작동");
                                // ok.
                            }
                            else // 내용 다르면 기기껄로
                            {
                                Debug.Log("기기->서버 업로드");
                                yield return StartCoroutine(AuthManager.instance.saveDataToFB()); // [대기] 업로드
                            }
                        }
                        else // 이거 데이터 다른데??---------------------------
                        {
                            Debug.Log("서버랑 기기 데이터 고유번호 비일치");
                            Debug.Log(AuthManager.instance.UniqueNum + " == " + BaseManager.userGameData.UniqueNumber);

                            string removekey = "";
                            result = false;

                            WindowManager.instance.Win_accountList.open((string selectKey)=> {
                                removekey = selectKey;
                                result = true;
                            }, false); // 교체할 슬롯 선택

                            yield return new WaitUntil(() => result == true); // [대기] 고를때까지 기다려주기
                        }
                    }
                    else // 기기에는 데이터 있는데 서버에는 데이터 없어??
                    {
                        Debug.Log("기기 데이터 : true / 서버 데이터 : false");
                        Debug.Log("기기->서버 업로드");
                        yield return StartCoroutine(AuthManager.instance.saveDataToFB()); // [대기] 업로드
                    }
                }
                else // 기기에 유저 데이터 없음
                {
                    yield return StartCoroutine(AuthManager.instance.chkExistData()); // [대기] 서버 데이터 유무 체크

                    bool result = AuthManager.instance.IsExist;
                    if (result)
                    {
                        Debug.Log("기기 데이터 : false / 서버 데이터 : true");

                        string removekey = "";
                        result = false;

                        WindowManager.instance.Win_accountList.open((string selectKey) => {
                            removekey = selectKey;
                            result = true;
                        }, false); // 교체할 슬롯 선택

                        yield return new WaitUntil(() => result == true); // [대기] 고를때까지 기다려주기
                        yield return StartCoroutine(AuthManager.instance.loadAndChangeDataFromFB(removekey)); // [대기] 데이터 로드 및 교체
                    }
                    else
                    {
                        Debug.Log("저는 이 게임 처음입니다.");

                        BaseManager.userGameData = new UserGameData(); // 만들고
                        AuthManager.instance.removeAndSaveDataToLocal(); // 데이터 리스트 저장
                        AuthManager.instance.AllSaveUserEntity(); // 기기, 서버 저장
                    }
                }
            }
            else // 인터넷 연결해제
            {
                if (AuthManager.instance.showDataInDevice()) // 기본 유저 데이터 존재
                {
                    Debug.Log("인터넷 연결해제 : 기기에 유저 데이터 있음");

                    bool result = false;

                    WindowManager.instance.Win_accountList.open((string selectKey) => {
                        AuthManager.instance.Uid = selectKey;
                        result = true;
                    }); // 플레이 할 슬롯 선택

                    yield return new WaitUntil(() => result == true); // [대기] 고를때까지 기다려주기

                    string offData = ES3.Load<string>(AuthManager.instance.Uid);
                    UserEntity _uet = JsonConvert.DeserializeObject<UserEntity>(offData, new ObscuredValueConverter());
                    BaseManager.userGameData.loadDataFromLocal(_uet);
                }
                else // 기본 유저 데이터 없음
                {
                    Debug.Log("인터넷 연결해제 : 기기에 유저 데이터 없음");
                    BaseManager.userGameData = new UserGameData();
                    BaseManager.userGameData.saveDataToLocal(); // 기기저장
                    AuthManager.instance.removeAndSaveDataToLocal(); // 테이터 리스트 저장
                }
            }

            gauge += 0.15f;

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
            //selectAccount = true;
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
            //selectAccount = true;
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