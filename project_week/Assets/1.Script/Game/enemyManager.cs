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
            public float _eterm;
            float _etime;

            public mobData(Mob mob, float term)
            {
                _mob = mob;
                _eterm = term;
            }

            public bool chkTime(float deltime)
            {
                _etime += deltime;
                if (_etime > _eterm)
                {
                    _etime = 0;
                    return true;
                }
                return false;
            }
        }

        GameScene _gs; 

        MapManager _map;
        dmgFontManager _dmgFont;
        EnemyProjManager _epm;
        effManager _efm;
        clockManager _clock;

        Transform _playerPos;

        mobData[] mobDatas;
        int[] mobCount;
        List<Mob> _nowTurnEnemyList;
        List<MobControl> _mobList;
        List<bossControl> _bossList;

        float[] _initBff;                       // 생성시 적용 버프

        List<BuffEffect> _bffList;              // 버프 리스트
        Dictionary<eBuff, float> _fieldBuff;    // 버프리스트가 적용된 값

        public int _blindRate; // 실명 확률

        int _stageEnemyAmount = 1;

        int _rangeOfRound;

        float _high;
        int _thisRound;

        public List<MobControl> EnemyList { get => _mobList; }
        public List<bossControl> BossList { get => _bossList; }
        public float[] InitBff { get => _initBff; set => _initBff = value; }
        public Dictionary<eBuff, float> FieldBuff { get => _fieldBuff; set => _fieldBuff = value; }
        public bool isEnemyBlind
        {
            get
            {
                for (int i = 0; i < _bffList.Count; i++)
                {
                    if (_bffList[i].Bff == eBuff.blind)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary> 초기화 </summary>
        public void Init(GameScene gs)
        {
            _gs = gs;

            _map = _gs.MapMng;
            _dmgFont = _gs.DmgfntMng;
            _epm = _gs.EnProjMng;
            _efm = _gs.EfMng;
            _clock = _gs.ClockMng;

            _playerPos = _gs.Player.transform;

            _mobList = new List<MobControl>();
            _bossList = new List<bossControl>();
            _nowTurnEnemyList = new List<Mob>();
            mobCount = new int[(int)Mob.max];

            _initBff = new float[3] { 1f, 1f, 0f };
            _bffList = new List<BuffEffect>();
            _fieldBuff = new Dictionary<eBuff, float>();
            _fieldBuff.Add(eBuff.att, 1f);
            _fieldBuff.Add(eBuff.def, 0f);
            _fieldBuff.Add(eBuff.speed, 1f);
            _fieldBuff.Add(eBuff.size, 1f);
            _fieldBuff.Add(eBuff.blind, 1f);

            mobDatas = new mobData[(int)Mob.max];
            for (Mob i = Mob.fire; i < Mob.max; i++)
            {
                mobDatas[(int)i] = new mobData(
                    i,
                    DataManager.GetTable<float>(DataTable.monster, (i).ToString(), MonsterData.appear_term.ToString())
                    );
            }
        }

        #region [mop]

        public IEnumerator startMakeEnemy()
        {
            float deltime = 0;
            Mob mob;
            int ss = 0;

            //makeEnemySet(Mob.mob_stick);
            //yield break;

            yield return new WaitUntil(() => _gs.StagePlay == true);

            while (_gs.GameOver == false)
            {
                yield return new WaitUntil(() => _gs.Pause == false);

                deltime = Time.deltaTime;
                ss = 3 * (int)_clock.Season;

                if (mobDatas[(int)Mob.fire].chkTime(deltime))
                {
                    for (int j = 0; j < _stageEnemyAmount; j++)
                    {
                        makeEnemySet(Mob.fire);
                    }
                }

                if (_clock.chk1Wave)
                {
                    if (mobDatas[(int)Mob.closed].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.closed);
                        }
                    }
                }
                if (_clock.chk2Wave)
                {
                    if (mobDatas[(int)Mob.ranged].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.ranged);
                        }
                    }
                }
                if (_clock.chk3Wave)
                {
                    if (mobDatas[(int)Mob.hard].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.hard);
                        }
                    }
                }

                yield return new WaitForEndOfFrame();

                deBuffChk(deltime);
                // ExpRefresh();
                //StartCoroutine(chk());
            }
        }

        IEnumerator chk()
        {
            while (true)
            {
                Debug.Log("spd : " + _fieldBuff[eBuff.speed]);
                yield return new WaitForSeconds(1f);
            }
        }

        public void makeEnemySet(Mob type)
        {
            makeEnemy(type, mopRespawnsPos());
        }

        void makeEnemy(Mob type, Vector3 pos)
        {
            // 있으면 찾아쓰고
            foreach (MobControl ec in _mobList)
            {
                if (ec.getType == type && ec.IsUse == false)
                {
                    ec.transform.position = pos;
                    ec.RepeatInit(_gs.ClockMng.Season);

                    return;
                }
            }

            if (mobCount[(int)type] > 20)
            {
                return;
            }

            // 없으면 생성
            MobControl ect = Instantiate(DataManager.MobFabs[type]).GetComponent<MobControl>();
            _mobList.Add(ect);
            ect.transform.parent = transform;
            ect.transform.position = pos;

            ect.setting(_gs);
            ect.FixInit(_gs.ClockMng.Season);// _playerPos.position - ect.transform.position);
            mobCount[(int)type]++;
        }

        public void allDestroy()
        {
            foreach (MobControl ec in _mobList)
            {
                if (ec.IsUse)
                {
                    ec.Destroy();
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

            bct.setting(_gs, _dmgFont.getText, _gs.getBossKill);

            bct.setHome(bo, home);
            bct.transform.position = home;

            bct.FixInit();

            return bct;
        }

        #region [enemy batch ctrl]

        /// <summary> 버프/디버프 </summary>
        public BuffEffect setDeBuff(eBuff bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            BuffEffect DBuff = new BuffEffect(bff, term, val, isterm);

            _fieldBuff[bff] *= DBuff.Val;

            _bffList.Add(DBuff);

            return DBuff;
        }

        /// <summary> 버프 수동 삭제 </summary>
        public void manualRemoveDeBuff(BuffEffect bff)
        {
            eBuff ebff = bff.Bff;

            _bffList.Remove(bff);

            reCalBuff(ebff);
        }

        void deBuffChk(float delTime)
        {
            for (int i = 0; i < _bffList.Count; i++)
            {
                _bffList[i].Term -= delTime;

                if (_bffList[i].TermOver)
                {
                    eBuff bff = _bffList[i].Bff;

                    _bffList.RemoveAt(i);

                    reCalBuff(bff);
                    i--;
                }
            }
        }

        /// <summary> 삭제된 타입 버프 일괄계산 </summary>
        void reCalBuff(eBuff bff)
        {
            if (bff != eBuff.att && bff != eBuff.def && bff != eBuff.speed && bff != eBuff.blind)
                Debug.LogError("잘못된 요청 : " + bff);

            _fieldBuff[bff] = (bff == eBuff.def) ? 0f : 1f;

            for (int i = 0; i < _bffList.Count; i++)
            {
                if (_bffList[i].Bff == bff)
                {
                    if (bff == eBuff.def)
                    {
                        _fieldBuff[bff] += _bffList[i].Val;
                    }
                    else
                    {
                        _fieldBuff[bff] *= _bffList[i].Val;
                    }
                }
            }
        }

        #region [ 전체몹 컨트롤 ]

        /// <summary> 전체 몹 데미지 </summary>
        public void enemyDamaged(float dmg)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].IsUse)
                {
                    EnemyList[i].getDamaged(dmg);
                }
            }
        }

        /// <summary> 전체 몹중 범위내 몹 데미지 </summary>
        public void enemyDamagedRange(float dmg, float range)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].IsUse && EnemyList[i].PlayerDist < range)
                {
                    EnemyList[i].getDamaged(dmg);
                }
            }
        }

        /// <summary> 전체 몹 슬로우 </summary>
        public void enemySlow(float term, float val, BuffEffect.buffTermType bf = BuffEffect.buffTermType.term)
        {
            setDeBuff(eBuff.speed, term, val, bf);
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
            setDeBuff(eBuff.blind, term, 1f, bf);
            _blindRate = rate;
        }

        #endregion

        #endregion
    }
}