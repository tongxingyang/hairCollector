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

        List<LaunchSkillCtrl> _launchSkills;
        List<RangeSkillCtrl> _rangeSkills;
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

            _launchSkills = new List<LaunchSkillCtrl>();
            _rangeSkills = new List<RangeSkillCtrl>();
            _skillMarks = new List<skillMarkCtrl>();

            _shotSkillList = new List<BaseProjControl>();
            _suddenList = new List<SsuddenAppearCtrl>();
            _hailList = new List<hailSkill>();
        }

        /// <summary> [launch] 투사체 가져오기  </summary>
        public LaunchSkillCtrl getLaunch(SkillKeyList sk)
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

            LaunchSkillCtrl lsc = Instantiate(DataManager.ShotFabs[sk]).GetComponent<LaunchSkillCtrl>();
            _launchSkills.Add(lsc);
            lsc.fixedInit(_gs, _efm);
            lsc.transform.parent = transform;
            return lsc;
        }

        /// <summary> [range] 발사체 가져오기  </summary>
        public RangeSkillCtrl getRange(SkillKeyList sk)
        {
            // 혹시나 에러 체크
            if (sk < SkillKeyList.Iceball || sk > SkillKeyList.IceBat)
            {
                Debug.LogError("range스킬생성 요청에러: " + sk.ToString());
                return null;
            }

            for (int i = 0; i < _rangeSkills.Count; i++)
            {
                if (_rangeSkills[i].IsUse == false)
                {
                    return _rangeSkills[i];
                }
            }

            RangeSkillCtrl rsc = Instantiate(DataManager.ShotFabs[sk]).GetComponent<RangeSkillCtrl>();
            _rangeSkills.Add(rsc);
            rsc.fixedInit(_gs, this);
            rsc.transform.parent = transform;
            return rsc;
        }

        /// <summary> [range]-[mark] 스킬 마크/스킬 오브젝트 가져오기  </summary>
        public skillMarkCtrl getRangeMark(SkillKeyList sk)
        {
            // 혹시나 에러 체크
            if (sk < SkillKeyList.Iceball || sk >= SkillKeyList.IceBat)
            {
                Debug.LogError("range스킬생성 요청에러: " + sk.ToString());
                return null;
            }

            // 풀에 잔여 오브젝트 있음
            for (int i = 0; i < _skillMarks.Count; i++)
            {
                if (_skillMarks[i].IsUse == false)
                {
                    return _skillMarks[i];
                }
            }

            // 새로 생성 필요
            skillMarkCtrl mark = Instantiate(DataManager.ShotFabs[sk]).GetComponent<skillMarkCtrl>();
            _skillMarks.Add(mark);
            mark.fixedInit(_gs, this);
            mark.transform.parent = transform;
            return mark;
        }

        public RushCloseCtrl getBat()
        {
            if (_bat == null)
            {
                _bat = Instantiate(DataManager.ShotFabs[SkillKeyList.IceBat]).GetComponent<RushCloseCtrl>();
                _bat.fixedInit(_gs);
            }

            return _bat;
        }

        public RushCloseCtrl getFlurry()
        {
            if (_flurry == null)
            {
                _flurry = Instantiate(DataManager.ShotFabs[SkillKeyList.Flurry]).GetComponent<RushCloseCtrl>();
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

            BaseProjControl pjt = Instantiate(DataManager.ShotFabs[sk]).GetComponent<BaseProjControl>();
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

            SsuddenAppearCtrl sac = Instantiate(DataManager.ShotFabs[sk]).GetComponent<SsuddenAppearCtrl>();
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

            hailSkill hail = Instantiate(DataManager.ShotFabs[SkillKeyList.Hail]).GetComponent<hailSkill>();
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