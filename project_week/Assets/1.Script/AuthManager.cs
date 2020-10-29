using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

namespace week
{
    public class AuthManager : TSingleton<AuthManager>
    {
        FirebaseAuth auth;
        
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