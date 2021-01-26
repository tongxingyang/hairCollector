using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillRangeTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_rangeBase,
            b_rangeUp,
            b_hail,
            b_meteor,
            b_snowBomb,
            b_snowMissile,
            b_circle,
            b_poison,
            b_thunder,
            b_poisonBomb,
            b_thuncall
        }

        Enum GetEnumBox()
        {
            return new e_box();
        }

        protected override void OtherSetContent()
        {
            if (GetEnumBox() != null)
            {
                mBoxes = SetComponent<skillBox>(GetEnumBox());
            }
        }

        #endregion

        [Header("Item")]
        [SerializeField] GameObject _poisonTems;
        [SerializeField] GameObject _thunderTems;

        public bool _getPoisonTem()
        {
            // 플레이어? 템리스트에서 독템 있는지 체크 반환

            return false;
        }

        public bool _getThunderTem()
        {
            // 플레이어? 템리스트에서 번개템 있는지 체크 반환

            return false;
        }

        #region [ override ] --------------------------------------------------------------------
        public override void mBoxSetting()
        {
            List<SkillKeyList>[] skl
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.Hail, SkillKeyList.SnowBomb, SkillKeyList.Circle } };

            mBoxes[(int)e_box.b_rangeBase]  .Init(_tree, _player.Skills[SkillKeyList.Iceball])  .setDefault(skl);
            mBoxes[(int)e_box.b_rangeUp]    .Init(_tree, _player.Skills[SkillKeyList.Iceball])  .setRein();

            mBoxes[(int)e_box.b_hail]       .Init(_tree, _player.Skills[SkillKeyList.Hail])     .setFrom(new SkillKeyList[] { SkillKeyList.Iceball });
            mBoxes[(int)e_box.b_meteor]     .Init(_tree, _player.Skills[SkillKeyList.Meteor])   .setFrom(new SkillKeyList[] { SkillKeyList.Hail });
            mBoxes[(int)e_box.b_snowBomb]   .Init(_tree, _player.Skills[SkillKeyList.SnowBomb]) .setFrom(new SkillKeyList[] { SkillKeyList.Iceball });
            mBoxes[(int)e_box.b_snowMissile].Init(_tree, _player.Skills[SkillKeyList.SnowMissile]) .setFrom(new SkillKeyList[] { SkillKeyList.SnowMissile });
            mBoxes[(int)e_box.b_circle]     .Init(_tree, _player.Skills[SkillKeyList.Circle])   .setFrom(new SkillKeyList[] { SkillKeyList.Iceball });

            mBoxes[(int)e_box.b_poison]     .Init(_tree, _player.Skills[SkillKeyList.Poison])   .setFrom(new SkillKeyList[] { SkillKeyList.Iceball })  .setTem(_poisonTems, _getPoisonTem);
            mBoxes[(int)e_box.b_thunder]    .Init(_tree, _player.Skills[SkillKeyList.Lightning]).setFrom(new SkillKeyList[] { SkillKeyList.Iceball })  .setTem(_thunderTems, _getThunderTem);

            mBoxes[(int)e_box.b_poisonBomb] .Init(_tree, _player.Skills[SkillKeyList.PoisonBomb]).setFrom(new SkillKeyList[] { SkillKeyList.SnowMissile }, new SkillKeyList[] { SkillKeyList.Poison });
            mBoxes[(int)e_box.b_thuncall]   .Init(_tree, _player.Skills[SkillKeyList.Thuncall]) .setFrom(new SkillKeyList[] { SkillKeyList.Lightning }, new SkillKeyList[] { SkillKeyList.Circle });
        }

        #endregion
    }
}