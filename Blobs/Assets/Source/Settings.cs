using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Legend
{
    public static class Settings
    {
        public static float MusicVolume
        {
            get { return PlayerPrefs.GetFloat("Music Volume", 0.15f); }
            set
            {
                //Debug.Log("MusicVolume " + value);
                PlayerPrefs.SetFloat("Music Volume", value);
                PlayerPrefs.Save();
                SetValues();
            }
        }

        public static float SoundVolume
        {
            get { return PlayerPrefs.GetFloat("Sound Volume", 1f); }
            set
            {
                PlayerPrefs.SetFloat("Sound Volume", value);
                PlayerPrefs.Save();
                SetValues();
            }
        }

        public static int MostRecentSave
        {
            get { return PlayerPrefs.GetInt("MostRecentSave", 1); }
            set
            {
                PlayerPrefs.SetInt("MostRecentSave", value);
                PlayerPrefs.Save();
            }
        }

        public static string LastChat
        {
            get { return PlayerPrefs.GetString("LastChat", null); }
            set
            {
                PlayerPrefs.SetString("LastChat", value);
                PlayerPrefs.Save();
            }
        }

        //public static bool PostProcessingEffects
        //{
        //    get { return PlayerPrefs.GetInt("PostProcessingEffects", 1) != 0; }
        //    set
        //    {
        //        PlayerPrefs.SetInt("PostProcessingEffects", value ? 1 : 0);
        //        PlayerPrefs.Save();
        //        SetValues();
        //    }
        //}

        public static void SetValues()
        {
            //Player.Instance.Movement.RpgCamera.InvertMouseY = !InvertMouse;
            //Player.Instance.PostProcessingEffects.enabled = PostProcessingEffects;

            AudioListener.volume = SoundVolume;
            //Debug.Log("Sound Volume Set to " + SoundVolume);
            if (MusicManager.Instance != null)
                MusicManager.Instance.UpdateMusicVolume();
        }
    }
}