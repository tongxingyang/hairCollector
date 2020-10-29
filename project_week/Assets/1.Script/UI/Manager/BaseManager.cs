using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace week
{
    public class BaseManager : MonoBehaviour
    {
        static public BaseManager instance;

        static public UserGameData userGameData;
        private static PreGameData preGameData;

        public static PreGameData PreGameData { set => preGameData = value; }

        Action _sceneLoadStart;
        Action _sceneLoadComplete;
        public Action SceneLoadStart { set => _sceneLoadStart = value; }
        public Action SceneLoadComplete { set => _sceneLoadComplete = value; }

        // Use this for initialization
        void Start()
        {
            Debug.Log("베이스 스타트");
            instance = this;

            StartCoroutine(StartLogoScene());
        }

        // 로고 씬
        IEnumerator StartLogoScene()
        {
            AsyncOperation AO = SceneManager.LoadSceneAsync(SceneNum.LogoScene.ToString(), LoadSceneMode.Additive);
            while (!AO.isDone)
            {
                yield return new WaitForSeconds(0.3f);
            }
        }

        // 씬 전환
        IEnumerator LoadingScene(string remove, int load)
        {
            _sceneLoadStart?.Invoke();

            yield return new WaitForSeconds(0.2f);

            AsyncOperation AO;
            if (remove != string.Empty)
            {
                AO = SceneManager.UnloadSceneAsync(remove);
                while (!AO.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            AO = SceneManager.LoadSceneAsync(load, LoadSceneMode.Additive);
            while (!AO.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

            _sceneLoadComplete?.Invoke();

            yield return new WaitForSeconds(0.2f);
        }

        // 씬 전환
        public void convertScene(string close, SceneNum open)
        {
            StartCoroutine(LoadingScene(close, (int)open));
        }

        public string convertToTime(int time)
        {
            int s = time % 60;
            int m = (time % 3600) / 60;
            int h = time / 3600;
            
            if (time > 3600)
            {
                return $"{h:D2}:{m:D2}:{s:D2}";
            }
            else
            {
                return $"{m:D2}:{s:D2}";
            }
        }
    }
}