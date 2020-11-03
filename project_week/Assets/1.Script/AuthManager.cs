using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

namespace week
{
    [Serializable]
    public class rankData
    {
        [SerializeField] public int _version;
        [SerializeField] public string _uid;
        [SerializeField] public string _nick;
        [SerializeField] public int _time;
        [SerializeField] public int _boss;
        [SerializeField] public int _skin;

        public rankData() { }
        public rankData(string uid, string nick, int time, int boss, int skin)
        {
            _uid = uid;
            _nick = nick;
            _time = time;
            _boss = boss;
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
        DatabaseReference serverTime;

        string _uid = "abcdefg";

        int _lastVersion;
        long _lastLogin;
        List<rankData> _leaders;

        public bool IsExist { get; set; }
        public long LoadedFirstJoin { get; set; }
        public long LoadedLastSave { get; set; }
        
        public bool isLogin
        {
            get { return Social.localUser.authenticated; }
        }

        public bool isLoginFb { get; set; }
        public List<rankData> Leaders { get => _leaders; }
        public int LastVersion { get => _lastVersion; }
        public long LastLogin { get => _lastLogin; }

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
                Build());
            
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            auth = FirebaseAuth.DefaultInstance;
            IsExist = false;
            LoadedFirstJoin = 0;
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
        void googleLogin(Action afterLogin = null)
        {
            if (isLogin)
                return;
            
            Social.localUser.Authenticate((bool success, string str) =>
            {
                if (success)
                {
                    Debug.Log("구글 로그인 성공 : " + isLogin + " : " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());

                    if (afterLogin != null)
                        afterLogin();
                }
                else Debug.Log("구글 로그인 실패 : " + str);
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
                //Debug.Log($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
                _uid = newUser.UserId;

                isLoginFb = true;
            });
        }

        /// <summary> 파이어 베이스 초기화 </summary>
        public void firebaseDatabaseEventInit()
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowadventure-91260115.firebaseio.com/");

            serverTime = reference.Child("Profile").Child("serverTime");

            serverTime.ValueChanged += (object sender, ValueChangedEventArgs args) => {

                if (args.DatabaseError != null)
                {
                    Debug.Log("실패" + args.DatabaseError.Message);
                    return;
                }

                _lastLogin = (long)args.Snapshot.Value;
                DateTime utcCreated = gameValues.epoch.AddMilliseconds(_lastLogin);
                Debug.Log(utcCreated.ToString());
            };
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
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Exist Check an error. : " + task.Exception);
                    return;
                }

