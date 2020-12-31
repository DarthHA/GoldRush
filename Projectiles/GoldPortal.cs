using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldPortal : ModProjectile
    {
        public static readonly int PortalD = 600;
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
            if (!owner.active || owner.dead || owner.ghost)
            {
                projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<GoldRushItem>())
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


        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = SpriteEffects.None;
            Rectangle Frame = new Rectangle(0, projectile.frame * tex.Height / 4, tex.Width, tex.Height / 4);
            spriteBatch.Draw(tex, projectile.Center - new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame, Color.White * projectile.Opacity, projectile.rotation, Frame.Size() / 2, new Vector2(0.2f, 1) * projectile.scale * 0.5f, SP, 0);
            spriteBatch.Draw(tex, projectile.Center + new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame, Color.White * projectile.Opacity, projectile.rotation, Frame.Size() / 2, new Vector2(0.2f, 1) * projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }


    }
}