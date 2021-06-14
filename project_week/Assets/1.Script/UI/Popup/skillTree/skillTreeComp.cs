using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

namespace week
{
    public enum btnType { rootType, standardType, choiceType, lockType }
    public class skillBtnData
    {
        public skill Refer { get; private set; }
        public btnType BtnType { get; set; }
        public skillBtnData() { reset(); }
        public skillBtnData(skill Refer, btnType btnType)
        {
            this.Refer = Refer;
            this.BtnType = btnType;
        }
        public void reset()
        {
            Refer = null;
            BtnType = btnType.standardType;
        }
        //public static bool operator ==(skillBtnData a, skillBtnData b)
        //{
        //    return (a._type == b._type) && (a._btnType == b._btnType);
        //}
        //public static bool operator !=(skillBtnData a, skillBtnData b)
        //{
        //    return !((a._type == b._type) && (a._btnType == b._btnType));
        //}
        public void copy(skillBtnData d)
        {
            Refer = d.Refer;
            BtnType = d.BtnType;
        }
    }

    public enum skillType { launch, range, rush, shield, environment, summon }
    public class skillTreeComp : MonoBehaviour
    {
        
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] Image _titleBox;
        [SerializeField] Image _panel;
        [SerializeField] TextMeshProUGUI _explainName;
        [SerializeField] TextMeshProUGUI _explain;
        [SerializeField] TextMeshProUGUI _medalCount;

        [SerializeField] GameObject[] _tapObjs;
        [SerializeField] Image _apply;
        [SerializeField] TextMeshProUGUI _applyTxt;
        [SerializeField] Button _up;
        [SerializeField] TextMeshProUGUI _upTxt;

        [Header("Unlock")]
        [SerializeField] Button _unlock;
        [SerializeField] Button _unlockSkill;
        [SerializeField] TextMeshProUGUI _unlockTitle;
        [SerializeField] Image _unBack;
        [SerializeField] Image _unSkill;
        [SerializeField] Image _unCase;
        [SerializeField] CanvasGroup _unLight;
        [SerializeField] TextMeshProUGUI _unPrice;
        [SerializeField] Animator _ani;
        [SerializeField] GameObject _guide;
        [SerializeField] GameObject _particle;
        bool _isUnlocking = false;
        bool _unlocked;
        bool IsUnlocking
        {
            get => _isUnlocking;
            set { _isUnlocking = value; _guide.SetActive(!value); }
        }

        [Header("info")]
        [SerializeField] RectTransform _infoTriangle;
        [SerializeField] RectTransform _infoBox;
        [SerializeField] TextMeshProUGUI _info;

        public Sprite getRankBacks(int rank) => _gameResource._rankBack[rank % 10];
        public Sprite getRankCases(int rank) => _gameResource._rankCase[rank % 10]; 
        public Sprite _lockCase { get => _gameResource._rankLockCase; }
        public Sprite getWRankCases(int rank) => _gameResource._wRankCase[rank % 10];
        public Sprite _wlockCase { get => _gameResource._wRankLockCase; }

        ISkillTap[] _taps;
        Action _refresh;
        Action _whenCloseUpgradePanel;

        PlayerCtrl _player;
        upgradePopup _upgrade;
        GameResource _gameResource;
        public PlayerCtrl player { get => _player; }
        skillType _nowSkillType;
        public skillBtnData _nowData { get; set; }

        public void Init(GameScene gs, Action whenCloseUpgradePanel)
        {
            _player = gs.Player;
            _gameResource = gs._gameResource;
            _upgrade = gs.UpGradePopup;

            _whenCloseUpgradePanel = whenCloseUpgradePanel;
            _nowData = new skillBtnData();

            _taps = new ISkillTap[_tapObjs.Length];
            for (int i = 0; i < _taps.Length; i++)
            {
                _tapObjs[i].SetActive(false);
                _taps[i] = _tapObjs[i].GetComponent<ISkillTap>();
                _taps[i].Init(gs, this);
            }

            _up.onClick.AddListener(upSkill);
            
            _infoTriangle.gameObject.SetActive(false);

            unlockInit();
        }

