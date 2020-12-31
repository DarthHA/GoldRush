
using GoldRush;
using GoldRush.Buffs;
using GoldRush.Items;
using GoldRush.Projectiles;
using GoldRush.Sky;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace GoldRush
{
    public class GoldRush : Mod
    {
        public static GoldRush Instance;
        public override void Load()
        {
            Instance = this;
            Filters.Scene["GoldRush:GoldRushSky"] = new Filter(new GoldRushSkyScreenShaderData("FilterMiniTower").UseColor(0.9f, 0.9f, 0.9f).UseOpacity(0.2f), EffectPriority.VeryHigh);
            SkyManager.Instance["GoldRush:GoldRushSky"] = new GoldRushSky();
        }
        public override void Unload()
        {
            SkyManager.Instance["GoldRush:GoldRushSky"].Deactivate();
            Instance = null;
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Roland 3");
                priority = MusicPriority.BossHigh;
            }
        }



    }

    public class GoldRushPlayer : ModPlayer
    {
        public int GRCD = 0;
        public bool UseGR = false;
        public int CurrentPortal = 0;
        public int CurrentState = 0;
        public int RushTimer = 0;
        public Vector2 ScreenPos = Vector2.Zero;
        public int ScreenLocked = 0;
        public int GreedyPowerLevel = 0;
        public override void ModifyScreenPosition()
        {
            if (UseGR)
            {
                Main.screenPosition = ScreenPos - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            }
            else
            {
                if (ScreenLocked > 0)
                {
                    if (ScreenLocked < 15)
                    {
                        if (ScreenPos != player.Center)
                        {
                            ScreenPos += (player.Center - ScreenPos) / 5;
                        }
                    }
                    Main.screenPosition = ScreenPos - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    //Main.screenPosition += VirtualMovement(player.Center, 20);
                }
            }
        }
        public override void OnEnterWorld(Player player)
        {
            ScreenPos = player.Center;
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (player.HeldItem.type == ModContent.ItemType<GoldRushItem>()) 
            {
                drawInfo.drawHeldProjInFrontOfHeldItemAndBody = true;
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (UseGR)
            {
                if (CurrentState == 0) //dont draw player
                {
                    while (layers.Count > 0)
                        layers.RemoveAt(0);
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (GRCD > 0 && !UseGR)
            {
                GRCD--;
            }
            if (UseGR)
            {
                //player.immune = true;
                //player.immuneNoBlink = true;
                player.controlDown = false;
                player.controlHook = false;
                player.controlJump = false;
                player.controlLeft = false;
                player.controlUp = false;
                player.controlThrow = false;
                player.controlUseTile = false;
                player.controlTorch = false;
                player.controlRight = false;
                player.controlMount = false;
                player.mount.Dismount(player);
                player.velocity = Vector2.Zero;
                if (CurrentState == 0)           //瞬移状态
                {
                    RushTimer = 0;
                    if (CurrentPortal >= GRPortalUtils.PortalCount(player.whoAmI))
                    {
                        RemoveGR();
                        return;
                    }
                    if (GRPortalUtils.GetPortal(player.whoAmI, CurrentPortal) == -1)
                    {
                        RemoveGR();
                        return;
                    }

                    ScreenPos += VirtualMovement(GRPortalUtils.GetPortalCenter(player.whoAmI, CurrentPortal), 40);
                    player.Center = GRPortalUtils.GetPortalEnter(player.whoAmI, CurrentPortal);
                    player.direction = Math.Sign(Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, CurrentPortal)].localAI[0]);
                    if (Vector2.Distance(ScreenPos, GRPortalUtils.GetPortalCenter(player.whoAmI, CurrentPortal)) < 10)
                    {
                        //player.Center = GRPortalUtils.GetPortalEnter(player.whoAmI, CurrentPortal);
                        CurrentState = 1;
                        Main.PlaySound(SoundID.Item14, player.Center);
                    }
                }
                else if (CurrentState == 1)            //冲刺状态
                {
                    RushTimer++;
                    if (GRPortalUtils.GetPortal(player.whoAmI, CurrentPortal) == -1)
                    {
                        RemoveGR();
                    }
                    else
                    {
                        Vector2 Dest = GRPortalUtils.GetPortalOut(player.whoAmI, CurrentPortal);
                        float vel = 40;
                        if (player.Distance(Dest) < 40)
                        {
                            vel = player.Distance(Dest);
                        }
                        if (Dest != player.Center)
                        {
                            player.velocity = Vector2.Normalize(Dest - player.Center) * 0.01f;
                            player.direction = Math.Sign(player.velocity.X);
                            player.position += Vector2.Normalize(player.velocity) * vel;
                        }
                        if (Vector2.Distance(player.Center, Dest) <= 5 || RushTimer > 60)
                        {
                            player.oldPosition = player.position;
                            CurrentPortal++;
                            CurrentState = 0;
                            Main.PlaySound(SoundID.Item14, player.Center);
                            if (CurrentPortal >= GRPortalUtils.PortalCount(player.whoAmI))
                            {
                                player.immune = true;
                                int invince = player.longInvince ? 60 : 40;
                                if (player.HasBuff(ModContent.BuffType<BlissPower>()))
                                {
                                    invince = 60;
                                }
                                player.immuneTime = invince;
                                player.hurtCooldowns[0] = invince;
                                player.hurtCooldowns[1] = invince;
                                player.velocity = new Vector2(player.direction * 10, 0);
                                GRCD = (int)(150f * GRPortalUtils.PortalCount(player.whoAmI) / 6);
                                if (player.HasBuff(ModContent.BuffType<BlissPower>()))
                                {
                                    GRCD = (int)(GRCD * 0.66f);
                                }
                                RemoveGR();
                                return;
                            }
                        }

                    }
                }
            }
            else
            {
                if (ScreenLocked > 0)
                {
                    ScreenLocked--;
                    ScreenPos += VirtualMovement(player.Center, 30f * (float)(120f - ScreenLocked) / 10);
                    if (player.Distance(ScreenPos) < 5)
                    {
                        ScreenLocked = 0;
                    }
                }
            }
        }



        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("GoldRush:GoldRushSky", player.HasBuff(ModContent.BuffType<BlissPower>()), default);
        }

        public override void ResetEffects()
        {
            if (HasGR())
            {
                UseGR = true;
            }
            else
            {
                UseGR = false;
            }

            if (!player.HasBuff(ModContent.BuffType<GreedyPower>()))
            {
                GreedyPowerLevel = 0;
            }

        }
        public override void PreUpdateBuffs()
        {
            //Main.NewText(GreedyPowerLevel);
            if (player.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                player.ClearBuff(ModContent.BuffType<GreedyPower>());
                player.ClearBuff(ModContent.BuffType<BlissPower>());
            }
            if (player.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                GreedyPowerLevel = 0;
                player.ClearBuff(ModContent.BuffType<GreedyPower>());
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!target.immortal && proj.melee && player.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                player.HealEffect(5);
                player.statLife = Utils.Clamp(player.statLife + 5, 0, player.statLifeMax2);
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (proj.melee && player.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                player.HealEffect(5);
                player.statLife = Utils.Clamp(player.statLife + 5, 0, player.statLifeMax2);
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (UseGR)
            {
                return false;
            }
            return true;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (UseGR)
            {
                return false;
            }
            return true;
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (UseGR)
            {
                return false;
            }
            return true;
        }


        public bool HasGR()
        {
            foreach (Projectile gr in Main.projectile)
            {
                if (gr.active && gr.type == ModContent.ProjectileType<GoldRushProj>() && gr.owner == player.whoAmI)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveGR()
        {
            foreach (Projectile gr in Main.projectile)
            {
                if (gr.active && gr.type == ModContent.ProjectileType<GoldRushProj>() && gr.owner == player.whoAmI)
                {
                    gr.Kill();
                }
                if (gr.active && gr.type == ModContent.ProjectileType<GoldRushGlow>() && gr.owner == player.whoAmI)
                {
                    gr.Kill();
                }
            }
            foreach (Projectile gr in Main.projectile)
            {
                if (gr.active && gr.type == ModContent.ProjectileType<GoldPortal>() && gr.owner == player.whoAmI)
                {
                    gr.ai[1] = 1;
                }
            }
            player.itemTime = 0;
            player.itemAnimation = 0;
        }


        public Vector2 VirtualMovement(Vector2 dest, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = dest - ScreenPos;
            float length = (dist == Vector2.Zero) ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }
            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }
            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }
            Vector2 Vel = (length == 0f) ? Vector2.Zero : Vector2.Normalize(dist);
            Vel *= moveSpeed;
            Vel *= velMultiplier;
            return Vel;
        }
    }


}