using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using week;

public class playerSkillManager : MonoBehaviour
{
    GameScene _gs;
    effManager _efm;

    List<BaseProjControl> _shotSkillList;
    List<SsuddenAppearCtrl> _suddenList;
    List<hailSkill> _hailList;

    GameObject _icetornado;

    public List<SsuddenAppearCtrl> IcewallList { get => _suddenList; }

    public void Init(GameScene gs)
    {
        _gs = gs;
        _efm = gs.EfMng;

        _shotSkillList = new List<BaseProjControl>();
        _suddenList = new List<SsuddenAppearCtrl>();
        _hailList = new List<hailSkill>();
    }

    /// <summary> 투사체 생성 </summary>
    public BaseProjControl getPrej(SkillKeyList sk)
    {
        if (sk < SkillKeyList.snowball)
        {
            Debug.LogError("잘못된, 능력치 생성 요청");
            return null;
        }

        for (int i = 0; i < _shotSkillList.Count; i++)
        {
            if(_shotSkillList[i].IsUse == false && _shotSkillList[i].getSkillType == sk)
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

        hailSkill hail = Instantiate(DataManager.ShotFabs[SkillKeyList.hail]).GetComponent<hailSkill>();
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
