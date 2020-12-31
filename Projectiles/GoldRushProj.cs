using GoldRush.Buffs;
using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushProj : ModProjectile
    {
        public static readonly Vector2 OffSet = new Vector2(10, 5);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold Rush");
            DisplayName.AddTranslation(GameCulture.Chinese, "闪金 冲锋");

        }

        public override void SetDefaults()
        {
            projectile.width = 150;
            projectile.height = 150;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            projectile.timeLeft = 9999;
            Player owner = Main.player[projectile.owner];
            GoldRushPlayer modowner = owner.GetModPlayer<GoldRushPlayer>();
            if (!owner.active || owner.dead || owner.ghost)
            {
                projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<GoldRushItem>())
            {
                projectile.Kill();
                return;
            }
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.heldProj = projectile.whoAmI;
            projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y * Math.Sign(owner.gravDir));

            if (modowner.UseGR && modowner.CurrentState == 0)
            {
                owner.heldProj = -1;
                projectile.hide = true;
            }
            else
            {
                owner.heldProj = projectile.whoAmI;
                projectile.hide = false;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player owner = Main.player[projectile.owner];

            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rot = 0;
            if (owner.gravDir < 0)
            {
                rot += MathHelper.Pi;
                SP = owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            spriteBatch.Draw(tex, projectile.Center - new Vector2(owner.direction * 40, 0) - Main.screenPosition, null, lightColor * 0.3f, rot, tex.Size() / 2, projectile.scale * 0.4f, SP, 0);
            spriteBatch.Draw(tex, projectile.Center - new Vector2(owner.direction * 20, 0) - Main.screenPosition, null, lightColor * 0.3f, rot, tex.Size() / 2, projectile.scale * 0.4f, SP, 0);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, rot, tex.Size() / 2, projectile.scale * 0.4f, SP, 0);
            return false;
        }


    }
}