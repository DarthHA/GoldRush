using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushDash : ModProjectile   //35%
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.timeLeft = 99999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.6f;
        }
        public override void AI()
        {
            Projectile.timeLeft = 9999;
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
            owner.itemTime = 30;
            owner.itemAnimation = 30;
            owner.heldProj = Projectile.whoAmI;
            Vector2 OffSet = new Vector2(10, 5);
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                OffSet = new Vector2(10, -5);
            }
            Projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y * Math.Sign(owner.gravDir));

            if (modowner.CurrentState == GoldRushPlayer.GRState.TPing)
            {
                owner.heldProj = -1;
                Projectile.hide = true;
            }
            else
            {
                owner.heldProj = Projectile.whoAmI;
                Projectile.hide = false;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            float scale = 0.4f;
            Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRush1").Value;
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRush2").Value;
                scale = 0.18f;
            }
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rot = 0;
            if (owner.gravDir < 0)
            {
                rot += MathHelper.Pi;
                SP = owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - new Vector2(owner.direction * 40, 0) - Main.screenPosition, null, lightColor * 0.3f, rot, tex.Size() / 2, Projectile.scale * scale, SP, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - new Vector2(owner.direction * 20, 0) - Main.screenPosition, null, lightColor * 0.3f, rot, tex.Size() / 2, Projectile.scale * scale, SP, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, rot, tex.Size() / 2, Projectile.scale * scale, SP, 0);

            foreach (Projectile glow in Main.projectile)
            {
                if (glow.active && glow.type == ModContent.ProjectileType<GoldRushDashGlow>() && glow.owner == Projectile.owner)
                {
                    (glow.ModProjectile as GoldRushDashGlow).DrawAlt(Main.spriteBatch);
                }
            }
            return false;
        }


    }
}