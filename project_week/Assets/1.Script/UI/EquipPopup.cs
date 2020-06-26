using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace week
{
    public class EquipPopup : UIBase
    {
        #region [UIBase]

        enum eText
        {
            stat,
            tagText
        }

        enum eImage
        {
            selectEquip
        }

        enum eTransform
        {
            Content
        }

        protected override Enum GetEnumTransform() { return new eTransform(); }
        protected override Enum GetEnumText() { return new eText(); }
        protected override Enum GetEnumImage() { return new eImage(); }

        #endregion

        [SerializeField] GameObject buttonfabs;
        List<EquipTile> tiles;
        int tileCount = 5;

        // Start is called before the first frame update
        void Start()
        {
            tiles = new List<EquipTile>();
            for (int i = 0; i < tileCount; i++)
            {
                EquipTile tile = Instantiate(buttonfabs).GetComponent<EquipTile>();
                tile.transform.parent = m_pkTransforms[(int)eTransform.Content];
                tile.transform.localScale = Vector3.one;
                tiles.Add(tile);
            }

            gameObject.SetActive(false);
        }

        public void setEquipPopup(DataTable type)
        {
            for (int i = 0; i < DataManager.bgCount;i++)
            {
                tiles[i].setting(type, i);
            }
        }
    }
}