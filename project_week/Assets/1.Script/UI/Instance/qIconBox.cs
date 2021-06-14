using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using NaughtyAttributes; // 지워도 댐

namespace week
{
    public class qIconBox : MonoBehaviour
    {
        [SerializeField] public RectTransform _showBox;
        [SerializeField] public TextMeshProUGUI _qInfo;
        [SerializeField] public Image _progress;
        [SerializeField] public RectTransform _newEff;
        [SerializeField] public RectTransform _effRect;

        inquest _iq;

        Sequence startSeq;
        Sequence EndSeq;
        public bool IsUse { get; private set; }
        bool _isComplete;

        /// <summary> 사용 전 초기화 </summary>
        public void Init(inquest iq)
        {
            _iq = iq;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        /// <summary> 새 퀘스트 설정 </summary>
        public void set_newQuest()
        {
            IsUse = true;
            _isComplete = false;

            _progress.fillAmount = 0;
            if (_iq.isUsed)
            {
                _qInfo.text = _iq._title
                    + System.Environment.NewLine
                    + $"<size=50>0/{_iq.limit}</size>";
            }
            else
            {
                _qInfo.text = "None";
            }

            //startSeq.Restart();
        }

        /// <summary> 새 퀘스트 진행상황 고침 </summary>
        public void progressAmount(float val)
        {
            if (_iq.isUsed && _isComplete == false)
            {
                _progress.fillAmount = val;
                _qInfo.text = _iq._title
                        + System.Environment.NewLine
                        + string.Format("<size=50>{0:0}/{1:0}</size>", _iq.questVal, _iq.limit);

                //if (val == 1f)
                //{
                //    _isComplete = true;
                //    boxOff();
                //}
            }
            else
            {
                _progress.fillAmount = 0;
                _qInfo.text = "None";
            }
        }

        [Button]
        public void boxOff()
        {
            _isComplete = true;

            _effRect.localScale = new Vector3(0f, 1f);
            _qInfo.text = "의뢰 완료!";
            _progress.fillAmount = 1f;

            EndSeq = DOTween.Sequence();
            EndSeq.SetAutoKill(false)
                .Append(_effRect.DOScaleX(2f, 0.5f))
                .Join(_effRect.DOScaleY(0f, 0.5f))
                .Insert(0.5f, _showBox.DOAnchorPosX(0f, 0.5f).SetEase(Ease.InBack))
                .OnComplete(() => { gameObject.SetActive(false); IsUse = false; });
        }

        void OnEnable()
        {
            _showBox.anchoredPosition = Vector3.zero;
            _newEff.sizeDelta = new Vector2(600f, 150f);

            startSeq = DOTween.Sequence();
            startSeq.Append(_showBox.DOAnchorPosX(-500f, 2f).SetEase(Ease.OutBack))
                .AppendInterval(1f)
                .Append(_newEff.DOSizeDelta(new Vector2(0f, 150f), 0.3f)).SetEase(Ease.OutCirc);
        }

        void OnDisable()
        {
            startSeq.Kill();
        }
    }
}