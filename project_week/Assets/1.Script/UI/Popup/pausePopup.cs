using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class pausePopup : MonoBehaviour
    {
        [Header("panel")]
        [SerializeField] Image _pausePanel;

        [SerializeField] GameObject _pauseCase;
        [SerializeField] GameObject _optionPanel;
        [SerializeField] GameObject _chkExitPanel;
        [SerializeField] GameObject _chkSkillPanel;

        [SerializeField] TextMeshProUGUI _resumeTxt;
        [Header("option")]
        [SerializeField] Slider _bgmSlider;
        [SerializeField] Slider _sfxSlider;
        [SerializeField] Image _bgmHandle;
        [SerializeField] Image _sfxHandle;
        [SerializeField] Toggle _onShakeToggle;
        [SerializeField] Toggle _dfmToggle;

        [Header("skill view")]
        [SerializeField] GameObject _contents;
        [SerializeField] GameObject _skillCase;

        [SerializeField] Sprite[] _imgs;
        [Space]
        [SerializeField] GameObject _cheat;
        bool cheatOn = false;
        public void cheat()
        {
            cheatOn = !cheatOn;
            _cheat.SetActive(cheatOn);
        }

        GameScene _gs;

        List<getSkillCase> _skillList;

        public void Init(GameScene gs)
        {
            _gs = gs;
            _skillList = new List<getSkillCase>();

            whenOpen();
            _dfmToggle.isOn = false;

            _chkExitPanel.SetActive(false);
            _optionPanel.SetActive(false);
            _chkSkillPanel.SetActive(false);

            _pauseCase.SetActive(false);
            _pausePanel.gameObject.SetActive(false);
        }

        #region [pause-Popup]

        void whenOpen()
        {
            _bgmSlider.value = SoundManager.instance.masterVolumeBGM;
            _sfxSlider.value = SoundManager.instance.masterVolumeSFX;
            _onShakeToggle.isOn = BaseManager._innerData.OnShake;
        }

        public void openPause()
        {
            gameObject.SetActive(true);
            _pauseCase.SetActive(true);
            _resumeTxt.gameObject.SetActive(false);
            whenOpen();

            _gs.whenPause();

            //Time.timeScale = 0;
        }

        public void pauseResume()
        {
            StartCoroutine(resume());
        }

        IEnumerator resume()
        {
            _pauseCase.SetActive(false);
            _resumeTxt.gameObject.SetActive(true);

            float time = 0;

            _pausePanel.CrossFadeAlpha(0, 1.5f, true);
            while (time < 1.5f)
            {
                time += Time.unscaledDeltaTime;

                _resumeTxt.text = Mathf.CeilToInt(3 - (time * 2)).ToString();

                yield return new WaitForEndOfFrame();
            }

            _resumeTxt.gameObject.SetActive(false);
            _pausePanel.gameObject.SetActive(false);

            _gs.whenResume();
        }

        #endregion

        #region [option-Popup]

        public void optionOpen()
        {
            _pauseCase.gameObject.SetActive(false);
            _optionPanel.SetActive(true);

            _bgmHandle.sprite = (BaseManager._innerData.BgmVol < 0.1f) ? _imgs[0] : _imgs[1];
            _sfxHandle.sprite = (BaseManager._innerData.SfxVol < 0.1f) ? _imgs[0] : _imgs[1];
        }

        public void optionClose()
        {
            _optionPanel.SetActive(false);
            _pauseCase.gameObject.SetActive(true);
        }

        public void bgmCtrl(float val)
        {
            SoundManager.instance.SetVolumeBGM(val);
            _bgmHandle.sprite = (val < 0.1f) ? _imgs[0] : _imgs[1];
            
        }
        public void sfxCtrl(float val)
        {
            SoundManager.instance.SetVolumeSFX(val);
            _sfxHandle.sprite = (val < 0.1f) ? _imgs[0] : _imgs[1];
        }

        public void OnShakeToggle()
        {
            BaseManager._innerData.OnShake = _onShakeToggle.isOn;
        }

        public void dmgFontToggle()
        {
            _gs.DmgfntMng.Toggle = _dfmToggle.isOn;
        }

        #endregion

        #region [chkExit-Popup]

        public void chkExitOpen()
        {
            _pausePanel.gameObject.SetActive(false);

            _gs.gameOver();
        }

        //public void chkExitClose()
        //{
        //    _chkExitPanel.SetActive(false);
        //}

        #endregion

        #region [chkSKill-Popup]

        public void chkSkillOpen()
        {
            setSkillList();

            _pauseCase.gameObject.SetActive(false);
            _chkSkillPanel.SetActive(true);
        }
        public void chkSkillClose()
        {
            _chkSkillPanel.SetActive(false);
            _pauseCase.gameObject.SetActive(true);
        }

        void setSkillList()
        { 
            int num = 0;
            getSkillCase gsc;

            for (SkillKeyList sk = SkillKeyList.HP; sk < SkillKeyList.SnowBall; sk++)
            {
                if (_gs.Player.Abils[sk].active)
                {
                    if (_skillList.Count <= num)
                    {
                        gsc = Instantiate(_skillCase).GetComponent<getSkillCase>();
                        gsc.transform.SetParent(_contents.transform);
                        gsc.transform.localPosition = Vector3.zero;
                        gsc.transform.localScale = Vector3.one;
                        _skillList.Add(gsc);
                    }

                    _skillList[num].Init(_gs, sk, _gs.Player.Abils[sk].Lvl);
                    num++;
                }            
            }

            for (SkillKeyList sk = SkillKeyList.SnowBall; sk < SkillKeyList.non; sk++)
            {
                if (_gs.Player.Skills[sk].active)
                {
                    if (_skillList.Count <= num)
                    {
                        gsc = Instantiate(_skillCase).GetComponent<getSkillCase>();
                        gsc.transform.SetParent(_contents.transform);
                        gsc.transform.localPosition = Vector3.zero;
                        gsc.transform.localScale = Vector3.one;
                        _skillList.Add(gsc);
                    }

                    _skillList[num].Init(_gs, sk, _gs.Player.Skills[sk].Lvl);
                    num++;
                }
            }

            if (_skillList.Count > num)
            {
                for (int i = num; i < _skillList.Count; i++)
                {
                    _skillList[num].gameObject.SetActive(false);
                }
            }
        }

        #endregion


    }
}