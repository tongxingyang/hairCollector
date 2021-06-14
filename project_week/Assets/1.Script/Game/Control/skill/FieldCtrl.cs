using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace week
{
    public class FieldCtrl : MonoBehaviour
    {        
        [Header("Blizzard Handle")]
        [SerializeField] Material[] _flakeMaterial;
        [SerializeField] ParticleSystemRenderer _blizz;
        [Header("WhiteOut Handle")]
        //[SerializeField] Material[] _flakeMaterial;
        [Space]
        [Header("Aurora Handle")]
        [SerializeField] ParticleSystem _Aurora;
        [SerializeField] ParticleSystemRenderer _Aurr;
        [Header("Iceage Handle")]
        [SerializeField] Canvas _fieldCanvas;
        [SerializeField] Animator _iceage;
        
        GameScene _gs;
        enemyManager _enm;
        SnowController _snow;

        SkillKeyList _skill;
        
        private ParticleSystem.ColorOverLifetimeModule _colorModule;

        enum colorModule { blue, green, purple, rainbow }
        Gradient[] _colorGradient;

        public void Init(GameScene gs)
        {            
            _gs = gs;
            _enm = _gs.EnemyMng;
            _fieldCanvas.worldCamera = _gs.Player.MainCamera;
            _fieldCanvas.planeDistance = 6;
            _snow = GetComponent<SnowController>();

            _colorModule = _Aurora.colorOverLifetime;

            //_Aurora.Stop();
            _Aurora.gameObject.SetActive(false);
            _iceage.gameObject.SetActive(false);

            stopSnow();
        }

        public void getField(SkillKeyList sk, float keep, float val, float dmg = 0f)
        {
            _skill = sk;

            SFX sfx = SFX.field;

            switch (_skill)
            {
                case SkillKeyList.SnowStorm:
                case SkillKeyList.Blizzard:
                    StartCoroutine(storming(keep, val, dmg));
                    break;
                case SkillKeyList.SnowFog:
                case SkillKeyList.WhiteOut:
                    StartCoroutine(foging(keep, (int)val, dmg));
                    break;
                case SkillKeyList.Aurora:
                case SkillKeyList.SubStorm:
                    StartCoroutine(auroring(keep, val));
                    break;
                case SkillKeyList.IceAge:
                    sfx = SFX.iceage;
                    StartCoroutine(iceage(keep, val));
                    break;
            }

            SoundManager.instance.PlaySFXtimer(sfx, keep);
        }

        /// <summary> 눈보라 -> 블리자드 </summary>
        IEnumerator storming(float keep, float slow, float dmg)
        {
            bool isUp = _skill == SkillKeyList.Blizzard;

            _blizz.material = _flakeMaterial[(isUp) ? 1 : 0];
            _blizz.lengthScale = (isUp) ? 3f : 1f;
            setSnowWeather(isUp);

            float time = 0f, term = 1f;

            int cnt = 0;

            _enm.enemySlow(keep, (100 - slow) * 0.01f); // 눈보라 - 슬로우
            _blizz.material.DOFade(0.8f, 0f);

            // +블리자드의 공격력
            while (time < keep)
            {
                time += Time.deltaTime;
                term += Time.deltaTime;

                if (term > 1f)
                {
                    term = 0;
                    cnt++;

                    if (isUp)
                    {
                        Debug.Log("쓰");
                        _enm.enemyDamaged(dmg, _skill);
                    }
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            _blizz.material.DOFade(0.4f, 1f);
            yield return new WaitForSeconds(0.5f);

            stopSnow();
        }

        /// <summary> 눈안개 -> 화이트아웃 </summary>
        IEnumerator foging(float keep, int rate, float dmg)
        {
            bool isUp = (_skill == SkillKeyList.WhiteOut);
            setSnowWeather(isUp);

            float time = 0f, term = 1f;

            int cnt = 0;

            _enm.enemyBlind(keep, rate); // 눈안개 실명

            yield return new WaitForSeconds(0.5f);

            while (time < keep)
            {
                time += Time.deltaTime;
                term += Time.deltaTime;

                if (term > 1f)
                {
                    term = 0;
                    cnt++;

                    if (isUp)
                        _enm.enemyDamaged(dmg, _skill);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0.5f);

            stopSnow();
        }

        /// <summary> 오로라 -> 서브스톰 </summary>
        IEnumerator auroring(float keep, float val)
        {
            if (_colorGradient == null)
            {
                _colorGradient = new Gradient[4];
                setAuroraColor();
            }

            colorModule col = (_skill == SkillKeyList.SubStorm) ? colorModule.rainbow : (colorModule)UnityEngine.Random.Range(0, 3);

            switch (col)
            {
                case colorModule.blue: // 체감
                    _enm.InitBff[0] *= (100 - val) * 0.01f;
                    break;
                case colorModule.green: // 공감
                    _enm.InitBff[1] *= (100 - val) * 0.01f;
                    break;
                case colorModule.purple: // 방감
                    _enm.InitBff[2] -= val;
                    break;
                case colorModule.rainbow:
                    _enm.InitBff[0] *= (100 - val) * 0.01f;
                    _enm.InitBff[1] *= (100 - val) * 0.01f;
                    _enm.InitBff[2] -= val;
                    break;
            }

            //_Aurora.pl .Play();
            _Aurora.gameObject.SetActive(true);
            _colorModule.color = _colorGradient[(int)col];
            _Aurr.material.DOFade(1f, 1f);

            float time = 0f;

            while (time < keep)
            {
                time += Time.deltaTime;

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            _Aurr.material.DOFade(0f, 1f);
            yield return new WaitForSeconds(1f);

            _enm.InitBff = new float[3] { 1f, 1f, 0f };
            _Aurora.gameObject.SetActive(false);
            //_Aurora.Stop();
        }

        /// <summary> 아이스에이지 </summary>
        IEnumerator iceage(float keep, float dmg)
        {
            _iceage.gameObject.SetActive(true);
            // _iceage.SetTrigger("iceage");

            _enm.enemyFrozen(keep);
            _enm.enemyDamaged(dmg, SkillKeyList.IceAge);

            yield return new WaitForSeconds(2f);

            _iceage.SetTrigger("off");
            yield return new WaitForSeconds(1f);

            _iceage.gameObject.SetActive(false);
        }

        void setSnowWeather(bool isUp = false)
        {
            _snow.OnMasterChanged(1f);
            _snow.OnWindChanged((isUp) ? 0.6f : 0.3f);

            float storm = (_skill == SkillKeyList.SnowStorm || _skill == SkillKeyList.Blizzard) ? 0.5f : 0f;
            _snow.OnSnowChanged(storm);

            float fog = (_skill == SkillKeyList.SnowFog || _skill == SkillKeyList.WhiteOut) ? 1f : 0f;
            _snow.OnFogChanged((isUp) ? fog : fog * 0.5f);
        }

        void stopSnow()
        {
            Debug.Log("종료");
            _snow.OnMasterChanged(1f);
            _snow.OnSnowChanged(0f);
            _snow.OnWindChanged(0f);
            _snow.OnFogChanged(0f);
        }

        /// <summary> 오로라 색 설정 </summary>
        void setAuroraColor()
        {
            _colorGradient[(int)colorModule.blue] = new Gradient();
            _colorGradient[(int)colorModule.blue].colorKeys = new GradientColorKey[2]
                {
                    new GradientColorKey(new Color(0f, 0.2f, 1f),0f),
                    new GradientColorKey(new Color(0f, 0.2f, 1f),1f)
                };
            _colorGradient[(int)colorModule.blue].alphaKeys = new GradientAlphaKey[3]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                };

            _colorGradient[(int)colorModule.green] = new Gradient();
            _colorGradient[(int)colorModule.green].colorKeys = new GradientColorKey[2]
                {
                    new GradientColorKey(new Color(0f, 1f, 0.4f), 0f),
                    new GradientColorKey(new Color(0f, 1f, 0.4f), 1f)
                };
            _colorGradient[(int)colorModule.green].alphaKeys = new GradientAlphaKey[3]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                };

            _colorGradient[(int)colorModule.purple] = new Gradient();
            _colorGradient[(int)colorModule.purple].colorKeys = new GradientColorKey[2]
                {
                    new GradientColorKey(new Color(0.6f, 0f, 1f), 0f),
                    new GradientColorKey(new Color(0.6f, 0f, 1f), 1f)
                };
            _colorGradient[(int)colorModule.purple].alphaKeys = new GradientAlphaKey[3]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                };

            _colorGradient[(int)colorModule.rainbow] = new Gradient();
            _colorGradient[(int)colorModule.rainbow].colorKeys = new GradientColorKey[4]
                {
                    new GradientColorKey(new Color(1f, 0.47f, 0.42f), 0f),
                    new GradientColorKey(new Color(0f, 1f, 0.4f), 1f),
                    new GradientColorKey(new Color(0f, 0.2f, 1f), 0f),
                    new GradientColorKey(new Color(0.6f, 0f, 1f), 1f)
                };
            _colorGradient[(int)colorModule.rainbow].alphaKeys = new GradientAlphaKey[3]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                };
        }
    }
}