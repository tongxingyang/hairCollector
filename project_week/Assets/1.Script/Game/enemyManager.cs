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

        //float[] _etime;
        int _stageEnemyAmount = 1;

        int _rangeOfRound;

        float _high;
        int _thisRound;

        public List<MobControl> EnemyList { get => _mobList; }
        public List<bossControl> BossList { get => _bossList; }

        // Start is called before the first frame update
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

            mobDatas = new mobData[(int)Mob.max];
            for (Mob i = Mob.mob_fire; i < Mob.max; i++)
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

                if (mobDatas[(int)Mob.mob_fire].chkTime(deltime))
                {
                    for (int j = 0; j < _stageEnemyAmount; j++)
                    {
                        makeEnemySet(Mob.mob_stick);
                    }
                }

                if (_clock.chk1Wave)
                {
                    if (mobDatas[(int)Mob.mob_ant+ ss].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.mob_ant+ ss);
                        }
                    }
                }
                if (_clock.chk2Wave)
                {
                    if (mobDatas[(int)Mob.mob_beetle+ ss].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.mob_beetle + ss);
                        }
                    }
                }
                if (_clock.chk3Wave)
                {
                    if (mobDatas[(int)Mob.mob_snail + ss].chkTime(deltime))
                    {
                        for (int j = 0; j < _stageEnemyAmount; j++)
                        {
                            makeEnemySet(Mob.mob_snail + ss);
                        }
                    }
                }

                yield return new WaitForEndOfFrame();

                // ExpRefresh();
            }
        }

        public void makeEnemySet(Mob type)
        {
            //if (type == Mob.mob_ant)
            //{
            //    Vector3 pos = mopRespawnsPos();
            //    for (int i = 0; i < 4; i++)
            //    {
            //        makeEnemy(type, pos + new Vector3(i / 2, i % 2));
            //    }
            //}
            //else
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
                    ec.RepeatInit();

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
            ect.FixInit();// _playerPos.position - ect.transform.position);
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

            bct.setting(_gs, _efm, _epm, _dmgFont.getText, _gs.getKill);

            bct.setHome(bo, home);
            bct.transform.position = home;

            bct.FixInit();

            return bct;
        }
    }
}