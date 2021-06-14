using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System;
using NaughtyAttributes;

namespace week
{
    public class GameScene : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] PlayerCtrl _player = null;
        [Header("Game")]
        [SerializeField] Joystick _joyStick = null;
        [SerializeField] SnowController _snow = null;
        [SerializeField] CanvasScaler _ui;
        [Header("Manager")]
        [SerializeField] MapManager _mapMng = null;
        ObstacleManager _obtMng;
        [SerializeField] enemyManager _enemyMng = null;
        EnemyProjManager _enProjMng;
        effManager _efMng;
        playerSkillManager _skillMng;
        dmgFontManager _dmgfntMng;
        [SerializeField] UserGameInterface _inGameInterface = null;
        [SerializeField] clockManager _clockMng = null;
        [Header("Popup")]
        [SerializeField] upgradePopup _upgradePanel;
        [SerializeField] pausePopup _pausePanel;
        [SerializeField] resultPopup _resultPopup;
        [SerializeField] adRebirthPopup _adRebirthPopup;
        [SerializeField] snowQuestPopup _inQuestPopup;

        // ========= [ none character data ] ===============

        /// <summary> 레벨 </summary>
        public int Lvl { get; private set; } = 1;
        /// <summary> 코인 </summary>
        public float Coin { get; private set; } = 0;
        /// <summary> 보석 </summary>
        public int Gem { get; private set; } = 0;
        /// <summary> 몹킬 </summary>
        public int MobKill { get; private set; } = 0;
        /// <summary> 보스킬 </summary>
        public int BossKill { get; private set; } = 0;
        /// <summary> 퀘 </summary>
        public int ClearQst { get; private set; } = 0;
        /// <summary> 부활 </summary>
        public int RebirthQst { get; set; } = 0;

        // ========= [  ] ===============

        public levelKey StageLevel { get; private set; }
        float _mobDayIncrease;
        float _coinLevel;

        // ========= [  ] ===============

        Vector3 targetPos;

        public PlayerCtrl Player { get => _player; }
        public bool Uping { get; set; }
        public bool Pause { get; private set; }
        public bool GameOver { get; private set; }
        public bool StagePlay { get; private set; }

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
        public snowQuestPopup InQuestPopup { get => _inQuestPopup; }
        public upgradePopup UpGradePopup { get => _upgradePanel; }

        public clockManager ClockMng { get => _clockMng; }
        public float RecordTime { get => _clockMng.RecordSecond; }
        public UserGameInterface InGameInterface { get => _inGameInterface; }

        public GameResource _gameResource { get; private set; }
        void test()
        {
            DataManager.LoadBGdata();
        }

        void Start()
        {
            // 이전 씬 소속 정리
            if (BaseManager.userGameData.RemoveAd == false)
            {
            }

            if ((float)Screen.height / Screen.width > 2960f / 1440f)
            {
                _ui.matchWidthOrHeight = 0f;
            }

            StageLevel = (levelKey)(int)BaseManager.userGameData.NowStageLevel;
            _mobDayIncrease = D_level.GetEntity(StageLevel.ToString()).f_increase;
            _coinLevel = D_level.GetEntity(StageLevel.ToString()).f_coin;

            // 새로운 씬 초기화
            //test();

            StagePlay = false;
            GameOver = false;

            managersManager();

            if(StageLevel == levelKey.hard)
                SoundManager.instance.PlayBGM(BGM.BattleHard);
            else
                SoundManager.instance.PlayBGM(BGM.Battle);

            // DataManager.getDataInGame();

            StartCoroutine(gameTimeFlow());
            // StartCoroutine(_enemyMng.startMakeEnemy());

            WindowManager.instance.Win_coinGenerator.RefreshFollowCost = wealthRefresh;
            BaseManager.userGameData.PlayCount++;

            StagePlay = true;
        }

        void managersManager()
        {
            _obtMng = _mapMng.GetComponent<ObstacleManager>();

            _enProjMng = _enemyMng.GetComponent<EnemyProjManager>();
            _efMng = _enemyMng.GetComponent<effManager>();
            _skillMng = _enemyMng.GetComponent<playerSkillManager>();
            _dmgfntMng = _enemyMng.GetComponent<dmgFontManager>();

            _gameResource = GetComponent<GameResource>();

            _inQuestPopup.Init(this);

            _clockMng.Init(this);
            _obtMng.Init(this);
            _enemyMng.Init(this);
            _mapMng.Init(this);

            _enProjMng.Init(this);

            _efMng.Init(this);
            _skillMng.Init(this);
            _dmgfntMng.Init();

            _inGameInterface.Init(this);
            _pausePanel.Init(this);

            _player.Init(this, gameOver);
            _upgradePanel.Init(this, whenCloseUpgradePanel);
            _resultPopup.Init(this);
        }

        IEnumerator gameTimeFlow()
        {
            yield return new WaitUntil(() => StagePlay == true);
            float deltime = 0f;

            while (GameOver == false)
            {
                yield return new WaitUntil(() => Pause == false);

                deltime = Time.deltaTime;

                _clockMng.accTime(deltime);
                _enemyMng.makeEnemy(deltime);
                move(deltime);

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary> 이동 및 맵 </summary>
        private void move(float deltime)
        {
            // 이동
            _player.setMove(_joyStick.Direction);

            // 맵 조작
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
        }

        #region EXP

        public void levelUp()
        {
            if (GameOver)
            {
                return;
            }

            whenPause();

            Lvl++;
            InGameInterface.LevelRefresh(Lvl);

            SoundManager.instance.PlaySFX(SFX.levelup);
            _upgradePanel.getSkillTreeOpen(NotiType.levelUp);
        }

        /// <summary> 바로 스킬 선택 </summary>
        public void confirm_skill(SkillKeyList sk, NotiType noti)
        {
            if (GameOver)
            {
                return;
            }

            whenPause();
            _upgradePanel.confirm_skillTree(sk, noti);
        }

        #endregion

        /// <summary>  </summary>
        public void kill_mob(float exp, float coin, Mob mob)
        {
            MobKill++;
            
            getCoin(coin);

            inQuest_goal_valtype igv = inQuest_goal_valtype.fire;
            switch (mob)
            {
                case Mob.closed:
                    igv = inQuest_goal_valtype.close;
                    break;
                case Mob.ranged:
                    igv = inQuest_goal_valtype.range;
                    break;
                case Mob.solid:
                    igv = inQuest_goal_valtype.hard;
                    break;
            }
            _inQuestPopup.getData(inQuest_goal_key.kill, igv, _player.hpRate);

            _player.getExp(exp);
        }

        public void kill_Boss(float _bossCoin, float _bossExp)
        {
            Debug.Log("보스킬");

            BossKill++;
            _inGameInterface.bossKillRefresh(BossKill);

            getCoin(_bossCoin);
            _player.getExp(_bossExp);
        }

        public void getCoin(float coin, bool isAni = false)
        {
            if (GameOver)
                return;

            Coin += coin * _player.Coin;

            _inGameInterface.getCoin(Coin);
        }

        public void getGem()
        {
            if (GameOver)
            {
                return;
            }

            Gem += 1;

            _inGameInterface.GemRefresh(Gem);
        }

        //public void getAp(int val)
        //{
        //    if (GameOver)
        //    {
        //        return;
        //    }

        //    _ap += val;

        //    _inGameInterface.ApRefresh(_ap);
        //}

        /// <summary> 퀘스트 얻기 </summary>
        public void getInQuest()
        {
            if (GameOver)
            {
                return;
            }

            whenPause();
            _inQuestPopup.open();
        }

        public void setInQuestData(inQuest_goal_key gk, inQuest_goal_valtype gv, float val)
        {            
            _inQuestPopup.getData(gk, gv, val);
        }
        public void InQuestTimeCheck(inQuest_goal_key gk, inQuest_goal_valtype gv, float val)
        {
            _inQuestPopup.getData_time(gk, gv, val);
        }

        public void setInQuestData(inQuest_goal_key gk, gainableTem tem, float val)
        {
            _inQuestPopup.getData(gk, tem, val);
        }

        /// <summary> 퀘스트 완료 (퀘보상)메달 지급 </summary>
        public void clearQuest()
        {
            Player.getTem(gainableTem.questKey);
            ClearQst++;
        }

        public void wealthRefresh()
        {
            _inGameInterface.wealthRefresh(Gem);
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

        /// <summary> 게임 종료 및 결산 </summary>
        public void gameOver()
        {
            whenPause();

            _enemyMng.allDestroy();
            StagePlay = false;
            GameOver = true;

            // BaseManager.userGameData.SaveDataServer();

            _upgradePanel.gameObject.SetActive(false);
            _pausePanel.gameObject.SetActive(false);
            _adRebirthPopup.gameObject.SetActive(false);

            _resultPopup.setRresult();
        }


        #region [Window]

        public void openPause()
        {
            if (GameOver)
            {
                return;
            }

            _pausePanel.openPause();
        }

        void whenCloseUpgradePanel()
        {
            Time.timeScale = 1;
            Uping = false;

            whenResume(true);

            _player.whenLevUpExp();
        }

        public void openAdRebirthPanel(Action watch, Action timeover)
        {
            _adRebirthPopup.setAction(watch, timeover);
            _adRebirthPopup.open();
        }

        #endregion

        public void whenPause()
        {
            Pause = true;

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
        public void whenResume(bool isAlmighty = false)
        {
            Pause = false;

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

            if (isAlmighty)
            {
                _player.setAlmighty();
            }
        }
    }
}