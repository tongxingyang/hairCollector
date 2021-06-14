using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class getSkillCase : MonoBehaviour
    {
        [SerializeField] Image _back;
        [SerializeField] Image _img;
        [SerializeField] Image _case;
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _lvl;

        GameResource _gr;
        Sprite getRankBacks(int rank) => _gr._rankBack[rank % 10];
        Sprite getRankCases(int rank) => _gr._rankCase[rank % 10];

        public void Init(GameScene gs, SkillKeyList skill, int lvl)
        {
            _gr = gs._gameResource;

            int rank = D_skill.GetEntity(skill.ToString()).f_rank;
            if (rank < 0)
                rank = 0;

            _img.sprite = DataManager.Skillicon[skill];

            _back.sprite = getRankBacks(rank);
            _case.sprite = getRankCases(rank);

            _lvl.text = $"lvl.{lvl}";
            _name.text = D_skill.GetEntity(skill.ToString()).f_skill_name;
        }
    }
}