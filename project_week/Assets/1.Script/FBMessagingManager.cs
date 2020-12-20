using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Messaging;

namespace week
{
    public class FBMessagingManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;

        }

        void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log("Received Registration Token: " + token.Token);
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from: " + e.Message.From);

            var notification = e.Message.Notification;
            if (notification != null)
            {
                Debug.Log("title: " + notification.Title);
                Debug.Log("body: " + notification.Body);

            }
            if (e.Message.From.Length > 0)
                Debug.Log("from: " + e.Message.From);
            if (e.Message.Data.Count > 0)
            {
                Debug.Log("data:");
                foreach (System.Collections.Generic.KeyValuePair<string, string> iter in e.Message.Data)
                {
                    Debug.Log("  " + iter.Key + ": " + iter.Value);
                }
            }
        }
    }
}