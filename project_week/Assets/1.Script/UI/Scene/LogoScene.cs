using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

namespace week
{
    public class LogoScene : UIBase
    {
        #region

        enum E_IMAGE
        {
            TeamLogo,
            GameLogo,

            pressButton
        }

        protected override Enum GetEnumImage() { return new E_IMAGE(); }

        #endregion

        [SerializeField] NotificationPopup _notif;
        [SerializeField] TextMeshProUGUI _load;
        [SerializeField] Slider _fill;
        
        Canvas _canvas;

        public bool ConnectComplete { get; private set; }
        float time = 0f;
        float gauge = 0f;

        // Start is called before the first frame update
        void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main;
            _canvas.planeDistance = 1400f;

            _fill.value = 0f;
            mImgs[(int)E_IMAGE.pressButton].gameObject.SetActive(false);
            _fill.gameObject.SetActive(true);
            _notif.gameObject.SetActive(false);

            StartCoroutine(showLogo());
        }

        private void Update()
        {            
            if (gauge > time)
            {
                time += Time.deltaTime;
                _fill.value += gauge;
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

            yield return new WaitUntil(()=> ConnectComplete && _fill.value == 1f);
            LoadComplete();
        }

        IEnumerator dataLoad()
        {
            // 버전 체크
            string[] lines = Application.version.Split('.');

            // BG 로드
            _load.text = "눈내리는 중..";
            bool result = DataManager.LoadBGdata();
            if (result)
            {
            }
            else
                Debug.LogError("앱 데이터 로드 에러");

            gauge += 0.1f;

            yield return new WaitForEndOfFrame();

            // 사전 게임 데이터 제작 [bg필요?]
            _load.text = "눈 쌓이는 중..";
            BaseManager.PreGameData = new PreGameData();
            gauge += 0.2f;

            // 윈도우 로드
            _load.text = "눈 모으는 중..";
            WindowManager.instance.LoadWin();
            BaseManager.instance.SceneLoadStart = WindowManager.instance.Win_loading.open;
            BaseManager.instance.SceneLoadComplete = WindowManager.instance.Win_loading.close;
            gauge += 0.2f;

            // 리소스 폴더에서 프리팹 로드
            _load.text = "눈 뭉치는 중..";
            DataManager.loadPrefabs();
            gauge += 0.15f;

            // 사운드매니저 로드
            _load.text = "뽀득뽀득..";
            BaseManager.instance.loadOption();

            BaseManager.instance.GetComponent<SoundManager>().startSound();
            BaseManager.instance.GetComponent<touchManager>().Init();
            SoundManager.instance.PlayBGM(BGM.Lobby);
            gauge += 0.1f;

            ConnectComplete = false;
            BaseManager.userGameData = new UserGameData();
#if UNITY_EDITOR

            if (AuthManager.instance.networkCheck())
            {
                AuthManager.instance.LoginWithEmail();

                yield return new WaitUntil(() => AuthManager.instance.isLoginFb == true); // [대기] 파베 로그인

                yield return StartCoroutine(AuthManager.instance.chkExistData()); // [대기] 서버 데이터 유무 체크

                result = AuthManager.instance.IsExist;
                if (result) // 서버 o
                {
                    _load.text = "서버데이터 확인중";

                    yield return StartCoroutine(AuthManager.instance.loadDataFromFB()); // [대기] 서버에서 데이터 가져오기
                }
                else // 서버 x -> 신규
                {
                    _load.text = "뉴비 체크완료";

                    BaseManager.userGameData = new UserGameData(); // 만들고
                                                                   // AuthManager.instance.SaveDataServer(); // 기기, 서버 저장
                    yield return StartCoroutine(AuthManager.instance.saveDataToFB(true));
                }

                BaseManager.instance.KeyRandomizing();
            }
            else
            {
                BaseManager.userGameData = new UserGameData().setTest();
            }

            ConnectComplete = true;
#else
            // 인터넷 - 데이터 체크 
            yield return StartCoroutine(userDataAfterNetChk());
            gauge += 0.1f;
#endif
            if (ConnectComplete == false)
            {
                yield break;
            }

            NanooManager.instance.setUid(AuthManager.instance.Uid); // 나누 접속
            //AnalyticsManager.instance.AnalyticsLogin(AuthManager.instance.Uid); // 애널리스틱

            // 여기 끝난거 체크해서 다음 씬 진행

            yield return new WaitUntil(() => ConnectComplete == true);

            readyLobby();

            gauge = 1f;
        }

#endregion

        void LoadingBar()
        {
            _fill.value = gauge;
        }

        void LoadComplete()
        {
            mImgs[(int)E_IMAGE.pressButton].gameObject.SetActive(true);
            // _fill.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            BaseManager.instance.convertScene(SceneNum.LogoScene.ToString(), SceneNum.LobbyScene);
        }

        #region 네트워크 안내판

        IEnumerator userDataAfterNetChk()
        {
            _load.text = "네트워크 연결체크";
            if (AuthManager.instance.networkCheck()) // 인터넷 연결
            {
                _load.text = "구글 로그인";
                AuthManager.instance.Login(); // 구글 로그인 ~> 파베 로그인

                _load.text = "AD 초기화";
                AdManager.instance.adStart(); // 광고 초기화
                yield return new WaitUntil(() => AuthManager.instance.isLoginFb == true); // [대기] 파베 로그인

                _load.text = "버전 확인";
                yield return StartCoroutine(AuthManager.instance.loadVersionFromFB());  // [대기] 버전 체크
                {
                    string[] releaseVersion = AuthManager.instance.Version.Split('.');
                    string[] installVersion = Application.version.Split('.');
                    Debug.Log(AuthManager.instance.Version + " // " + Application.version);

                    for (int i = 0; i < 3; i++)
                    {
                        int r_num = int.Parse(releaseVersion[i]);
                        int i_num = int.Parse(installVersion[i]);

                        if (r_num > i_num)
                        {
                            if (i < 2)
                            {
                                _notif.newVersionChker();
                                ConnectComplete = false;
                                yield break;
                            }
                            else
                            {
                                BaseManager.NeedPatch = true;
                            }
                        }
                    }
                }

                yield return StartCoroutine(AuthManager.instance.chkExistData()); // [대기] 서버 데이터 유무 체크

                bool result = AuthManager.instance.IsExist;
                if (result) // 서버 o
                {
                    _load.text = "서버데이터 확인중";

                    yield return StartCoroutine(AuthManager.instance.loadDataFromFB()); // [대기] 서버에서 데이터 가져오기
                }
                else // 서버 x -> 신규
                {
                    _load.text = "뉴비 체크완료";

                    BaseManager.userGameData = new UserGameData(); // 만들고
                    // AuthManager.instance.SaveDataServer(); // 기기, 서버 저장
                    yield return StartCoroutine(AuthManager.instance.saveDataToFB(true));
                }
            }
            else // 인터넷 연결해제
            {
                
                if (ES3.KeyExists(gameValues._offlineKey)) // 기본 오프라인 데이터 있음
                {
                    _load.text = "오프라인 : 기기 데이터 확인";
                    BaseManager.userGameData.loadOffLineData(); // 오프라인 데이터 로드                    

                    //bool result = false;
                    //WindowManager.instance.Win_accountList.open((string selectKey) =>
                    //{
                    //    AuthManager.instance.Uid = selectKey;
                    //    result = true;
                    //}); // 플레이 할 슬롯 선택

                    //yield return new WaitUntil(() => result == true); // [대기] 고를때까지 기다려주기

                    //string offData = ES3.Load<string>(AuthManager.instance.Uid);
                    //UserEntity _uet = JsonConvert.DeserializeObject<UserEntity>(offData, new ObscuredValueConverter());
                    //BaseManager.userGameData.loadDataFromLocal(_uet);
                }
                else // 기본 오프라인 데이터 없음
                {
                    _load.text = "오프라인 : 오프라인 뉴비";

                    BaseManager.userGameData = new UserGameData();  // 새 오프라인 데이터 생성
                    BaseManager.userGameData.saveOffLineData();     // 오프라인 데이터 저장
                }
            }

            gauge += 0.1f;

            BaseManager.userGameData.flashData();
            BaseManager.instance.KeyRandomizing();
            //Debug.Log("데이터 완료");
            _load.text = "데이터 로드 완료";
            ConnectComplete = true;
        }

        /// <summary> 로비가기전에 해결할일 </summary>
        public void readyLobby()
        {
            // 시즌랭킹코드가 없거나 다르면 새걸로 세팅
            if (string.IsNullOrEmpty(BaseManager.userGameData.NowSeasonRankKey) ||
                BaseManager.userGameData.NowSeasonRankKey.Equals(NanooManager.instance.getRANK_CODE) == false)
            {
                BaseManager.userGameData.whenRecordNewSeason();
            }
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