using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class effManager : MonoBehaviour
    {
        [SerializeField] GameObject _effs;

        [SerializeField] GameObject _rebirthFab;

        GameScene _gs;

        List<effControl> _effList;
        public List<effControl> EffList { get => _effList; set => _effList = value; }

        Animator _rebirth;

        public void Init(GameScene gs)
        {
            _gs = gs;
            _effList = new List<effControl>();
        }

        public void makeEff(string effName, Vector3 pos)
        {
            // 있으면 찾아쓰고
            foreach (effControl ec in _effList)
            {
                if (ec.isUse == false)
                {
                    ec.transform.position = pos;
                    ec.Init(effName);
                    return;
                }
            }

            // 없으면 생성
            effControl efc = Instantiate(_effs).GetComponent<effControl>();
            _effList.Add(efc);
            efc.transform.parent = transform;
            efc.transform.position = pos;
            efc.Init(effName);
        }

        public void getRebirth(Vector3 pos, Action reAct, Action endAct)
        {
            if (_rebirth == null)
            {
                _rebirth = Instantiate(_rebirthFab).GetComponent<Animator>();
                _rebirth.transform.parent = transform;
            }

            rebirthParticle rp = _rebirth.GetComponent<rebirthParticle>();
            rp.setAct(reAct, endAct);

            _rebirth.transform.position = pos;
            _rebirth.gameObject.SetActive(true);
            _rebirth.SetTrigger("rebirth");
        }
    }
}