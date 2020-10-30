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

namespace week
{
    public class AuthManager : TSingleton<AuthManager>
    {
        FirebaseAuth auth;

        DatabaseReference reference;
        public bool IsExist { get; set; }
        public long LoadedFirstJoin { get; set; }
        public long LoadedLastSave { get; set; }
        
        public bool isLogin
        {
            get { return Social.localUser.authenticated; }
        }

        public bool isLoginFb { get; set; }

        /// <summary> 초기화 </summary>
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

            Debug.Log("token : " + idToken);

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
                Debug.Log($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
                isLoginFb = true;
            });
        }

        /// <summary> 서버에 데이터 유무 확인 </summary>
        public IEnumerator chkExistData()
        {
            bool complete = false;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowadventure-91260115.firebaseio.com/");

            reference = FirebaseDatabase.DefaultInstance.RootReference;

            IsExist = false;
            reference.Child("User").Child(Social.localUser.id).GetValueAsync().ContinueWith(task =>
            {
                complete = true;
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

                IsExist = task.Result.Exists;
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 최초 가입 날짜 가져오기 </summary>
        public IEnumerator loadFirstJoinDate()
        {
            bool complete = false;
            LoadedFirstJoin = 0;
            FirebaseDatabase.DefaultInstance.GetReference("User").Child(Social.localUser.id).Child("_util").Child("_join").GetValueAsync().ContinueWith(task =>
            {
                complete = true;
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
            });

            yield return new WaitUntil(() => complete == true);
        }

        /// <summary> 마지막 접속 날짜 가져오기 </summary>
        public IEnumerator loadLastSaveDate()
        {
            bool complete = false;
            LoadedLastSave = 0;
            FirebaseDatabase.DefaultInstance.GetReference("User").Child(Social.localUser.id).Child("_util").Child("_lastSave").GetValueAsync().ContinueWith(task =>
            {
                complete = true;
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
            });

            yield return new WaitUntil(() => complete == true);
        }

        public void saveDataToFB()
        {
            //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowadventure-91260115.firebaseio.com/");
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            BaseManager.userGameData.IsSavedServer = true;

            string json = BaseManager.userGameData.getUserData();
            Debug.Log(json);

            reference.Child("User").Child(Social.localUser.id).SetRawJsonValueAsync(json);
        }

        public void loadDataFromFB()
        {
            //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowadventure-91260115.firebaseio.com/");
            //reference = FirebaseDatabase.DefaultInstance.RootReference;            

            reference.Child("User").Child(Social.localUser.id).GetValueAsync().ContinueWith(task =>
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
            });
        }

        void applyData(DataSnapshot snapshot)
        {
            DataSnapshot snapstruct;
            snapstruct = snapshot.Child("_property");
            BaseManager.userGameData.Property = new UserEntity.property (
                (string)snapstruct.Child("_nickName").Value,
                new int[3] { (int)snapstruct.Child("_currency").Child("0").Value, (int)snapstruct.Child("_currency").Child("1").Value, (int)snapstruct.Child("_currency").Child("2").Value },
                (int)snapstruct.Child("_hasSkin").Value,
                (int)snapstruct.Child("_skin").Value
                );

            snapstruct = snapshot.Child("_status");
            snapstruct = snapshot.Child("_quest");
            snapstruct = snapshot.Child("_payment");

            snapstruct = snapshot.Child("_option");
            BaseManager.userGameData.Option = new UserEntity.option(
                (float)snapstruct.Child("_bgmVol").Value,
                (float)snapstruct.Child("_sfxVol").Value
                );

            snapstruct = snapshot.Child("_util");
            BaseManager.userGameData.Util = new UserEntity.gameUtility (
                (long)snapstruct.Child("_join").Value,
                (bool)snapstruct.Child("_isSavedServer").Value,
                (long)snapstruct.Child("_lastSave").Value
                );
        }

        #endregion

        //public void loginSwitch(Action afterLogin, Action afterLogout)
        //{
        //    if (isLogin)
        //    {
        //        logOut(afterLogout);
        //    }
        //    else
        //    {
        //        Login(afterLogin);
        //    }
        //}
    }
}