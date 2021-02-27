using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace week
{
    /// <summary> 플레이어 스킬은 내가 다 관리한다!!! </summary>
    public class playerSkillManager : MonoBehaviour
    {
        GameScene _gs;
        effManager _efm;

        List<shotCtrl> _launchSkills;
        List<curvedShotCtrl> _rangeSkills;
        List<skillMarkCtrl> _skillMarks;
        RushCtrl _rushMng;
        ShieldCtrl _shieldMng;
        FieldCtrl _fieldMng;
        SpetCtrl _petMng;

        GameObject _icetornado;

        public RushCtrl RushMng
        {
            get 
            {
                if (_rushMng == null)
                {
                    _rushMng = Instantiate(DataManager.RushFabs, _gs.Player.transform).GetComponent<RushCtrl>();
                    _rushMng.Init(_gs);
                }

                return _rushMng; 
            }
        }
        public ShieldCtrl ShieldMng 
        {
            get 
            {
                if (_shieldMng == null)
                {
                    _shieldMng = Instantiate(DataManager.ShieldFabs, _gs.Player.transform).GetComponent<ShieldCtrl>();
                    _shieldMng.ShieldInit(_gs);
                }
                return _shieldMng;
            } 
        }

        public FieldCtrl FieldMng
        {
            get
            {
                if (_fieldMng == null)
                {
                    _fieldMng = Instantiate(DataManager.FieldFabs, _gs.Player.transform).GetComponent<FieldCtrl>();
                    _fieldMng.Init(_gs);
                }
                return _fieldMng;
            }
        }

        public SpetCtrl PetMng
        {
            get
            {
                if (_petMng == null)
                {
                    _petMng = Instantiate(DataManager.PetFabs, transform).GetComponent<SpetCtrl>();
                    _petMng.Init(_gs);
                }
                return _petMng;
            }
        }

        public void Init(GameScene gs)
        {
            _gs = gs;
            _efm = gs.EfMng;

            _launchSkills = new List<shotCtrl>();
            _rangeSkills = new List<curvedShotCtrl>();
            _skillMarks = new List<skillMarkCtrl>();
        }

        /// <summary> [launch] 투사체 가져오기  </summary>
        public shotCtrl getLaunch(SkillKeyList sk)
        {
            for (int i = 0; i < _launchSkills.Count; i++)
            {
                if (_launchSkills[i].IsUse == false)
                {
                    return _launchSkills[i];
                }
            }

            shotCtrl lsc = Instantiate(DataManager.ShotFabs).GetComponent<shotCtrl>();
            _launchSkills.Add(lsc);
            lsc.fixedInit(_gs, _efm);
            lsc.transform.parent = transform;
            return lsc;
        }

        /// <summary> [range] 곡사체 가져오기  </summary>
        public curvedShotCtrl getCurved()
        {      
            for (int i = 0; i < _rangeSkills.Count; i++)
            {
                if (_rangeSkills[i].IsUse == false)
                {
                    return _rangeSkills[i];
                }
            }

            curvedShotCtrl rsc = Instantiate(DataManager.CurvedFabs).GetComponent<curvedShotCtrl>();
            _rangeSkills.Add(rsc);
            rsc.fixedInit(_gs, this);
            rsc.transform.parent = transform;
            return rsc;
        }

        /// <summary> 스탬프 오브젝트 가져오기  </summary>
        public skillMarkCtrl getStamp()
        {      
            // 풀에 잔여 오브젝트 있음
            for (int i = 0; i < _skillMarks.Count; i++)
            {
                if (_skillMarks[i].IsUse == false)
                {
                    return _skillMarks[i];
                }
            }

            // 새로 생성 필요
            skillMarkCtrl mark = Instantiate(DataManager.StampFabs).GetComponent<skillMarkCtrl>();
            _skillMarks.Add(mark);
            mark.fixedInit(_gs, this);
            mark.transform.parent = transform;
            return mark;
        }

        public void onPause(bool bl)
        {
            //=============================
            foreach (shotCtrl sc in _launchSkills)
            {
                sc.onPause(bl);
            }
            foreach (curvedShotCtrl csc in _rangeSkills)
            {
                csc.onPause(bl);
            }
            foreach (skillMarkCtrl smc in _skillMarks)
            {
                smc.onPause(bl);
            }

            _petMng?.onPause(bl);

            _rushMng?.onPause(bl);
        }

        public void onClear()
        {
            //foreach (BaseProjControl bpc in _shotSkillList)
            //{
            //    bpc.Destroy();
            //}
            //foreach (SsuddenAppearCtrl sac in _suddenList)
            //{
            //    sac.Destroy();
            //}
            //foreach (hailSkill hs in _hailList)
            //{
            //    hs.Destroy();
            //}
        }
    }
}