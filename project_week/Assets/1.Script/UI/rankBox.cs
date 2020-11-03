using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class rankBox : MonoBehaviour
    {
        [SerializeField] Image _rankIcon;
        [SerializeField] TextMeshProUGUI _rank;
        [SerializeField] TextMeshProUGUI _nickName;
        [SerializeField] TextMeshProUGUI _time;
        [SerializeField] TextMeshProUGUI _boss;

        public void setRankBox(int rank, rankData data)
        {
            _rank.text = $"#{rank+1}";
            _nickName.text = data._nick;
            _time.text = BaseManager.userGameData.getLifeTime(data._time, false) + $"({data._time})";
            _boss.text = data._boss.ToString();
        }
    }
}