using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Firebase SDK 참조
using Firebase;

public class FCM : MonoBehaviour
{
    // 로그를 찍어줄 UGUI Text
    // public Text log;
    public string log;
    // 로그를 저장해줄 String
    public string addMessage = "";

    FirebaseApp app;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Firebase 클라우드 메시징 초기화
        // https://firebase.google.com/docs/cloud-messaging/unity/client?hl=ko#initialize
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

        // GooglePlay 서비스 버전 요구사항 확인
        // https://firebase.google.com/docs/cloud-messaging/unity/client?hl=ko#confirm_google_play_version
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                addMessage += "파이어 베이스 앱 초기화 완료 \n";

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                addMessage += "Could not resolve all Firebase dependencies";
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        lock (log)
        {
            if (string.IsNullOrEmpty(addMessage)) return;

            log += addMessage;

            addMessage = "";
        }
    }

    // 토큰을 수신하며 차후 토큰을 사용하도록 캐시한다.
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);

        addMessage += string.Format("Received Registration Token: {0} \n",token.Token);
    }

    // 메시지를 수신한다.
    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        var notification = e.Message.Notification;

        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);

        addMessage += string.Format("Received a new message from: {0} Title: {1} Message: {2} \n",e.Message.From ,notification.Title,notification.Body);
    }
}
