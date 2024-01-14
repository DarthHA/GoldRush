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
    public class GoldRushPenGlow1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.timeLeft = 99999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
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

            Vector2 OffSet = new Vector2(-25, 5);
            Projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y);
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 3)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = (10 - Projectile.ai[0]) / 7f;
                if (Projectile.ai[0] >= 10)
                {
                    Projectile.Kill();
                }
            }
            
        }

        public void DrawAlt(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRushPen1").Value;
            SpriteEffects SP = Main.player[Projectile.owner].direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale * 0.25f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Empty)
            {
                if (Main.rand.NextBool(3)&& !target.immortal)
                {
                    GoldRushPlayer.AddPower(owner, target.Center);
                }
            }
            else if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Bliss)
            {
                if (!target.immortal && !target.friendly && target.life > 5)
                {
                    GoldRushPlayer.HeadLife(owner, GoldRush.BlissHeal, GoldRush.BlissHealCD);
                    owner.AddBuff(ModContent.BuffType<BlissSpeed>(), GoldRush.BlissSpeedTime);
                }
            }
            GoldRushPlayer.GiveImmune(owner, GoldRush.NormalImmune);
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;
        }


        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.direction > 0)       //90 * 2 100
            {
                hitbox = new Rectangle((int)Projectile.Center.X - 20, (int)Projectile.Center.Y - 50, 110, 100);
            }
            else
            {
                hitbox = new Rectangle((int)Projectile.Center.X - 90, (int)Projectile.Center.Y - 50, 110, 100);
            }
        }

        public override bool? CanDamage()
        {
            return Projectile.Opacity == 1;
        }

        public override void CutTiles()
        {
            Player owner = Main.player[Projectile.owner];
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(90 * owner.direction, 0), 100, DelegateMethods.CutTiles);
        }
    }
}