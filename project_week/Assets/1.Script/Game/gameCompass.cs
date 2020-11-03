using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class gameCompass : MonoBehaviour
    {
        [SerializeField] GameObject _iconFab;
        [SerializeField] Transform _player;
        [Space]
        [SerializeField] Transform _pool;
        [SerializeField] Transform _web;
        [SerializeField] Sprite[] sprites; // 0:보스 1:유적
        class Icon
        {
            public Transform pos; // 담당하고 있는 실제위치
            public Transform mark; // 레이더 마크 위치
        }

        List<Icon> niddles;
        Queue<Icon> niddlePool;

        float _webAngle;

        private void Awake()
        {
            niddles = new List<Icon>();
            niddlePool = new Queue<Icon>();
        }

        public void newArea()
        {
            for (; 0 < niddles.Count;)
            {
                Icon nd = niddles[0];
                niddles.RemoveAt(0);
                nd.mark.gameObject.SetActive(false);

                niddlePool.Enqueue(nd);
            }
        }

        public void chkCompass(Transform pos, bool isBoss) // bool에서 enum으로 변경
        {
            if (niddlePool.Count > 0)
            {
                Icon ndl = niddlePool.Dequeue();
                ndl.pos = pos;
                //ndl.nid.gameObject.SetActive(true);

                niddles.Add(ndl);

                ndl.mark.GetComponentInChildren<Image>().sprite = sprites[(isBoss) ? 0 : 1];

                return;
            }

            Transform tr = Instantiate(_iconFab).transform;
            tr.SetParent(_pool);
            tr.localPosition = Vector3.zero;

            Icon nid = new Icon();
            nid.pos = pos;
            nid.mark = tr;
            niddles.Add(nid);

            nid.mark.GetComponentInChildren<Image>().sprite = sprites[(isBoss) ? 0 : 1];
        }

        public void comPassMove()
        {
            Vector3 _direct;
            float _dist;
            float _far = 43f - 20.48f;
            for (int i = 0; i < niddles.Count; i++)
            {
                niddles[i].mark.gameObject.SetActive(true);

                _direct = niddles[i].pos.position - _player.position;
                _dist = Vector3.Distance(niddles[i].pos.position, _player.position);

                if (_dist > 20.48f)
                {
                    _dist -= 20.48f;
                    _direct = _direct.normalized * 20f;
                    niddles[i].mark.localScale = Vector3.one * ((_far - _dist) / _far + 1) * 0.5f;
                }
                else
                {
                    niddles[i].mark.localScale = Vector3.one;
                }

                niddles[i].mark.localPosition = _direct * 7f;

                //Vector3 _direct = niddles[i].pos.position - _player.position;
                //float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
                //niddles[i].nid.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            }

            _webAngle += Time.deltaTime * 90f;
            _web.rotation = Quaternion.AngleAxis(_webAngle, Vector3.forward);
        }
    }
}