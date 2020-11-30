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
    public class rankData
    {
        [SerializeField] public string _version;
        //[SerializeField] public string _nick;
        //[SerializeField] public int _time;
        //[SerializeField] public int _boss;
        [SerializeField] public int _skin;

        public rankData() { }
        public rankData(int skin)
        {
            _version = Application.version;
            //_nick = BaseManager.userGameData.NickName;
            //_time = time;
            //_boss = boss;
            _skin = skin;
        }
    }

    public class AuthManager : TSingleton<AuthManager>
    {
        FirebaseAuth auth;
        DatabaseReference reference 
        {
            get { return FirebaseDatabase.DefaultInstance.RootReference; }
        }
        DatabaseReference serverTime
        {
            get { return reference.Child("Profile").Child("serverTime"); }
        }

        ObscuredString _uid = "SinglePlay";

        string _version;
        // long _lastLogin;
        List<rankData> _leaders;
        public Action WhenTomorrow { get; set; }
        bool _succesGetTime;

        public bool IsExist { get; set; }
        public long LoadedLastSave { get; set; }
        
        public bool isLogin
        {
            get { return Social.localUser.authenticated; }
        }

        public bool isLoginFb { get; set; }
        public List<rankData> Leaders { get => _leaders; }
        public string Version { get => _version; }
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
            IsExist = false;
            LoadedLastSave = 0;
            isLoginFb = false;

            _leaders = new List<rankData>();
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
                Debug.Log(snap.ChildrenCount);
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
        public void SaveDataServer()
        {
            StartCoroutine(saveDataToFB());
        }

        /// <summary> 서버에 데이터 저장(과 동시에 마지막 저장시간 갱신) </summary>
        public IEnumerator saveDataToFB()
        {
            // 시간 저장
            yield return StartCoroutine(checkNextDay());

            // 데이터 저장
            bool complete = false;
            
            string json = BaseManager.userGameData.getUserData();

            reference.Child("User").Child(_uid).SetRawJsonValueAsync(json).ContinueWith(task => {
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

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
                string userData = (string)task.Result.GetRawJsonValue();

                UserEntity entity = JsonConvert.DeserializeObject<UserEntity>(userData, new ObscuredValueConverter());

                if (BaseManager.userGameData == null)
                {
                    BaseManager.userGameData = new UserGameData();                    
                }

                BaseManager.userGameData.setUserEntity(entity);

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
            Debug.Log("서버에서 데이터 로드 완료");
        }

        //public IEnumerator loadAndChangeDataFromFB(string remove)
        //{
        //    Debug.Log("서버 데이터로 로컬 데이터 교체 시도");
        //    bool complete = false;

        //    reference.Child("User").Child(_uid).GetValueAsync().ContinueWith(task =>
        //    {
        //        if (task.IsCanceled)
        //        {
        //            Debug.LogError("Load SignInWithCredentialAsync was canceled.");
        //            return;
        //        }

        //        if (task.IsFaulted)
        //        {
        //            Debug.LogError("Load SignInWithCredentialAsync encountered an error. : " + task.Exception);
        //            return;
        //        }

        //        string userData = (string)task.Result.GetRawJsonValue();

        //        UserEntity entity = JsonConvert.DeserializeObject<UserEntity>(userData, new ObscuredValueConverter());
                
        //        if (BaseManager.userGameData == null)
        //        {
        //            BaseManager.userGameData = new UserGameData();
        //        }

        //        // BaseManager.userGameData.loadDataFromLocal(entity);
        //        ES3.Save(_uid, userData);

        //        // 교체 ==================================================

        //        // removeAndSaveDataToLocal(remove);

        //        complete = true;
        //    });

        //    yield return new WaitUntil(() => complete == true);
        //    Debug.Log("서버에서 데이터 로드 완료");
        //}

        /// <summary> 버전 가져오기 (_lastVersion에서 버전 확인가능) </summary>
        public IEnumerator loadVersionFromFB()
        {
            bool complete = false;

            reference.Child("Profile").Child("version").GetValueAsync().ContinueWith(task =>
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

                DataSnapshot shot = task.Result;
                _version = (string)shot.Value;
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        #endregion

        /*
        #region [ rank ]

        /// <summary> 신기록 세우거나 유저 요청시 </summary>
        public void saveRankDataFromFB()
        {
            if (networkCheck() == false)
                return;

            StartCoroutine(userRankChecking());
        }

        /// <summary> 유저 랭킹 저장 </summary>
        IEnumerator userRankChecking()
        {
            // 최신판 로드 및 정렬
            yield return StartCoroutine(loadRankDataFromFB());

            saveRankDataTransaction(); // 트랜잭션 저장
        }

        /// <summary> 로드 </summary>
        public IEnumerator loadRankDataFromFB()
        {
            bool complete = false;

            reference.Child("Rank").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Rank Load SignInWithCredentialAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Rank Load SignInWithCredentialAsync encountered an error. : " + task.Exception);
                    return;
                }

                DataSnapshot snap = task.Result;

                _leaders.Clear();

                foreach (DataSnapshot leader in snap.Children)
                { 
                    rankData rankBox = JsonConvert.DeserializeObject<rankData>(leader.GetRawJsonValue(), new ObscuredValueConverter());

                    _leaders.Add(rankBox);
                }

                complete = true;
                BaseManager.userGameData._rankRefreshTime = DateTime.Now;
            });

            yield return new WaitUntil(() => complete == true);

            sortingLeaders();
            BaseManager.userGameData._minRank = _leaders.Last()._time;
        }

        /// <summary> 트랜잭션 저장 </summary>
        void saveRankDataTransaction()
        {
            reference.Child("Rank").RunTransaction(MutableData => { 
            
                List<object> tLeaders = MutableData.Value as List<object>;

                Debug.Log("랭커 수 : " + tLeaders.Count);
                if (tLeaders == null)
                {
                    tLeaders = new List<object>();
                }
                else if (MutableData.ChildrenCount >= 28)
                {
                    long min = long.MaxValue;
                    object minData = null;

                    foreach (var child in tLeaders)
                    {
                        if (!(child is Dictionary<string, object>))
                            continue;

                        string childid = (string)((Dictionary<string, object>)child)["_uid"];
                        long childtime = (long)((Dictionary<string, object>)child)["_time"];

                        if (childid.Equals(_uid))
                        {
                            ((Dictionary<string, object>)child)["_nick"] = BaseManager.userGameData.NickName;
                            ((Dictionary<string, object>)child)["_version"] = ((int)gameValues._version);
                            ((Dictionary<string, object>)child)["_time"] = ((int)BaseManager.userGameData.TimeRecord);
                            ((Dictionary<string, object>)child)["_boss"] = ((int)BaseManager.userGameData.BossRecord);
                            ((Dictionary<string, object>)child)["_skin"] = ((int)BaseManager.userGameData.RecordSkin);

                            MutableData.Value = tLeaders;
                            return TransactionResult.Success(MutableData);
                        }
                        else if (min > childtime)
                        {
                            minData = child;
                            min = childtime;
                        }
                    }

                    if (min > BaseManager.userGameData.TimeRecord)
                    {
                        return TransactionResult.Abort();
                    }

                    tLeaders.Remove(minData);
                }
                else
                {
                    foreach (var child in tLeaders)
                    {
                        if (!(child is Dictionary<string, object>))
                            continue;

                        string childid = (string)((Dictionary<string, object>)child)["_uid"];

                        Debug.Log(childid + " => " + (childid.Equals(_uid)) + " <= " + _uid);
                        if (childid.Equals(_uid))
                        {
                            ((Dictionary<string, object>)child)["_nick"] = BaseManager.userGameData.NickName;
                            ((Dictionary<string, object>)child)["_version"] = ((int)gameValues._version);
                            ((Dictionary<string, object>)child)["_time"] = ((int)BaseManager.userGameData.TimeRecord);
                            ((Dictionary<string, object>)child)["_boss"] = ((int)BaseManager.userGameData.BossRecord);
                            ((Dictionary<string, object>)child)["_skin"] = ((int)BaseManager.userGameData.RecordSkin);

                            MutableData.Value = tLeaders;
                            return TransactionResult.Success(MutableData);
                        }
                    }
                }

                tLeaders.Add(BaseManager.userGameData.getRankData(_uid));
                MutableData.Value = tLeaders;
                return TransactionResult.Success(MutableData);
            });
        }

        #endregion
        */

        /// <summary> 랭킹 정렬 </summary>
        void sortingLeaders()
        {
            Leaders.Sort((rankData A, rankData B) =>
            {
                //if (A._time > B._time) return -1;
                //else if (A._time < B._time) return 1;
                return 0;
            });
        }

        #region [ 특정요소 저장/로드 ]
        
        /// <summary> 최근 저장 시간 저장 - (시간만 갱신) - 기기, 서버 </summary>
        IEnumerator saveLastLogin()
        {
            int complete = 0;

            NanooManager.instance.getTimeStamp(() =>
            {
                complete = 1;
            });

            yield return new WaitUntil(() => complete == 1);

            // 서버에도 저장
            reference.Child("User").Child(_uid).Child("_util").Child("_lastSave").SetValueAsync(BaseManager.userGameData.LastSave).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("성공");
                }

                complete = 2;
            });

            yield return new WaitUntil(() => complete == 2);

        }

        /// <summary> 서버 마지막 저장시간-지금 시간 비교(저장/로드없고 오로지 비교) 
        /// - 로비에서만 (1. 모든 시간 저장시, 2. 일정시간마다) </summary>
        public IEnumerator checkNextDay()
        {
            BaseManager.userGameData.TimeCheck = 0;
            BaseManager.userGameData.WholeAccessTime += BaseManager.instance.PlayTimeMng.TimeStack;

            // (서버에 저장된) 마지막 저장 시간
            bool complete = false;
            long lastTime = 0;
            reference.Child("User").Child(_uid).Child("_util").Child("_lastSave").GetValueAsync().ContinueWith(task => {

                lastTime = (long)task.Result.Value;
                complete = true;
            });
            yield return new WaitUntil(() => complete == true);

            // 현재 서버시간 (가져오면서 저장)
            yield return StartCoroutine(saveLastLogin());            

            // 비교
            DateTime lastDate = gameValues.epoch.AddMilliseconds(lastTime);
            DateTime nowDate = gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave);

            // Debug.Log(nowDate.Day + " > " + lastDate.Day);
            if (nowDate.Day > lastDate.Day)
            {
                Debug.Log(nowDate.ToString("yyyy-MM-dd HH:mm:ss"));
                Debug.Log(lastDate.ToString("yyyy-MM-dd HH:mm:ss"));
                
                WhenTomorrow?.Invoke();
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

            reference.Child("User").OrderByChild("_property/_nickName").EqualTo(nick).GetValueAsync().ContinueWith(task =>
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
                // Debug.Log(shot.Value + " : " + ());
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
    }


}