using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Text;

namespace Legend
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        static AudioSource currentSource;
        static AudioSource alternateSource;

        static AudioClip levelMusic;
        static AudioClip tempMusic;

        static bool isDucked;

        public MusicManager()
        {
            Instance = this;
        }

        void Awake()
        {
            var music1 = new GameObject("Music 1");
            music1.transform.SetParent(transform);
            currentSource = music1.AddComponent<AudioSource>();
            currentSource.playOnAwake = false;
            currentSource.loop = true;
            currentSource.ignoreListenerVolume = true;
            currentSource.volume = Settings.MusicVolume;
            var music2 = new GameObject("Music 2");
            music2.transform.SetParent(transform);
            alternateSource = music2.AddComponent<AudioSource>();
            alternateSource.playOnAwake = false;
            alternateSource.loop = true;
            alternateSource.ignoreListenerVolume = true;
            alternateSource.volume = Settings.MusicVolume;
        }

        public void UpdateMusicVolume()
        {
            if (currentSource != null)
                currentSource.volume = Settings.MusicVolume;
            if (alternateSource != null)
                alternateSource.volume = Settings.MusicVolume;
        }

        //const float fadeTime = 2.3f;

        static void PlayMusic(AudioClip music, float fadeTime = 2.3f)
        {
            if (music == null || currentSource == null || Level.IsHeadlessMode)
            {
                //print("playing music null");
                return;
            }
            //print("playing music " + music.name);

            if (currentSource.isPlaying)
            {
                if (currentSource.clip == music)
                    return;

                var temp = currentSource;
                currentSource = alternateSource;
                alternateSource = temp;

                iTween.AudioTo(alternateSource.gameObject, 0, 1, fadeTime);
                Instance.Delay(() => alternateSource.Stop(), fadeTime);
            }

            currentSource.clip = music;
            currentSource.volume = isDucked ? Math.Min(Settings.MusicVolume, 0.05f) : 0;
            currentSource.Play();
            if (!isDucked)
                iTween.AudioTo(currentSource.gameObject, Settings.MusicVolume, 1, fadeTime);
        }

        public static void SetLevelMusic(AudioClip music)
        {
            //print("playing level music " + music.name);
            levelMusic = music;
            if (tempMusic == null)
                PlayMusic(music);
        }

        static Coroutine tempMusicStopper;

        public static void PlayTempMusic(AudioClip music, float fadeTime = 0.5f)
        {
            if (tempMusic == music)
                return;

            //print("started");
            tempMusic = music;
            PlayMusic(music, fadeTime);

            if (tempMusicStopper != null)
            {
                Instance.StopCoroutine(tempMusicStopper);
                tempMusicStopper = null;
            }
            tempMusicStopper = Instance.Delay(() =>
            {
                //print("stoped");
                StopTempMusic();
            }, music.length - 2.3f);
        }

        public static void StopTempMusic()
        {
            //print("stopping temp music");
            if (tempMusic != null && Instance != null)
            {
                tempMusic = null;
                PlayMusic(levelMusic);
                if (tempMusicStopper != null)
                {
                    Instance.StopCoroutine(tempMusicStopper);
                    tempMusicStopper = null;
                }
            }
        }

        public static void DuckVolume(float duration)
        {
            isDucked = true;
            iTween.AudioTo(currentSource.gameObject, Math.Min(Settings.MusicVolume, 0.001f), 1, 0.5f);

            Instance.Delay(() =>
            {
                isDucked = false;
                iTween.AudioTo(currentSource.gameObject, Settings.MusicVolume, 1, 2f);
            }, duration);
        }
    }
}