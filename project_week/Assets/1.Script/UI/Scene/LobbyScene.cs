using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using week;

public class LobbyScene : UIBase
{
    #region [UIBase]

    enum eText
    { 
        nickName,
        hairCoinTxt,
        hpTxt
    }

    enum eImage
    { 
        hair,
        eyebrow,
        beard,
        cloth,

        hairPrev,
        eyebrowPrev,
        beardPrev,
        clothPrev,

        EquipPopup,
        lobbyOption
    }

    enum eRect
    {
        hpCase
    }

    protected override Enum GetEnumText() { return new eText(); }
    protected override Enum GetEnumImage() { return new eImage(); }
    protected Enum GetEnumRect() { return new eRect(); }
    public RectTransform[] m_pkRect;

    protected override void OtherSetContent()
    {
        if (GetEnumRect() != null)
        {
            m_pkRect = SetComponent<RectTransform>(GetEnumRect());
        }
    }

    #endregion

    EquipPopup _ep;

    // Start is called before the first frame update
    void Start()
    {
        closeEquipTap();
        closeOption();
        _ep = m_pkImages[(int)eImage.EquipPopup].GetComponent<EquipPopup>();

        m_pkTexts[(int)eText.nickName].text = BaseManager.userEntity.NickName;
        refreshUI();

        for (int i = 0; i < 4; i++)
        {
            if (BaseManager.userEntity.Item[i] > -1)
            {
                setEquip((DataTable)i);
            }
            else
            {
                m_pkImages[(int)eImage.hair + i].gameObject.SetActive(false);
                m_pkImages[(int)eImage.hairPrev + i].gameObject.SetActive(false);
            }
        }
    }

    public void openEquipTap(int i)
    {
        m_pkImages[(int)eImage.EquipPopup].gameObject.SetActive(true);
        _ep.setEquipPopup((DataTable)i);
    }

    public void closeEquipTap()
    {
        m_pkImages[(int)eImage.EquipPopup].gameObject.SetActive(false);
    }

    public void setEquip(DataTable type)
    {
        int val = (int)type;
        m_pkImages[(int)eImage.hair + val].sprite     = DataManager.GetTable<Sprite>(type, BaseManager.userEntity.Item[val].ToString(), "sprite");
        m_pkImages[(int)eImage.hairPrev + val].sprite = DataManager.GetTable<Sprite>(type, BaseManager.userEntity.Item[val].ToString(), "sprite");

        m_pkImages[(int)eImage.hair + val].gameObject.SetActive(true);
        m_pkImages[(int)eImage.hairPrev + val].gameObject.SetActive(true);
    }

    public void refreshUI()
    {
        m_pkTexts[(int)eText.hairCoinTxt].text = BaseManager.userEntity.HairCoin.ToString();
        m_pkTexts[(int)eText.hpTxt].text = "Total 체력 " + BaseManager.userEntity.Hp;
        m_pkRect[(int)eRect.hpCase].sizeDelta = new Vector2(BaseManager.userEntity.Hp + 20, 120);

    }

    public void PlayGame()
    {
        BaseManager.instance.convertScene(SceneNum.LobbyScene.ToString(), SceneNum.GameScene);
    }

    public void openOption()
    {
        m_pkImages[(int)eImage.lobbyOption].gameObject.SetActive(true);
    }

    public void closeOption()
    {
        m_pkImages[(int)eImage.lobbyOption].gameObject.SetActive(false);
    }
}
