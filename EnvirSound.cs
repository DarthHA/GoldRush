using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace GoldRush
{
    public class EnvirSound
    {
        public SoundEffectInstance sound;
        public bool ShouldPlay = false;
        public float MaxVolume = 1;
        public float Step = 0.015f;
        public float Volume = 0;
        public EnvirSound(SoundEffect soundEffect, float maxVolume = 1, float step = 0.015f)
        {
            sound = soundEffect.CreateInstance();
            sound.Pan = 0;
            sound.Pitch = 0;
            sound.IsLooped = true;
            MaxVolume = maxVolume;
            Step = step;
        }

        public void QuickBegin()
        {
            ShouldPlay = true;
            Volume = 1;
            if (sound.State != SoundState.Playing)
            {
                sound.Play();
            }
        }

        public void QuickStop()
        {
            ShouldPlay = false;
            Volume = 0;
            if (sound.State != SoundState.Stopped)
            {
                sound.Stop(true);
            }
        }

        public void Update()
        {
            if (ShouldPlay)
            {
                if (Volume > MaxVolume - Step)
                {
                    Volume = MaxVolume;
                }
                else
                {
                    Volume += Step;
                }
            }
            else
            {
                if (Volume < Step)
                {
                    Volume = 0;
                }
                else
                {
                    Volume -= Step;
                }
            }
            if (sound.State != SoundState.Playing && Volume > 0)
            {
                sound.Play();
            }
            else if (Volume == 0 && sound.State != SoundState.Stopped)
            {
                sound.Stop(true);
            }
            sound.Volume = Volume * Main.soundVolume;

        }

        public void Dispose()
        {
            if (!sound.IsDisposed)
            {
                sound.Dispose();
            }
        }
    }
}