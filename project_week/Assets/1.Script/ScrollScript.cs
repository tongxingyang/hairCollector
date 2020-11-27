using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace week
{
    public class ScrollScript : ScrollRect
    {
        bool _forParent;
        NestedScrollManager _nest;
        ScrollRect _rect;

        protected override void Start()
        {
            _nest = GetComponentInParent<NestedScrollManager>();
            _rect = _nest.GetComponent<ScrollRect>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            if (_forParent)
            {
                _nest.OnBeginDrag(eventData);
                _rect.OnBeginDrag(eventData);
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_forParent)
            {
                _nest.OnDrag(eventData);
                _rect.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_forParent)
            {
                _nest.OnEndDrag(eventData);
                _rect.OnEndDrag(eventData);
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }
    }
}