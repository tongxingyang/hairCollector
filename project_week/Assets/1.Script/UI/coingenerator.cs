using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class coingenerator : MonoBehaviour
    {        
        [SerializeField] GameObject _fab;

        List<curFabs> _fabs;

        Action _refreshFollowCost;
        public Action RefreshFollowCost { set => _refreshFollowCost = value; }

        private void Awake()
        {
            _fabs = new List<curFabs>();
        }

        /// <summary>  </summary>
        public void getWealth2Point(Vector3 start, Vector3 direct, currency cur, int count, int posNum = 0, int cnt = 0)
        {
            StartCoroutine(getAnimation(start, direct, cur, count, posNum, cnt));
        }

        IEnumerator getAnimation(Vector3 pos, Vector3 pos2, currency cur, int cost, int posNum, int cnt)
        {
            if (cnt == 0)
            {
                cnt = (cost > 10) ? 10 : (cost > 5) ? 5 : cost;
            }

            curFabs cf;

            for (int i = 0; i < cnt; i++)
            {
                cf = getFab(pos2);

                cf.transform.position = new Vector3(pos.x, pos.y, 500f);

                int num = cost / (cnt - i);
                cost -= num;

                cf.setCur(cur, posNum, ()=>
                {
                    if (cur == currency.coin)
                        BaseManager.userGameData.followCoin += num;
                    else if (cur == currency.gem)
                        BaseManager.userGameData.followGem += num;

                    _refreshFollowCost?.Invoke(); 
                });
                SoundManager.instance.PlaySFX(SFX.coin2);

                yield return new WaitForSeconds(0.05f);
            }
        }

        public void getDirect(Vector3 target, currency cur, int count)
        {
            StartCoroutine(getDirectAni(target, cur, count));
        }

        IEnumerator getDirectAni(Vector3 target, currency cur, int count)
        {
            int cnt = count;

            curFabs cf;

            for (int i = 0; i < cnt; i++)
            {
                cf = getFab(target);
                cf.transform.position = new Vector3(transform.position.x, transform.position.y, -100f);

                cf.setCurinGame(cur, () =>
                {
                    _refreshFollowCost?.Invoke();
                });

                yield return new WaitForSeconds(0.05f);
            }
        }

        /// <summary> 이미지팹 생산 </summary>
        curFabs getFab(Vector3 pos)
        {
            for (int i = 0; i < _fabs.Count; i++)
            {
                if (_fabs[i].IsUse == false)
                {
                    _fabs[i].setLastPos(pos);
                    return _fabs[i];
                }
            }

            curFabs cf = Instantiate(_fab).GetComponent<curFabs>();
            cf.setLastPos(pos);
            cf.transform.SetParent(transform);
            _fabs.Add(cf);
            return cf;
        }
    }
}