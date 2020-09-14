using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using week;

namespace week
{
    public class LobbyScene : UIBase
    {
        #region [UIBase]

        enum eText
        {
            nickName,
            CoinTxt
        }

        enum eImage
        {
            hair,
            eyebrow,
            beard,
            cloth
        }

        protected override Enum GetEnumText() { return new eText(); }
        protected override Enum GetEnumImage() { return new eImage(); }
        
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            m_pkTexts[(int)eText.nickName].text = BaseManager.userEntity.NickName;
            refreshUI();
        }

        public void setEquip(DataTable type)
        {
            int val = (int)type;
        }

        public void refreshUI()
        {
        }

        public void PlayGame()
        {
            BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
        }
    }
}