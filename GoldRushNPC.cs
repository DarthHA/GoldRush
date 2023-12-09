using GoldRush.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GoldRush
{
    public class GoldRushNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Vector2? SavedVel = null;
        public int SavedLife;

        public int GoldRushDeathCheckTime = 0;
        public int GoldRushStunTime = 0;

        public override void SetDefaults(NPC npc)
        {
            SavedLife = npc.lifeMax;
        }
        public override bool PreAI(NPC npc)
        {
            if (HasGoldRushStunBuff(npc))
            {
                if (SavedVel == null)
                {
                    SavedVel = npc.velocity;
                }
                npc.velocity = Vector2.Zero;
                return false;
            }
            else
            {
                if (SavedVel != null)
                {
                    npc.velocity = SavedVel.Value;
                    SavedVel = null;
                }
            }
            return true;
        }

        public override void OnKill(NPC npc)
        {
            if (HasGoldRushDeathCheck(npc))
            {
                if (Main.LocalPlayer.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
                {
                    GoldRushPlayer.HeadLife(Main.LocalPlayer, GoldRush.GreedKillHeal, GoldRush.GreedKillHealCD);
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (SavedLife < npc.life) SavedLife = npc.life;

            if (npc.boss)
            {
                if (SavedLife - npc.life >= npc.lifeMax * 0.1f)
                {
                    SavedLife = npc.life;
                    if (HasGoldRushDeathCheck(npc))
                    {
                        if (Main.LocalPlayer.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
                        {
                            GoldRushPlayer.HeadLife(Main.LocalPlayer, GoldRush.GreedKillHeal, GoldRush.GreedKillHealCD);
                        }
                    }
                }
            }

            if (GoldRushDeathCheckTime > 0)
            {
                GoldRushDeathCheckTime--;
            }
            if (GoldRushStunTime > 0)
            {
                GoldRushStunTime--;
                npc.buffImmune[ModContent.BuffType<BlissStun>()] = false;
                npc.AddBuff(ModContent.BuffType<BlissStun>(), GoldRushStunTime);
            }
        }

        public static void DeepApplyStunned(NPC target,int buffTime)
        {
            if (target.realLife == -1)
            {
                ApplyGoldRushStunBuff(target, buffTime);
            }
            if (target.realLife >= 0)
            {
                if (Main.npc[target.realLife].active)
                {
                    ApplyGoldRushStunBuff(Main.npc[target.realLife], buffTime);

                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active)
                        {
                            if (npc.realLife == target.realLife)
                            {
                                if (npc.whoAmI != target.whoAmI && npc.whoAmI != target.realLife)
                                {
                                    ApplyGoldRushStunBuff(npc, buffTime);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DeepAddBuff(NPC target, int buffType, int buffTime, bool dot = false)
        {
            if (!dot || target.realLife == -1)
            {
                target.buffImmune[buffType] = false;
                target.AddBuff(buffType, buffTime);
            }
            if (target.realLife >= 0)
            {
                if (Main.npc[target.realLife].active)
                {
                    Main.npc[target.realLife].buffImmune[buffType] = false;
                    Main.npc[target.realLife].AddBuff(buffType, buffTime);
                    if (!dot)
                    {
                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active)
                            {
                                if (npc.realLife == target.realLife)
                                {
                                    if (npc.whoAmI != target.whoAmI && npc.whoAmI != target.realLife)
                                    {
                                        npc.buffImmune[buffType] = false;
                                        npc.AddBuff(buffType, buffTime);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }


        private static void ApplyGoldRushStunBuff(NPC npc,int time)
        {
            GoldRushNPC modNPC = npc.GetGlobalNPC<GoldRushNPC>();
            if (modNPC.GoldRushStunTime < time)
            {
                modNPC.GoldRushStunTime = time;
            }
        }

        public static bool HasGoldRushStunBuff(NPC npc)
        {
            GoldRushNPC modNPC = npc.GetGlobalNPC<GoldRushNPC>();
            if (modNPC.GoldRushStunTime > 0)
            {
                return true;
            }
            return false;
        }


        public static void ApplyGoldRushDeathCheck(NPC npc)
        {
            GoldRushNPC modNPC = npc.GetGlobalNPC<GoldRushNPC>();
            if (modNPC.GoldRushDeathCheckTime < 3)
            {
                modNPC.GoldRushDeathCheckTime = 3;
            }
        }

        public static bool HasGoldRushDeathCheck(NPC npc)
        {
            GoldRushNPC modNPC = npc.GetGlobalNPC<GoldRushNPC>();
            if (modNPC.GoldRushDeathCheckTime > 0)
            {
                return true;
            }
            return false;
        }

    }

}