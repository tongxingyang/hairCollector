using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace week
{
    public class ShieldCtrl : MonoBehaviour
    {
        GameScene _gs;
        dmgFontManager _dfm;

        #region [ 일반 실드 ] 

        [Header("NORMAL")]
        [SerializeField] Animator _normalShield;
        [Space]
        [SerializeField] Transform _nShield_A;
        [SerializeField] Transform _nShield_B;
        [SerializeField] GameObject _ballFab;

        SkillKeyList _norType;
        bool _isUsedNormal;
        float _shieldMount = 0;
        List<shieldCollider> _balls;
        bool _isLightningPlay;
        float _subRate;

        public bool IsUseNormal { get => _isUsedNormal; set => _isUsedNormal = value; }
        float LightningDmg(SkillKeyList skilln) { return _gs.Player.Skills[skilln].val0 * _gs.Player.Att; }

        #endregion

        #region [ 더좋은 실드 ] 

        [Header("HIGH")]
        [SerializeField] Animator _highShield;
        [SerializeField] shieldCollider _chillShield;
        
        SkillKeyList _hghType;
        bool _isUsedHigh;
        float _keep;
        float _dmgStack;
        public bool IsUsedHigh { get => _isUsedHigh; set => _isUsedHigh = value; }

        #endregion

        #region [ 초기화 ]

        private void Awake()
        {
            _isUsedNormal = false;
            _isUsedHigh = false;

            // _coll.Init(this, _gs);

            _normalShield.enabled = false;
            _nShield_A.gameObject.SetActive(false);
            _nShield_B.gameObject.SetActive(false);
            _highShield.gameObject.SetActive(false);
        }

        public void ShieldInit(GameScene gs)
        {
            _gs = gs;
            _dfm = gs.DmgfntMng;
        }

        /// <summary> 일반방패 설정 </summary>
        public ShieldCtrl setNormalShield(SkillKeyList shield, float hp)
        {
            _norType = shield;
            _isUsedNormal = true;

            _shieldMount = hp;

            return this;
        }

        public ShieldCtrl setSubRate(float rate)
        {
            _subRate = rate;
            return this;
        }

        /// <summary> 특수방패 설정 </summary>
        public ShieldCtrl setHighShield(SkillKeyList high, float keep)
        {
            _hghType = high;

            _keep = keep;
            _dmgStack = 0;

            _chillShield.Init(this, _gs, high);            

            return this;            
        }

        /// <summary> 작동 </summary>
        public void play(bool isNor)
        {
            if (isNor)
            {
                _normalShield.enabled = false;

                _nShield_B.gameObject.SetActive(false);

                _normalShield.enabled = true;
                _normalShield.SetTrigger(_norType.ToString());
            }
            else
            {
                _highShield.gameObject.SetActive(true);
                StartCoroutine(playHighShield());
                _highShield.SetTrigger(_hghType.ToString());
            }
        }

        IEnumerator playHighShield()
        {
            _isUsedHigh = true;
            _highShield.SetTrigger(_hghType.ToString());

            if (_hghType == SkillKeyList.Hide || _hghType == SkillKeyList.Chill)
            {
                _gs.Player.pSpine.skeleton.SetColor(new Color(1f, 1f, 1f, 0.3f));

                //Debug.Log(Physics2D.GetLayerCollisionMask(8));
                UnityEngine.Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("player"), LayerMask.GetMask("obstacle", "environment", "trap", "ignoreOb"));
                //Debug.Log(Physics2D.GetLayerCollisionMask(8));

                if (_hghType == SkillKeyList.Chill)
                {
                    _gs.Player.setDeBuff(eBuff.speed, _keep, _subRate);
                }
            }

            float time = 0;
            while (time < _keep)
            {
                time += Time.deltaTime;

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            if (_hghType == SkillKeyList.Absorb)
            {
                _gs.Player.getHealed(_dmgStack * _subRate);
            }
            else if (_hghType == SkillKeyList.Hide || _hghType == SkillKeyList.Chill)
            {
                _gs.Player.pSpine.skeleton.SetColor(new Color(1f, 1f, 1f, 1f));
                UnityEngine.Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("player"), LayerMask.GetMask("monster", "boss", "obstacle", "environment", "trap", "enWeapon", "ignoreOb"));
            }

            _isUsedHigh = false;
            _highShield.gameObject.SetActive(false);
        }

        public void normalDestroy()
        {
            _isUsedNormal = false;

            _normalShield.enabled = false;
            _nShield_A.gameObject.SetActive(false);
        }

        public void highDestroy()
        {
            _isUsedNormal = false;

            gameObject.SetActive(false);
        }

        #region [ 번개/충전 방패 ]

        /// <summary> 번개방패 설정 </summary>
        public ShieldCtrl setLightShield(SkillKeyList shield, bool first)
        {
            _norType = shield;

            if (first)
            {
                _balls = new List<shieldCollider>();
                _isLightningPlay = false;
            }
            else
            {
                for (int i = 0; i < _balls.Count; i++)
                {
                    _balls[i].Init(this, _gs, shield);
                }
            }

            return this;
        }

        /// <summary> 번개방패 추가설정 </summary>
        public ShieldCtrl setLightning(int n)
        {
            shieldCollider Scol;
            int cnt = _balls.Count;

            if (cnt < n)
            {
                for (int i = 0; i < n - cnt; i++)
                {
                    Scol = Instantiate(_ballFab, _nShield_B).GetComponent<shieldCollider>();
                    _balls.Add(Scol);
                    Scol.Init(this, _gs, SkillKeyList.LightningShield);
                }

                float degree = 360 / n;
                for (int i = 0; i < _balls.Count; i++)
                {
                    _balls[i].transform.position = Vector3.zero;
                    _balls[i].transform.rotation = Quaternion.Euler(Vector3.zero);

                    _balls[i].transform.rotation = Quaternion.AngleAxis(degree * i, Vector3.back);
                    _balls[i].transform.Translate(Vector3.up * 1f, Space.Self);
                }
            }

            return this;
        }

        /// <summary> 번개플레이 </summary>
        public void lightningPlay()
        {
            if (_isLightningPlay == false)
            {
                _normalShield.enabled = false;

                _nShield_A.gameObject.SetActive(false);
                StartCoroutine(lightSpin());
            }
        }

        IEnumerator lightSpin()
        {
            _isLightningPlay = true;
            _nShield_B.gameObject.SetActive(true);

            while (true)
            {
                _nShield_B.Rotate(new Vector3(0f, 0f, 2f), Space.Self);

                for (int i = 0; i < _balls.Count; i++)
                {
                    Color col = _balls[i].Render.color;
                    
                    if (col == Color.white)
                        continue;

                    col.r = (col.r < 1f) ? col.r * 1.1f : 1f;
                    col.g = (col.g < 1f) ? col.g * 1.1f : 1f;
                    col.b = (col.b < 1f) ? col.b * 1.1f : 1f;

                    _balls[i].Render.color *= col;
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void setShield(float shield)
        {
            _isUsedNormal = true;
            _shieldMount += shield;
            if (_norType == SkillKeyList.ChargeShield)
            {
                float heal = _gs.Player.MaxHp * _gs.Player.Skills[SkillKeyList.ChargeShield].val1 * 0.01f;
                _gs.Player.getHealed(heal);
            }

            _normalShield.enabled = true;
            _normalShield.SetTrigger(SkillKeyList.Shield.ToString());
        }

        #endregion

        #endregion

        /// <summary> 실드에 데미지 계산 (데미지/독뎀여부) </summary>
        public bool getDamage(ref float dmg, EnemyCtrl enemy = null)
        {
            if (_isUsedHigh)
            {
                if (_hghType == SkillKeyList.Invincible)
                {
                    _dfm.getText(transform, "피해방지", dmgTxtType.shield, true);
                }
                else if (_hghType == SkillKeyList.Absorb)
                {
                    _dmgStack += dmg;
                    _dfm.getText(transform, "흡수", dmgTxtType.shield, true);
                }

                return true;
            }
            else if(_isUsedNormal)
            {
                dmg *= (enemy == null) ? 2f : 1f; // 중립뎀은 실드에 데미지 2배

                if (_norType == SkillKeyList.GiantShield)                       // 방어력 적용
                {
                    dmg *= (100 - _gs.Player.Def * 0.5f) * 0.01f;
                }
                else if (_norType == SkillKeyList.ThornShield)                  // 공격자 반사
                {
                    float? val = enemy?.getDamaged(dmg * _subRate);
                    Debug.Log(val);
                }
                else if (_norType == SkillKeyList.ReflectShield)                // 전체 반사
                {
                    _gs.EnemyMng.enemyDamagedRange(dmg * _subRate, 2.5f); 
                    Debug.Log(dmg * _subRate);
                }

                //Debug.Log(dmg);
                _dfm.getText(transform, Convert.ToInt32(dmg).ToString(), dmgTxtType.shield, true);

                if (_shieldMount >= dmg)
                {
                    _shieldMount -= dmg;
                    dmg = 0;
                }
                else
                {
                    dmg -= _shieldMount;
                    _shieldMount = 0; 
                }

                if (_shieldMount <= 0)
                {
                    normalDestroy();
                }

                return dmg > 0;
            }

            return false;
        }

        public void onTriggerEnemy(IDamage id, SkillKeyList skill)
        {
            switch (skill)
            {
                case SkillKeyList.LightningShield:
                case SkillKeyList.ChargeShield:
                    id.getDamaged(LightningDmg(skill), false);
                    setShield(LightningDmg(skill) * _gs.Player.Skills[SkillKeyList.Lightning].val1);
                    break;
                case SkillKeyList.Invincible:
                case SkillKeyList.Absorb:
                    break;
                case SkillKeyList.Chill:
                    id.setFrozen(2f);
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + skill);
                    break;
            }
        }

        public void onPause(bool bl)
        {
            //_normalShield.speed = (bl) ? 0f : 1f;
        }
    }
}