        /// <summary> 각 스킬별 탭이 열릴때 </summary>
        public void openTap(SkillKeyList type)
        {
            _medalCount.text = $"x{_player.Inventory[gainableTem.questKey]}";

            for (int i = 0; i < _taps.Length; i++)
            {
                _tapObjs[i].SetActive(false);
            }

            int rank = D_skill.GetEntity(type.ToString()).f_rank;
            _nowSkillType = (skillType)(rank / 10);
            _upTxt.text = $"+{_player.skillAddDmg[(int)_nowSkillType]}%";
            Color col = Color.white;

            switch (_nowSkillType)
            {
                case skillType.launch:
                    _title.text = "발사<size=30>(눈덩이류)</size>";                    
                    ColorUtility.TryParseHtmlString("#3197DDFF", out col);                    
                    break;
                case skillType.range:
                    _title.text = "광역<size=30>(얼덩이류)</size>";
                    ColorUtility.TryParseHtmlString("#468B96FF", out col);
                    break;
                case skillType.rush:
                    _title.text = "돌파<size=30>(전방류)</size>";
                    ColorUtility.TryParseHtmlString("#474D96FF", out col);
                    break;
                case skillType.shield:
                    _title.text = "실드<size=30>(실드류)</size>";
                    ColorUtility.TryParseHtmlString("#874696FF", out col);
                    break;
                case skillType.environment:
                    _title.text = "환경<size=30>(디버프류)</size>";
                    ColorUtility.TryParseHtmlString("#B76C27FF", out col);
                    break;
                case skillType.summon:
                    _title.text = "소환<size=30>(보조류)</size>";
                    ColorUtility.TryParseHtmlString("#F8DB58FF", out col);
                    break;
                default:
                    Debug.LogError("잘못습득");
                    break;
            }
            _titleBox.color = col;
            _panel.color = Color.white;

            _nowData.reset();
            chkOpenApply(false, false);
            _explainName.text = "--";
            _explain.text = "--";

            _taps[(int)_nowSkillType].OnOpen();
            _refresh = _taps[(int)_nowSkillType].refreshSkill;
        }

        public void setTitle(string str) { _title.text = str; }
        public void setInfo(string name, string expl) 
        {
            _explainName.text = name;
            _explain.text = expl;

            _refresh?.Invoke();
        }

        /// <summary> 스킬 선택
        /// 확인 or 해금 </summary>
        public void SelectSKill()
        {
            if (_nowData.Refer.IsLock)
            {
                // 해금 창 오픈
                openUnlock();
            }
            else
            {
                _player.getSkill(_nowData.Refer.Type, _upgrade.Noti);
                gameObject.SetActive(false);
                _whenCloseUpgradePanel?.Invoke();
            }
        }

        /// <summary> 맨밑에 버튼 </summary>
        public void chkOpenApply(bool isNowLock, bool isOn)
        {
            _apply.raycastTarget = isOn;

            if (isNowLock)
            {
                bool bl = (isOn && _nowData.Refer != null && _player.Inventory[gainableTem.questKey] >= _nowData.Refer.shouldHaveKey);
                _apply.color = bl ? Color.cyan : Color.grey;
                _apply.raycastTarget = bl;
                _applyTxt.text = "해 금";
            }
            else
            {
                _apply.color = (isOn) ? Color.red : Color.grey;
                _apply.raycastTarget = isOn;
                _applyTxt.text = "선 택";
            }
        }

        /// <summary> 스킬 트리별 강화 </summary>
        public void upSkill()
        {
            _player.skillAddDmg[(int)_nowSkillType]++;

            gameObject.SetActive(false);
            _whenCloseUpgradePanel?.Invoke();
        }

