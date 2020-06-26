using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace week
{
    public class BaseManager : MonoBehaviour
    {
        static public BaseManager instance;

        static public UserEntity userEntity;
        public bool isDataLoadSuccess { get; private set; }

        // Use this for initialization
        void Start()
        {
            Debug.Log("베이스 스타트");
            instance = this;
            isDataLoadSuccess = false;       

            if (ES3.KeyExists("userEntity"))
            {
                userEntity = ES3.Load<UserEntity>("userEntity");
            }
            else
            {
                Debug.Log("기본 유저 데이터가 없으므로 제작함");
                userEntity = new UserEntity();
            }

            StartCoroutine(baseStart());

            StartCoroutine(StartLogoScene());
        }
        IEnumerator baseStart()
        {
            bool result = DataManager.LoadBGdata();
            if (result)
            {
                isDataLoadSuccess = true;
                Debug.Log(DataManager.bgCount);
            }
            else
                Debug.LogError("앱 데이터 로드 에러");

            yield return new WaitForEndOfFrame();
        }

        // 씬 전환
        IEnumerator LoadingScene(string remove, int load)
        {
            AsyncOperation AO;
            if (remove != string.Empty)
            {
                AO = SceneManager.UnloadSceneAsync(remove);
                while (!AO.isDone)
                {
                    yield return new WaitForSeconds(0.25f);
                }
            }

            //yield return new WaitForSeconds(0.25f);


            AO = SceneManager.LoadSceneAsync(load, LoadSceneMode.Additive);
            while (!AO.isDone)
            {
                yield return new WaitForSeconds(0.25f);
            }


            //yield return new WaitForSeconds(0.4f);

            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

            //yield return new WaitForSeconds(0.5f);
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
        public void convertScene(string close, SceneNum open)
        {
            StartCoroutine(LoadingScene(close, (int)open));
        }
    }
}