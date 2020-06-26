using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class UserEntity
    {
        #region private

        /// <summary> 닉넴 </summary>
        string nickName;

        /// <summary> 모발코인 </summary>
        int _hairCoin;

        /// <summary> 체력 </summary>
        int _hp = 20;
        /// <summary> 발모력 </summary>
        float _hairGrowth = 1f;
        /// <summary> 모근력 </summary>
        float _hairRoot = 1f;
        /// <summary> 필드 클리어 대기 시간 </summary>
        float _clearTime = 90f;

        int[] hasEquip;

        #endregion

        int[] _Item;

        public int Hp 
        {
            get
            {
                if (_Item[0] > -1)
                {
                    return _hp + DataManager.GetTable<int>(DataTable.hair, _Item[0].ToString(), "value");
                }
                else
                {
                    return _hp;
                }
            }
        }

        public float Growth
        {
            get
            {
                if (_Item[1] > -1)
                {
                    return _hairGrowth * DataManager.GetTable<float>(DataTable.eyebrow, _Item[1].ToString(), "value");
                }
                else
                {
                    return _hairGrowth;
                }
            }
        }
        public float Root
        {
            get
            {
                if (_Item[2] > -1)
                {
                    return _hairRoot * DataManager.GetTable<float>(DataTable.beard, _Item[2].ToString(), "value");
                }
                else
                {
                    return _hairRoot;
                }
            }
        }
        public float ClearTime
        {
            get
            {
                if (_Item[3] > -1)
                {
                    return _clearTime - DataManager.GetTable<int>(DataTable.cloth, _Item[3].ToString(), "value");
                }
                else
                {
                    return _clearTime;
                }
            }
        }
        public string NickName { get => nickName; set => nickName = value; }
        public int HairCoin { get => _hairCoin; set => _hairCoin = value; }

        public int[] Item { get => _Item; set => _Item = value; }
        public string HairItem { get => _Item[0].ToString(); }
        public string EyebrowItem { get => _Item[1].ToString(); }
        public string BeardItem { get => _Item[2].ToString(); }
        public string ClothItem { get => _Item[3].ToString(); }
        
        public int[] HasEquip { get => hasEquip; set => hasEquip = value; }

        public UserEntity()
        {
            nickName = "ready_Player_1";

            _hairCoin = 100;

            _Item = new int[4] { -1, -1, -1, -1 };
            hasEquip = new int[4] { 0, 0, 0, 0 };
        }
    }
}