using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace week
{
    public enum NotiType { levelUp, clearQuest, takeQuest,
                           boss, heal, coin, gem, exp, sward, present, non }
    public class notiData
    { 
        public NotiType _type;
        public SkillKeyList _skill;
        public InQuestKeyList _questType;
        public inQuest_reward_key _rewardKey;
        public inQuest_reward_valtype _rewardType;

        public notiData(NotiType type)
        {
            _type = type;
        }
    }

    public class qNotiBox : MonoBehaviour
    {
        [Header("head")]
        [SerializeField] RectTransform _box;
        CanvasGroup _group;
        [SerializeField] TextMeshProUGUI _title;
        [Header("goal")]
        [SerializeField] GameObject _goal;
        [SerializeField] Image _goalImg;
        [SerializeField] Image _marker;
        [Header("reward")]
        [SerializeField] Transform _reward;
        [SerializeField] Image _rewardImg;

        GameScene _gs;
        NotiType _noti;
        int _order;

        Sequence _startSeq;
        Action<qNotiBox, int> _whenMidNoti;
        Action<qNotiBox> _whenEndNoti;

        private void Awake()
        {
            _group = _box.GetComponent<CanvasGroup>();
        }

        public void Init(GameScene gs, Action<qNotiBox, int> whenMidNoti, Action<qNotiBox> whenEndNoti)
        {
            _gs = gs;
            _whenMidNoti = whenMidNoti;
            _whenEndNoti = whenEndNoti;
        }

        public void setting(notiData noti, int num)
        {
            _noti = noti._type;
            _order = num;

            switch (_noti)
            {
                case NotiType.levelUp:
                    {
                        _title.text = "레벨업!";
                        _goal.SetActive(true);
                        _goalImg.sprite = DataManager.SkinSprite[_gs.Player.Skin];
                        //_marker
                        _reward.localPosition = new Vector3(35f, -25f);
                        _rewardImg.sprite = DataManager.Skillicon[noti._skill];
                    }
                    break;
                case NotiType.clearQuest:
                    {
                        _title.text = "퀘스트 완료";
                        _goal.SetActive(true);
                        _goalImg.sprite = DataManager.QuestSprite[noti._questType];
                        //_marker
                        _reward.localPosition = new Vector3(35f, -25f);
                        if (noti._rewardKey == inQuest_reward_key.get_stat
                            || noti._rewardKey == inQuest_reward_key.get_skill)
                        {
                            _rewardImg.sprite = DataManager.Skillicon[noti._skill];
                        }
                        else if (noti._rewardKey == inQuest_reward_key.get_coin)
                        {
                            _rewardImg.sprite = DataManager.NotiSprite[NotiType.coin];
                        }
                    }
                    break;
                case NotiType.present:
                    {
                        _title.text = "선물 습득";
                        _goal.SetActive(true);
                        _goalImg.sprite = DataManager.NotiSprite[_noti]; // 선물이미지
                        //_marker
                        _reward.localPosition = new Vector3(35f, -25f);
                        _rewardImg.sprite = DataManager.Skillicon[noti._skill];
                    }
                    break;
                case NotiType.boss:
                    {
                        _title.text = "보스 처치!";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
                case NotiType.takeQuest:
                    {
                        _title.text = "의뢰";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.QuestSprite[noti._questType];
                    }
                    break;
                case NotiType.heal:
                    {
                        _title.text = "체력 회복";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
                case NotiType.coin:
                    {
                        _title.text = "코인 획득";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
                case NotiType.gem:
                    {
                        _title.text = "보석 획득";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
                case NotiType.exp:
                    {
                        _title.text = "경험치꽃 획득";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
                case NotiType.sward:
                    {
                        _title.text = "용사의 검 획득";
                        _goal.SetActive(false);
                        _reward.localPosition = new Vector3(-25f, -25f);
                        _rewardImg.sprite = DataManager.NotiSprite[_noti];
                    }
                    break;
            }
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            _box.localPosition = new Vector3(0f, -155f);
            _group.alpha = 0;

            _startSeq = DOTween.Sequence();
            _startSeq.Append(_box.DOAnchorPosY(0f, 1f).SetEase(Ease.OutSine))
                .Join(_group.DOFade(1f, 1f)).SetEase(Ease.OutSine)
                .AppendInterval(gameValues._notiDuration)
                .AppendCallback(()=> { _whenMidNoti?.Invoke(this, _order); })
                .Append(_box.DOAnchorPosY(75f, 0.3f))
                .Join(_group.DOFade(0f, 0.3f))
                .OnComplete(()=> { _whenEndNoti?.Invoke(this); gameObject.SetActive(false); });  
        }
    }
}