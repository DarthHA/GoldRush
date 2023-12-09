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
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.scale = 0.05f;
            Projectile.timeLeft = 99999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center + new Vector2(PortalD / 2, 0), 0.9f, 0.9f, 0.9f);
            Lighting.AddLight(Projectile.Center - new Vector2(PortalD / 2, 0), 0.9f, 0.9f, 0.9f);

            Projectile.timeLeft = 9999;
            Projectile.velocity = Vector2.Zero;
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 4;
            }
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || owner.ghost)
            {
                Projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                Projectile.ai[1] = 1;
            }
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.scale < 1)
                {
                    Projectile.scale += 0.05f;
                }
            }
            else
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale <= 0.05f)
                {
                    Projectile.Kill();
                    return;
                }
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldPortalGlow").Value;
            SpriteEffects SP = SpriteEffects.None;
            Rectangle Frame = new Rectangle(0, Projectile.frame * tex.Height / 4, tex.Width, tex.Height / 4);
            Rectangle Frame2 = new Rectangle(0, (3 - Projectile.frame) * tex2.Height / 4, tex2.Width, tex2.Height / 4);
            Main.spriteBatch.Draw(tex, Projectile.Center - new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame, Color.White * Projectile.Opacity, Projectile.rotation, Frame.Size() / 2, new Vector2(0.2f, 1) * Projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center - new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame2, Color.White * Projectile.Opacity, Projectile.rotation, Frame2.Size() / 2, new Vector2(0.2f, 1) * Projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame, Color.White * Projectile.Opacity, Projectile.rotation, Frame.Size() / 2, new Vector2(0.2f, 1) * Projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center + new Vector2(PortalD / 2, 0) - Main.screenPosition, Frame2, Color.White * Projectile.Opacity, Projectile.rotation, Frame2.Size() / 2, new Vector2(0.2f, 1) * Projectile.scale * 0.5f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }


    }
}