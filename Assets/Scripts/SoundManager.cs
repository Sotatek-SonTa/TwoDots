using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] AudioSource sfxSound;
     public void PlaySound(SoundName audioClipName, float volumne)
        {
            var audioClip =  audioClips.Find(x => x.name == audioClipName.ToString());
            if(audioClip)
            {
              sfxSound.clip = audioClip;
              sfxSound.volume = volumne;
              sfxSound.Play();
            }
        }
}
public enum SoundName
{
    LOOSE,
    WIN,
    SELECTDOT,
    MATCHING,
    BUTTONCLICK,
}
