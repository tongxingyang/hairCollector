using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class effManager : MonoBehaviour
    {
        [SerializeField] GameObject _effs;
        [SerializeField] GameObject _dust;

        GameScene _gs;

        List<walkDust> _dustList;
        List<effControl> _effList;
        public List<effControl> EffList { get => _effList; set => _effList = value; }

        public void Init(GameScene gs)
        {
            _gs = gs;
            _effList = new List<effControl>();
            _dustList = new List<walkDust>();
        }

        public void makeEff(effAni efa, Vector3 pos)
        {
            // 있으면 찾아쓰고
            foreach (effControl ec in _effList)
            {
                if (ec.isUse == false)
                {
                    ec.transform.position = pos;
                    ec.Init(efa);
                    return;
                }
            }

            // 없으면 생성
            effControl efc = Instantiate(_effs).GetComponent<effControl>();
            _effList.Add(efc);
            efc.transform.parent = transform;
            efc.transform.position = pos;
            efc.Init(efa);
        }

        public void makeDust(Vector3 pos)
        {
            // 있으면 찾아쓰고
            foreach (walkDust wd in _dustList)
            {
                if (wd.IsUse == false)
                {
                    wd.transform.position = pos;
                    wd.init();
                    return;
                }
            }

            // 없으면 생성
            walkDust wdd = Instantiate(_dust).GetComponent<walkDust>();
            _dustList.Add(wdd);
            wdd.transform.parent = transform;
            wdd.transform.position = pos;
            wdd.init();
        }
    }
}