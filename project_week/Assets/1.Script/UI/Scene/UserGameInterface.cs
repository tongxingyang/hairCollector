using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using NaughtyAttributes; // 지우슈

namespace week
{
    public class UserGameInterface : MonoBehaviour
    {
        [SerializeField] RectTransform _ExpBar;
        [Space]
        [SerializeField] GameObject _coinIcon;
        [SerializeField] TextMeshProUGUI _coinTxt;
        [SerializeField] GameObject _gemIcon;
        [SerializeField] TextMeshProUGUI _gemTxt;
        //[SerializeField] GameObject _apIcon;
        //[SerializeField] TextMeshProUGUI _apTxt;

        [Space]
        [SerializeField] GameObject _bossKillMark;
        [SerializeField] TextMeshProUGUI _bossKillTxt;
        [Header("etc")]
        [SerializeField] TextMeshProUGUI _lvlTmp;

        [Space]
        [Header("Quest icon")]
        [SerializeField] GameObject _qNotiFab;
        [SerializeField] Transform _noti_parent;
        Queue<qNotiBox> _waitNoti;
        List<qNotiBox> _usingNoti;
        Vector3 origin = new Vector3(0, 440f);
        Vector3 interv = new Vector3(0, -165f);
        int _lastShow = 0;
        float _expWidth;

        public bool gemIcon { set { _gemIcon.SetActive(value); _gemTxt.gameObject.SetActive(value); } }

        GameScene _gs;
        Vector3 wealth;

        float ExpFillAmount
        {
            set => _ExpBar.sizeDelta = new Vector2(_expWidth * value, 80f);
        }

        public void Init(GameScene gs)
        {
            _gs = gs;

            gemIcon = false;

            ExpFillAmount = 0f;

            // 알림 아이콘
            _waitNoti = new Queue<qNotiBox>();
            _usingNoti = new List<qNotiBox>();

            wealth = _gemIcon.GetComponentInParent<HorizontalLayoutGroup>().transform.localPosition;

            _expWidth = Screen.width * 2960f / Screen.height;
            // StartCoroutine(notiRoutine());
        }

        /// <summary> 레벨 새로고침 (UI상에서) </summary>
        public void LevelRefresh(int val)
        {
            _lvlTmp.text = $"lvl.{val.ToString()}";
        }

        /// <summary> 경험치 새로고침 (UI상에서) </summary>
        public void ExpRefresh(float val)
        {
            ExpFillAmount = val;
        }

        /// <summary> 보스킬 새로고침 (UI상에서) </summary>
        public void bossKillRefresh(int val)
        {
            _bossKillMark.SetActive(true);
            _bossKillTxt.text = val.ToString();
        }

        /// <summary> 코인 새로고침 (UI상에서) </summary>
        public void getCoin(float coin, bool isAni = false)
        {
            if (isAni)
            {
                WindowManager.instance.Win_coinGenerator.getDirect(_coinIcon.transform.position, currency.coin, 1);
            }

            _coinTxt.text = Convert.ToInt32(coin).ToString();
        }

        /// <summary> 젬 새로고침 (UI상에서) </summary>
        public void GemRefresh(int val)
        {
            gemIcon = true;
            WindowManager.instance.Win_coinGenerator.getDirect(_gemIcon.transform.localPosition + wealth, currency.gem, 1);
        }

        public void wealthRefresh(int gem)
        {
            _gemTxt.text = gem.ToString();
        }

        //================================[ Quest Notifi ]=====================================

        IEnumerator notiRoutine()
        {
            while (_gs.GameOver == false)
            {
                for (int i = 0; i < _usingNoti.Count; i++)
                {
                    _usingNoti[i].transform.localPosition = Vector2.MoveTowards(_usingNoti[i].transform.localPosition, origin + interv * i, 100f * Time.deltaTime);
                }

                yield return null;
            }
        }

        public void getNotiUI(notiData data)
        {
            qNotiBox noti = getNoti();

            _usingNoti.Add(noti);
            _lastShow++;

            noti.transform.localPosition = origin + interv * _lastShow;
            noti.transform.localScale = Vector3.one;
            noti.setting(data, _lastShow);
        }

        qNotiBox getNoti()
        {
            if (_waitNoti.Count > 0)
            {
                return _waitNoti.Dequeue();
            }

            qNotiBox noti = Instantiate(_qNotiFab).GetComponent<qNotiBox>();
            noti.transform.SetParent(_noti_parent);
            noti.Init(_gs,
                (notibox, n) => {
                    _usingNoti.Remove(notibox);
                    if (_lastShow == n)
                        _lastShow = 0; 
                },
                (notibox) => { _waitNoti.Enqueue(notibox); });

            return noti;
        }
    }
}