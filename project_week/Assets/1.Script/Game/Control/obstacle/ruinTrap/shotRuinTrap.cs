using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace week
{
    public class shotRuinTrap : baseRuinTrap
    {
        class charger
        {
            public bool _onCharger;
            public bool _isCharge;

            public float _chargeMount;
            float _shotTerm = 0.05f;
            float _chkTerm;
            SpriteRenderer _render;
            Transform _pos;

            public Color _color
            {
                set { _render.color = value; }
            }

            public charger(Transform pos)
            {
                _onCharger = true;
                _chargeMount = 0;
                _pos = pos;
                _render = pos.GetComponentInParent<SpriteRenderer>();
            }

            public void chargingColor()
            {
                float b = (_chargeMount < 0.5f) ? 1f - (_chargeMount * 2f) : 0f;
                float g = (_chargeMount < 0.5f) ? 1f : (1f - _chargeMount) * 2f;
                _color = new Color(1f, g, b);
            }

            public bool shotTermChk(float f)
            {
                _chkTerm += f;
                if(_chkTerm > _shotTerm)
                {
                    _shotTerm = 0.05f;//Random.Range(0.15f, 0.3f);
                    _chkTerm = 0;
                    return true;    
                }
                return false;
            }
        }

        [SerializeField] Transform[] _shotPos;
        charger[] _chargers;

        EnSkill_Proj _proj;

        protected override void whenFixedInit()
        { 
            int leng = _shotPos.Length;
            _chargers = new charger[leng];
            
            for(int i = 0; i < leng; i++)
            {
                _chargers[i] = new charger(_shotPos[i]);
            }
        }

        protected override void whenRepeatInit()
        {

        }

        public override void operate()
        {
            StartCoroutine(shot());
        }

        IEnumerator shot()
        {
            float time = 0;
            
            yield return new WaitForSeconds(1f);

            while (true)
            {
                time = Time.deltaTime;
                for (int i = 0; i < _chargers.Length; i++)
                {
                    if (_chargers[i]._isCharge) // 충전중
                    {
                        _chargers[i]._chargeMount += time * 2;
                        _chargers[i].chargingColor();
                        if (_chargers[i]._chargeMount > 1f)
                        {
                            _chargers[i]._isCharge = false;
                        }
                    }
                    else // 방전중
                    {
                        if (_chargers[i].shotTermChk(time))
                        {
                            _proj = (EnSkill_Proj)_epm.makeEnProj(EnShot.lightning);
                            _proj.transform.position = _shotPos[i].position;

                            _proj.operation(_player.transform.position, 0);

                            _chargers[i]._chargeMount -= 0.04f;

                            _chargers[i].chargingColor();

                            if (_chargers[i]._chargeMount <= 0f)
                            {
                                _chargers[i]._isCharge = true;
                            }
                        }
                    }
                    yield return new WaitUntil(() => (_gs.Pause == false && _onTrap));
                }
            }
        }



        public override void onPause(bool bl)
        {
        }

        public override void Destroy()
        {
            preDestroy();
        }
    }
}