using BansheeGz.BGDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class purchasePopup : MonoBehaviour, UIInterface
    {
        public enum purchaseType { store, status }

        [Header("store Purchase")]

        [SerializeField] RectTransform      _storePurchasePanel;
        [SerializeField] TextMeshProUGUI    _storePurchaseTitle;
        [SerializeField] Image              _storePurchasePresent;
        [SerializeField] TextMeshProUGUI    _storePurchaseNotice;

        [Header("store Purchase")]

        [SerializeField] GameObject _skinPurchasePanel;
        [SerializeField] TextMeshProUGUI _skinName;
        [SerializeField] Image _skin;
        [SerializeField] Image _skinback;
        [SerializeField] Image _skincase;

        [Header("status Purchase")]

        [SerializeField] GameObject         _statusPurchasePanel;
        [SerializeField] TextMeshProUGUI    _statusPurchaseTitle;   // 이름
        [SerializeField] Image              _statusPurchasePresent; // 이미지
        [SerializeField] Image[]            _statusPurchaseCase;    // 랭크케이스
        [SerializeField] TextMeshProUGUI[] _statusPurchaseRankInfo; // 랭크값
        [SerializeField] TextMeshProUGUI[]  _statusPurchaseVal;     // 스탯값
        [SerializeField] TextMeshProUGUI    _statusPurchaseInfo;    // 세부 정보      
        [Space]
        [SerializeField] Color _newGreen; // 새 초록색

        enum type { store, skin, status }
        void openPurchaseCheck(type t)
        {
            _storePurchasePanel.gameObject.SetActive(t == type.store);
            _skinPurchasePanel.SetActive(t == type.skin);
            _statusPurchasePanel.SetActive(t == type.status);
        }

        Action _after = null;
        bool _closable = true;

        public Action WhenClose { get; set; }

        void Awake()
        {
            close();
        }

        /// <summary> 상점 구매 </summary>
        public purchasePopup setStorePurchase(Sprite sp, string str, Action after = null)
        {
            SoundManager.instance.PlaySFX(SFX.purchase);
            _closable = false;
            openPurchaseCheck(type.store);

            _storePurchaseTitle.text = str + System.Environment.NewLine + "구매 완료!";

            _storePurchasePresent.sprite = sp; 
            _storePurchasePresent.SetNativeSize();
            _storePurchasePresent.GetComponent<RectTransform>().sizeDelta = Vector3.one * 750f;
            //_storePurchasePresent.transform.localScale = Vector3.one * 2f;

            _storePurchaseNotice.text = "※구매하신 품목은 우편함으로 보내집니다.";

            _after = after;
            open();

            StartCoroutine(chkTime(0.5f));

            return this;
        }

        /// <summary> 스킨 구매 </summary>
        public purchasePopup setSkinPurchase(SkinKeyList sk, Action after = null)
        {
            SoundManager.instance.PlaySFX(SFX.purchase);
            _closable = false;
            openPurchaseCheck(type.skin);

            _skinName.text = D_skin.GetEntity(sk.ToString()).f_skinname;
            _skinName.color = gameValues._skinRankColor[D_skin.GetEntity(sk.ToString()).f_rank];
            _skin.sprite = DataManager.SkinSprite[sk];
            _skinback.color = gameValues._skinRankColor[D_skin.GetEntity(sk.ToString()).f_rank];
            _skincase.color = gameValues._skinRankColor[D_skin.GetEntity(sk.ToString()).f_rank];

            _after = after;
            open();

            StartCoroutine(chkTime(0.5f));

            return this;
        }

        /// <summary> 쿠폰 습득 </summary>
        public purchasePopup setCoupon(Sprite sp, string str, Action after = null)
        {
            SoundManager.instance.PlaySFX(SFX.purchase);
            _closable = false;
            openPurchaseCheck(type.store);

            _storePurchaseTitle.text = str;

            _storePurchasePresent.sprite = sp;
            _storePurchasePresent.SetNativeSize();
            _storePurchasePresent.GetComponent<RectTransform>().sizeDelta = Vector3.one * 750f;
            //_storePurchasePresent.transform.localScale = Vector3.one * 2f;

            _storePurchaseNotice.text = "※감사합니다!";

            _after = after;
            open();

            StartCoroutine(chkTime(0.5f));

            return this;
        }

        /// <summary> 능력치 구매 </summary>
        public purchasePopup setStatusPurchase(statusKeyList stat, int upMount, Action after = null)
        {
            SoundManager.instance.PlaySFX(SFX.statup);
            _closable = false;
            openPurchaseCheck(type.status);

            _statusPurchaseTitle.text = D_status.GetEntity(stat.ToString()).f_sName;
            _statusPurchaseInfo.text = D_status.GetEntity(stat.ToString()).f_info;

            _statusPurchasePresent.sprite = DataManager.Statusicon[stat];
            ((RectTransform)_statusPurchasePresent.transform).sizeDelta = Vector2.one * 500f;

            // 현재 스탯 레벨
            int level = BaseManager.userGameData.StatusLevel[(int)stat];

            // 강화 이전
            int prev = getStatusRank(stat, -upMount);
            _statusPurchaseRankInfo[0].text     = $"{gameValues._statusRankData[prev]._name} {level - upMount - gameValues._tier[prev]}";
            _statusPurchaseRankInfo[0].color    = gameValues._statusRankData[prev]._color;
            _statusPurchaseCase[0].color        = gameValues._statusRankData[prev]._color;

            float val = BaseManager.userGameData.getAddit(stat, -upMount);
            if (stat == statusKeyList.cool)
                val = (1 - val) * 100f;
            _statusPurchaseVal[0].text = val.ToString();

            // 강화 후
            int next = getStatusRank(stat);
            string str = (prev == next) ? $"{level - gameValues._tier[next]}" : "승급!";
            _statusPurchaseRankInfo[1].text     = $"{gameValues._statusRankData[next]._name} {str}";
            _statusPurchaseRankInfo[1].color    = (prev == next) ? gameValues._statusRankData[next]._color : _newGreen;
            _statusPurchaseCase[1].color        = gameValues._statusRankData[next]._color;
            val = BaseManager.userGameData.getAddit(stat);
            if (stat == statusKeyList.cool)
                val = (1 - val) * 100f;
            _statusPurchaseVal[1].text          = val.ToString();

            _after = after;
            open();

            StartCoroutine(chkTime(0.5f));

            return this;
        }

        /// <summary> 해당 랭크 가져오기 </summary>
        int getStatusRank(statusKeyList stat, int add = 0)
        {
            int lvl = BaseManager.userGameData.StatusLevel[(int)stat] + add;

            int[] tier = new int[4] { D_status.GetEntity(stat.ToString()).f_bronze,
                                        D_status.GetEntity(stat.ToString()).f_silver,
                                        D_status.GetEntity(stat.ToString()).f_gold,
                                        D_status.GetEntity(stat.ToString()).f_platinum };

            for (int i = 0; i < 4; i++)
            {
                if (lvl < tier[i])
                {
                    return i;                    
                }
            }

            return 4;
        }

        public void setImgSize(bool isBig=true)
        {
            if (isBig)
            {
                _storePurchasePanel.sizeDelta = new Vector2(950f, 1350f);
                _storePurchasePresent.transform.localPosition = Vector3.zero;
            }
            else
            {
                _storePurchasePanel.sizeDelta = new Vector2(850f, 1150f);
                _storePurchasePresent.transform.localPosition = new Vector3(0, -75f);
            }
        }

        public void open()
        {
            gameObject.SetActive(true);
        }

        IEnumerator chkTime(float t)
        {
            while (t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            _closable = true;
        }

        public void close()
        {
            if (_closable)
            {
                WhenClose?.Invoke();
                _after?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}