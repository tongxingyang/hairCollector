using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class RushCtrl : MonoBehaviour
    {
        enum rush { bat, flurry, storm }
        [SerializeField] Transform _followAngle;
        [SerializeField] Transform[] _shotPos;
        [SerializeField] rushCollider[] _cols;
        Animator[] _Ani;

        GameScene _gs;
        float _batDmg;
        float _flurryDmg;
        float _stormDmg;
        shotCtrl _bullet;

        public Transform[] openPos { get => _shotPos; }
        public Transform baltPos { get => _shotPos[2]; }
        public bool OnStorm { get; set; }

        public void Init(GameScene gs)
        {
            _gs = gs;

            _Ani = new Animator[3];
            for (int i = 0; i < 3; i++)
            {
                _cols[i].Init(this, _gs);
                _Ani[i] = _cols[i].GetComponent<Animator>();
            }
        }

        public void setDirection(Vector3 target)
        {
            float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;
            _followAngle.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }

        #region [Bat]

        public void setBat(float dmg)
        {
            _batDmg = dmg;

            _Ani[(int)rush.bat].gameObject.SetActive(true);
            // _Ani[(int)rush.bat].SetTrigger("swing");
        }

        #endregion

        #region [Flurry]

        public void setFlurry(SkillKeyList sk, float dmg)
        {
            _flurryDmg = dmg;

            _Ani[(int)rush.flurry].gameObject.SetActive(true);
            _cols[1].Skill = sk;

            if (sk == SkillKeyList.EyeOfFlurry)
            {
                _cols[1].transform.localScale = Vector3.one * 2f;
            }
            else
            {
                _cols[1].transform.localScale = Vector3.one;
            }
        }

        #endregion

        #region [Storm]

        public void setStorm(SkillKeyList skill, float dmg)
        {
            _stormDmg = dmg;

            _Ani[(int)rush.storm].gameObject.SetActive(true);

            _cols[2].Skill = skill;
            if (skill == SkillKeyList.ColdStorm)
            {
            }
            else if (skill == SkillKeyList.RotateStorm)
            {
                OnStorm = false;
                _cols[2].StormCnt = 0;
                _cols[2].transform.localScale = Vector3.one;
            }
            else if(skill == SkillKeyList.LockOn)
            {
                _cols[2].transform.localScale = Vector3.one * 1.6f;
            }
        }

        #endregion

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        public void onTriggerEnemy(GameObject go, SkillKeyList skill)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            // 아직 크리티컬 없음

            Vector3 nor;
            // 타입별
            switch (skill)
            {
                case SkillKeyList.IceBat:
                    id.getDamaged(_batDmg, false);
                    // Vector3 nor = (go.transform.position - _gs.Player.transform.position).normalized * 0.05f;
                    nor = (go.transform.position - _gs.Player.transform.position).normalized * 1.5f;
                    id.getKnock(nor, 1f, 0.3f);
                    break;
                case SkillKeyList.Flurry:
                    id.getDamaged(_flurryDmg, false);
                    break;
                case SkillKeyList.EyeOfFlurry:
                    id.getDamaged(_flurryDmg, false);
                    nor = (_shotPos[3].transform.position - go.transform.position);
                    id.getKnock(nor, 0.5f, 0.1f);
                    break;
                case SkillKeyList.ColdStorm:
                    id.getDamaged(_stormDmg, false);
                    break;
                case SkillKeyList.RotateStorm:
                    id.getDamaged(_stormDmg, false);                    
                    OnStorm = true;
                    break;
                case SkillKeyList.LockOn:
                    id.getDamaged(_stormDmg, false);
                    _gs.Player.getBalt(SkillKeyList.LockOn, go.transform.position);
                    break;
            }
        }

        public void onPause(bool bl)
        {
            for (int i = 0; i < 3; i++)
            {
                _Ani[i].speed = (bl) ? 0 : 1;
            }
        }
    }
}