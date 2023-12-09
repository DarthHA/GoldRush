using GoldRush.Buffs;
using GoldRush.Items;
using GoldRush.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush
{

    public class GoldRushPlayer : ModPlayer
    {
        /// <summary>
        /// 闪金冲刺冷却时间
        /// </summary>
        public int GRCD = 0;
        /// <summary>
        /// 单次冲刺时间，防止卡住
        /// </summary>
        public int RushTimer = 0;

        /// <summary>
        /// 目前在穿的传送门
        /// </summary>
        public int CurrentPortal = 0;

        /// <summary>
        /// 冲刺状态
        /// </summary>
        public enum GRState
        {
            Normal,  //静息
            TPing,   //瞬移
            Rushing, //冲刺
        }
        public GRState CurrentState = GRState.Normal;

        /// <summary>
        /// 是否冲刺中
        /// </summary>
        public bool UsingGR
        {
            get
            {
                return CurrentState != GRState.Normal;
            }
        }

        /// <summary>
        /// 屏幕位置 
        /// </summary>
        public Vector2 ScreenPlayerPos = Vector2.Zero;

        /// <summary>
        /// 相对位置
        /// </summary>
        public Vector2 ScreenRelaPos = Vector2.Zero;

        /// <summary>
        /// 屏幕恢复时间
        /// </summary>
        public int ScreenLocked = 0;

        public int GRImmuneTime = 0;
        public int HealLifeCD = 0;
        public int VictoryPowerLevel = 0;
        public int BlissCount = 0;
        public enum FType
        {
            Empty,
            Greed,
            Bliss
        }
        public FType FinalState;

        public override void ModifyScreenPosition()
        {
            if (UsingGR)
            {
                Main.screenPosition = ScreenPlayerPos - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            }
            else
            {
                if (ScreenLocked > 0)
                {
                    Main.screenPosition = ScreenPlayerPos - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                    //Main.screenPosition += VirtualMovement(Player.Center, 20);
                }
            }
        }

        public override void OnEnterWorld(Player Player)
        {
            ScreenPlayerPos = Player.Center;
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Player.HeldItem.type == ModContent.ItemType<GoldRushItem>())
            {
                drawInfo.heldProjOverHand = true;
            }
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (CurrentState == GRState.TPing) //dont draw Player
            {
                foreach (var layer in PlayerDrawLayerLoader.Layers)
                {
                    layer.Hide();
                }
            }
        }
  


        public override void PostUpdateMiscEffects()
        {
            if (Player.HeldItem.type == ModContent.ItemType<GoldRushItem>())
            {
                Player.noKnockback = true;
                Player.buffImmune[BuffID.VortexDebuff] = true;
                Player.buffImmune[BuffID.WindPushed] = true;
                Player.buffImmune[BuffID.Webbed] = true;
            }
            else
            {
                foreach (Item diamond in Main.item)
                {
                    if (diamond.active && diamond.type == ModContent.ItemType<GreedDiamond>())
                    {
                        diamond.active = false;
                    }
                }
            }

            if (VictoryPowerLevel >= 50)
            {
                string str;
                Color color;
                VictoryPowerLevel = 0;
                if (BlissCount >= 4)
                {
                    str = Language.GetTextValue("Mods.GoldRush.Misc.RoG");
                    color = Color.Gold;
                    FinalState = FType.Bliss;
                }
                else
                {
                    str = Language.GetTextValue("Mods.GoldRush.Misc.RoK");
                    color = Color.Red;
                    FinalState = FType.Greed;
                }
                CombatText.NewText(Player.Hitbox, color, str);
                BlissCount = 0;
                HeadLife(Player, Player.statLifeMax2, -1);
                if (!UsingGR)
                {
                    foreach (Projectile portal in Main.projectile)
                    {
                        if (portal.active && portal.type == ModContent.ProjectileType<GoldPortal>() && portal.owner == Player.whoAmI)
                        {
                            portal.ai[1] = 1;
                        }
                    }
                }
                foreach(Item diamond in Main.item)
                {
                    if(diamond.active && diamond.type == ModContent.ItemType<GreedDiamond>())
                    {
                        diamond.active = false;
                    }
                }
            }
            else if (VictoryPowerLevel > 0)
            {
                Player.AddBuff(ModContent.BuffType<VictoryPower>(), 2);
                Player.GetDamage(DamageClass.Melee) += 0.015f * VictoryPowerLevel;
            }

            if (FinalState == FType.Bliss)
            {
                Player.AddBuff(ModContent.BuffType<BlissPower>(), 2);
                Player.GetDamage(DamageClass.Melee) += 1f;
                Player.longInvince = true;
            }
            else if (FinalState == FType.Greed)
            {
                Player.AddBuff(ModContent.BuffType<GreedyPower>(), 2);
                Player.GetDamage(DamageClass.Melee) += 1f;
                Player.statDefense /= 4;
                Player.endurance /= 4;
                Player.GetArmorPenetration(DamageClass.Melee) += 50f;
            }

            if (GRCD > 0)
            {
                Player.AddBuff(ModContent.BuffType<GoldRushCD>(), GRCD);
            }
            if (GRImmuneTime > 0)
            {
                GRImmuneTime--;
            }
            if (HealLifeCD > 0)
            {
                HealLifeCD--;
            }
            if (FinalState != FType.Empty)
            {
                SkyManager.Instance.Activate("GoldRush:GoldRushSky");
            }
            else
            {
                SkyManager.Instance.Deactivate("GoldRush:GoldRushSky");
            }

        }

        public override void PostUpdateEquips()
        {
            if (UsingGR)
            {
                Player.controlDown = false;
                Player.controlHook = false;
                Player.controlJump = false;
                Player.controlLeft = false;
                Player.controlUp = false;
                Player.controlThrow = false;
                Player.controlUseTile = false;
                Player.controlTorch = false;
                Player.controlRight = false;
                Player.controlMount = false;
                Player.mount.Dismount(Player);
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Stoned] = true;
                Player.buffImmune[BuffID.Suffocation] = true;
                Player.buffImmune[BuffID.TheTongue] = true;
                Player.buffImmune[BuffID.Burning] = true;
                Player.buffImmune[BuffID.Cursed] = true;
                Player.lavaImmune = true;
                Player.velocity = Vector2.Zero;
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].aiStyle == 7)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (CurrentState == GRState.TPing)           //瞬移状态
                {
                    RushTimer = 0;
                    if (CurrentPortal >= GRPortalUtils.PortalCount(Player.whoAmI))
                    {
                        RemoveDashProj();
                        return;
                    }
                    if (GRPortalUtils.GetPortal(Player.whoAmI, CurrentPortal) == -1)
                    {
                        RemoveDashProj();
                        return;
                    }

                    ScreenPlayerPos += VirtualMovement(GRPortalUtils.GetPortalCenter(Player.whoAmI, CurrentPortal), 40);
                    Player.Center = GRPortalUtils.GetPortalEnter(Player.whoAmI, CurrentPortal);
                    Player.direction = Math.Sign(Main.projectile[GRPortalUtils.GetPortal(Player.whoAmI, CurrentPortal)].localAI[0]);
                    if (Vector2.Distance(ScreenPlayerPos, GRPortalUtils.GetPortalCenter(Player.whoAmI, CurrentPortal)) < 10)
                    {
                        //Player.Center = GRPortalUtils.GetPortalEnter(Player.whoAmI, CurrentPortal);
                        CurrentState = GRState.Rushing;
                        if (FinalState == FType.Greed)
                        {
                            SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/StrongAtk2"), Player.Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/StrongAtk"), Player.Center);
                        }
                    }
                }
                else if (CurrentState == GRState.Rushing)            //冲刺状态
                {
                    RushTimer++;
                    if (GRPortalUtils.GetPortal(Player.whoAmI, CurrentPortal) == -1)
                    {
                        RemoveDashProj();
                    }
                    else
                    {
                        Vector2 Dest = GRPortalUtils.GetPortalOut(Player.whoAmI, CurrentPortal);
                        float vel = 40;
                        if (Player.Distance(Dest) < 40)
                        {
                            vel = Player.Distance(Dest);
                        }
                        if (Dest != Player.Center)
                        {
                            Player.velocity = Vector2.Normalize(Dest - Player.Center) * 0.01f;
                            Player.direction = Math.Sign(Player.velocity.X);
                            Player.position += Vector2.Normalize(Player.velocity) * vel;
                        }
                        if (Vector2.Distance(Player.Center, Dest) <= 5 || RushTimer > 60)
                        {
                            Player.oldPosition = Player.position;
                            CurrentPortal++;
                            CurrentState = GRState.TPing;
                            SoundEngine.PlaySound(SoundID.Item14, Player.Center);
                            if (CurrentPortal >= GRPortalUtils.PortalCount(Player.whoAmI))
                            {
                                int invince = Player.longInvince ? 60 : 40;
                                GiveImmune(Player, invince);

                                ScreenRelaPos = ScreenPlayerPos - Player.Center;
                                ScreenLocked = 120;
                                Player.velocity = new Vector2(Player.direction * 10, 0);

                                if (FinalState == FType.Greed)
                                {
                                    GRCD = GoldRush.GoldRushGreedBaseCD;
                                }
                                else
                                {
                                    GRCD = GoldRush.GoldRushBaseCD + GoldRush.GoldRushAddCD * GRPortalUtils.PortalCount(Player.whoAmI);
                                }
                                QuickStop(false);
                                return;
                            }
                        }

                    }
                }
            }
            else
            {
                if (GRCD > 0)
                {
                    GRCD--;
                }

                if (ScreenLocked > 0)
                {
                    ScreenLocked--;
                    ScreenRelaPos = Vector2.Normalize(ScreenRelaPos) * ScreenRelaPos.Length() * 0.9f;
                    ScreenPlayerPos = Player.Center + ScreenRelaPos;
                    if (ScreenRelaPos.Length() < 2)
                    {
                        ScreenLocked = 0;
                    }
                }
            }
        }


        public override void ResetEffects()
        {
            if (Player.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                VictoryPowerLevel = 0;
                BlissCount = 0;
                FinalState = FType.Empty;
            }
        }
        public override void UpdateDead()
        {
            VictoryPowerLevel = 0;
            BlissCount = 0;
            FinalState = FType.Empty;
            QuickStop();
        }
        public override void PreUpdateBuffs()
        {
            //Main.NewText(GreedyPowerLevel);
            if (Player.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                Player.ClearBuff(ModContent.BuffType<GreedyPower>());
                Player.ClearBuff(ModContent.BuffType<BlissPower>());
            }
            if (Player.HasBuff(ModContent.BuffType<BlissPower>()))
            {
                VictoryPowerLevel = 0;
                Player.ClearBuff(ModContent.BuffType<GreedyPower>());
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (UsingGR)
            {
                return false;
            }
            if (GRImmuneTime > 0)
            {
                return false;
            }
            return true;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (UsingGR)
            {
                return false;
            }
            if (GRImmuneTime > 0)
            {
                return false;
            }
            if (FinalState == FType.Greed)
            {
                if (damage > Player.statLifeMax2 / 3)
                {
                    damage = Player.statLifeMax2 / 3;
                }
            }
            else if (Player.HeldItem.type == ModContent.ItemType<GoldRushItem>())
            {
                if (damage > Player.statLifeMax2 / 2)
                {
                    damage = Player.statLifeMax2 / 2;
                }
            }
            return true;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (UsingGR)
            {
                return false;
            }
            if (GRImmuneTime > 0)
            {
                return false;
            }
            return true;
        }



        private void RemoveDashProj()
        {
            foreach (Projectile gr in Main.projectile)
            {
                if (gr.active && gr.type == ModContent.ProjectileType<GoldRushDash>() && gr.owner == Player.whoAmI)
                {
                    gr.Kill();
                }
                if (gr.active && gr.type == ModContent.ProjectileType<GoldRushDashGlow>() && gr.owner == Player.whoAmI)
                {
                    gr.Kill();
                }
            }
            foreach (Projectile gr in Main.projectile)
            {
                if (gr.active && gr.type == ModContent.ProjectileType<GoldPortal>() && gr.owner == Player.whoAmI)
                {
                    gr.ai[1] = 1;
                }
            }
        }


        private Vector2 VirtualMovement(Vector2 dest, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = dest - ScreenPlayerPos;
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

        public static void InitDash(Player Player)
        {
            Player.GetModPlayer<GoldRushPlayer>().ScreenPlayerPos = Player.Center;
            Player.GetModPlayer<GoldRushPlayer>().CurrentPortal = 0;
            Player.GetModPlayer<GoldRushPlayer>().CurrentState = GRState.TPing;
        }

        public void QuickStop(bool IgnoreScreen = true)
        {
            if (IgnoreScreen)
            {
                ScreenPlayerPos = Player.Center;
                ScreenRelaPos = Vector2.Zero;
                ScreenLocked = 0;
            }
            CurrentPortal = 0;
            CurrentState = GRState.Normal;
            RushTimer = 0;
            RemoveDashProj();
        }

        public bool AnyProj(int type)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type && proj.owner == Player.whoAmI)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSpare(Player Player)
        {
            return !Player.GetModPlayer<GoldRushPlayer>().AnyProj(ModContent.ProjectileType<GoldRushDash>()) &&
                !Player.GetModPlayer<GoldRushPlayer>().AnyProj(ModContent.ProjectileType<GoldRushPen1>()) &&
                !Player.GetModPlayer<GoldRushPlayer>().AnyProj(ModContent.ProjectileType<GoldRushPen2>()) &&
                !Player.GetModPlayer<GoldRushPlayer>().AnyProj(ModContent.ProjectileType<GoldRushSlash1>()) &&
                !Player.GetModPlayer<GoldRushPlayer>().AnyProj(ModContent.ProjectileType<GoldRushSlash2>());
        }


        public static void GiveImmune(Player Player, int time)
        {
            if (Player.GetModPlayer<GoldRushPlayer>().GRImmuneTime < time)
            {
                Player.GetModPlayer<GoldRushPlayer>().GRImmuneTime = time;
            }
        }

        public static void HeadLife(Player Player, int life, int cd = -1)
        {
            if (cd == -1)
            {
                Player.HealEffect(life);
                Player.statLife += life;
                if (Player.statLife > Player.statLifeMax2)
                {
                    Player.statLife = Player.statLifeMax2;
                }
                return;
            }
            if (Player.GetModPlayer<GoldRushPlayer>().HealLifeCD == 0)
            {
                Player.GetModPlayer<GoldRushPlayer>().HealLifeCD = cd;
                Player.HealEffect(life);
                Player.statLife += life;
                if (Player.statLife > Player.statLifeMax2)
                {
                    Player.statLife = Player.statLifeMax2;
                }
            }
        }

        public static void AddPower(Player Player, Vector2 Pos)
        {
            GoldRushPlayer modPlayer = Player.GetModPlayer<GoldRushPlayer>();
            if (modPlayer.FinalState == FType.Empty)
            {
                modPlayer.VictoryPowerLevel++;
                if (modPlayer.VictoryPowerLevel > 50)
                {
                    modPlayer.VictoryPowerLevel = 50;
                }

                if (modPlayer.VictoryPowerLevel % 10 == 0 && modPlayer.VictoryPowerLevel < 50)
                {
                    Vector2 RandomUnit = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2();
                    int itemtmp = Item.NewItem(null, Pos, ModContent.ItemType<GreedDiamond>(), 1);
                    SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/MakeDiamond"), Player.Center);
                    Main.item[itemtmp].velocity = 10 * RandomUnit;
                }
            }
        }

    }

}