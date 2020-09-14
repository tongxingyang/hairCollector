using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class GameScene : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] PlayerCtrl _player = null;
        [Header("Game")]
        [SerializeField] Joystick _joyStick = null;
        [SerializeField] SnowController _snow = null;
        [Header("Manager")]
        [SerializeField] MapManager _mapMng = null;
        ObstacleManager _obtMng;
        [SerializeField] enemyManager _enemyMng = null;
        EnemyProjManager _enProjMng;
        effManager _efMng;
        playerSkillManager _skillMng;
        dmgFontManager _dmgfntMng;
        [SerializeField] clockManager _clockMng = null;
        [Header("Popup")]
        [SerializeField] upgradePopup _upgradePanel;
        [SerializeField] pausePopup _pausePanel;
        [SerializeField] resultPopup _resultPopup;
        [Header("UI")]
        [SerializeField] Image _ExpBar;
        [SerializeField] TextMeshProUGUI _totalCoin;
        [SerializeField] TextMeshProUGUI _killCount;
        [Header("etc")]
        [SerializeField] TextMeshProUGUI _lvlTmp;

        int _lvl = 0;

        int _coin = 0;
        int _gem = 0;
        int _ap = 0;

        int _kill = 0;

        Vector3 targetPos;        

        float _mopCoin;
        float _bossCoin;

        bool _pause;
        bool _gameOver;
        bool _stagePlay;

        public PlayerCtrl Player { get => _player; }
        public bool Pause { get => _pause; set => _pause = value; }
        public bool GameOver { get => _gameOver; }
        public bool StagePlay { get => _stagePlay; }

        public Vector2 pVector { get => _joyStick.Direction; }

        bossControl _boss;
        public MapManager MapMng { get => _mapMng; }
        public ObstacleManager ObtMng { get => _obtMng; }
        public enemyManager EnemyMng { get => _enemyMng; }
        public EnemyProjManager EnProjMng { get => _enProjMng; }
        public effManager EfMng { get => _efMng; }
        public playerSkillManager SkillMng { get => _skillMng; }
        public dmgFontManager DmgfntMng { get => _dmgfntMng; }

        public float RecordTime { get => _clockMng.RecordTime; }
        public clockManager ClockMng { get => _clockMng; set => _clockMng = value; }

        void test()
        {
            DataManager.LoadBGdata();
        }
        void Start()
        {
            //test();

            _stagePlay = false;
            _gameOver = false;

            managersManager();

            _mopCoin = gameValues._firstMopCoin;
            _bossCoin = gameValues._firstBossCoin;

            _player._gameOver = gameOver;
            _player.EnemyDamage = enemyDamaged;
            _player.Blizzard = blizzard;
            _player.EnemyFrozen = enemyFrozen;
            _ExpBar.fillAmount = 0f;

            _pausePanel.pauseStart(this);
            _upgradePanel.setting(_player, closeUpgradePanel);

            SoundManager.instance.PlayBGM(BGM.Battle);
            standardSnow();

            StartCoroutine(move());
            StartCoroutine(_enemyMng.startMakeEnemy());

            _stagePlay = true;
        }

        void managersManager()
        {
            _obtMng = _mapMng.GetComponent<ObstacleManager>();

            _enProjMng = _enemyMng.GetComponent<EnemyProjManager>();
            _efMng = _enemyMng.GetComponent<effManager>();
            _skillMng = _enemyMng.GetComponent<playerSkillManager>();
            _dmgfntMng = _enemyMng.GetComponent<dmgFontManager>();

            _enProjMng.Init(this);
            _enemyMng.Init(this);
            _obtMng.Init(this);
            _mapMng.Init(this);

            _efMng.Init(this);
            _skillMng.Init(this);
            _dmgfntMng.Init();

            _clockMng.Init(this);
            _player.Init(this);
        }

        IEnumerator move()
        {
            yield return new WaitUntil(() => _stagePlay == true);

            while (_gameOver == false)
            {
                yield return new WaitUntil(() => _pause == false);

                _clockMng.accTime(Time.deltaTime);

                _player.setMove(_joyStick.Direction);

                if (_mapMng.middleTilePos.y - _player.transform.position.y < -10.24f)
                {
                    _mapMng.playerMoveUp();
                }
                else if (_mapMng.middleTilePos.y - _player.transform.position.y > 10.24f)
                {
                    _mapMng.playerMoveDown();
                }

                if (_mapMng.middleTilePos.x - _player.transform.position.x < -10.24f)
                {
                    _mapMng.playerMoveRight();
                }
                else if (_mapMng.middleTilePos.x - _player.transform.position.x > 10.24f)
                {
                    _mapMng.playerMoveLeft();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #region EXP

        void ExpRefresh()
        {
            _ExpBar.fillAmount = _player.ExpRate;
        }

        public void levelUp()
        {
            if (_gameOver)
            {
                return;
            }

            whenPause();            
            //Time.timeScale = 0;

            _lvl++;
            _lvlTmp.text = $"lvl.{_lvl.ToString()}";

            _upgradePanel.levelUpOpen();
        }

        public void getEquip()
        {
            //whenPause();
            ////Time.timeScale = 0;

            //_upgradePanel.presentOpen();
            StartCoroutine(getEquipCo());
        }

        IEnumerator getEquipCo()
        {
            yield return new WaitUntil(() => Pause == false);

            whenPause();
            //Time.timeScale = 0;

            _upgradePanel.presentOpen();
        }

        #endregion

        public void getKill(int exp)
        {
            _killCount.text = (++_kill).ToString();
            _player.getExp(exp*3);
            getCoin(cointype.mopCoin);
            ExpRefresh();
        }

        public void getBossKill()
        {
            _player.getExp(50);
        }

        public void getCoin(cointype ctype)
        {
            if (_gameOver)
            {
                return;
            }

            switch (ctype) 
            {
                case cointype.mopCoin:
                    _coin += (int)_mopCoin;
                    break;
                case cointype.extraCoin:
                    _coin += (int)(_bossCoin);
                    break;
            }
            _totalCoin.text = _coin.ToString();
        }

        #region 카메라 안 군중제어

        /// <summary> 플레이어 공격시 첫타겟 선택용 </summary>
        public Vector3 mostCloseEnemy(Transform from)
        {
            float dist = float.MaxValue;
            float val;

            if (_enemyMng.BossList.Count > 0)
            {
                for (int i = 0; i < _enemyMng.BossList.Count; i++)
                {
                    if (_enemyMng.BossList[i].IsUse)
                    {
                        val = Vector3.Distance(_enemyMng.BossList[i].transform.position, from.position);
                        if (val < dist && val < 5f)
                        {
                            dist = val;
                            targetPos = _enemyMng.BossList[i].transform.position;
                        }
                    }
                }
            }

            if (dist < float.MaxValue)
                return targetPos;

            if (_enemyMng.EnemyList.Count > 0)
            {
                for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
                {
                    if (_enemyMng.EnemyList[i].IsUse)
                    {
                        val = _enemyMng.EnemyList[i].PlayerDist;
                        if (val < dist)
                        {
                            dist = val;
                            targetPos = _enemyMng.EnemyList[i].transform.position;
                        }
                    }
                }
            }

            return targetPos;
        }

        /// <summary> 튕길때 최소사거리 이상 적 선택용 </summary>
        public Vector3 mostCloseEnemy(Transform from, float min)
        {
            float dist = float.MaxValue;
            float val;

            for (int i = 0; i < _enemyMng.BossList.Count; i++)
            {
                if (_enemyMng.BossList[i].IsUse)
                {
                    val = Vector3.Distance(_enemyMng.BossList[i].transform.position, from.position);
                    if (val > min && val < dist && val < 5f)
                    {
                        dist = val;
                        targetPos = _enemyMng.BossList[i].transform.position;
                    }
                }
            }

            for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
            {
                if (_enemyMng.EnemyList[i].IsUse)
                {
                    val = Vector3.Distance(_enemyMng.EnemyList[i].transform.position, from.position);
                    if (val > min && val < dist)
                    {
                        dist = val;
                        targetPos = _enemyMng.EnemyList[i].transform.position;
                    }
                }
            }

            return targetPos;
        }

        /// <summary> 최대사거리 이내 아무나 </summary>
        public Vector3 randomCloseEnemy(Transform from, float max)
        {
            float val;

            if (_enemyMng.BossList.Count > 0)
            {
                for (int i = 0; i < _enemyMng.BossList.Count; i++)
                {
                    if (_enemyMng.BossList[i].IsUse)
                    {
                        val = Vector3.Distance(_enemyMng.BossList[i].transform.position, from.position);
                        if (val < max)
                        {
                            return _enemyMng.BossList[i].transform.position;
                        }
                    }
                }
            }

            if (_enemyMng.EnemyList.Count > 0)
            {
                for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
                {
                    if (_enemyMng.EnemyList[i].IsUse)
                    {
                        val = _enemyMng.EnemyList[i].PlayerDist;
                        if (val < max)
                        {
                            targetPos = _enemyMng.EnemyList[i].transform.position;
                            break;
                        }
                    }
                }
            }

            return targetPos;
        }

        public void enemyFrozen(float term)
        {
            for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
            {
                _enemyMng.EnemyList[i].setFrozen(term);
            }
        }

        public void enemyDamaged(float dmg)
        {
            for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
            {
                if (_enemyMng.EnemyList[i].IsUse)
                {
                    _enemyMng.EnemyList[i].getDamaged(dmg);
                }
            }
        }

        public void enemySlow(bool last, float term, float slow)
        {
            for (int i = 0; i < _enemyMng.EnemyList.Count; i++)
            {
                _enemyMng.EnemyList[i].setBuff(eDeBuff.slow, last, term, slow);
            }
        }

        #endregion
           
        public void gameOver()
        {
            whenPause();

            _enemyMng.allDestroy();
            _stagePlay = false;
            _gameOver = true;

            int coinResult = _coin * BaseManager.userEntity.DoubleCoin;
            int gemResult = _gem * BaseManager.userEntity.DoubleCoin;
            int apResult = _ap * BaseManager.userEntity.DoubleCoin;

            BaseManager.userEntity.Coin += coinResult;
            BaseManager.userEntity.Gem += gemResult;
            BaseManager.userEntity.Ap += apResult;

            BaseManager.instance.saveUserData();

            _resultPopup.resultInit(_clockMng.RecordTime, coinResult, gemResult, apResult);
        }

        #region [Window]

        public void openPause()
        {
            _pausePanel.openPause();
        }

         void closeUpgradePanel()
        {
            Time.timeScale = 1;
            whenResume();
            _player.setAlmighty();
        }

        #endregion

        public void whenPause()
        {
            _pause = true;

            _player.onPause(true);

            foreach (MobControl mc in _enemyMng.EnemyList)
            {
                mc.onPause(true);
            }
            foreach (bossControl bc in _enemyMng.BossList)
            {
                bc.onPause(true);
            }
            foreach (effControl ec in _efMng.EffList)
            {
                ec.onPause(true);
            }
        }
        public void whenResume()
        {
            _pause = false;

            _player.onPause(false); 

            foreach (MobControl mc in _enemyMng.EnemyList)
            {
                mc.onPause(false);
            }
            foreach (bossControl bc in _enemyMng.BossList)
            {
                bc.onPause(false);
            }
            foreach (effControl ec in _efMng.EffList)
            {
                ec.onPause(false);
            }
        }

        public void blizzard(bool bl)
        {
            if (bl)
            {
                hardSnow();
            }
            else
            {
                standardSnow();
            }
        }

        void standardSnow()
        {
            _snow.OnMasterChanged(1f);
            _snow.OnSnowChanged(0f);
            _snow.OnWindChanged(0f);
            _snow.OnFogChanged(0f);
        }

        void hardSnow()
        {
            _snow.OnMasterChanged(1f);
            _snow.OnSnowChanged(1f);
            _snow.OnWindChanged(0.5f);
            _snow.OnFogChanged(0.5f);
        }
    }
}