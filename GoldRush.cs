using GoldRush.Buffs;
using GoldRush.Projectiles;
using GoldRush.Sky;
using Microsoft.Xna.Framework;
using rail;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldRush
{
    public class GoldRush : Mod
    {
        public static GoldRush Instance;

        public const int GoldRushBaseCD = 90;
        public const int GoldRushAddCD = 30;
        public const int GoldRushGreedBaseCD = 150;
        public const int BlissHeal = 15;
        public const int BlissHealCD = 15;
        public const int BlissStunTime = 60;
        public const int GreedKillHeal = 200;
        public const int GreedKillHealCD = 120;
        public const int BlissSpeedTime = 300;
        public const float GreedDamageModifier = 0.4f;
        public const float DashModifier = 3f;
        public const int NormalImmune = 15;
        public const int GreedImmune = 30;

        public static bool SoundLoaded = false;
        public static EnvirSound PortalSound;

        public override void Load()
        {
            Instance = this;
            //Filters.Scene["GoldRush:GoldRushSky"] = new Filter(new GoldRushSkyScreenShaderData("FilterMiniTower").UseColor(1.0f, 1.0f, 1.0f).UseOpacity(0.0f), EffectPriority.VeryHigh);
            SkyManager.Instance["GoldRush:GoldRushSky"] = new GoldRushSky();

            PortalSound = new EnvirSound(new SoundStyle("GoldRush/Sounds/PortalWait").GetRandomSound());
            SoundLoaded = true;
        }


        public override void Unload()
        {
            SkyManager.Instance["GoldRush:GoldRushSky"].Deactivate();
            PortalSound.Dispose();
            PortalSound = null;
            SoundLoaded = false;
            Instance = null;
        }
    }


    public class MusicSystem : ModSystem
    {
        public override void Load()
        {
            On.Terraria.Main.UpdateAudio += Main_UpdateAudio;
            On.Terraria.Main.UpdateAudio_DecideOnTOWMusic += Main_UpdateAudio_DecideOnTOWMusic;
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;
        }

        private void Main_UpdateAudio_DecideOnNewMusic(On.Terraria.Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
        {
            orig.Invoke(self);
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active) return;

            int music = Main.newMusic;
            UpdateMusic(ref music);
            Main.newMusic = music;
        }

        private void Main_UpdateAudio_DecideOnTOWMusic(On.Terraria.Main.orig_UpdateAudio_DecideOnTOWMusic orig, Main self)
        {
            orig.Invoke(self);
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active) return;

            int music = Main.newMusic;
            UpdateMusic(ref music);
            Main.newMusic = music;
        }

        private void UpdateMusic(ref int music)
        {
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<GreedyPower>()))
            {
                music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Roland 2");
            }
            else if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Roland 3");
            }
        }

        public override void Unload()
        {
            On.Terraria.Main.UpdateAudio -= Main_UpdateAudio;
            On.Terraria.Main.UpdateAudio_DecideOnTOWMusic -= Main_UpdateAudio_DecideOnTOWMusic;
            On.Terraria.Main.UpdateAudio_DecideOnNewMusic -= Main_UpdateAudio_DecideOnNewMusic;
        }

        private void Main_UpdateAudio(On.Terraria.Main.orig_UpdateAudio orig, Main self)
        {
            orig.Invoke(self);
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active)
            {
                if (GoldRush.SoundLoaded)
                {
                    GoldRush.PortalSound.QuickStop();
                }
                return;
            }
            if (GoldRush.SoundLoaded)
            {
                GoldRush.PortalSound.Update();
            }
        }

        public override void PostUpdateProjectiles()
        {
            bool ExistPortal = false;
            foreach (Projectile portal in Main.projectile)
            {
                if (portal.active && portal.type == ModContent.ProjectileType<GoldPortalFake>() && portal.owner == Main.myPlayer)
                {
                    ExistPortal = true;
                    break;
                }
            }
            if (ExistPortal)
            {
                GoldRush.PortalSound.ShouldPlay = true;
            }
            else
            {
                GoldRush.PortalSound.ShouldPlay = false;
            }


        }
    }
}