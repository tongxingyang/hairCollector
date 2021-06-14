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
        [SerializeField] Transform _normalShield;
        [SerializeField] SpriteRenderer _nShield;
        SpriteMask _nMask;
        Transform _rotateHome;
        [SerializeField] GameObject _ballFab;

        SkillKeyList _norType;
        bool _isUsedNormal;
        float _shieldMount = 0;
        List<shieldCollider> _balls;
        bool _isLightningPlay;
        public float NsubRate { get; set; }        

        public bool IsUseNormal { get => _isUsedNormal; set => _isUsedNormal = value; }

        #endregion

        #region [ 더좋은 실드 ] 

        [Header("HIGH")]
        [SerializeField] Animator _highShield;
        [SerializeField] shieldCollider _chillShield;
        
        SkillKeyList _hghType;
        bool _isUsedHigh;
        float _keep;
        float _dmgStack;
        attackData _adata = new attackData(); 
        public float HsubRate { get; set; }
        public bool IsUsedHigh { get => _isUsedHigh; set => _isUsedHigh = value; }

        #endregion

        #region [ 초기화 ]

        private void Awake()
        {
            _isUsedNormal = false;
            _isUsedHigh = false;

            _nMask = _nShield.GetComponent<SpriteMask>();

            _nShield.gameObject.SetActive(false);
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
                _nShield.gameObject.SetActive(true);

                _nShield.sprite = DataManager.ShieldImgs[_norType];
                _nMask.sprite = DataManager.ShieldImgs[_norType];
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

                UnityEngine.Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("player"), LayerMask.GetMask("obstacle", "environment", "trap", "ignoreOb"));

                if (_hghType == SkillKeyList.Chill)
                {
                    _gs.Player.setDeBuff(SkillKeyList.SPEED, _keep, HsubRate);
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
                _gs.Player.getHealed(_dmgStack * HsubRate * _gs.Player.getAddDmg(skillType.shield));
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

            _nShield.gameObject.SetActive(false);
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
                _rotateHome = new GameObject().transform;
                _rotateHome.parent = _normalShield;
                _rotateHome.transform.localPosition = Vector3.zero;

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
                    Scol = Instantiate(_ballFab, _rotateHome).GetComponent<shieldCollider>();
                    _balls.Add(Scol);
                    Scol.Init(this, _gs, SkillKeyList.ChargeShield);
                }

                float degree = 360 / n;
                for (int i = 0; i < _balls.Count; i++)
                {
                    _balls[i].transform.position = _rotateHome.position;
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
                _nShield.gameObject.SetActive(false);

                StartCoroutine(lightSpin());
            }
        }

        IEnumerator lightSpin()
        {
            _isLightningPlay = true;

            while (true)
            {
                _rotateHome.Rotate(new Vector3(0f, 0f, 2f), Space.Self);

                //for (int i = 0; i < _balls.Count; i++)
                //{
                //    Color col = _balls[i].Render.color;
                    
                //    if (col == Color.white)
                //        continue;

                //    col.r = (col.r < 1f) ? col.r * 1.1f : 1f;
                //    col.g = (col.g < 1f) ? col.g * 1.1f : 1f;
                //    col.b = (col.b < 1f) ? col.b * 1.1f : 1f;

                //    _balls[i].Render.color *= col;
                //}

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        /// <summary> 차지실드로 실드 충전 </summary>
        void setChargeShield(float shield)
        {
            Debug.Log("차지 :+ " + shield);
            _isUsedNormal = true;
            _shieldMount += shield;

            _nShield.sprite = DataManager.ShieldImgs[SkillKeyList.ThornShield];
            _nMask.sprite = DataManager.ShieldImgs[SkillKeyList.Shield];

            _nShield.gameObject.SetActive(true);
        }

        #endregion

        #endregion

        /// <summary> 실드에 데미지 계산 (데미지/독뎀여부) </summary>
        public bool getDamage(ref float _dmg, EnemyCtrl enemy = null)
        {
            if (_isUsedHigh)
            {
                if (_hghType == SkillKeyList.Invincible)
                {
                    _dfm.getText(transform, "피해방지", dmgTxtType.shield, true);
                }
                else if (_hghType == SkillKeyList.Absorb)
                {
                    _dmgStack += _dmg;
                    _dfm.getText(transform, "흡수", dmgTxtType.shield, true);
                }

                return true;
            }
            else if(_isUsedNormal)
            {
                SoundManager.instance.PlaySFX(SFX.shield);
                if (enemy == null) // 중립데미지
                {
                    _dmg *= 2f;// 중립뎀은 실드에 데미지 2배
                }
                else // 몹에게서 데미지
                {
                    if (_norType == SkillKeyList.ThornShield 
                        || _norType== SkillKeyList.ChargeShield)                  // 공격자 반사
                    {

                        shotCtrl _shotBullet = _gs.SkillMng.getLaunch(SkillKeyList.GigaDrill);
                        _shotBullet.transform.position = _gs.Player.transform.position;
                        _shotBullet.setTarget(enemy.transform.position);
                        _shotBullet.repeatInit(SkillKeyList.ThornShield, _dmg * NsubRate, 1f)
                            .play();
                    }
                    else if (_norType == SkillKeyList.ReflectShield)                // 전체 반사
                    {
                        _gs.EnemyMng.enemyRangeShot(_dmg * NsubRate, 3f, _norType);
                    }
                }

                if (_norType == SkillKeyList.GiantShield)                       // 방어력 적용
                {
                    float r_Def = (_gs.Player.Def > 90f) ? 90f : _gs.Player.Def;
                    float df = _gs.Player.Skills[SkillKeyList.GiantShield].val1;
                    Debug.Log(_dmg + " -* " + r_Def + "의 " + df * 100f + "% " + _dmg * (100f - r_Def * df) * 0.01f);
                    _dmg *= (100f - r_Def * df) * 0.01f;
                }
                
                _dfm.getText(transform, Convert.ToInt32(_dmg).ToString(), dmgTxtType.shield, true);

                if (_shieldMount >= _dmg)
                {
                    _shieldMount -= _dmg;
                    _dmg = 0;
                }
                else
                {
                    _dmg -= _shieldMount;
                    _shieldMount = 0; 
                }

                if (_shieldMount <= 0)
                {
                    normalDestroy();
                }

                return _dmg > 0;
            }

            return false;
        }

        public void onTriggerEnemy(IDamage id, SkillKeyList skill)
        {
            switch (skill)
            {
                case SkillKeyList.ChargeShield:
                    {
                        _adata.set(_gs.Player.Att * _gs.Player.Skills[skill].val0, _norType, false);
                        float val = id.getDamaged(_adata);

                        //setShield(LightningDmg(skill));
                        setChargeShield(val * _gs.Player.Skills[skill].val1 * _gs.Player.getAddDmg(skillType.shield));                      
                    }
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