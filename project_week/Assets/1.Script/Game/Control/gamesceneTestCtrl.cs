using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class gamesceneTestCtrl : MonoBehaviour
    {
        [SerializeField] GameScene _gs;
        [SerializeField] Dropdown _drop;

        private void Start()
        {
            List<string> sts = new List<string>();
            for (SkillKeyList i = 0; i < SkillKeyList.non; i++) 
            {
                //sts.Add(DataManager.GetTable<string>(DataTable.skill, i.ToString(), SkillValData.skill_name.ToString()));
                sts.Add(i.ToString());
            }
            _drop.AddOptions(sts);
        }

        public void skill_reset()
        {
            for (SkillKeyList i = SkillKeyList.SnowBall; i < SkillKeyList.non; i++)
            {
                _gs.Player.Skills[i].clear();
            }
        }

        public void getSkill()
        {
            Debug.Log(_drop.captionText.text);
            SkillKeyList sk = EnumHelper.StringToEnum<SkillKeyList>(_drop.captionText.text);
            _gs.Player.getSkill(sk);
        }

        public void onoff()
        {
            _gs.EnemyMng.mobOff = !_gs.EnemyMng.mobOff;
        }
    }
}