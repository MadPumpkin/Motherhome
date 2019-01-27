using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class OptionsMenu : Menu
    {
        public static OptionsMenu Instance;

        //public Slider MasterVolume;
        public Slider MusicVolume;
        public Slider SoundVolume;

        public Toggle FullScreen;

        public Dropdown ResolutionSelect;

        public OptionsMenu()
        {
            Instance = this;
        }

        bool initializing;
        void Start()
        {
            initializing = true;

            MusicVolume.value = Settings.MusicVolume;
            SoundVolume.value = Settings.SoundVolume;

            FullScreen.isOn = Screen.fullScreen;

            ResolutionSelect.ClearOptions();
            var options = Screen.resolutions.Select(r => r.ToString()).ToList();
            ResolutionSelect.AddOptions(options);

            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                var rez = Screen.resolutions[i];
                if (Screen.width == rez.width && Screen.height == rez.height)
                {
                    print(rez.ToString());
                    ResolutionSelect.value = i;
                    break;
                }
            }

            initializing = false;
        }

        public void SetMusicVolume(float value)
        {
            //print("SetMusicVolume");
            Settings.MusicVolume = value;
        }

        public void SetSoundsVolume(float value)
        {
            //print("SetSoundsVolume");
            Settings.SoundVolume = value;
        }

        public void SetFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }

        public void SetResolution(int value)
        {
            if (!initializing)
            {
                //print(value);
                var res = Screen.resolutions[value];
                //print(res);
                Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
            }
        }
    }
}