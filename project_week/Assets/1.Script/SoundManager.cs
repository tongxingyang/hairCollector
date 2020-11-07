using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {        
        public static SoundManager instance;

        public float masterVolumeBGM = 1f;
        public float masterVolumeSFX = 1f;

        [SerializeField] AudioClip[] BGMClip;
        [SerializeField] AudioClip[] SFXClip;

        AudioSource bgmPlayer;
        [SerializeField] AudioSource sfxPlayer;

        public void startSound()
        {
            instance = this;
            bgmPlayer = GetComponent<AudioSource>();
            masterVolumeBGM = BaseManager.option.BgmVol;
            masterVolumeSFX = BaseManager.option.SfxVol;
        }

        public void PlayBGM(BGM bgm)
        {
            if (bgmPlayer.isPlaying)
            {
                return;
            }
                        
            bgmPlayer.clip = BGMClip[(int)bgm];
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
            sfxPlayer.PlayOneShot(SFXClip[(int)sfx], masterVolumeSFX);
        }

        public void SetVolumeBGM(float a_volume)
        {
            masterVolumeBGM = a_volume;
            BaseManager.option.BgmVol = masterVolumeBGM;
            bgmPlayer.volume = masterVolumeBGM;
        }

        public void SetVolumeSFX(float a_volume)
        {
            masterVolumeSFX = a_volume;
            BaseManager.option.SfxVol = masterVolumeSFX;
        }
    }
}