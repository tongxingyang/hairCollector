using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillPetTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_petBase,

            b_pet,
            b_pet2,
            b_pet3,

            b_icewall,
            b_iceberg,
            b_shard,

            b_mine
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

        public bool getMineTem()
        {
            // 플레이어? 템리스트에서 무적 있는지 체크 반환

            return false;
        }

        public bool getBeardTem()
        {
            // 플레이어? 템리스트에서 무적 있는지 체크 반환

            return false;
        }

        #region [ override ]
        public override void mBoxSetting()
        {
            gameObject.SetActive(false);

            mBoxes[(int)e_box.b_petBase].Init(_tree, _player.Skills[SkillKeyList.Summon])
                .setRoot(new List<SkillKeyList>[] {});

            mBoxes[(int)e_box.b_pet]    .Init(_tree, _player.Skills[SkillKeyList.Pet])      .setFrom();
            mBoxes[(int)e_box.b_pet2]   .Init(_tree, _player.Skills[SkillKeyList.Pet2])     .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.BPet } });
            mBoxes[(int)e_box.b_pet3]   .Init(_tree, _player.Skills[SkillKeyList.BPet])     .setFrom();//  .setTem(() => getTem(gainableTem.beardTem));

            mBoxes[(int)e_box.b_icewall].Init(_tree, _player.Skills[SkillKeyList.IceWall])  .setFrom()  
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.IceBerg, SkillKeyList.Shard } });
            mBoxes[(int)e_box.b_iceberg].Init(_tree, _player.Skills[SkillKeyList.IceBerg])  .setFrom()  .setChoiceType(new SkillKeyList[] { SkillKeyList.Shard });
            mBoxes[(int)e_box.b_shard]  .Init(_tree, _player.Skills[SkillKeyList.Shard])    .setFrom()  .setChoiceType(new SkillKeyList[] { SkillKeyList.IceBerg });

            mBoxes[(int)e_box.b_mine]   .Init(_tree, _player.Skills[SkillKeyList.Mine])     .setFrom();//  .setTem(() => getTem(gainableTem.mineTem));
        }

        #endregion
    }
}