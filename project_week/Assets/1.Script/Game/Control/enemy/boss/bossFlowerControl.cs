using DG.Tweening;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace week
{
    public class bossFlowerControl : bossControl
    {
        #region [skill value]

        [SerializeField] SpriteRenderer _head;
        [SerializeField] SpriteRenderer[] _allbody;
        [SerializeField] Sprite[] _headType;
        [Space]
        [SerializeField] Transform[] _stem;
        [SerializeField] Transform _foot;
        [Space]
        [SerializeField] Animator _mark;

        int _stemLeng;

        Vector3 _dirVector;

        bool _isNormalPlay;
        float _normalCoolTime;
        float _normalCool = 5f;
        
        float _skillCoolTime;
        float _runningTime;
        float _xPos;

        Vector3[] _stemDir;
        float[] _stemHight;
        float _time;

        EnSkillControl _esc;

        #endregion

        #region [Init - Destroy]

        protected override void otherWhenFixInit()
        {
            _stemLeng = _stem.Length;
            _stemDir = new Vector3[_stemLeng];
            _stemHight = new float[_stemLeng];
            _standardStt[(int)snowStt.speed] = 1f;

            for (int i = 0; i < _stemLeng; i++)
            {
                _stemDir[i] = _stem[i].position;
                _stemHight[i] = _stemDir[i].y - _foot.position.y;
            }
        }

        protected override void otherWhenRepeatInit()
        {
            setAllColor(Color.white);
            _isNormalPlay = false;
            _normalCoolTime = UnityEngine.Random.Range(3f, 5f);
            StartCoroutine(setStemPosAni());
        }

        protected override void otherWhenDie()
        {
        }

        #endregion

        protected override IEnumerator lifeCycle()
        {
            //StartCoroutine(chkPlayer());

            while (_isDie == false)
            {
                _time = Time.deltaTime;

                switch (_stats)
                {
                    case stat.Idle:
                        idle(_time);
                        break;
                    case stat.skill0:
                        StartCoroutine(skillA());
                        switchStat(stat.Idle);
                        break;
                    case stat.skill1:
                        skillB();
                        switchStat(stat.Idle);
                        break;
                    default:
                        break;
                }
                
                checkDir();

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return null;
        }

        #region [haviour]

        IEnumerator setStemPosAni()
        {
            while (IsUse)
            {
                for (int i = 0; i < _stemLeng; i++)
                {
                    _stem[i].position = Vector3.MoveTowards(_stem[i].position, _stemDir[i], 3f * Speed * Time.deltaTime);
                }
                
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        /// <summary> 대기 </summary>
        void idle(float delTime)
        {
            if (_isNormalPlay == false)
            {                
                _runningTime += delTime;

                if (_standardStt[(int)snowStt.speed] > 1f)
                {
                    _standardStt[(int)snowStt.speed] -= delTime;
                }

                for (int i = 0; i < _stemLeng; i++)
                {
                    _xPos = Mathf.Sin(_runningTime + 0.35f * i) * 0.5f;

                    _stemDir[i] = new Vector2(_foot.position.x + _xPos * i / _stemLeng, _stemHight[i] + _foot.position.y);
                }
            }

            if (Vector3.Distance(transform.position, _player.transform.position) < _bossRange)
            {
                // 타임체크 - 스킬선택
                _skillCoolTime += delTime;
                if (_skillCoolTime > _bossSkillCool)
                {
                    _skillCoolTime = 0;

                    switchStat(stat.skill0 + Random.Range(0, 2));
                }

                _normalCoolTime += delTime;
                if (_isNormalPlay == false && _normalCoolTime > _normalCool)
                {                    
                    _normalCoolTime = 0;
                    _normalCoolTime = UnityEngine.Random.Range(3f, 5f);

                    StartCoroutine(normalAttack());
                }
            }
        }

        /// <summary> 기본공격 - 박치기 </summary>
        IEnumerator normalAttack()
        {
            _isNormalPlay = true;

            Vector3 targetPos = _player.transform.position;
            float delTime = 0;
            float waitTime = 0;

            _mark.transform.position = _player.transform.position;
            _mark.SetTrigger("redzone");

            while (waitTime < 1f)
            {
                delTime = Time.deltaTime;

                waitTime += delTime;
                _runningTime += delTime * 10f;

                for (int i = 0; i < _stemLeng; i++)
                {
                    _xPos = Mathf.Sin(_runningTime + 0.35f * i) * 0.5f;

                    _stemDir[i] = new Vector2(_foot.position.x + _xPos * i / _stemLeng, _stemHight[i] + _foot.position.y);
                }

                targetPos = _player.transform.position;
                _mark.transform.position = Vector3.MoveTowards(_mark.transform.position, targetPos, 1.5f * Time.deltaTime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            targetPos = _mark.transform.position;

            Vector3 dist;
            float R;
            float r;
            float x;

            float rPow;
            float posy;

            _standardStt[(int)snowStt.speed] = 15f;
            while (Vector3.Distance(_head.transform.position, targetPos) > 0.05f)
            {
                _stemDir[_stemLeng - 1] = Vector3.MoveTowards(_head.transform.position, targetPos, 15f * Time.deltaTime);

                dist = _stemDir[_stemLeng - 1] - _foot.position;
                R = _stemDir[_stemLeng - 1].x - _foot.position.x;
                r = R * 0.5f;
                rPow = r * r;

                for (int i = 0; i < _stemLeng - 1; i++)
                {
                    x = (R * i / _stemLeng);
                    posy = Mathf.Sqrt(x * (2 * r - x)) * 0.5f;
                    _stemDir[i] = new Vector3(_foot.position.x + (dist.x * i / _stemLeng), _foot.position.y + (dist.y * i / _stemLeng) + posy);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            _mark.SetTrigger("crack");
            _player.cameraShake();

            _normalCoolTime = 0;
            _isNormalPlay = false;
        }

        /// <summary> 1번스킬 - 가시발사 </summary>
        IEnumerator skillA()
        {
            float degree = 360f / 10;
            float addAngle;
            float time = 0;

            for (int i = 0; i < 4; i++)
            {
                addAngle = (i % 2) * (degree / 2); 
                time = 0;

                for (int j = 0; j < 10; j++)
                {
                    _esc = _enProjMng.makeEnProj(EnShot.flower_thorn);
                    _esc.transform.position = transform.position;
                    _esc.operation(degree * j + addAngle);
                }

                while (time < 0.5f)
                {
                    time += Time.deltaTime;
                    yield return new WaitUntil(() => _gs.Pause == false);
                }
            }
        }

        /// <summary> 2번스킬 - 지뢰 뿌리기 </summary>
        void skillB()
        {
            for (int i = 0; i < 10; i++)
            {
                _esc = _enProjMng.makeEnProj(EnShot.flower_mine);
                _esc.transform.position = transform.position;
                _esc.operation(transform.position + (Vector3)(Random.insideUnitCircle * 5));
            }
        }

        #endregion

        #region [util]
        public override void whenEnemyEnter()
        {
        }

        /// <summary> 목적을 향한 방향 체크 </summary>
        void checkDir()
        {
            _dirVector = (_player.transform.position - transform.position).normalized;
            if (_dirVector.y > 0 && _dirVector.y > _dirVector.x)
            {
                _head.sprite = _headType[2];
            }
            else if (_isNormalPlay)
            {
                _head.sprite = _headType[1];                
            }
            else
            {
                _head.sprite = _headType[0];
            }

            if (_dirVector.x > 0)
            {
                _head.flipX = true;
            }
            else
            {
                _head.flipX = false;
            }
        }

        /// <summary> 상태변화 </summary>
        protected override void switchStat(stat st)
        {
            _stats = st;
            switch (st)
            {
                case stat.Idle:
                    //_mark.SetTrigger("none");
                    break;
                case stat.skill0:
                    //_skill_finished = false;
                    break;
                case stat.skill1:
                    //_skill_finished = false;
                    break;
                default:
                    break;
            }
        }

        void setAllColor(Color col)
        {
            _head.color = col;
            for (int i = 0; i < _allbody.Length; i++)
            {
                _allbody[i].color = col;
            }
        }

        protected override IEnumerator damageAni()
        {
            _isDmgAction = true;
            setAllColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            setAllColor(_originColor);

            yield return new WaitForSeconds(0.1f);

            setAllColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            setAllColor(_originColor);

            _isDmgAction = false;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(Att);
            }
        }

        public override void onPause(bool bl)
        {
            //spinePause(bl);
        }

        #endregion
    }
}
