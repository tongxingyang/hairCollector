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
        RushCloseCtrl _bat;
        RushCloseCtrl _flurry;

        List<BaseProjControl> _shotSkillList;
        List<SsuddenAppearCtrl> _suddenList;
        List<hailSkill> _hailList;

        GameObject _icetornado;

        public List<SsuddenAppearCtrl> IcewallList { get => _suddenList; }
        

        public void Init(GameScene gs)
        {
            _gs = gs;
            _efm = gs.EfMng;

            _launchSkills = new List<shotCtrl>();
            _rangeSkills = new List<curvedShotCtrl>();
            _skillMarks = new List<skillMarkCtrl>();

            _shotSkillList = new List<BaseProjControl>();
            _suddenList = new List<SsuddenAppearCtrl>();
            _hailList = new List<hailSkill>();
        }

        /// <summary> [launch] 투사체 가져오기  </summary>
        public shotCtrl getLaunch(SkillKeyList sk)
        {
            // 혹시나 에러 체크
            if (sk < SkillKeyList.Snowball || sk > SkillKeyList.Iceball)
            {
                Debug.LogError("launch스킬생성 요청에러: " + sk.ToString());
                return null;
            }

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

        public RushCloseCtrl getBat()
        {
            if (_bat == null)
            {
                _bat = Instantiate(DataManager.ShotFabs).GetComponent<RushCloseCtrl>();
                _bat.fixedInit(_gs);
            }

            return _bat;
        }

        public RushCloseCtrl getFlurry()
        {
            if (_flurry == null)
            {
                _flurry = Instantiate(DataManager.ShotFabs).GetComponent<RushCloseCtrl>();
                _flurry.fixedInit(_gs);
            }

            return _flurry;
        }

        /// <summary> 투사체 생성 </summary>
        public BaseProjControl getPrej(SkillKeyList sk)
        {
            if (sk < SkillKeyList.Snowball)
            {
                Debug.LogError("잘못된, 능력치 생성 요청");
                return null;
            }

            for (int i = 0; i < _shotSkillList.Count; i++)
            {
                if (_shotSkillList[i].IsUse == false && _shotSkillList[i].getSkillType == sk)
                {
                    //_shotSkillList[i].select();
                    return _shotSkillList[i];
                }
            }

            BaseProjControl pjt = Instantiate(DataManager.ShotFabs).GetComponent<BaseProjControl>();
            _shotSkillList.Add(pjt);
            pjt.fixedInit(_gs, _efm);

            pjt.transform.parent = transform;
            return pjt;
        }

        /// <summary> 갑분등 스킬 생성 </summary>
        public SsuddenAppearCtrl getSudden(SkillKeyList sk)
        {
            for (int i = 0; i < _suddenList.Count; i++)
            {
                if (_suddenList[i].getSkillType == sk && _suddenList[i].IsUse == false)
                {
                    _suddenList[i].select();
                    return _suddenList[i];
                }
            }

            SsuddenAppearCtrl sac = Instantiate(DataManager.ShotFabs).GetComponent<SsuddenAppearCtrl>();
            _suddenList.Add(sac);
            sac.select();
            sac.setting(_gs);
            sac.transform.parent = transform;
            return sac;
        }

        /// <summary> 우박 생성 </summary>
        public hailSkill getHail()
        {
            for (int i = 0; i < _hailList.Count; i++)
            {
                if (_hailList[i].IsUse == false)
                {
                    _hailList[i].select();
                    return _hailList[i];
                }
            }

            hailSkill hail = Instantiate(DataManager.ShotFabs).GetComponent<hailSkill>();
            _hailList.Add(hail);
            hail.setting(_gs, _efm);
            hail.select();
            hail.transform.parent = transform;
            return hail;
        }

        public void onPause(bool bl)
        {
            foreach (BaseProjControl bp in _shotSkillList)
            {
                bp.onPause(bl);
            }

            foreach (SsuddenAppearCtrl sc in _suddenList)
            {
                sc.onPause(bl);
            }

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
        }

        public void onClear()
        {
            foreach (BaseProjControl bpc in _shotSkillList)
            {
                bpc.Destroy();
            }
            foreach (SsuddenAppearCtrl sac in _suddenList)
            {
                sac.Destroy();
            }
            foreach (hailSkill hs in _hailList)
            {
                hs.Destroy();
            }
        }
    }
}