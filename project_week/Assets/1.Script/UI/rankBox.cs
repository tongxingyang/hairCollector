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
            _skinIcon.color = Color.white;
            _rank.text = $"#{rank}";
            _nickName.text = nick;

            int time = (int)(record * 0.001f);
            int boss = (int)(record % 1000);
            _time.text = BaseManager.userGameData.getLifeTime(time, false) + $"({time})";
            _boss.text = boss.ToString();
            _version.text = data._version.ToString();
        }

        public void blink()
        {
            _skinIcon.sprite = DataManager.SkinSprite[SkinKeyList.snowman];
            _skinIcon.color = Color.gray;
            _rank.text = $"#--";
            _nickName.text = BaseManager.userGameData.NickName;

            _time.text = "응애 나 아기눈사람";
            _boss.text = "-";
            _version.text = "ver-";
        }
    }
}