        #region [ unlock ]

        void unlockInit()
        {
            _unlock.onClick.AddListener(closeUnlock);
            _unlockSkill.onClick.AddListener(skillUnlock);

            closeUnlock();
        }

        /// <summary> 해금 오픈 </summary>
        void openUnlock()
        {
            _unlock.gameObject.SetActive(true);
            _particle.SetActive(false);
            _ani.enabled = true;
            IsUnlocking = false;
            _unlocked = false;

            _unlockTitle.text = D_skill.GetEntity(_nowData.Refer.Type.ToString()).f_skill_name;

            _unBack.sprite = _gameResource._rankBack[0];
            _unSkill.sprite = DataManager.Skillicon[_nowData.Refer.Type];
            _unCase.sprite = _gameResource._rankLockCase;

            _unPrice.text = $"x {_nowData.Refer.shouldHaveKey}";
            _unPrice.raycastTarget = true;

            _unLight.alpha = 0f;
        }

        /// <summary> 해금 닫기 </summary>
        void closeUnlock()
        {
            if (_isUnlocking == false)
            {
                bool isNowLock = true;
                if (_unlocked)
                {
                    _taps[(int)_nowSkillType].OnOpen();
                    isNowLock = false;
                }

                _unlock.gameObject.SetActive(false);

                chkOpenApply(isNowLock, true);
            }
        }

        /// <summary> 해금! </summary>
        void skillUnlock()
        {
            if (_unlocked)
                return;

            if (_player.Inventory[gainableTem.questKey] >= _nowData.Refer.shouldHaveKey)
            {
                IsUnlocking = true;
                _unlocked = true;
                _ani.enabled = false;

                _player.Inventory[gainableTem.questKey] -= _nowData.Refer.shouldHaveKey;

                _unLight.DOFade(1f, 2f).OnComplete(() =>
                {
                    _nowData.Refer.unLock();

                    _particle.SetActive(true);

                    _unBack.sprite = getRankBacks(D_skill.GetEntity(_nowData.Refer.Type.ToString()).f_rank);
                    _unSkill.sprite = DataManager.Skillicon[_nowData.Refer.Type];
                    _unCase.sprite = getRankCases(D_skill.GetEntity(_nowData.Refer.Type.ToString()).f_rank);

                    _unPrice.text = "해금완료";
                    _unPrice.raycastTarget = false;

                    _unLight.alpha = 0f;

                    IsUnlocking = false;
                });
            }
        }

        #endregion

        #region [ info ]

        public void openInfo(RectTransform pos, SkillKeyList skl)
        {   
            StartCoroutine(infoSet(pos, skl));
        }

        IEnumerator infoSet(RectTransform skillPos, SkillKeyList skl)
        {
            _infoTriangle.gameObject.SetActive(true);
            _infoBox.localPosition = new Vector2(Screen.width, Screen.height);

            ContentSizeFitter csf = _infoBox.GetComponent<ContentSizeFitter>();
            csf.enabled = false;

            _infoTriangle.anchoredPosition = skillPos.anchoredPosition + new Vector2(0f, 50f);

            string str = $"<size=50>{D_skill.GetEntity(skl.ToString()).f_skill_name}</size>" + System.Environment.NewLine
                + D_skill.GetEntity(skl.ToString()).f_info + System.Environment.NewLine
                + D_skill.GetEntity(skl.ToString()).f_explain;
            _info.text = str;

            yield return new WaitForEndOfFrame();

            csf.enabled = true;

            yield return new WaitForEndOfFrame();

            float rate = (skillPos.anchoredPosition.x) / (Screen.width * 0.5f);
            float width = (_infoBox.rect.width - 50f) * 0.5f * rate;
            _infoBox.localPosition = new Vector3(-width, 80f);
        }

        public void closeInfo()
        {
            _infoTriangle.gameObject.SetActive(false);
        }

        #endregion
    }
}