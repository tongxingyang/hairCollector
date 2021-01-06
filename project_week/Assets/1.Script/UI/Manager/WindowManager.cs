using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class WindowManager : TSingleton<WindowManager>
    {
        #region [ member Window ]

        loadScene           win_loading;
        messagePopup        win_message;
        celebrationEffect   win_celebrate;
        coingenerator       win_coinGenerator;
        purchasePopup       win_purchase;
        accountListWin win_accountList;

        public loadScene        Win_loading         { get => win_loading; }
        public messagePopup     Win_message         { get => win_message; }
        public celebrationEffect Win_celebrate      { get => win_celebrate; }
        public coingenerator    Win_coinGenerator   { get => win_coinGenerator; }
        public purchasePopup    Win_purchase        { get => win_purchase; }
        public accountListWin   Win_accountList     { get => win_accountList; }

        #endregion

        #region member Dictionary

        /// <summary> 윈도우 팩 </summary>
        Dictionary<Windows, GameObject> _windowPack;

        /// <summary> 윈도우 팩 </summary>
        public Dictionary<Windows, GameObject> WindowPack
        {
            get { return _windowPack; }
        }

        #endregion

        protected override void Init()
        {
            gameObject.layer = 5;

            Canvas canvas = gameObject.AddComponent<Canvas>();            
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingLayerName = "popup";
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1300f;

            RectTransform rect = (RectTransform)canvas.transform;
            rect.position = new Vector3(0, 0, -300f);
            rect.sizeDelta = new Vector2(1440f, 2960f);
        }

        #region win

        /// <summary> 윈도우 프리랩 로드 -> 생성 -> 팩에 저장 </summary>
        public bool LoadWin()
        {
            _windowPack = new Dictionary<Windows, GameObject>();

            for (Windows win = (Windows)0; win < Windows.max; win++)
            {
                if (!WindowPack.ContainsKey(win))
                {
                    GameObject go = Resources.Load("Windows/" + win.ToString()) as GameObject;

                    if (go == null)
                    {
                        Debug.Log(win.ToString());
                    }
                    WindowPack.Add(win, Instantiate(go));
                    WindowPack[win].transform.SetParent(transform);
                    WindowPack[win].transform.localPosition = Vector3.zero;
                    WindowPack[win].transform.localScale = Vector3.one;
                    ((RectTransform)WindowPack[win].transform).sizeDelta = Vector2.zero;
                }
            }

            windowsInit();

            return true;
        }

        public void windowsInit()
        {
            win_loading         = _windowPack[Windows.win_loadScene].GetComponent<loadScene>();
            win_message         = _windowPack[Windows.win_message].GetComponent<messagePopup>();
            win_purchase        = _windowPack[Windows.win_purchase].GetComponent<purchasePopup>();
            win_celebrate       = _windowPack[Windows.win_celebrateEff].GetComponent<celebrationEffect>();
            win_coinGenerator   = _windowPack[Windows.win_coinAni].GetComponent<coingenerator>();
            win_accountList     = _windowPack[Windows.win_accountList].GetComponent<accountListWin>();

            win_purchase.WhenClose = win_celebrate.allClose;
        }

        /// <summary> 팩에서 원하는 창 온오프 </summary>
        public GameObject openWin(Windows win)
        {
            if (WindowPack.ContainsKey(win))
            {
                WindowPack[win].GetComponent<UIInterface>().open();
            }
            else
            {
                LoadWin();
                Debug.Log("해당 창은 아직 미생성이거나 방금 생성됬거나 없습니다.");
            }

            if (!WindowPack.ContainsKey(win))
            {
                Debug.Log("??? 뭐지? 없대.");
                return null;
            }

            return WindowPack[win];
        }

        /// <summary> 팩에서 원하는 창 게임오브젝트로 받아옴 </summary>
        public GameObject getWin(Windows win)
        {
            if (WindowPack.ContainsKey(win))
            {
                return WindowPack[win];
            }
            else
            {
                LoadWin();
            }

            if (!WindowPack.ContainsKey(win))
            {
                Debug.Log("??? 뭐지? 없대.");
                return null;
            }

            return WindowPack[win];
        }

        /// <summary> 메세지 + 메세지 후 이벤트 </summary>
        public void showActMessage(string str, Action act)
        {
            win_message.showActMessage(str, act);
        }

        /// <summary> 팩에 있는 메세지 창을 통한 메세지 출력 (비어 있음) </summary>
        public void showMessage(string str)
        {
            win_message.showMessage(str);
        }

        #endregion
    }
}