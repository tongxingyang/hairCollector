using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using week;

public class EquipTile : MonoBehaviour
{
    [SerializeField] Image Button; // 색, 레이
    [SerializeField] Text BtnTxt; // 텍스트
    [SerializeField] GameObject hairCoin; // 액팁브
    [SerializeField] Text price; // 가격
    [SerializeField] Image Equip; 
    [SerializeField] Text stat;

    DataTable _nowData;
    int _equips;
    int _cost;
    bool _isSet;
    bool _isGet;

    int _num;

    LobbyScene _ls;
    EquipPopup _ep;

    private void Start()
    {
        _ls = GetComponentInParent<LobbyScene>();
        _ep = GetComponentInParent<EquipPopup>();
    }

    public void setting(DataTable data, int equips)
    {
        _nowData = data;
        _equips = equips;
        _isSet = BaseManager.userEntity.Item[(int)data].Equals(equips);
        _num = 1 << equips;
        _isGet = (BaseManager.userEntity.HasEquip[(int)_nowData] & _num) > 0;

        Equip.sprite = DataManager.GetTable<Sprite>(data, equips.ToString(), "sprite");
        stat.text = DataManager.GetTable<string>(data, equips.ToString(), "explain");
        _cost = DataManager.GetTable<int>(data, equips.ToString(), "price");
        price.text = _cost.ToString();        

        Button.color = (_isSet) ? Color.gray : new Color(1, 0.29f, 0);
        Button.raycastTarget = !_isSet;
                
        BtnTxt.text = (_isSet) ? "셋팅" : (_isGet) ? "장착!" : "구매!";
        hairCoin.SetActive(!_isGet);
    }

    public void pushBtn()
    {
        if (_isGet)
        {
            BaseManager.userEntity.Item[(int)_nowData] = _equips;
            _ls.setEquip(_nowData);
        }
        else
        {
            if (_cost > BaseManager.userEntity.HairCoin)
            {
                Debug.Log("구매불가");
            }
            else 
            {
                BaseManager.userEntity.HairCoin -= _cost;
                BaseManager.userEntity.HasEquip[(int)_nowData] |= _num;
                BaseManager.userEntity.Item[(int)_nowData] = _equips;
                _ep.setEquipPopup(_nowData);
                _ls.setEquip(_nowData);
                _ls.refreshUI();
            }
        }
    }
}
