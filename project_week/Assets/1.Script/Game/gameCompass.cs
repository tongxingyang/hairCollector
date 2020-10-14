using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class gameCompass : MonoBehaviour
    {
        [SerializeField] GameObject _niddleFab;
        [SerializeField] Transform _player;

        class niddle
        {
            public Transform pos;
            public Transform nid;
        }

        List<niddle> niddles;
        Queue<niddle> niddlePool;

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
                ndl.nid.gameObject.SetActive(true);

                niddles.Add(ndl);

                return;
            }

            Transform tr = Instantiate(_niddleFab).transform;
            tr.SetParent(transform);
            tr.localPosition = Vector3.zero;

            niddle nid = new niddle();
            nid.pos = pos;
            nid.nid = tr;
            niddles.Add(nid);
        }

        public void comPassMove()
        {
            for (int i = 0; i < niddles.Count; i++)
            {
                Vector3 _direct = niddles[i].pos.position - _player.position;

                float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
                niddles[i].nid.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            }
        }
    }
}