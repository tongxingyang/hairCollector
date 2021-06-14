using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class levelPopup : MonoBehaviour, UIInterface
    {
        [SerializeField] Button[] _lvls;
        [SerializeField] Image[] _cases;
        [Space]
        [SerializeField] TextMeshProUGUI _lvlName;
        [SerializeField] TextMeshProUGUI _info;
        [SerializeField] GameObject _lock;
        [SerializeField] TextMeshProUGUI _conTxt;

        [SerializeField] Image _infoImg;
        [SerializeField] Button _select;
        [SerializeField] Button[] _cancels;
        [Space]
        [SerializeField] Sprite[] _lockCase;

        levelKey _stglvl;
        Action _levelIcon;

        bool _isOpen = false;

        public void Init(Action levelIcon)
        {
            _levelIcon = levelIcon;
            _levelIcon?.Invoke();

            for (int i = 0; i < _lvls.Length; i++)
            {
                levelKey lvl = (levelKey)i;
                _lvls[i].onClick.AddListener(() => {
                    _stglvl = lvl;
                    changeLevelshowInfo();
                });
            }

            _select.onClick.AddListener(() => {
                if (_isOpen)
                {
                    BaseManager.userGameData.NowStageLevel = _stglvl;
                    _levelIcon?.Invoke();
                    close();
                }
                else
                {
                    WindowManager.instance.Win_message.showMessage("해당 난이도는 아직 잠겨있눈!");
                }
            });

            for (int i = 0; i < _cancels.Length; i++)
            {
                _cancels[i].onClick.AddListener(() => close());
            }

            close();
        }

        void changeLevelshowInfo()
        {
            _isOpen = BaseManager.userGameData.IsLevelOpen(_stglvl);
            _lvlName.text = $"[{D_level.GetEntity(_stglvl.ToString()).f_trans}]";

            string str =  $"- 몹 기본 능력치 {D_level.GetEntity(_stglvl.ToString()).f_mobrate}배" + Environment.NewLine
                 + $"- 이동속도 {D_level.GetEntity(_stglvl.ToString()).f_speed}배" + Environment.NewLine
                 + $"- 코인 배수 {D_level.GetEntity(_stglvl.ToString()).f_coin}배";

            string tmp = "";
            for (levelVal i = levelVal.mapinfo; i <= levelVal.seasoninfo; i++)
            {
                tmp = D_level.GetEntity(_stglvl.ToString()).Get<string>(i.ToString());
                if (tmp.Equals("-") == false)
                {
                    str += Environment.NewLine + $"- {tmp}";
                }
            }
            _info.text = str;
                        
            for (int i = 0; i < 3; i++)
            {
                _lvls[i].transform.localScale = (i == (int)_stglvl) ? Vector3.one : Vector3.one * 0.9f;
                _cases[i].sprite = _lockCase[(i == (int)_stglvl) ? 1 : 0];
            }
            _infoImg.sprite = DataManager.LevelImgs[(int)_stglvl];

            if (_isOpen)
            {
                _conTxt.text = "";
                _lock.SetActive(false);
                _infoImg.color = Color.white;
            }
            else
            {
                _conTxt.text = "-해금 :" + System.Environment.NewLine + $"난이도 {D_level.GetEntity((_stglvl - 1).ToString()).f_trans} 3계절 이상 버티기";
                _lock.SetActive(true);
                _infoImg.color = Color.black;
            }            

            for (levelKey i = levelKey.easy; i < levelKey.max; i++)
            {
                _lvls[(int)i].transform.localScale = (i == _stglvl) ? Vector3.one * 1.1f : Vector3.one * 0.95f;
            }
        }

        public void open()
        {
            _stglvl = (levelKey)(int)BaseManager.userGameData.NowStageLevel;

            changeLevelshowInfo();

            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

    }
}