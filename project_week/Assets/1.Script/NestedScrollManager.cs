using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace week
{
    public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("scrolls")]
        [SerializeField] ScrollScript[] _scrolls;
        [Space]
        [SerializeField] Scrollbar _scrollbar;
        [SerializeField] Slider _tapSlider;
        [SerializeField] RectTransform[] _btns;
        [Header("canvas")]
        [SerializeField] Canvas[] _canvases;

        TextMeshProUGUI[] _btnTxts;

        const int SIZE = 4;
        int width = 288;
        int maxFont = 160, minFont = 80;

        float[] _pos = new float[SIZE];
        float _distance;
        float _curPos;
        float _targetPos;
        int _targetIndex;

        bool _isDrag;
        float _tapbarDist = 1f / 3f;

        // Start is called before the first frame update
        void Start()
        {
            _distance = 1f / (SIZE - 1);
            for (int i = 0; i < SIZE; i++)
            {
                _pos[i] = _distance * i;
            }

            int leng = _btns.Length;

            _btnTxts = new TextMeshProUGUI[leng];
            for (int i = 0; i < leng; i++)
            {
                _btnTxts[i] = _btns[i].GetComponentInChildren<TextMeshProUGUI>();
            }

            _tapSlider.value = _scrollbar.value = _curPos = _targetPos = _tapbarDist * 2;
            TapClick(2);

            StartCoroutine(ScrollControl());
        }

        // Update is called once per frame
        IEnumerator ScrollControl()
        {
            yield return new WaitForEndOfFrame();
            float btnSizeY = _btns[0].sizeDelta.y;

            bool enableChk = false;
            while (true)
            {
                _tapSlider.value = _scrollbar.value;

                if (_isDrag == false)
                {
                    _scrollbar.value = Mathf.Lerp(_scrollbar.value, _targetPos, 0.1f);

                    for (int i = 0; i < _btns.Length; i++)
                    {
                        _btns[i].sizeDelta = new Vector2(i == _targetIndex ? width * 2 : width, btnSizeY);
                        _btnTxts[i].fontSize = i == _targetIndex ? maxFont : minFont;

                        enableChk = (Mathf.Abs(_scrollbar.value - i * _tapbarDist) > _tapbarDist);
                        _canvases[i].enabled = !enableChk;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _curPos = setPos();
        }

        public void OnDrag(PointerEventData eventData) => _isDrag = true;

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDrag = false;

            _targetPos = setPos();

            if (_curPos == _targetPos)
            {
                if (eventData.delta.x > 18 && _curPos - _distance >= 0)
                {
                    --_targetIndex;
                    _targetPos = _curPos - _distance;
                }
                else if (eventData.delta.x < -18 && _curPos + _distance <= 1.01f)
                {
                    ++_targetIndex;
                    _targetPos = _curPos + _distance;
                }
            }

            for (int i = 0; i < SIZE; i++)
            {
                if (_scrolls[i] != null && _curPos != _targetPos && _targetPos != _pos[i])
                {
                    _scrolls[i].verticalScrollbar.value = 1f;
                }
            }
        }

        float setPos()
        {
            for (int i = 0; i < SIZE; i++)
            {
                if (_scrollbar.value < _pos[i] + _distance * 0.5f && _scrollbar.value > _pos[i] - _distance * 0.5f)
                {
                    _targetIndex = i;
                     return _pos[i];
                }
            }

            return 0;
        }

        public void TapClick(int n)
        {
            if (_curPos != _targetPos && _scrolls[n] != null)
            {
                _scrolls[n].verticalScrollbar.value = 1f;
            }

            _targetIndex = n;
            _targetPos = _pos[n];
        }
    }
}