using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] Transform _player = null;
        [SerializeField] Transform _map = null;
        [SerializeField] Transform _pool = null;

        [SerializeField] GameObject[] _mops;
        [SerializeField] GameObject _hairFab;

        [Space(20)]

        [SerializeField] Image _optionPanel;
        [SerializeField] Image _chkExitPanel;

        [Space(20)]

        [SerializeField] RectTransform _hpBarCase;
        [SerializeField] Image _hpBar;

        [Space]

        [SerializeField] Text _totalHair;
        [SerializeField] Text _nowHair;

        List<EnemyControl> _enemyList;
        List<hairCoinControl> _hairList;

        float _speed = 1f;

        float maxHp = 10;
        float playerHp = 10;

        float _etime;
        float _eterm = 2.5f;

        float _htime;
        float _hterm = 2.5f;

        int _hair = 0;

        // Start is called before the first frame update
        void Start()
        {
            _enemyList = new List<EnemyControl>();
            _hairList = new List<hairCoinControl>();
            playerHp = maxHp = BaseManager.userEntity.Hp;
            _hpBarCase.sizeDelta = new Vector2(maxHp + 20, 120);
            _totalHair.text = BaseManager.userEntity.HairCoin.ToString() + "모";
            _nowHair.text = _hair.ToString() + "모";
            _hpBar.fillAmount = 1f;

            optionStart();
        }

        // Update is called once per frame
        void Update()
        {
            _map.position -= new Vector3(0, _speed * Time.deltaTime);

            if (_map.position.y <= -10.24f)
            {
                _map.position = Vector3.zero;
            }

            _etime += Time.deltaTime;
            if (_etime > _eterm)
            {
                _etime = 0;
                _eterm -= 0.01f;
                makeEnemy();
            }

            _htime += Time.deltaTime;
            if (_htime > _hterm)
            {
                _htime = 0;
                _hterm -= 0.01f;
                makeHair();
            }
        }

        Vector2 mopRespawnsPos()
        {
            if (Random.Range(0, 2) == 0)
            {
                float x = (Random.Range(0, 2) == 0) ? 3.5f : -3.5f;
                return new Vector2(x, Random.Range(0, 6f));
            }
            else
            {
                return new Vector2(Random.Range(-3.5f, 3.5f), 6f);
            }
        }

        void makeEnemy()
        {
            int val = Random.Range(0, 10);
            Enemy type = (val == 9) ? Enemy.middleBoss : (val >= 6) ? Enemy.mop2 : Enemy.mop1;

            foreach (EnemyControl ec in _enemyList)
            {
                if (ec.getType == type && ec.isUse == false)
                {
                    ec.transform.position = mopRespawnsPos();
                    ec.Init(_player.position - ec.transform.position);

                    return;
                }
            }

            EnemyControl ect = Instantiate(_mops[(int)type]).GetComponent<EnemyControl>();
            _enemyList.Add(ect);
            ect.transform.parent = _pool;
            ect.transform.position = mopRespawnsPos();
            ect.setting(this);
            ect.Init(_player.position - ect.transform.position);
        }

        void makeHair()
        {
            foreach (hairCoinControl hcc in _hairList)
            {
                if (hcc.isUse == false)
                {
                    hcc.transform.position = new Vector2(Random.Range(-3f, 3f), 6f);
                    hcc.Init();
                    return;
                }
            }

            hairCoinControl hcct = Instantiate(_hairFab).GetComponent<hairCoinControl>();
            _hairList.Add(hcct);
            hcct.transform.parent = _pool;
            hcct.transform.position = new Vector2(Random.Range(-3f, 3f), 6f);
            hcct.setting(this);
            hcct.Init();
        }

        public void getDamaged(int dmg)
        {
            playerHp -= dmg;
            _hpBar.fillAmount = playerHp / maxHp;
            Debug.Log(playerHp);
            if (playerHp < 1)
            {
                BaseManager.userEntity.HairCoin += _hair;
                BaseManager.instance.convertScene(SceneNum.GameScene.ToString(), SceneNum.LobbyScene);
            }
        }

        public void getHair(int val)
        {
            _hair += val;
            _nowHair.text = _hair.ToString() + "모";
        }

        #region [option]

        void optionStart()
        {
            _optionPanel.gameObject.SetActive(false);
            _chkExitPanel.gameObject.SetActive(false);
        }

        public void openOption()
        {
            _optionPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }        
        public void optionResume()
        {
            Time.timeScale = 1;
            _optionPanel.gameObject.SetActive(false);
        }

        public void chkExitOpen()
        {
            _chkExitPanel.gameObject.SetActive(true);
        }
        public void chkExitClose()
        {
            _chkExitPanel.gameObject.SetActive(false);
        }
        public void exitGame()
        {
            Time.timeScale = 1;
            BaseManager.instance.convertScene(SceneNum.GameScene.ToString(), SceneNum.LobbyScene);
        }

        #endregion
    }
}