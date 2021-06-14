using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class RushCtrl : MonoBehaviour
    {
        enum rush { bat, flurry, storm }
        [SerializeField] Transform _followAngle;
        [SerializeField] Transform _followFixedAngle;
        [SerializeField] Transform[] _shotPos;
        [SerializeField] rushCollider[] _cols;
        Animator[] _Ani;

        GameScene _gs;
        float _batDmg;
        float _flurryDmg;
        float _stormDmg;
        shotCtrl _bullet;
        attackData _adata = new attackData();

        public Transform[] openPos { get => _shotPos; }
        public Transform baltPos { get => _shotPos[0]; }

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

        public void setFixDir()
        {
            _followFixedAngle.rotation = _followAngle.rotation;
        }

        #region [Bat]

        public void setBat(float dmg)
        {
            _batDmg = dmg;

            _Ani[(int)rush.bat].gameObject.SetActive(true);
            SoundManager.instance.PlaySFX(SFX.icebat);
        }

        #endregion

        #region [Flurry]

        public void setFlurry(SkillKeyList sk, float dmg)
        {
            _flurryDmg = dmg;

            SoundManager.instance.PlaySFX(SFX.flurry);
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
            _cols[2].sizeReset(skill);
        }

        #endregion

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        public void onTriggerEnemy(GameObject go, SkillKeyList skill, bool isboss = false)
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
                    _adata.set(_batDmg, skill, false);
                    nor = (go.transform.position - _gs.Player.transform.position).normalized * 1.5f;

                    if (isboss)
                    {
                        bossControl bc = go.GetComponentInParent<bossControl>();

                        if (bc != null)
                            isboss = bc.getType == Boss.boss_flower;
                    }

                    if (isboss == false)
                    {
                        id.getKnock(nor, 1f, 0.3f);
                    }
                    break;
                case SkillKeyList.Flurry:
                    _adata.set(_flurryDmg, skill, false);
                    break;
                case SkillKeyList.EyeOfFlurry:
                    _adata.set(_flurryDmg, skill, false);
                    nor = (_shotPos[3].transform.position - go.transform.position);
                    if (isboss == false)
                    {
                        id.getKnock(nor, 0.5f, 0.1f);
                    }
                    break;
                case SkillKeyList.ColdStorm:
                    _adata.set(_stormDmg, skill, false);
                    break;
                case SkillKeyList.RotateStorm:
                    _adata.set(_stormDmg, skill, false);
                    _cols[2].OnStorm = true;
                    break;
                case SkillKeyList.LockOn:
                    _adata.set(_stormDmg, skill, false);
                    _gs.Player.getBalt(SkillKeyList.LockOn, go.transform.position);
                    break;
            }

            id.getDamaged(_adata);
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