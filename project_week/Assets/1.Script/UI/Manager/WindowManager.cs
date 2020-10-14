using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class WindowManager : TSingleton<WindowManager>
    {
        #region member Dictionary

        /// <summary> 윈도우 팩 </summary>
        Dictionary<Windows, GameObject> _windowPack;

        messagePopup MSp;

        #endregion



        #region member Dictionary properties

        /// <summary> 윈도우 팩 </summary>
        public Dictionary<Windows, GameObject> WindowPack
        {
            get { return _windowPack; }
        }

        #endregion

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
                }
            }

            MSp = _windowPack[Windows.messagePopup].GetComponent<messagePopup>();
            Debug.Log("window 로딩 완료.");
            return true;
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
            MSp.showActMessage(str, act);
        }

        /// <summary> 팩에 있는 메세지 창을 통한 메세지 출력 (비어 있음) </summary>
        public void showMessage(string str)
        {
            MSp.showMessage(str);
        }

        #endregion
    }
}