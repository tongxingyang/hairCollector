using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class LogoScene : UIBase
    {
        #region

        enum E_IMAGE
        {
            TeamLogo,
            GameLogo,
                        
            Fill,

            pressButton
        }

        protected override Enum GetEnumImage() { return new E_IMAGE(); }
        
        #endregion

        float time = 0f;
        float gauge = 0.001f;

        // Start is called before the first frame update
        void Start()
        {
            m_pkImages[(int)E_IMAGE.Fill].fillAmount = 0f;
            m_pkImages[(int)E_IMAGE.pressButton].gameObject.SetActive(false);
            m_pkImages[(int)E_IMAGE.Fill].gameObject.SetActive(true);
            StartCoroutine(showLogo());
        }

        private void Update()
        {
            time += Time.deltaTime;
            if (time > 0.2f)
            {
                time = 0;
                gauge += 0.04f;
                LoadingBar();
            }
        }

        #region 로고 오프닝

        /// <summary> 로고 오프닝 (로그인) </summary>
        IEnumerator showLogo()
        {
            // 로고 보여주는 부분
            m_pkImages[(int)E_IMAGE.TeamLogo].gameObject.SetActive(true);
            m_pkImages[(int)E_IMAGE.GameLogo].gameObject.SetActive(false);
            
            yield return new WaitForSeconds(1.5f);
                        
            // 오프닝 화면과 데이터 로드
            m_pkImages[(int)E_IMAGE.TeamLogo].gameObject.SetActive(false);
            m_pkImages[(int)E_IMAGE.GameLogo].gameObject.SetActive(true);

            yield return new WaitUntil(()=> m_pkImages[(int)E_IMAGE.Fill].fillAmount == 1f);
            LoadComplete();
        }

        #endregion

        void LoadingBar()
        {            
            m_pkImages[(int)E_IMAGE.Fill].fillAmount = gauge;
        }

        void LoadComplete()
        {
            m_pkImages[(int)E_IMAGE.pressButton].gameObject.SetActive(true);
            m_pkImages[(int)E_IMAGE.Fill].gameObject.SetActive(false);
        }

        public void StartGame()
        {
            BaseManager.instance.convertScene(SceneNum.LogoScene.ToString(), SceneNum.LobbyScene);
        }

        #region 네트워크 안내판

        ///// <summary> 네트워크 연결 안내문 표시 </summary>
        //public void popNetException()
        //{
        //    m_pkImages[(int)E_IMAGE.networkExcep].gameObject.SetActive(true);
        //}

        ///// <summary> 접속 재시도 </summary>
        //public void reConnect()
        //{
        //    m_pkImages[(int)E_IMAGE.networkExcep].gameObject.SetActive(false);

        //    StartCoroutine(showLogo());
        //}

        /// <summary> 종료 </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        #endregion
    }
}