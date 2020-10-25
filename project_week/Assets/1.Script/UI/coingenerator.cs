using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class coingenerator : MonoBehaviour
    {        
        [SerializeField] GameObject _fab;
        [SerializeField] Transform _pool;

        [SerializeField] Transform[] _l_Pos;

        List<curFabs> _fabs;

        LobbyScene _lobby;

        private void Awake()
        {
            _lobby = GetComponentInParent<LobbyScene>();
            _fabs = new List<curFabs>();
        }

        public void getCurrent(Vector3 pos, currency cur, int count, int posNum = 0)
        {
            StartCoroutine(getAnimation(pos, cur, count, posNum));
        }

        IEnumerator getAnimation(Vector3 pos, currency cur, int count, int posNum)
        {
            int chk = 0;
            int cnt = (count > 10) ? 10 : count;

            int val = count / cnt;
            curFabs cf;

            for (int i = 0; i < cnt; i++)
            {
                cf = getFab();
                cf.transform.position = pos;
                cf.setCur(cur, posNum, ()=>
                {
                    _lobby.refreshCost();
                });

                chk++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        curFabs getFab()
        {
            for (int i = 0; i < _fabs.Count; i++)
            {
                if (_fabs[i].IsUse == false)
                {
                    return _fabs[i];
                }
            }

            curFabs cf = Instantiate(_fab).GetComponent<curFabs>();
            cf.setLastPos(_l_Pos[0], _l_Pos[1]);
            cf.transform.SetParent(_pool);
            _fabs.Add(cf);
            return cf;
        }
    }
}