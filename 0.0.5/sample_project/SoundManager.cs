using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace GXPEngine
{
    public static class SoundManager
    {
        private static Dictionary<SoundEffect, Sound> storeSound = new Dictionary<SoundEffect, Sound>() { 
            { SoundEffect.BOOM, new Sound("BOOM.mp3") },
            { SoundEffect.JUMP, new Sound("JUMP.mp3") },
            { SoundEffect.SCREAM, new Sound("SCREAM.mp3") } };

        private static SoundChannel musicChannel;

        private static Timer fadeTimer;

        private static bool fadeDone = false;

        public static void StoreSound(SoundEffect tag, Sound sound)
        {
            storeSound.Add(tag, sound);
        }

        public enum SoundEffect
        {
            BOOM,
            JUMP,
            SCREAM
        }

        public static void PlaySound(SoundEffect SoundEffect, float soundVolume = 1, float pan = 0)
        {
            if (soundVolume > 1)
            {
                soundVolume = 1;
            }
            else if (soundVolume < 0)
            {
                soundVolume = 0;
            }
            if (pan < -1)
            {
                pan = -1;
            }
            else if (pan > 1)
            {
                pan = 1;
            }

            var effect = storeSound[SoundEffect];

            SoundChannel soundChannel = effect.Play();
            soundChannel.Pan = pan;
            soundChannel.Volume = soundVolume;
        }

        public static void PlayMusic(string filename = "")
        {
            StopMusic(false);
            Sound music = new Sound(filename);
            musicChannel = music.Play();
        }

        public static void StopMusic(bool fadeOut = false)
        {
            if (fadeOut)
            {
                if (musicChannel != null)
                {
                    if (musicChannel.IsPlaying)
                    {
                        if (fadeTimer == null) //als timer nog niet bestaat
                        {
                            //fading = true;
                            fadeDone = false;
                            fadeTimer = new Timer(2000); //maak de timer aan
                            fadeTimer.Elapsed += OnTimerFadeMusic;
                            fadeTimer.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                if (fadeTimer != null)
                {
                    fadeTimer.Stop();
                    fadeTimer = null;
                    if (fadeDone)
                    {
                        System.Windows.Forms.MessageBox.Show("Music stopped");
                    }
                }
                if (musicChannel != null)
                {
                    musicChannel.Stop();
                    musicChannel = null;
                }
            }
        }

        static void OnTimerFadeMusic(Object sender, ElapsedEventArgs e)
        {
            musicChannel.Volume = musicChannel.Volume - 0.2f;
            if (musicChannel.Volume <= 0)
            {
                fadeDone = true;
                StopMusic(false);
            }
        }
    }
}
