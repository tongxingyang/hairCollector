using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using Firebase.Database;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CodeStage.AntiCheat.ObscuredTypes;
using NaughtyAttributes;

namespace week
{
    [Serializable]
    public class rankSubData
    {
        [SerializeField] public string _version;
        //[SerializeField] public string _nick;
        //[SerializeField] public int _time;
        //[SerializeField] public int _boss;
        [SerializeField] public int _skin;

        public rankSubData() { }
        public rankSubData(int skin)
        {
            _version = Application.version;
            //_nick = BaseManager.userGameData.NickName;
            //_time = time;
            //_boss = boss;
            _skin = skin;
        }
    }

    [Serializable]
    public class Profile
    {
        [SerializeField] public bool inspection;
        [SerializeField] public string version;

        public Profile()
        {
            inspection = true;
            version = "9.9.9";
        }
    }

    public class AuthManager : TSingleton<AuthManager>
    {
        FirebaseAuth auth;
        DatabaseReference reference 
        {
            get { return FirebaseDatabase.DefaultInstance.RootReference; }
        }
        //DatabaseReference serverTime
        //{
        //    get { return reference.Child("Profile").Child("serverTime"); }
        //}

        ObscuredString _uid = "SinglePlay";

        Profile _profile;
        // long _lastLogin;
        List<rankSubData> _leaders;
        public Action<int> WhenTomorrow { get; set; }
        bool _succesGetTime;

        public bool IsExist { get; set; }
        public long LoadedLastSave { get; set; }
        
        public bool isLogin
        {
            get { return Social.localUser.authenticated; }
        }

        public bool isLoginFb { get; set; }
        public List<rankSubData> Leaders { get => _leaders; }
        public Profile profile { get => _profile; }
        // public long LastLogin { get => _lastLogin; }
        public ObscuredString Uid { get => _uid; set => _uid = value; }

        public string chkTest()
        {
            return "췍";
        }

        /// <summary> 인터넷 체크 </summary>
        public bool networkCheck()
        {
            return (Application.internetReachability != NetworkReachability.NotReachable);
        }

        /// <summary> 로그인 초기화 </summary>
        protected override void Init()
        {
            PlayGamesPlatform.InitializeInstance(
                new PlayGamesClientConfiguration.
                Builder().
                RequestIdToken().
                RequestEmail().
                Build());
            
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            auth = FirebaseAuth.DefaultInstance;
            _profile = new Profile();
            IsExist = false;
            LoadedLastSave = 0;
            isLoginFb = false;

            _leaders = new List<rankSubData>();
        }

        /// <summary> 로그인 </summary>
        public void Login()
        {
            googleLogin(() => StartCoroutine(firebaseLogin()));
        }

        /// <summary> 로그아웃 </summary>
        public void Logout()
        {
            googleLogout();
        }

        #region [ Google ]

        /// <summary> 구글 로그인 </summary>
        void googleLogin(Action afterLogin)
        {
            if (isLogin)
                return;
            
            Social.localUser.Authenticate((bool success, string str) =>
            {
                if (success)
                {
                    Debug.Log("구글 로그인 성공 : " + isLogin + " : " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
                                        
                    afterLogin?.Invoke();
                }
                else 
                {
                    Debug.Log("구글 로그인 실패 : " + str);

                    //afterLogin?.Invoke();
                }
            });
        }

        /// <summary> 구글 로그아웃 </summary>
        public void googleLogout(Action afterLogout = null)
        {
            if (isLogin)
            {
                PlayGamesPlatform.Instance.SignOut(); //((PlayGamesPlatform)Social.Active).SignOut();
                auth.SignOut();
                isLoginFb = false;
                Debug.Log("로그아웃");

                if (afterLogout != null)
                    afterLogout();
            }
        }

        // COuyE11dnKTWUJ773upzn04bLcX2
        public Task LoginWithEmail()
        {
            return auth.SignInWithEmailAndPasswordAsync("abcde@gmail.com", "123456789").ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                _uid = newUser.UserId;
                
                isLoginFb = true;
                //user = task.Result;
                //SignInState.SetState(SignInState.State.EMail);
                Debug.Log("이메일 로그인 성공!");
            });
        }

