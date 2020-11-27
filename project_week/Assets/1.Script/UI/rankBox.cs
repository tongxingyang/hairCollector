using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class rankBox : MonoBehaviour
    {
        [SerializeField] Image _skinIcon;
        
        [SerializeField] TextMeshProUGUI _rank;
        [SerializeField] TextMeshProUGUI _nickName;
        [SerializeField] TextMeshProUGUI _time;
        [SerializeField] TextMeshProUGUI _boss;
        [SerializeField] TextMeshProUGUI _version;

        public void setRankBox(int rank, string nick, int record, rankData data)
        {
            _skinIcon.sprite = DataManager.SkinSprite[(SkinKeyList)data._skin];
            _rank.text = $"#{rank + 1}";
            _nickName.text = nick;

            int time = (int)(record * 0.001f);
            int boss = (int)(record % 1000);
            _time.text = BaseManager.userGameData.getLifeTime(time, false) + $"({time})";
            _boss.text = boss.ToString();
            _version.text = data._version.ToString();
        }
    }
}