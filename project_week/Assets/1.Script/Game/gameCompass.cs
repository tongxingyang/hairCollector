using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class gameCompass : MonoBehaviour
    {
        [SerializeField] GameObject _niddleFab;
        [SerializeField] Transform _player;
        [Space]
        [SerializeField] Transform _pool;
        [SerializeField] Transform _web;
        [SerializeField] Sprite[] sprites;
        class niddle
        {
            public Transform pos;
            public Transform nid;
        }

        List<niddle> niddles;
        Queue<niddle> niddlePool;

        float _webAngle;

        private void Awake()
        {
            niddles = new List<niddle>();
            niddlePool = new Queue<niddle>();
        }

        public void newArea()
        {
            for (; 0 < niddles.Count;)
            {
                niddle nd = niddles[0];
                niddles.RemoveAt(0);
                nd.nid.gameObject.SetActive(false);

                niddlePool.Enqueue(nd);
            }
        }

        public void chkCompass(Transform pos, bool isBoss)
        {
            if (niddlePool.Count > 0)
            {
                niddle ndl = niddlePool.Dequeue();
                ndl.pos = pos;
                //ndl.nid.gameObject.SetActive(true);

                niddles.Add(ndl);

                ndl.nid.GetComponentInChildren<Image>().sprite = sprites[(isBoss) ? 0 : 1];

                return;
            }

            Transform tr = Instantiate(_niddleFab).transform;
            tr.SetParent(_pool);
            tr.localPosition = Vector3.zero;

            niddle nid = new niddle();
            nid.pos = pos;
            nid.nid = tr;
            niddles.Add(nid);

            nid.nid.GetComponentInChildren<Image>().sprite = sprites[(isBoss) ? 0 : 1];
        }

        public void comPassMove()
        {
            Vector3 _direct;
            float _dist;
            float _far = 43f - 20.48f;
            for (int i = 0; i < niddles.Count; i++)
            {
                niddles[i].nid.gameObject.SetActive(true);

                _direct = niddles[i].pos.position - _player.position;
                _dist = Vector3.Distance(niddles[i].pos.position, _player.position);

                if (_dist > 20.48f)
                {
                    _dist -= 20.48f;
                    _direct = _direct.normalized * 20f;
                    niddles[i].nid.localScale = Vector3.one * ((_far - _dist) / _far + 1) * 0.5f;
                }
                else
                {
                    niddles[i].nid.localScale = Vector3.one;
                }

                niddles[i].nid.localPosition = _direct * 7f;

                //Vector3 _direct = niddles[i].pos.position - _player.position;
                //float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
                //niddles[i].nid.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            }

            _webAngle += Time.deltaTime * 90f;
            _web.rotation = Quaternion.AngleAxis(_webAngle, Vector3.forward);
        }
    }
}