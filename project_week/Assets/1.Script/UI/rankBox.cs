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
        [SerializeField] GameObject _topRanker;
        [SerializeField] Image _medal;

        [SerializeField] TextMeshProUGUI _rank;
        [SerializeField] TextMeshProUGUI _nickName;
        [SerializeField] TextMeshProUGUI _time;
        [SerializeField] TextMeshProUGUI _boss;
        [SerializeField] TextMeshProUGUI _version;
        [SerializeField] TextMeshProUGUI _preRank;

        /// <summary> 세팅 </summary>
        public rankBox setRankBox(levelKey lvl, int rank, string nick, int record, rankSubData data, System.Func<int, Sprite> getsprite)
        {
            _skinIcon.sprite = DataManager.SkinSprite[(SkinKeyList)data._skin];
            _skinIcon.color = Color.white;
            _rank.text = $"#{rank}";
            if (rank < 4)
            {
                _topRanker.SetActive(true);
                _medal.sprite = getsprite(rank-1);
            }
            else 
            { 
                _topRanker.SetActive(false); 
            }
            _nickName.text = nick;

            int time = (int)(record * 0.001f);
            int boss = (int)(record % 1000);
            _time.text = BaseManager.userGameData.getLifeTime(lvl, time) + $"({time})";
            _boss.text = boss.ToString();
            _version.text = data._version.ToString();

            _preRank.text = (BaseManager.userGameData.preRank == -1) ? "#--" : $"#{BaseManager.userGameData.preRank}";
            
            return this;
        }

        /// <summary> 빈거 세팅 </summary>
        public rankBox blink()
        {
            _skinIcon.sprite = DataManager.SkinSprite[SkinKeyList.snowman];
            _skinIcon.color = Color.gray;
            _rank.text = $"#--";
            _nickName.text = BaseManager.userGameData.NickName;

            _time.text = "응애 나 아기눈사람";
            _boss.text = "-";
            _version.text = "ver-";

            _preRank.text = (BaseManager.userGameData.preRank == -1) ? "#--" : $"#{BaseManager.userGameData.preRank}";

            return this;
        }

        //public void setBoxType(bool isSeason)
        //{
        //    _preRank.gameObject.SetActive(isSeason);
        //}
    }
}