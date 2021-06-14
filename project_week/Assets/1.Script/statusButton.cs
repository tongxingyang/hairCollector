using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace week
{
    public class statusButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _level;
        
        [SerializeField] GameObject _glowObj;
        [SerializeField] Image _cases;
        [SerializeField] Image _img;
        [SerializeField] Button _button;

        statusKeyList _stat;
        Material _glow;

        /// <summary> 초기화 (변경 없는 값) </summary>
        public statusButton setInit(statusKeyList stat, Material material)
        {
            _stat = stat;

            _name.text = D_status.GetEntity(_stat.ToString()).f_sName;
            _img.sprite = DataManager.Statusicon[stat];
                        
            _glow = material;
            _glowObj.GetComponent<Image>().material = _glow;

            _button.onClick.AddListener(OnClick);

            return this;
        }

        /// <summary> 데이터 설정 (변경 있는 값) </summary>
        public void setData(int level)
        {
            Color _color = Color.white;
            int rankLvl = 0;

            for (int i = 0; i < gameValues._tier.Length; i++)
            {
                if (level <= gameValues._tier[i + 1]) 
                {
                    _color = gameValues._statusRankData[i]._color;
                    rankLvl = i;
                    break;
                }
            }

            _level.text = $"Lv.{level- gameValues._tier[rankLvl]} <size=35>/ {gameValues._tier[rankLvl + 1]}</size>";

            _cases.color = _color;

            _color *= 1.4f;
            _glow.SetColor("_Color", _color);
            _glow.SetColor("_EmissionColor", _color);
        }

        public void OnClick()
        { 
            
        }

        public void glowOn(bool bl)
        {
            _glowObj.SetActive(bl);
        }

        [Button]
        public void changeColor()
        {
            Color _color = gameValues._statusRankData[UnityEngine.Random.Range(0, 4)]._color;

            _cases.color = _color;

            _glow = _glowObj.GetComponent<Image>().material;
            _glow.SetColor("_Color", _color);
            _glow.SetColor("_EmissionColor", _color);
        }
    }
}