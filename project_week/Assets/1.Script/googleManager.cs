using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine;

namespace week
{
    public class googleManager : TSingleton<googleManager>
    {
        public bool isLogin
        {
            get { return Social.localUser.authenticated; }
        }

        public void LoginToStart()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            Login();
        }

        public void Login(Action afterLogin = null)
        {
            Social.localUser.Authenticate((bool success, string str) =>
            {
                if (success) 
                {
                    Debug.Log("구글 로그인 성공");
                    if (afterLogin != null)
                        afterLogin();
                }
                else Debug.Log("구글 로그인 실패 : " + str);
            });
        }

        public void logOut(Action afterLogout = null)
        {
            //((PlayGamesPlatform)Social.Active).SignOut();
            //Debug.Log("구글 로그아웃");
            if (afterLogout != null)
                afterLogout();
        }

        public void loginSwitch(Action afterLogin, Action afterLogout)
        {
            if (isLogin)
            {
                logOut(afterLogout);
            }
            else
            {
                Login(afterLogin);
            }
        }
    }
}