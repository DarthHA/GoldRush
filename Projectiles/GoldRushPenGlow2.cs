using GoldRush.Buffs;
using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushPenGlow2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.timeLeft = 99999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.light = 0.9f;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Player owner = Main.player[Projectile.owner];

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
            Vector2 OffSet = new Vector2(-25, 0);
            Projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y);
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 10)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = (30 - Projectile.ai[0]) / 20f;
                if (Projectile.ai[0] >= 30)
                {
                    Projectile.Kill();
                }
            }
            
        }
        public void DrawAlt(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRushPen2").Value;
            SpriteEffects SP = Main.player[Projectile.owner].direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale * 0.35f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player owner = Main.player[Projectile.owner];
            if (target.life <= 0)
            {
                if (!GoldRushNPC.HasGoldRushDeathCheck(target))
                {
                    GoldRushPlayer.HeadLife(Main.LocalPlayer, GoldRush.GreedKillHeal, GoldRush.GreedKillHealCD);
                }
            }
            //GoldRushNPC.DeepAddBuff(target, ModContent.BuffType<GreedyDeathCheck>(), 3);
            GoldRushNPC.ApplyGoldRushDeathCheck(target);
            GoldRushPlayer.GiveImmune(owner, GoldRush.GreedImmune);
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.direction > 0)       //125 * 2 120
            {
                hitbox = new Rectangle((int)Projectile.Center.X - 25, (int)Projectile.Center.Y - 60, 150, 120);
            }
            else
            {
                hitbox = new Rectangle((int)Projectile.Center.X - 125, (int)Projectile.Center.Y - 60, 150, 120);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }

        public override bool? CanDamage()
        {
            return Projectile.Opacity == 1;
        }

        public override void CutTiles()
        {
            Player owner = Main.player[Projectile.owner];
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(125 * owner.direction, 0), 120, DelegateMethods.CutTiles);
        }
    }
}