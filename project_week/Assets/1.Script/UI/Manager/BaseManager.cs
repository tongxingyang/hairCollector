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
        public static DeviceData _innerData;
        public static PreGameData PreGameData;               

        public Camera _main;
        PlayTimeManager _playTimeMng;

        /// <summary> 패치 </summary>
        public static bool NeedPatch { get; set; } = false;

        Action _sceneLoadStart;
        Action _sceneLoadComplete;
        public Action SceneLoadStart { set => _sceneLoadStart = value; }
        public Action SceneLoadComplete { set => _sceneLoadComplete = value; }
        public PlayTimeManager PlayTimeMng { get => _playTimeMng; }

        private void Awake()
        {
            Newtonsoft.Json.Utilities.AotHelper.EnsureList<ObscuredInt>();
        }

        // Use this for initialization
        void Start()
        {
            _main = Camera.main;

            //test();
            Debug.Log("베이스 스타트");
            
            instance = this;
            _innerData = new DeviceData();

            _playTimeMng = gameObject.AddComponent<PlayTimeManager>();
            _playTimeMng.Init();

            StartCoroutine(StartLogoScene());
        }
        
        void test()
        {
            Debug.Log("테스트 시작");
            //string pay = "{\"_chkList\":5,\"_heroPack\":true,\"_leftFreeGem\":0,\"_mulCoinList\":23,\"_vampPack\":true}";
            string ppt = "{\"_currency\":[21431,2746,280],\"_hasSkin\":49153,\"_isSetRader\":false,\"_lastRaderTime\":0,\"_nickName\":\"\ud0a4\ud0a4 \ub2e4 \uc784\ub9c8\",\"_nowStageLevel\":2,\"_skin\":0,\"_skinLevel\":[10,0,0,4,0,1,5,0,8,0,0,1,0,0,0,4,6,2,0]}" ;
            string qst = "{\"_lvlBossReward\":[8,1,1],\"_lvlTimeReward\":[5,5,1],\"_publishDate\":20210524,\"_questChk\":[0,0,0,0,0,0],\"_questRein\":0,\"_questRequest\":0,\"_questSkill\":[7,17,18],\"_questSkin\":5}" ;
            string rcd = "{\"_reinRecord\":-132,\"_requestRecord\":-150,\"_season_RecordBoss\":[0,0,0],\"_season_RecordLevel\":[1,8,2],\"_season_RecordSkin\":[0,0,0],\"_season_TimeRecord\":[247,0,139],\"_wholeTimeRecord\":4987}" ;
            //string sti = "{\"_playCount\":86,\"_storeUseCount\":67,\"_wholeAccessTime\":38928}";
            string stt = "{\"_statusLevel\":[29,8,11,33,6,5,2,4]}";
            //string utl = "{\"_chkList\":3,\"_lastSave\":1621818547000}";

            //UserEntity.payment e1 = JsonConvert.DeserializeObject<UserEntity.payment>(pay, new ObscuredValueConverter());
            Debug.Log("테스트 1");
            //UserEntity.gameUtility e7 = JsonConvert.DeserializeObject<UserEntity.gameUtility>(utl, new ObscuredValueConverter());
            Debug.Log("테스트 2");
            //UserEntity.statistics e5 = JsonConvert.DeserializeObject<UserEntity.statistics>(sti, new ObscuredValueConverter());
            Debug.Log("테스트 3");
            UserEntity.status e6 = JsonConvert.DeserializeObject<UserEntity.status>(stt, new ObscuredValueConverter());
            Debug.Log("테스트 4");
            UserEntity.quest e3 = JsonConvert.DeserializeObject<UserEntity.quest>(qst, new ObscuredValueConverter());
            Debug.Log("테스트 5");
            UserEntity.record e4 = JsonConvert.DeserializeObject<UserEntity.record>(rcd, new ObscuredValueConverter());
            Debug.Log("테스트 6");
            UserEntity.property e2 = JsonConvert.DeserializeObject<UserEntity.property>(ppt, new ObscuredValueConverter());

            //UserGameData ata = new UserGameData();
            //ata.setUserEntity(entity);

            Debug.Log("테스트 끝");
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
            Debug.Log(remove + " -> " + (SceneNum)load);
            AsyncOperation AO;
            if (remove != string.Empty)
            {
                AO = SceneManager.UnloadSceneAsync(remove);
                while (!AO.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            //if (load == (int)SceneNum.GameScene)
            //    _main.enabled = false;
            //else if (load == (int)SceneNum.LobbyScene)
            //    _main.enabled = true;

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

        public void saveDeviceData()
        {
            ES3.Save("deviceData", JsonUtility.ToJson(_innerData));
        }

        public void loadOption()
        {
            if (ES3.KeyExists("deviceData"))
            {
                string str = ES3.Load<string>("deviceData");
                _innerData = JsonUtility.FromJson<DeviceData>(str);
            }
        }

        public void KeyRandomizing()
        {            
            StartCoroutine(userGameData.RandomizeKey_Coroutine());
        }
    }    
}