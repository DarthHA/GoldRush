using GoldRush.Buffs;
using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace GoldRush.Projectiles
{
    public class GoldRushDashGlow : ModProjectile
    {
        public bool GreedAOE = false;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.timeLeft = 99999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.light = 0.9f;
        }

        public override void AI()
        {
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Player owner = Main.player[Projectile.owner];
            GoldRushPlayer modowner = owner.GetModPlayer<GoldRushPlayer>();
            if (!owner.active || owner.dead || owner.ghost)
            {
                Projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                Projectile.Kill();
                return;
            }

            Vector2 OffSet = new Vector2(-20, 5);
            Projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y);
            if (modowner.UsingGR)
            {
                if (modowner.CurrentState == GoldRushPlayer.GRState.TPing)
                {
                    Projectile.Opacity = 0;
                }
                else
                {
                    for(int i = 0; i < 4; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.HeatRay);
                        dust.noLight = false;
                        dust.noGravity = false;
                        dust.scale = 1.5f;
                    }
                    Projectile.Opacity = 1;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }
        }

        public void DrawAlt(SpriteBatch spriteBatch)
        {
            Player owner = Main.player[Projectile.owner];
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRushDash1").Value;
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRushDash2").Value;
            }
            SpriteEffects SP = Main.player[Projectile.owner].direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale * 0.35f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                return true;
            }
            return null;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                if (target.life <= target.lifeMax / 2)
                {
                    modifiers.SetCrit();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Empty)
            {
                GoldRushPlayer.AddPower(owner, target.Center);
            }
            else if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                if (target.life <= 0)
                {
                    if (!GoldRushNPC.HasGoldRushDeathCheck(target))
                    {
                        GoldRushPlayer.HeadLife(Main.LocalPlayer, GoldRush.GreedKillHeal, GoldRush.GreedKillHealCD);
                    }
                }
                GoldRushNPC.ApplyGoldRushDeathCheck(target);
                //GoldRushNPC.DeepAddBuff(target, ModContent.BuffType<GreedyDeathCheck>(), 3);
                if (!GreedAOE)
                {
                    GreedAOE = true;
                    Projectile.NewProjectile(null,Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<GreedAOE>(), (int)(damageDone / GoldRush.DashModifier / 2f), 0, Projectile.owner);
                    Projectile.NewProjectile(null,target.Center, Vector2.Zero, ModContent.ProjectileType<GreedAOEEffect>(), 0, 0, Projectile.owner);
                }
            }
            else if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Bliss)
            {
                // GoldRushNPC.DeepAddBuff(target, ModContent.BuffType<BlissStun>(), GoldRush.BlissStunTime);
                GoldRushNPC.DeepApplyStunned(target, GoldRush.BlissStunTime);
                if (!target.immortal && !target.friendly && target.life > 5)
                {
                    GoldRushPlayer.HeadLife(owner, GoldRush.BlissHeal * 6, GoldRush.BlissHealCD * 3);
                }
            }
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;
        }


        public override bool? CanDamage()
        {
            return Projectile.Opacity == 1;
        }
    }
}