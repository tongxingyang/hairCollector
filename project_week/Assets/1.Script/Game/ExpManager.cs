using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class ExpManager : MonoBehaviour
    {
        [SerializeField] GameObject _Tem;

        [SerializeField] Transform _playerPos;

        GameScene _gs;

        List<expCoinControl> _expList;

        void Awake()
        {
            _gs = GetComponentInParent<GameScene>();
            _expList = new List<expCoinControl>();
        }

        public void makeItem()
        {
            // 있으면 찾아쓰고
            foreach (expCoinControl ec in _expList)
            {
                if (ec.isUse == false)
                {
                    ec.transform.position = itemRespawnsPos();
                    ec.Init();

                    return;
                }
            }

            // 없으면 생성
            expCoinControl ecc = Instantiate(_Tem).GetComponent<expCoinControl>();
            _expList.Add(ecc);
            ecc.transform.parent = transform;
            ecc.transform.position = itemRespawnsPos();
            ecc.expFunc = _gs.Player.getExp;
            ecc.coinFunc = _gs.getCoin;
            ecc.Init();
        }

        Vector2 itemRespawnsPos()
        {
            return new Vector2(Random.Range(-2.5f, 2.5f), 6f);
        }
    }
}