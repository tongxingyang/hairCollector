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
            b_shieldUp,
            b_hugeShield,
            b_thornShield,
            b_lightningShield,
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

        [Header("Item")]
        [SerializeField] GameObject _InvTems;
        [SerializeField] GameObject _HidTems;

        public bool _getInvTem()
        {
            // 플레이어? 템리스트에서 무적 있는지 체크 반환

            return false;
        }

        public bool _getHidTem()
        {
            // 플레이어? 템리스트에서 은신 있는지 체크 반환

            return false;
        }

        #region [ override ]
        public override void mBoxSetting()
        {
            List<SkillKeyList>[] skl
                = new List<SkillKeyList>[2] {
                    new List<SkillKeyList>() { SkillKeyList.HugeShield, SkillKeyList.ThornShield, SkillKeyList.LightningShield },
                    new List<SkillKeyList>() { SkillKeyList.Invincible, SkillKeyList.Hide }
                };

            mBoxes[(int)e_box.b_shieldBase]     .Init(_tree, _player.Skills[SkillKeyList.Shield])           .setDefault(skl);
            mBoxes[(int)e_box.b_shieldUp]       .Init(_tree, _player.Skills[SkillKeyList.Shield])           .setRein();

            mBoxes[(int)e_box.b_hugeShield]     .Init(_tree, _player.Skills[SkillKeyList.HugeShield])       
                .setSelectType(new SkillKeyList[2] { SkillKeyList.ThornShield, SkillKeyList.LightningShield })  .setFrom(new SkillKeyList[] { SkillKeyList.Shield });
            mBoxes[(int)e_box.b_giantShield]    .Init(_tree, _player.Skills[SkillKeyList.GiantShield])          .setFrom(new SkillKeyList[] { SkillKeyList.HugeShield });
            mBoxes[(int)e_box.b_thornShield]    .Init(_tree, _player.Skills[SkillKeyList.ThornShield])
                .setSelectType(new SkillKeyList[2] { SkillKeyList.HugeShield, SkillKeyList.LightningShield })   .setFrom(new SkillKeyList[] { SkillKeyList.Shield });
            mBoxes[(int)e_box.b_reflectShield]  .Init(_tree, _player.Skills[SkillKeyList.ReflectShield])        .setFrom(new SkillKeyList[] { SkillKeyList.ThornShield });
            mBoxes[(int)e_box.b_lightningShield].Init(_tree, _player.Skills[SkillKeyList.LightningShield])
                .setSelectType(new SkillKeyList[2] { SkillKeyList.HugeShield, SkillKeyList.ThornShield })       .setFrom(new SkillKeyList[] { SkillKeyList.Shield });
            mBoxes[(int)e_box.b_chargeShield]   .Init(_tree, _player.Skills[SkillKeyList.ChargeShield])         .setFrom(new SkillKeyList[] { SkillKeyList.LightningShield });
            
            mBoxes[(int)e_box.b_invincible]     .Init(_tree, _player.Skills[SkillKeyList.Invincible])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.Hide })                                       .setFrom(new SkillKeyList[] { SkillKeyList.Shield }).setTem(_InvTems, _getInvTem);
            mBoxes[(int)e_box.b_absorb]         .Init(_tree, _player.Skills[SkillKeyList.Absorb])               .setFrom(new SkillKeyList[] { SkillKeyList.Invincible });
            mBoxes[(int)e_box.b_hide]           .Init(_tree, _player.Skills[SkillKeyList.Hide])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.Invincible })                                 .setFrom(new SkillKeyList[] { SkillKeyList.Shield }).setTem(_HidTems, _getHidTem);
            mBoxes[(int)e_box.b_chill]          .Init(_tree, _player.Skills[SkillKeyList.Chill])                .setFrom(new SkillKeyList[] { SkillKeyList.Hide });
        }

        #endregion
    }
}