        #endregion

        #region [ FireBase ]

        /// <summary> 파이어베이스 로그인 </summary>
        IEnumerator firebaseLogin()
        {
            string idToken = null;
            string accessToken = null;

            while (string.IsNullOrEmpty(idToken))
            {
                idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
                yield return null;
            }

            Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error. : " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.Log($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})({newUser.Email})");
                _uid = newUser.UserId;

                isLoginFb = true;
            });
        }

        /// <summary> 파이어 베이스 초기화 </summary>
        public void firebaseDatabaseEventInit()
        {
            Debug.Log("파이어 데이터 베이스 초기화");
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowadventure-91260115.firebaseio.com/");            
        }

        /// <summary> 서버에 데이터 유무 확인 </summary>
        public IEnumerator chkExistData()
        {
            Debug.Log("서버에 데이터 체크");
            firebaseDatabaseEventInit();

            bool complete = false;            

            IsExist = false;
            reference.Child("User").Child(_uid).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Exist Check was canceled.");
                    complete = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Exist Check an error. : " + _uid + "//" + task.Exception);
                    complete = true;
                    return;
                }

                DataSnapshot snap = task.Result;
                IsExist = snap.HasChildren;
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 마지막 접속 날짜 가져오기 </summary>
        public IEnumerator loadLastSaveDate()
        {
            Debug.Log("마지막 날짜 가져오기");
            bool complete = false;
            LoadedLastSave = 0;
            FirebaseDatabase.DefaultInstance.GetReference("User").Child(_uid).Child("_util").Child("_lastSave").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Load SignInWithCredentialAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Load SignInWithCredentialAsync encountered an error. : " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                LoadedLastSave = (long)snapshot.Value;
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 서버 저장 </summary>
        public void SaveDataServer(bool wantWait)
        {
            Action endWin = null;

            if (wantWait)
            {
                WindowManager.instance.openWin(Windows.win_serverLoad);
                endWin = WindowManager.instance.Win_serverWait.close;
            }

            StartCoroutine(saveDataToFB(endWin));
        }

        /// <summary> 서버에 데이터 저장(과 동시에 마지막 저장시간 갱신) </summary>
        public IEnumerator saveDataToFB(Action endWin, bool isFirst = false)
        {
            // 시간 저장
            yield return StartCoroutine(checkNextDay(isFirst));

            // 데이터 저장
            bool complete = false;
            
            string json = BaseManager.userGameData.getUserData();
            
            reference.Child("User").Child(_uid).SetRawJsonValueAsync(json).ContinueWith(task => 
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception);
                }
                
                Debug.Log("저장 완료");

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            endWin?.Invoke();
        }

        enum dataKey { _payment, _property, _quest, _record, _statistics, _status, _util, max }
        public IEnumerator loadDataFromFB()
        {
            Debug.Log("서버에서 데이터 로드 시도");
            bool complete = false;

            reference.Child("User").Child(_uid).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Load SignInWithCredentialAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Load SignInWithCredentialAsync encountered an error. : " + task.Exception);
                    return;
                }

                string userData = task.Result.GetRawJsonValue();
                //Debug.Log(userData);                
                //=======================================================
                UserEntity entity = JsonConvert.DeserializeObject<UserEntity>(userData, new ObscuredValueConverter());

                if (BaseManager.userGameData == null)
                {
                    BaseManager.userGameData = new UserGameData(); 
                }

                BaseManager.userGameData.setUserEntity(entity);
                //Debug.Log(BaseManager.userGameData.getUserData());

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
            Debug.Log("서버에서 데이터 로드 완료");
        }


        public void saveSelectSkin()
        {
            StartCoroutine(saveSkinToFB());
        }
        /// <summary> 스킨 교체시 저장 (FB REALTIME DATABASE) </summary>
        IEnumerator saveSkinToFB()
        {
            // 데이터 저장
            bool complete = false;

            string json = BaseManager.userGameData.getUserData();

            reference.Child("User").Child(_uid).Child("_property").Child("_skin").SetValueAsync((int)BaseManager.userGameData.Skin).ContinueWith(task => {
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 버전 가져오기 (_lastVersion에서 버전 확인가능) </summary>
        public IEnumerator loadProFileFromFB()
        {
            bool complete = false;

            reference.Child("Profile").GetValueAsync().ContinueWith(task =>
            {
                Debug.Log("버전 체크");
                if (task.IsCanceled)
                {
                    Debug.LogError("Load SignInWithCredentialAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Load SignInWithCredentialAsync encountered an error. : " + task.Exception);
                    return;
                }

                Debug.Log(task.Result.GetRawJsonValue());
                _profile = JsonUtility.FromJson<Profile>(task.Result.GetRawJsonValue());

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        #endregion

        #region [ 특정요소 저장/로드 ]
        
        /// <summary> 최근 저장 시간 저장 - (시간만 갱신) - 기기, 서버 </summary>
        IEnumerator saveLastLogin()
        {
            bool complete = false;

            NanooManager.instance.getTimeStamp(() =>
            {
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            complete = false;
            // 서버에도 저장
            reference.Child("User").Child(_uid).Child("_util").Child("_lastSave").SetValueAsync((long)BaseManager.userGameData.LastSave).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("성공");
                }

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

        }

        /// <summary> 서버 마지막 저장시간-지금 시간 비교(저장/로드없고 오로지 비교 = 내일판별용) 
        /// - 로비에서만 (1. 모든 시간 저장시, 2. 일정시간마다) </summary>
        public IEnumerator checkNextDay(bool isFirst = false)
        {
            if (isFirst)
            {
                yield break;
            }

            BaseManager.userGameData.TimeCheck = 0;
            BaseManager.userGameData.WholeAccessTime += BaseManager.instance.PlayTimeMng.TimeStack;

            // (서버에 저장된) 마지막 저장 시간
            bool complete = false;
            long lastSaveTime = 0;
            reference.Child("User").Child(_uid).Child("_util").Child("_lastSave").GetValueAsync().ContinueWith(task => {
                
                lastSaveTime = (long)task.Result.Value;
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            // 현재 서버시간 (가져오면서 새로저장)
            yield return StartCoroutine(saveLastLogin());

            // 비교
            int lastDate = Convert.ToInt32(gameValues.epoch.AddMilliseconds(lastSaveTime).ToString("yyyyMMdd"));
            int nowDate = Convert.ToInt32(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave).ToString("yyyyMMdd"));

            if (nowDate > lastDate)
            {
                Debug.Log(nowDate.ToString("yyyy-MM-dd HH:mm:ss"));
                Debug.Log(lastDate.ToString("yyyy-MM-dd HH:mm:ss"));
                
                WhenTomorrow?.Invoke(nowDate);
            }
        }

        #endregion

        public string findKey = "";
        [Button]
        public void queryTest_findKey()
        {
            StartCoroutine(searchNickName(findKey, (chk)=> { }));
        }

        public IEnumerator searchNickName(string nick, Action<bool> chker)
        {
            bool complete = false;
            Debug.Log("11");
            reference.Child("User").OrderByChild("_property/_nickName").EqualTo(nick).GetValueAsync().ContinueWith(task =>
            {
                Debug.Log("111");
                if (task.IsCanceled)
                {
                    Debug.LogError("testNickName : cancel");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("testNickName : failed");
                    return;
                }

                DataSnapshot shot = task.Result;
                chker(shot.HasChildren);

                Debug.Log("testNickName : 성공");
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        [Button]
        public void queryTest_allCheck()
        {
            StartCoroutine(searchAllNick());
        }

        public IEnumerator searchAllNick()
        {
            bool complete = false;

            reference.Child("User").OrderByChild("_property/_nickName").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("testNickName : cancel");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("testNickName : failed");
                    return;
                }

                DataSnapshot shot = task.Result;
                Debug.Log(shot.ChildrenCount.ToString() + " : " + shot.GetRawJsonValue());
                
                var dic = shot.Value as Dictionary<string, object>;

                foreach (var v in dic)
                {
                    Debug.Log((string)shot.Child(v.Key).Child("_property").Child("_nickName").Value);
                    if (shot.Child(v.Key).HasChildren == false)
                    {
                        //Debug.Log((string)shot.Child(v.Key).GetRawJsonValue());
                        Debug.Log((long)shot.Child(v.Key).Value);
                    }
                }

                Debug.Log("searchAllNick : 성공");
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        #region [ data load / save ]

        ///// <summary> 기기에서 uid와 일치하는 데이터 불러오기 </summary>
        //public bool showDataInDevice()
        //{
        //    string[] _deviceList;
        //    if (ES3.KeyExists(gameValues._deviceListKey))
        //    {
        //        ObscuredString _deviceListJson = ES3.Load<string>(gameValues._deviceListKey);
        //        _deviceList = JsonConvert.DeserializeObject<string[]>(_deviceListJson, new ObscuredValueConverter());

        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (string.IsNullOrEmpty(_deviceList[i]) == false)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        ///// <summary> 기기에서 uid와 일치하는 데이터 불러오기 </summary>
        //public bool loadMatchingData()
        //{
        //    string[] _deviceList;
        //    if (ES3.KeyExists(gameValues._deviceListKey))
        //    {
        //        // Debug.Log(ES3.Load<string>(gameValues._deviceListKey));
        //        ObscuredString _deviceListJson = ES3.Load<string>(gameValues._deviceListKey);

        //        _deviceList = JsonConvert.DeserializeObject<string[]>(_deviceListJson, new ObscuredValueConverter());
        //    }
        //    else
        //    {
        //        _deviceList = new string[] { "", "", "" };
        //    }

        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (_deviceList[i] == _uid)
        //        {
        //            Debug.Log(_deviceList[i] + " ~ " + _uid);
        //            string load = ES3.Load<string>(_deviceList[i]);
        //            UserEntity _entity = JsonConvert.DeserializeObject<UserEntity>(load, new ObscuredValueConverter());

        //            BaseManager.userGameData.loadDataFromLocal(_entity);
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        ///// <summary> (데이터 리스트) 삭제 및 저장 </summary>
        //public void removeAndSaveDataToLocal(string remove = "")
        //{
        //    ObscuredString[] _deviceList;
        //    if (ES3.KeyExists(gameValues._deviceListKey))
        //    {
        //        ObscuredString _deviceListJson = ES3.Load<string>(gameValues._deviceListKey);
        //        _deviceList = JsonConvert.DeserializeObject<ObscuredString[]>(_deviceListJson, new ObscuredValueConverter());
        //    }
        //    else
        //    {
        //        _deviceList = new ObscuredString[] { "", "", "" };
        //    }

        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (_deviceList[i].Equals(remove))
        //        {
        //            _deviceList[i] = _uid;

        //            if (ES3.KeyExists(remove))
        //            {
        //                ES3.DeleteKey(remove);
        //            }

        //            break;
        //        }
        //    }

        //    string str = JsonConvert.SerializeObject(_deviceList, new ObscuredValueConverter());
        //    ES3.Save(gameValues._deviceListKey, str);
        //}

        #endregion

        //[Button]
        //public void OnChanged()
        //{
        //    _uid = "7f2bCCkxcMUb77Z3IGc16Zw0tum1";
            
        //    SaveDataServer();
        //}
    }


}