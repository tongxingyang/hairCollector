﻿using System.Collections;
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
        [Space]
        [SerializeField] GameObject _coinIcon;
        [SerializeField] TextMeshProUGUI _coinTxt;
        [SerializeField] GameObject _gemIcon;
        [SerializeField] TextMeshProUGUI _gemTxt;
        [SerializeField] GameObject _apIcon;
        [SerializeField] TextMeshProUGUI _apTxt;

        public bool gemIcon { set { _gemIcon.SetActive(value); _gemTxt.gameObject.SetActive(value); } }
        public bool apIcon { set { _apIcon.SetActive(value); _apTxt.gameObject.SetActive(value); } }

        [Space]
        [SerializeField] GameObject _bossKillMark;
        [SerializeField] TextMeshProUGUI _killCount;
        [Header("etc")]
        [SerializeField] TextMeshProUGUI _lvlTmp;

        public int _lvl = 1;

        float _coin = 0;
        int _gem = 0;
        int _ap = 0;

        int _bossKill = 0;
        int _mobKill = 0;
        int _getArti = 0;

        Vector3 targetPos;

        bool _pause;
        bool _gameOver;
        bool _stagePlay;

        public PlayerCtrl Player { get => _player; }
        public bool Uping { get; set; }
        public bool Pause { get => _pause; set => _pause = value; }
        public bool GameOver { get => _gameOver; }
        public bool StagePlay { get => _stagePlay; }

        Vector2 _pVec;
        public Vector2 pVector
        {
            get
            {
                if (_joyStick.Direction != Vector2.zero)
                {
                    _pVec = _joyStick.Direction;
                }

                return _pVec;
            }
        }

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
            // 이전 씬 소속 정리
            if (BaseManager.userGameData.RemoveAd == false)
            {
                BaseManager.userGameData.IsSetRader = false;
            }

            // 새로운 씬 초기화
            //test();

            _stagePlay = false;
            _gameOver = false;
            gemIcon = false;
            apIcon = false;

            managersManager();

            _player._gameOver = gameOver;
            _ExpBar.fillAmount = 0f;

            _pausePanel.pauseStart(this);
            _upgradePanel.setting(_player, whenCloseUpgradePanel);

            SoundManager.instance.PlayBGM(BGM.Battle);

            StartCoroutine(move());
            StartCoroutine(_enemyMng.startMakeEnemy());

            WindowManager.instance.Win_coinGenerator.RefreshFollowCost = wealthRefresh;
            BaseManager.userGameData.PlayCount++;

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

        public void ExpRefresh(float val)
        {
            _ExpBar.fillAmount = val;
        }

        public void levelUp()
        {
            if (_gameOver)
            {
                return;
            }

            whenPause();        

            _lvl++;
            _lvlTmp.text = $"lvl.{_lvl.ToString()}";

            _upgradePanel.levelUpOpen();
        }

        /// <summary> 유물 대신 렙업스킬 </summary>
        public void getEquip()
        {
            if (_gameOver)
            {
                return;
            }

            // _getArti++;
            whenPause();
            _upgradePanel.levelUpOpen();
            // StartCoroutine(getEquipCo());
        }

        //IEnumerator getEquipCo()
        //{
        //    yield return new WaitUntil(() => Pause == false);

        //    whenPause();

        //    _upgradePanel.presentOpen();
        //}

        #endregion

        public void getKill(float exp, float coin)
        {
            _mobKill++;
            
            getCoin(coin);

            _player.getExp(exp);
        }

        public void getBossKill(float _bossCoin)
        {
            Debug.Log("보스킬");

            _bossKillMark.SetActive(true);

            _bossKill++;
            _killCount.text = _bossKill.ToString();

            getCoin(_bossCoin);

            _player.getExp(gameValues._startBobExp);
        }

        public void getCoin(float coin, bool isAni = false)
        {
            if (_gameOver)
                return;

            float seasonCoin = 1f;// ((_clockMng.Season == season.fall) ? 1.2f : 1f);
            _coin += coin * _player.Coin * seasonCoin;

            if (isAni)
            {
                WindowManager.instance.Win_coinGenerator.getDirect(_coinIcon.transform.position, currency.coin, 1);
            }
            else
            {
                _coinTxt.text = Convert.ToInt32(_coin).ToString();
            }
        }

        public void getGem()
        {
            if (_gameOver)
            {
                return;
            }

            _gem += 1;
            //_gemTxt.text = _gem.ToString();

            WindowManager.instance.Win_coinGenerator.getDirect(_gemIcon.transform.position, currency.gem, 1);
        }

        public void getAp(int val)
        {
            if (_gameOver)
            {
                return;
            }

            _ap += val;
            //_apTxt.text = _ap.ToString();

            WindowManager.instance.Win_coinGenerator.getDirect(_apIcon.transform.position, currency.ap, 1);
        }

        public void wealthRefresh()
        {
            _gemTxt.text = _gem.ToString();
            _apTxt.text = _ap.ToString();

            if (_gem > 0)
                gemIcon = true;

            if (_ap > 0)
                apIcon = true;
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

            if (BaseManager.userGameData.BossRecord < _bossKill)
            {
                BaseManager.userGameData.BossRecord = _bossKill;
            }
            if (BaseManager.userGameData.ArtifactRecord < _getArti)
            {
                BaseManager.userGameData.ArtifactRecord = _getArti;
            }

            // BaseManager.userGameData.SaveDataServer();

            _upgradePanel.gameObject.SetActive(false);
            _pausePanel.gameObject.SetActive(false);
            _adRebirthPopup.gameObject.SetActive(false);

            _resultPopup.resultInit(_clockMng.RecordTime, coinResult, gemResult, apResult, _mobKill, _bossKill, _getArti);
        }


        #region [Window]

        public void openPause()
        {
            if (_gameOver)
            {
                return;
            }

            _pausePanel.openPause();
        }

        void whenCloseUpgradePanel()
        {
            Time.timeScale = 1;
            Uping = false;

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
    }
}