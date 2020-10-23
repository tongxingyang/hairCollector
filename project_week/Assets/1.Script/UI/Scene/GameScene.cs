using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System;

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
        [SerializeField] gameCompass _compass = null;
        [Header("Popup")]
        [SerializeField] upgradePopup _upgradePanel;
        [SerializeField] pausePopup _pausePanel;
        [SerializeField] resultPopup _resultPopup;
        [SerializeField] adRebirthPopup _adRebirthPopup;
        [Header("UI")]
        [SerializeField] Image _ExpBar;
        [SerializeField] TextMeshProUGUI _totalCoin;
        [SerializeField] GameObject _bossKillMark;
        [SerializeField] TextMeshProUGUI _killCount;
        [Header("etc")]
        [SerializeField] TextMeshProUGUI _lvlTmp;

        int _lvl = 0;

        float _coin = 0;
        int _gem = 0;
        int _ap = 0;

        int _bossKill = 0;
        int _mobKill = 0;
        int _getArti = 0;

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
        public clockManager ClockMng { get => _clockMng; }
        public gameCompass Compass { get => _compass; }

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
            _player.EnemyDamage = _enemyMng.enemyDamaged;
            _player.Blizzard = blizzard;
            _player.EnemyFrozen = _enemyMng.enemyFrozen;
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
            _getArti++;
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

        public void getKill()
        {
            _mobKill++;
            _coin += (_clockMng.Season == season.fall) ? _mopCoin * 1.2f : _mopCoin;
            _totalCoin.text = _coin.ToString();

            _player.getExp(1 * 3);
            ExpRefresh();
        }

        public void getBossKill(float val)
        {
            Debug.Log("보스킬");

            _bossKillMark.SetActive(true);

            _bossKill++;
            _killCount.text = _bossKill.ToString();

            _coin += _bossCoin * val;
            _totalCoin.text = _coin.ToString();

            _player.getExp(50);
            ExpRefresh();
        }

        public void getGem()
        {
            if (_gameOver)
            {
                return;
            }

            _gem += 1;
        }

        public void getAp(int val)
        {
            if (_gameOver)
            {
                return;
            }

            _ap += val;
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

        #endregion

        public void preGameOver()
        {            
            _player.whenPlayerDie();

            whenPause();
        }

        /// <summary> 게임 종료 및 결산 </summary>
        public void gameOver()
        {
            whenPause();

            _enemyMng.allDestroy();
            _stagePlay = false;
            _gameOver = true;

            int coinResult = (int)(_coin);
            int gemResult = _gem;
            int apResult = _ap;

            BaseManager.userGameData.Coin += coinResult;
            BaseManager.userGameData.Gem += gemResult;
            BaseManager.userGameData.Ap += apResult;

            if (BaseManager.userGameData.BossRecord < _bossKill)
            {
                BaseManager.userGameData.BossRecord = _bossKill;
            }
            if (BaseManager.userGameData.ArtifactRecord < _getArti)
            {
                BaseManager.userGameData.ArtifactRecord = _getArti;
            }

            BaseManager.userGameData.saveUserEntity();

            _resultPopup.resultInit(_clockMng.RecordTime, coinResult, gemResult, apResult, _mobKill, _bossKill, _getArti);
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

        public void openAdRebirthPanel(Action watch, Action timeover)
        {
            _adRebirthPopup.setAction(watch, timeover);
            _adRebirthPopup.open();
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