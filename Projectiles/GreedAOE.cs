using GoldRush.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Projectiles
{
    public class GreedAOE : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1000;
        }
        public override void AI()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 9999;
            Projectile.position = Main.screenPosition;
            Projectile.width = Main.screenWidth;
            Projectile.height = Main.screenHeight;

            if (Projectile.ai[0] == 0)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[0] = 1;
                }
            }
            else
            {
                Projectile.alpha += 7;
                if (Projectile.alpha > 250)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * Projectile.Opacity);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.realLife != -1)
            {
                NPC owner = Main.npc[target.realLife];
                if (owner.active)
                {
                    int count = SegmentCounts(owner);
                    if (count == 0) count = 1;
                    damage /= (int)Math.Pow(count, 0.666);
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }
        public int SegmentCounts(NPC npc)
        {
            int result = 0;
            Rectangle rectangle = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            foreach (NPC segs in Main.npc)
            {
                if (segs.active && segs.realLife != -1 && segs.realLife == npc.whoAmI && segs.Hitbox.Intersects(rectangle) && !segs.dontTakeDamage)
                {
                    result++;
                }
            }
            return result;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            GoldRushNPC.ApplyGoldRushDeathCheck(target);
            //GoldRushNPC.DeepAddBuff(target, ModContent.BuffType<GreedyDeathCheck>(), 3);
        }
    }
}