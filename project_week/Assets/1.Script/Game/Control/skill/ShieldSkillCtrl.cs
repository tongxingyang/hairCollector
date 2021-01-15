using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class ShieldSkillCtrl : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _norSprite;
        [SerializeField] Transform _norLightSprite;
        [SerializeField] SpriteRenderer _epcSprite;

        GameScene _gs;
        PlayerCtrl _player;

        SkillKeyList _nor, _epc;
        bool _onNor, _onEpc;
        Action _whenDestNor;
        Action _whenEndShield;
        //Action<float> _whenDmg;

        float _shieldMount;
        float _keep; 
        float _attMount;

        public SkillKeyList getNowNorShield { get => _nor; }
        public SkillKeyList getNowEpcShield { get => _epc; }
        public bool OnNor { get => _onNor; }
        public bool OnEpc { get => _onEpc; }

        // Start is called before the first frame update
        void Start()
        {
            _onNor = _onEpc = false;
            _shieldMount = 0f;
            _norSprite.gameObject.SetActive(false);
            _norLightSprite.gameObject.SetActive(false);
            _epcSprite.gameObject.SetActive(false);
        }

        public void FixedInit(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;
        }

        /// <summary> 일반/거대/가시 </summary>
        public void setNorShield(SkillKeyList sk, float mount, Action whenDestNor)
        {
            _onNor = true;

            _nor = sk;
            _shieldMount = mount;
            _whenDestNor = whenDestNor;

            _norSprite.sprite = DataManager.Skillicon[sk];
        }

        /// <summary> 번개보호막 </summary>
        public void setNorLightning(SkillKeyList sk, float mount)//, Action<float> whenDmg)
        {
            _onNor = true;

            _nor = sk;
            _attMount = mount * 0.01f;
            // _whenDmg = whenDmg;
            _whenDestNor = null;
        }

        public void setShieldtoDmg(float mount)
        {
            _onNor = true;

            _nor = SkillKeyList.Shield;
            _shieldMount += mount;
        }

        /// <summary> 무적/은신 </summary>
        public void setEpcShield(SkillKeyList sk, float keep, Action whenEndShield)
        {
            _epc = sk;
            _onEpc = true;

            _keep = keep;
            _whenEndShield = whenEndShield;

            _epcSprite.sprite = DataManager.Skillicon[sk];
            StartCoroutine(epcShield());
        }

        IEnumerator epcShield()
        { 
            float time = 0;
            
            while (time < _keep)
            {
                time += Time.deltaTime;
                yield return null;
            }

            _whenEndShield?.Invoke();
            closeEpcShield();
        }

        /// <summary> 실드 - 데미지 = 계산 </summary>
        public float getDamage(float dmg)
        {
            if (_onEpc)
            {
                return 0;
            }

            if (_shieldMount >= dmg)
            {
                _shieldMount -= dmg;
                if (_shieldMount <= 0f)
                {
                    closeNorShield();
                }

                return 0;
            }
            else
            {
                _shieldMount = 0f;
                dmg -= _shieldMount;
                closeNorShield();

                return dmg;
            }
        }

        void closeNorShield()
        {
            _onNor = false;
            _norSprite.gameObject.SetActive(false);
            _whenDestNor?.Invoke();
        }

        void closeEpcShield()
        {
            _onEpc = false;
            _epcSprite.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            _onNor = _onEpc = false;
            _norSprite.gameObject.SetActive(false);
            _norLightSprite.gameObject.SetActive(false);
            _epcSprite.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                onTriggerEnemy(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("Boss"))
            {
                onTriggerEnemy(collision.gameObject);
            }
        }

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        void onTriggerEnemy(GameObject go)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            // 아직 크리티컬 없음

            float val = id.getDamaged(_player.Att * _attMount, false);
            setShieldtoDmg(val);
            // _whenDmg?.Invoke(val);

            //_efm.makeEff(effAni.attack, transform.position);
        }
    }
}