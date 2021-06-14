using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace week
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {        
        public static SoundManager instance;

        public float masterVolumeBGM = 1f;
        public float masterVolumeSFX = 1f;

        AudioSource bgmPlayer;
        List<AudioSource> sfxPlayers;
        readonly int sfxMax = 10;

        public void startSound()
        {
            instance = this;
            bgmPlayer = GetComponent<AudioSource>();
            masterVolumeBGM = BaseManager._innerData.BgmVol;
            masterVolumeSFX = BaseManager._innerData.SfxVol;

            sfxPlayers = new List<AudioSource>();
            for (int i = 0; i < 3; i++)
            {
                GameObject go = new GameObject();
                go.name = $"SFX";
                go.transform.parent = transform;
                sfxPlayers.Add(go.AddComponent<AudioSource>());                
            }            
        }

        public void PlayBGM(BGM bgm)
        {
            if (bgmPlayer.isPlaying)
            {
                return;
            }
                        
            bgmPlayer.clip = DataManager.Bgm[bgm];
            bgmPlayer.volume = masterVolumeBGM;
            bgmPlayer.Play();
            bgmPlayer.loop = true;
        }

        public void StopBGM()
        {
            bgmPlayer.Stop();
        }

        public void PlaySFX(SFX sfx)
        {
            getSFXAudio()?.PlayOneShot(DataManager.Sfx[sfx], masterVolumeSFX);
        }

        public void PlaySFXtimer(SFX sfx, float time)
        {
            AudioSource asc = getSFXAudio();
            if (asc != null)
            {
                asc.PlayOneShot(DataManager.Sfx[sfx], masterVolumeSFX);

                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(time - 1f);
                seq.Join(getSFXAudio().DOFade(0f, 1f));
                seq.OnComplete(asc.Stop);
            }
        }

        public void SetVolumeBGM(float a_volume)
        {
            masterVolumeBGM = a_volume;
            BaseManager._innerData.BgmVol = masterVolumeBGM;
            bgmPlayer.volume = masterVolumeBGM;
        }

        public void SetVolumeSFX(float a_volume)
        {
            masterVolumeSFX = a_volume;
            BaseManager._innerData.SfxVol = masterVolumeSFX;
        }

        //==============
        AudioSource getSFXAudio()
        {
            for (int i = 0; i < sfxPlayers.Count; i++)
            {
                if (sfxPlayers[i].isPlaying == false)
                {
                    return sfxPlayers[i];
                }
            }

            if(sfxPlayers.Count < 10)
            {
                GameObject go = new GameObject();
                go.name = $"SFX";
                go.transform.parent = transform;

                AudioSource asc = go.AddComponent<AudioSource>();
                sfxPlayers.Add(asc);
                return asc;
            }
            return null;
        }
    }
}