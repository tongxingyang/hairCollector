using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    public class NotificationPopup : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] TextMeshProUGUI _explain;
        [SerializeField] GameObject _2ndBtn;
        [SerializeField] TextMeshProUGUI[] _btnTxt;

        enum pType { notif, net }
        pType ptype;

        public void networkError()
        {
            gameObject.SetActive(true);

            ptype = pType.net;

            _title.text = "네트워크 연결 오류";
            _explain.text = "네트워크 연결을 확인해 주세요.";
            _2ndBtn.SetActive(true);
            _btnTxt[0].text = "재시도";
            _btnTxt[1].text = "종료";
        }

        public void newVersionChker()
        {
            gameObject.SetActive(true);

            ptype = pType.notif;

            _title.text = "업데이트";
            _explain.text = "새 업데이트가 나왔습니다."+System.Environment.NewLine+"스토어에서 새 버전으로 업데이트 해주세요.";
            _2ndBtn.SetActive(false);
            _btnTxt[0].text = "업데이트";
        }

        public void Btn_stand()
        {
            switch (ptype)
            {
                case pType.notif: // 업데이트
                    Debug.Log("업데이트");
                    Application.OpenURL("https://www.google.com/");
                    break;
                case pType.net: // 재시도
                    Debug.Log("재시도");
                    BaseManager.instance.convertScene(SceneNum.LogoScene.ToString(), SceneNum.LogoScene);
                    break;
            }
        }

        public void Btn_added()
        {
            switch (ptype)
            {
                case pType.notif: // 오프라인 플레이
                    Debug.Log("오프라인");
#if UNITY_ANDROID
                    // Application.Quit();
#endif
                    break;
                case pType.net: // 없음
                    Debug.LogError("Btn_added : Error");
                    break;
            }
        }
    }
}