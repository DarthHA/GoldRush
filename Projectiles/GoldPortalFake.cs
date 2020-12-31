using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using GoldRush;
using System;

namespace GoldRush.Projectiles
{
    public class GoldPortalFake : ModProjectile
    {
        public override string Texture => "GoldRush/Projectiles/GoldPortal";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold Portal");
            DisplayName.AddTranslation(GameCulture.Chinese, "闪金传送门");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.scale = 0.05f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.light = 0.3f;
        }
        public override void AI()
        {
            projectile.timeLeft = 9999;
            projectile.velocity = Vector2.Zero;
            if (++projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 4;
            }
            Player owner = Main.player[projectile.owner];
            GoldRushPlayer modowner = owner.GetModPlayer<GoldRushPlayer>();
            if (!owner.active || owner.dead || owner.ghost)
            {
                projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                projectile.ai[1] = 1;
            }
            if (GRPortalUtils.PortalCount(owner.whoAmI) <= 0)
            {
                projectile.ai[1] = 1;
            }
            if (projectile.ai[1] == 0)
            {
                if (projectile.scale < 1)
                {
                    projectile.scale += 0.05f;
                }
            }
            else
            {
                projectile.scale -= 0.05f;
                if (projectile.scale <= 0.05f)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (modowner.UseGR)
            {
                projectile.localAI[0] = 1;
                projectile.velocity = Vector2.Zero;
            }
            else
            {
                if (projectile.localAI[0] != 1)
                {
                    projectile.Center = owner.Center + new Vector2(owner.direction * 100, 0);
                }
                else
                {
                    projectile.velocity = Vector2.Zero;
                }
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = SpriteEffects.None;
            Rectangle Frame = new Rectangle(0, projectile.frame * tex.Height / 4, tex.Width, tex.Height / 4);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.White * projectile.Opacity, projectile.rotation, Frame.Size() / 2, new Vector2(0.2f, 1) * projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix); 
            return false;
        }


    }
}