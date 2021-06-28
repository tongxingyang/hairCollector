using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class enemyManager : MonoBehaviour
    {
        class mobData
        {
            public Mob _mob;
            public List<MobControl> _mobList;
            public Queue<MobControl> _offMobList;

            public float _eterm;
            public int _max;
            float _etime;

            public mobData(Mob mob, levelKey lvl)
            {
                _mob = mob;

                string[] str = D_monster.GetEntity(mob.ToString()).f_appear_term.Split(',');
                _eterm = float.Parse(str[(str.Length > 1) ? (int)lvl : 0]);

                _max = D_monster.GetEntity(mob.ToString()).f_mount;

                _mobList = new List<MobControl>();
                _offMobList = new Queue<MobControl>();
            }

            public bool chkTime(float deltime)
            {
                if (_eterm == -1)
                    return false;

                _etime += deltime;
                if (_etime > _eterm)
                {
                    _etime = 0;
                    return true;
                }
                return false;
            }
        }

        // ================== [ mng ] =========================
        GameScene _gs; 

        MapManager _map;
        dmgFontManager _dmgFont;
        EnemyProjManager _epm;
        effManager _efm;
        clockManager _clock;

        Transform _playerPos;

        // ================== [ mob ] =========================
        mobData[] _mobDatas; 
        List<bossControl> _bossList;
        attackData _adata = new attackData();

        // ================== [ mob addit status ] =========================
        float[] _initBff;                           // 생성시 적용 버프

        List<BuffEffect> _bffList;                  // 버프 리스트
        Dictionary<enemyStt, float> _fieldBuff;     // 버프리스트가 적용된 값

        public float MobRate { get; private set; }  // 난이도별 몹강화 배율
        public float MobDayRate { get; private set; } = 1f;  // 날짜별 몹강화 배율
        public float MobDefRate { get; private set; } // 날짜별 몹 방어력 배율
        public float MobIncSpeed { get; private set; } = 1f; // 날짜별 몹 추가 스피드
        int _mobWave;

        private mobData[] MobDatas { get => _mobDatas; set => _mobDatas = value; }
        public List<MobControl> EnemyList { get; set; } // 일단 지금 만들어진 몹 전부
        public List<bossControl> BossList { get => _bossList; }
        public float[] InitBff { get => _initBff; set => _initBff = value; }
        public Dictionary<enemyStt, float> FieldBuff { get => _fieldBuff; set => _fieldBuff = value; }

        // ================== [ 현재 변동 없음 + 굳이 필요없음 + 확장성 + 테스트 ] =========================
        
        public bool mobOff { get; set; } = false;   // 몹 켜기 끄기(테스트용)
        int _stageEnemyAmount = 1;                  // 한번에 생성될 몹양

        // =================================================

        /// <summary> 초기화 </summary>
        public void Init(GameScene gs)
        {
            _gs = gs;

            _map = _gs.MapMng;
            _dmgFont = _gs.DmgfntMng;
            _epm = _gs.EnProjMng;
            _efm = _gs.EfMng;
            _clock = _gs.ClockMng;
            //_clock.changeSS += cal_MobSeasonRate;
            _clock.changeDay += cal_MobDayRate;

            _playerPos = _gs.Player.transform;

            _bossList = new List<bossControl>();

            _initBff = new float[3] { 1f, 1f, 0f };
            _bffList = new List<BuffEffect>();
            _fieldBuff = new Dictionary<enemyStt, float>();
            _fieldBuff.Add(enemyStt.ATT, 1f);
            _fieldBuff.Add(enemyStt.DEF, 0f);
            _fieldBuff.Add(enemyStt.SPEED, 1f);
            _fieldBuff.Add(enemyStt.SIZE, 1f);

            _mobDatas = new mobData[(int)Mob.max];
            for (Mob i = Mob.fire; i < Mob.max; i++)
            {
                _mobDatas[(int)i] = new mobData(i, BaseManager.userGameData.NowStageLevel);
            }
            EnemyList = new List<MobControl>();

            MobRate = D_level.GetEntity(_gs.StageLevel.ToString()).f_mobrate;
        }

        /// <summary> 날짜바뀔때 </summary>
        void cal_MobDayRate()
        {
            // 몹 방어력
            MobDefRate += Mathf.Sqrt(12f / (_gs.ClockMng.RecordDay + 1));
            
            // 몹 강화
            int day = _gs.ClockMng._dayInSeason;
            MobDayRate = Mathf.Pow(gameValues._mobIncrease, day) + gameValues._mobIncrease2 * _gs.ClockMng.RecordMonth;

            // 몹 이속
            float sp = 1f + (_gs.ClockMng.RecordDay * 0.006f);
            MobIncSpeed = (sp > 1.1f) ? 1.1f : sp;
        }

        ///// <summary> 계절 바뀔때 (참조변수는 장식용) </summary>
        //void cal_MobSeasonRate(season ss)
        //{
            
        //}

        #region [mop]

        /// <summary> 몹 제작 루틴 </summary>
        //public IEnumerator startMakeEnemy()
        public void makeEnemy(float deltime)
        {
            if (mobOff)
                return;
            {
                if (_clock.chk3Wave)
                    _mobWave = 3;
                else if (_clock.chk2Wave)
                    _mobWave = 2;
                else if (_clock.chk2Wave)
                    _mobWave = 1;
                else
                    _mobWave = 0;

                if (_mobDatas[(int)Mob.fire].chkTime(deltime))
                {
                    for (int j = 0; j < _stageEnemyAmount; j++)
                    {
                        makeEnemySet(Mob.fire);
                    }
                }
                if (_mobDatas[(int)Mob.ash].chkTime(deltime))
                {
                    for (int j = 0; j < _stageEnemyAmount; j++)
                    {
                        makeEnemySet(Mob.ash);
                    }
                }

                deBuffChk(deltime);

                // 몹 생성
                // 근거리
                if (_clock.chk1Wave)
                {
                    if (_mobDatas[(int)Mob.closed].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.closed);
                        }
                    }
                }
                else
                    return;

                // 원거리
                if (_clock.chk2Wave)
                {
                    if (_mobDatas[(int)Mob.ranged].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.ranged);
                        }
                    }
                }
                else
                    return;

                // 튼튼이
                if (_clock.chk3Wave)
                {
                    if (_mobDatas[(int)Mob.solid].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.solid);
                        }
                    }
                }
            }
        }

        IEnumerator chk()
        {
            while (true)
            {
                Debug.Log("spd : " + _fieldBuff[enemyStt.SPEED]);
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary> [주문]몹 생성 </summary>
        public void makeEnemySet(Mob type)
        {
            makeEnemy(type, mopRespawnsPos());
        }

        /// <summary> [공장]몹 생성 </summary>
        void makeEnemy(Mob type, Vector3 pos)
        {
            if (_mobDatas[(int)type]._offMobList.Count > 0)
            {
                MobControl mc = _mobDatas[(int)type]._offMobList.Dequeue();
                _mobDatas[(int)type]._mobList.Add(mc);
                EnemyList.Add(mc);

                mc.transform.position = pos;
                mc.RepeatInit(_gs.ClockMng.NowSeason);

                return;
            }

            if (_mobDatas[(int)type]._mobList.Count >= gameValues._mobMaxCount * _mobDatas[(int)type]._max / BaseManager.PreGameData.MobFullRate[_mobWave])
            {
                return;
            }

            // 없으면 생성
            MobControl ect = Instantiate(DataManager.MobFabs[type]).GetComponent<MobControl>();
            _mobDatas[(int)type]._mobList.Add(ect);
            EnemyList.Add(ect);
            ect.transform.parent = transform;

            ect.setting(_gs);
            ect.transform.position = pos;
            ect.FixInit(_gs.ClockMng.NowSeason);
        }

        public void getOffEnemyList(MobControl mc)
        {
            EnemyList.Remove(mc);
            _mobDatas[(int)mc.EnemyType]._mobList.Remove(mc);
            _mobDatas[(int)mc.EnemyType]._offMobList.Enqueue(mc);
        }

        public void allDestroy()
        {
            foreach (mobData data in _mobDatas)
            {
                for (int i = 0; i < data._mobList.Count; i++)
                {
                    if (data._mobList[i].IsUse)
                    {
                        data._mobList[i].ForceDestroy();
                    }
                }                
            }
        }

        Vector3 mopRespawnsPos()
        {
            float x = Random.Range(-10f, 10f);
            float y = Mathf.Sqrt(100f - (x * x)) * (Random.Range(0, 2) == 0 ? 1 : -1);  

            //float angle = Random.Range(0f, 360f);
            float angle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;

            float pAngle = Mathf.Atan2(_gs.pVector.x, _gs.pVector.y) * Mathf.Rad2Deg;

            if (compAngle(pAngle + 90, angle))
            {
                x *= -1;
                y *= -1;
            }

            return _playerPos.position + new Vector3(x, y);
        }

        bool compAngle(float startA, float compA)
        {
            if (startA > compA)
            {
                return compA + 360f > startA && compA + 360f < startA + 180f;
            }
            else
            {
                return startA <= compA && startA + 180f >= compA;
            }
        }

        #endregion

        /// <summary> 보스 만들기 </summary>
        public bossControl makeBoss(Boss boss, LandObject bo, Vector3 home)
        {
            // 있으면 찾아쓰고
            foreach (bossControl bc in _bossList)
            {
                if (bc.getType == boss && bc.IsUse == false)
                {
                    bc.setHome(bo, home);
                    bc.transform.position = home;
                    bc.RepeatInit();

                    return bc;
                }
            }

            // 없으면 생성
            bossControl bct = Instantiate(DataManager.BobFabs[boss]).GetComponent<bossControl>();
            _bossList.Add(bct);
            bct.transform.parent = transform;

            bct.setting(_gs, _dmgFont.getText, _gs.kill_Boss);

            bct.setHome(bo, home);
            bct.transform.position = home;

            bct.FixInit();

            return bct;
        }

        #region [enemy batch ctrl]

        /// <summary> 버프/디버프 </summary>
        public BuffEffect setDeBuff(enemyStt bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            SkillKeyList sk = EnumHelper.StringToEnum<SkillKeyList>(bff.ToString());
            BuffEffect DBuff = new BuffEffect(sk, term, val, isterm);

            _fieldBuff[bff] *= DBuff.Val;

            _bffList.Add(DBuff);

            return DBuff;
        }

        /// <summary> 버프 수동 삭제 </summary>
        public void manualRemoveDeBuff(BuffEffect bff)
        {
            _bffList.Remove(bff);
            
            reCalBuff(bff.Bff);
        }

        /// <summary> 버프 기간 체크 </summary>
        void deBuffChk(float delTime)
        {
            for (int i = 0; i < _bffList.Count; i++)
            {
                _bffList[i].Term -= delTime;

                if (_bffList[i].TermOver)
                {
                    SkillKeyList bff = _bffList[i].Bff;

                    _bffList.RemoveAt(i);

                    reCalBuff(bff);
                    i--;
                }
            }
        }

        /// <summary> 삭제된 타입 버프 일괄계산 </summary>
        void reCalBuff(SkillKeyList bff)
        {
            if (bff != SkillKeyList.ATT && bff != SkillKeyList.DEF && bff != SkillKeyList.SPEED)
                Debug.LogError("잘못된 요청 : " + bff);

            enemyStt st = EnumHelper.StringToEnum<enemyStt>(bff.ToString());

            _fieldBuff[st] = (bff == SkillKeyList.DEF) ? 0f : 1f;   // 일단 초기화 처리

            for (int i = 0; i < _bffList.Count; i++)
            {
                if (_bffList[i].Bff == bff)
                {
                    if (bff == SkillKeyList.DEF) // 처리
                    {
                        _fieldBuff[st] += _bffList[i].Val;
                    }
                    else
                    {
                        _fieldBuff[st] *= _bffList[i].Val;
                    }
                }
            }
        }

        #region [ 전체몹 컨트롤 ]

        /// <summary> 전체 몹 데미지 </summary>
        public void enemyDamaged(float dmg, SkillKeyList sk)
        {
            _adata.set(dmg, sk, false);
            for (int i = EnemyList.Count - 1; i >= 0; i--)
            {
                EnemyList[i].getDamaged(_adata);
            }
        }

        /// <summary> 전체 몹중 범위내 몹 데미지 </summary>
        public void enemyDamagedRange(float dmg, float range, SkillKeyList sk)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].PlayerDist < range)
                {
                    _adata.set(dmg, sk, false);
                    EnemyList[i].getDamaged(_adata);
                }
            }
        }

        /// <summary> 전체 몹중 범위내 몹에게 발사 </summary>
        public void enemyRangeShot(float dmg, float range, SkillKeyList sk)
        {
            shotCtrl _shotBullet;

            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].PlayerDist < range)
                {
                    _shotBullet = _gs.SkillMng.getLaunch(sk);
                    _shotBullet.transform.position = _gs.Player.transform.position;
                    _shotBullet.setTarget(EnemyList[i].transform.position);
                    _shotBullet.repeatInit(sk, dmg, 1f)
                        .play();
                }
            }
        }

        

        /// <summary> 전체 몹 슬로우 </summary>
        public void enemySlow(float term, float val, BuffEffect.buffTermType bf = BuffEffect.buffTermType.term)
        {
            setDeBuff(enemyStt.SPEED, term, val, bf);
        }

        /// <summary> 전체 몹 빙결 </summary>
        public void enemyFrozen(float term)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                EnemyList[i].setFrozen(term);
            }
        }

        /// <summary> 전체 몹 실명 </summary>
        public void enemyBlind(float term, int rate, BuffEffect.buffTermType bf = BuffEffect.buffTermType.term)
        {
            _gs.Player.setDeBuff(SkillKeyList.DODGE, term, rate, bf);
        }

        #endregion

        #endregion
    }
}