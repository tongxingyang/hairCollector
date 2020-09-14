using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnemyProjManager : MonoBehaviour
    {
        GameScene _gs;
        effManager _efm;

        PlayerCtrl _player;

        public int _thisRound { private get; set; }

        List<EnSkillControl> _enProjList;

        public void Init(GameScene gs)
        {
            _gs = gs;
            _efm = _gs.EfMng;
            _player = _gs.Player;

            _enProjList = new List<EnSkillControl>();            
        }

        public EnSkillControl makeEnProj(EnShot type)
        {
            // 있으면 찾아쓰고
            foreach (EnSkillControl ec in _enProjList)
            {
                if (ec.IsUse == false && ec.getType == type)
                {
                    ec.recycleInit();
                    return ec;
                }
            }

            // 없으면 생성
            EnSkillControl esc = Instantiate(DataManager.EnProjFabs[type]).GetComponent<EnSkillControl>();
            _enProjList.Add(esc);
            esc.transform.parent = transform;

            esc.Init(_gs, _efm);
            return esc;
        }
    }
}