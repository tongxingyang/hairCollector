using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace week
{
    public class NotificationPopup : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] TextMeshProUGUI _explain;
        ContentSizeFitter _fitter;
        [SerializeField] Button[] _Btns;
        [SerializeField] Image _updateImg;
        [SerializeField] TextMeshProUGUI _updateExplain;
        [SerializeField] TextMeshProUGUI[] _btnTxt;

        enum pType { notif, net, inspection }
        pType ptype;

        private void Awake()
        {
            _Btns[0].onClick.AddListener(Btn_stand);
            _Btns[1].onClick.AddListener(Btn_AppClose);
            _btnTxt[1].text = "종료";

            _fitter = _explain.GetComponent<ContentSizeFitter>();
        }

        #region [ noti ]

        [Button]
        public void networkError()
        {
            StartCoroutine(netError_routine());
        }

        IEnumerator netError_routine()
        {
            gameObject.SetActive(true);

            ptype = pType.net;

            _title.text = "네트워크 연결 오류";
            _explain.text = "네트워크 연결을 확인해 주세요.";

            _updateImg.gameObject.SetActive(false);
            _updateExplain.gameObject.SetActive(false);

            _fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            yield return new WaitForEndOfFrame();
            _fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _btnTxt[0].text = "재시도";
            _Btns[1].gameObject.SetActive(true);
        }

        [Button]
        public void newVersionChker()
        {
            StartCoroutine(newVersion_routine());
        }

        IEnumerator newVersion_routine()
        {
            gameObject.SetActive(true);

            ptype = pType.notif;

            _title.text = "업데이트";
            _explain.text = "새 업데이트가 나왔습니다." + System.Environment.NewLine + "스토어에서 새 버전으로 업데이트 해주세요.";

            _updateImg.gameObject.SetActive(true);
            _updateExplain.gameObject.SetActive(true);

            _fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            yield return new WaitForEndOfFrame();
            _fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _btnTxt[0].text = "업데이트";
            _Btns[1].gameObject.SetActive(true);
        }

        [Button]
        public void setInspection()
        {
            gameObject.SetActive(true);
            StartCoroutine(inspection_routine());
        }

        IEnumerator inspection_routine()
        {
            gameObject.SetActive(true);

            ptype = pType.inspection;

            _title.text = "긴급점검";
            _explain.text = "눈사람님!" + System.Environment.NewLine +
                "다시 접속해주셔서 감사합니다!" + System.Environment.NewLine + System.Environment.NewLine +
                "현재 알 수 없는 오류" + System.Environment.NewLine +
                "혹은 더 나은 서비스를 위해" + System.Environment.NewLine +
                "긴급 점검중입니다." + System.Environment.NewLine +
                "잠시후에 다시 접속해주세요." + System.Environment.NewLine + System.Environment.NewLine +
                "이용에 불편을 드려죄송합니다." + System.Environment.NewLine +
                "문의사항은 까페를 통해 문의해주세요.";

            _updateImg.gameObject.SetActive(false);
            _updateExplain.gameObject.SetActive(false);

            _fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            yield return new WaitForEndOfFrame();
            _fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _btnTxt[0].text = "종료";
            _Btns[1].gameObject.SetActive(false);
        }

        #endregion

        public void Btn_stand()
        {
            switch (ptype)
            {
                case pType.notif: // 업데이트
                    Debug.Log("업데이트");
                    Application.OpenURL("https://play.google.com/store/apps/details?id=com.munzi.snowAdventure");
                    break;
                case pType.net: // 재시도
                    Debug.Log("재시도");
                    BaseManager.instance.convertScene(SceneNum.LogoScene.ToString(), SceneNum.LogoScene);
                    break;
                case pType.inspection: // 점검
                    Debug.Log("종료");
                    Application.Quit();
                    break;
            }
        }

        /// <summary> 종료 </summary>
        public void Btn_AppClose()
        {
            Application.Quit();            
        }
    }
}