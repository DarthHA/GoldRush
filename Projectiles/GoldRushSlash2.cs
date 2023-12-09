using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushSlash2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.6f;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
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
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            Vector2 OffSet = new Vector2(10, -8);
            Projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y * Math.Sign(owner.gravDir));
            if (owner.mount.Active)
            {
                Projectile.Center = owner.MountedCenter + new Vector2(OffSet.X * owner.direction, OffSet.Y * Math.Sign(owner.gravDir));
            }
            if (modowner.UsingGR)
            {
                Projectile.Kill();
            }
            Projectile.ai[0]++;
            float rot = -MathHelper.Pi / 2 + MathHelper.Pi / 6 * owner.direction;
            if (owner.gravDir < 0)
            {
                rot = new Vector2(rot.ToRotationVector2().X, -rot.ToRotationVector2().Y).ToRotation();
            }
            owner.itemRotation = (float)Math.Atan2(rot.ToRotationVector2().Y * owner.direction, rot.ToRotationVector2().X * owner.direction);
            if (Projectile.ai[0] <= 4)
            {
                Projectile.Center += (Projectile.ai[0] / 4f * 20f - 5) * rot.ToRotationVector2();
            }
            else
            {
                Projectile.Center += 15f * rot.ToRotationVector2();
            }
            if (Projectile.ai[0] == 1)
            {
                //Projectile.NewProjectile(null,owner.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushPenGlow1>(), Projectile.damage, 0, owner.whoAmI);
                Projectile.NewProjectile(null,owner.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushSlashGlow2>(), Projectile.damage, 0, owner.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Projectiles/GoldRush2").Value;
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rot = 0;
            if (owner.gravDir < 0)
            {
                rot += MathHelper.Pi;
                SP = owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            if (Projectile.ai[0] <= 4)
            {
                rot += -MathHelper.Pi / 5 * owner.direction * Projectile.ai[0] / 5f;
            }
            else
            {
                rot += -MathHelper.Pi / 5 * owner.direction;
            }
            if (owner.gravDir < 0)
            {
                rot = new Vector2(rot.ToRotationVector2().X, -rot.ToRotationVector2().Y).ToRotation();
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * (1 - owner.immuneAlpha / 255f), rot, tex.Size() / 2, Projectile.scale * 0.18f, SP, 0);

            foreach (Projectile glow in Main.projectile)
            {
                if (glow.active && glow.type == ModContent.ProjectileType<GoldRushSlashGlow2>() && glow.owner == Projectile.owner)
                {
                    (glow.ModProjectile as GoldRushSlashGlow2).DrawAlt(Main.spriteBatch);
                }
            }
            return false;
        }


    }
}