                DataSnapshot snap = task.Result;
                IsExist = snap.HasChildren;
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 최초 가입 날짜 가져오기 </summary>
        public IEnumerator loadFirstJoinDate()
        {
            Debug.Log("최초 가입날짜 가져오기");
            bool complete = false;
            LoadedFirstJoin = 0;
            FirebaseDatabase.DefaultInstance.GetReference("User").Child(_uid).Child("_util").Child("_join").GetValueAsync().ContinueWith(task =>
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
                LoadedFirstJoin = (long)snapshot.Value;

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

        public IEnumerator saveDataToFB()
        {
            BaseManager.userGameData.IsSavedServer = true;
            bool complete = false;

            yield return StartCoroutine(getTimestamp());

            string json = BaseManager.userGameData.getUserData();
            Debug.Log(json);

            reference.Child("User").Child(_uid).SetRawJsonValueAsync(json).ContinueWith(task => {
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        public IEnumerator loadDataFromFB()
        {
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

                BaseManager.userGameData.loadDataFromLocal(JsonUtility.FromJson<UserEntity>(userData));
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        public IEnumerator loadVersionFromFB()
        {
            Debug.Log("버전 요청");
            bool complete = false;

            reference.Child("Profile").Child("version").GetValueAsync().ContinueWith(task =>
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

                DataSnapshot shot = task.Result;
                _lastVersion = Convert.ToInt32(shot.Value);
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        #endregion

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
                    rankData rankBox = JsonUtility.FromJson<rankData>(leader.GetRawJsonValue());

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

                if (tLeaders == null)
                {
                    tLeaders = new List<object>();
                }
                else if (MutableData.ChildrenCount >= 28)
                {
                    long min = int.MaxValue;
                    object minData = null;

                    foreach (var child in tLeaders)
                    {
                        if (!(child is Dictionary<string, object>))
                            continue;

                        string childid = (string)((Dictionary<string, object>)child)["_uid"];
                        long childtime = (long)((Dictionary<string, object>)child)["_time"];

                        if (childid.Equals(_uid))
                        {
                            ((rankData)child)._time = BaseManager.userGameData.TimeRecord;
                            ((rankData)child)._boss = BaseManager.userGameData.BossRecord;
                            ((rankData)child)._nick = BaseManager.userGameData.NickName;
                            ((rankData)child)._skin = BaseManager.userGameData.RecordSkin;

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

                tLeaders.Add(BaseManager.userGameData.getRankData(_uid));
                MutableData.Value = tLeaders;
                return TransactionResult.Success(MutableData);
            });
        }

        #endregion

        /// <summary> 기기+서버 저장 </summary>
        public void AllSaveUserEntity()
        {
            BaseManager.userGameData.saveDataToLocal();

#if UNITY_EDITOR
            StartCoroutine(AuthManager.instance.saveDataToFB()); // 지워
#else
            StartCoroutine(AuthManager.instance.saveDataToFB());
#endif
        }

        /// <summary> 랭킹 정렬 </summary>
        void sortingLeaders()
        {
            Leaders.Sort((rankData A, rankData B) =>
            {
                if (A._time > B._time) return -1;
                else if (A._time < B._time) return 1;
                return 0;
            });
        }

        #region [ 특정요소 저장/로드 ]

        public IEnumerator saveAfterPurchase()
        {
            yield break;
        }

        /// <summary> 최근 저장 시간 </summary>
        public IEnumerator saveLastLogin()
        {
            yield return StartCoroutine(getTimestamp());
            BaseManager.userGameData.LastSave = _lastLogin;
            BaseManager.userGameData.saveDataToLocal();

            bool complete = false;

            reference.Child("User").Child(_uid).Child("_util").Child("_lastSave").SetValueAsync(_lastLogin).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("성공");
                }

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 서버 시간 가져오기 </summary>
        public IEnumerator getTimestamp()
        {
            Debug.Log("서버시간 가져오기");
            bool complete = false;
            // string json = @"{""serverTime"":{"".sv"":""timestamp""}}";

            serverTime.SetValueAsync(ServerValue.Timestamp).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("성공");
                }
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            // Set JSON for this document
            //profiles.SetRawJsonValueAsync(json).ContinueWith(t => {
            //    if (t.IsCompleted)
            //    {
            //        Debug.Log("성공");
            //        // Set the Firebase server timestamp on the datetime object
            //        //profiles.Child("timeStamp").UpdateChildrenAsync(new Dictionary<string, object> { { "utcCreatedUnix", ServerValue.Timestamp } });
            //        //profiles.Child("timeStamp").Child("utcCreatedUnix").SetValueAsync(ServerValue.Timestamp);
            //    }
            //    else
            //        Debug.Log("실패2");
            //});
        }

        #endregion

        //public void getTimestamp()
        //{
        //    Debug.Log("시간 가져오기");
        //    string json = @"{""serverTime"":{"".sv"":""timestamp""}}";

        //    DatabaseReference profiles = reference.Child("Profile");

        //    profiles.Child("timeStamp").ChildChanged += (object sender, ChildChangedEventArgs args) => {

        //        if (args.DatabaseError != null)
        //        {
        //            Debug.Log("실패1" + args.DatabaseError.Message);
        //            return;
        //        }

        //        Debug.Log(args.Snapshot.Key + ", 개수 : " + args.Snapshot.ChildrenCount);

        //        long milliseconds = (long)args.Snapshot.Value;
        //        DateTime utcCreated = gameValues.epoch.AddMilliseconds(milliseconds);
        //        Debug.Log(args.Snapshot.Key + "-val : " + milliseconds + " = " + utcCreated.ToString());

        //        //foreach (DataSnapshot ds in args.Snapshot.Children)
        //        //{
        //        //    long milliseconds = (long)ds.Value;
        //        //    DateTime utcCreated = gameValues.epoch.AddMilliseconds(milliseconds);
        //        //    Debug.Log(ds.Key + "-val : " + milliseconds + " = " + utcCreated.ToString());
        //        //}
        //    };

        //    // Set JSON for this document
        //    profiles.Child("timeStamp").SetRawJsonValueAsync(json).ContinueWith(t => {
        //        if (t.IsCompleted)
        //        {
        //            Debug.Log("성공");
        //            // Set the Firebase server timestamp on the datetime object
        //            profiles.Child("timeStamp").UpdateChildrenAsync(new Dictionary<string, object> { { "utcCreatedUnix", ServerValue.Timestamp } });
        //        }
        //        else
        //            Debug.Log("실패2");
        //    });
        //}
    }


}