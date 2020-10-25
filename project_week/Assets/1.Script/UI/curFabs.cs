using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace week
{
    public class curFabs : MonoBehaviour
    {
        [SerializeField] Image _img;
        [SerializeField] Image _light;
        [Space]
        [SerializeField] Sprite[] _sps;

        Transform _coin;
        Transform _gem;

        currency _cur;
        int _posNum;

        bool _isUse;
        public bool IsUse { get => _isUse; set => _isUse = value; }

        public void setLastPos(Transform coin, Transform gem)
        {
            _coin = coin;
            _gem = gem;
        }

        public void setCur(currency cur, int posNum, Action act)
        {
            _isUse = true;
            gameObject.SetActive(true);

            _cur = cur;
            _posNum = posNum;

            _img.sprite = _sps[(int)cur];
            _light.color = Color.white;

            move(act);
        }

        void move(Action act)
        {
            Vector3 pos = transform.position + ((Vector3)UnityEngine.Random.insideUnitCircle * 200f);

            if (_posNum == 1)            
                pos += Vector3.left * 175f;
            else if (_posNum == 2)
                pos += Vector3.right * 175f;
            else if (_posNum == 3)
                pos += Vector3.down * 230f;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOJump(pos, 300f, 1, 0.1f));
            seq.Append(transform.DOScale(1, 0.5f));
            seq.Append(transform.DOMove((_cur == currency.gem) ? _gem.position : _coin.position, 1f)).SetEase(Ease.InCubic)
                .OnComplete(()=> 
            {
                useOff();
                act();
            });
        }

        void useOff()
        {
            _isUse = false;
            gameObject.SetActive(false);
        }
    }
}