using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace week
{
    public class BaseManager : MonoBehaviour
    {
        static public BaseManager instance; // 

        public static UserGameData userGameData;
        public static Option option;
        private static PreGameData preGameData;        

        public static PreGameData PreGameData { set => preGameData = value; }
        PlayTimeManager _playTimeMng;

        /// <summary> 패치 </summary>
        public static bool NeedPatch { get; set; } = false;

        Action _sceneLoadStart;
        Action _sceneLoadComplete;
        public Action SceneLoadStart { set => _sceneLoadStart = value; }
        public Action SceneLoadComplete { set => _sceneLoadComplete = value; }
        public PlayTimeManager PlayTimeMng { get => _playTimeMng; }

        #region [ test ]

        public class t1
        {
            [SerializeField] public ObscuredInt[] arrayI;

            public t1(ObscuredInt[] arrayI)
            {
                this.arrayI = arrayI;
            }
        }

        public class t2
        {
            [SerializeField] public List<ObscuredInt> listI;

            public t2(List<ObscuredInt> listI)
            {
                this.listI = listI;
            }
        }

        void Test()
        {
            t1 tt1 = new t1(new ObscuredInt[] { 0, 1, 2 });
            string t1json = JsonConvert.SerializeObject(tt1, new ObscuredValueConverter());
            t1 t1s = JsonConvert.DeserializeObject<t1>(t1json, new ObscuredValueConverter());

            t2 tt2 = new t2(new List<ObscuredInt>() { 3, 4, 5 });
            string t2json = JsonConvert.SerializeObject(tt2, new ObscuredValueConverter());
            t2 t2s = JsonConvert.DeserializeObject<t2>(t2json, new ObscuredValueConverter());
        }

        #endregion

        // Use this for initialization
        void Start()
        {
            Debug.Log("베이스 스타트");
            instance = this;
            option = new Option();
            _playTimeMng = gameObject.AddComponent<PlayTimeManager>();
            _playTimeMng.Init();

            Test();

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

        public void saveOption()
        {
            ES3.Save("option", JsonUtility.ToJson(option));
        }

        public void loadOption()
        {
            if (ES3.KeyExists("option"))
            {
                string str = ES3.Load<string>("option");
                option = JsonUtility.FromJson<Option>(str);
            }
        }

        public void KeyRandomizing()
        {            
            StartCoroutine(userGameData.RandomizeKey_Coroutine());
        }
    }
}