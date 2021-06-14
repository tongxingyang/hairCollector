using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillShieldTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_shieldBase,

            b_hugeShield,
            b_thornShield,

            b_giantShield,
            b_reflectShield,
            b_chargeShield,

            b_invincible,
            b_absorb,

            b_hide,
            b_chill
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

        #region [ override ]
        public override void mBoxSetting()
        {
            gameObject.SetActive(false);

            mBoxes[(int)e_box.b_shieldBase]     .Init(_tree, _player.Skills[SkillKeyList.Shield])
                .setRoot(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.HugeShield, SkillKeyList.ThornShield } });

            mBoxes[(int)e_box.b_hugeShield]     .Init(_tree, _player.Skills[SkillKeyList.HugeShield])   .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.ThornShield });
            mBoxes[(int)e_box.b_thornShield]    .Init(_tree, _player.Skills[SkillKeyList.ThornShield])  .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.HugeShield })
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.ReflectShield, SkillKeyList.ChargeShield } });

            mBoxes[(int)e_box.b_giantShield]    .Init(_tree, _player.Skills[SkillKeyList.GiantShield])  .setFrom();                        
            mBoxes[(int)e_box.b_reflectShield]  .Init(_tree, _player.Skills[SkillKeyList.ReflectShield]).setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.ChargeShield });
            mBoxes[(int)e_box.b_chargeShield]   .Init(_tree, _player.Skills[SkillKeyList.ChargeShield]) .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.ReflectShield });

            mBoxes[(int)e_box.b_invincible]     .Init(_tree, _player.Skills[SkillKeyList.Invincible])   .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.Hide });
            mBoxes[(int)e_box.b_hide]           .Init(_tree, _player.Skills[SkillKeyList.Hide])         .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.Invincible });// .setTem(() => getTem(gainableTem.subShield));

            mBoxes[(int)e_box.b_absorb]         .Init(_tree, _player.Skills[SkillKeyList.Absorb])       .setFrom();            
            mBoxes[(int)e_box.b_chill]          .Init(_tree, _player.Skills[SkillKeyList.Chill])        .setFrom();
        }

        #endregion
    }
}