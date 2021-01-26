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

        [Header("Item")]
        [SerializeField] GameObject _petTem;
        [SerializeField] GameObject _mineTem;

        public bool _getPtTem()
        {
            // 플레이어? 템리스트에서 펫 있는지 체크 반환

            return false;
        }

        public bool _getMnTem()
        {
            // 플레이어? 템리스트에서 마인 있는지 체크 반환

            return false;
        }

        #region [ override ]
        public override void mBoxSetting()
        {
            List<SkillKeyList>[] skl
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.IceWall, SkillKeyList.Mine } };

            mBoxes[(int)e_box.b_petBase].Init(_tree, _player.Skills[SkillKeyList.Summon]).setDefault(skl);

            mBoxes[(int)e_box.b_pet].Init(_tree, _player.Skills[SkillKeyList.Pet]).setFrom(new SkillKeyList[] { SkillKeyList.Summon });
            mBoxes[(int)e_box.b_pet2].Init(_tree, _player.Skills[SkillKeyList.Pet2]).setFrom(new SkillKeyList[] { SkillKeyList.Pet });
            mBoxes[(int)e_box.b_pet3].Init(_tree, _player.Skills[SkillKeyList.BPet]).setFrom(new SkillKeyList[] { SkillKeyList.Pet2 }).setTem(_petTem, _getPtTem);

            List<SkillKeyList>[] skl2
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.Iceberg, SkillKeyList.Shard } };
            mBoxes[(int)e_box.b_icewall].Init(_tree, _player.Skills[SkillKeyList.IceWall])  .setFrom(new SkillKeyList[] { SkillKeyList.Summon }).setGoLine(skl2);
            mBoxes[(int)e_box.b_iceberg].Init(_tree, _player.Skills[SkillKeyList.Iceberg])  
                .setSelectType(new SkillKeyList[] { SkillKeyList.Shard })                   .setFrom(new SkillKeyList[] { SkillKeyList.IceWall });
            mBoxes[(int)e_box.b_shard].Init(_tree, _player.Skills[SkillKeyList.Shard])
                .setSelectType(new SkillKeyList[] { SkillKeyList.Iceberg })                 .setFrom(new SkillKeyList[] { SkillKeyList.IceWall });

            mBoxes[(int)e_box.b_mine].Init(_tree, _player.Skills[SkillKeyList.SnowDart]).setFrom(new SkillKeyList[] { SkillKeyList.Summon }).setTem(_mineTem, _getMnTem);
        }

        #endregion
    }
}