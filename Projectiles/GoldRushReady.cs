﻿using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushReady : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
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
            if (!GoldRushPlayer.IsSpare(owner))
            {
                Projectile.Kill();
                return;
            }
            owner.heldProj = Projectile.whoAmI;
            Vector2 offset = new Vector2(0, 5);
            if (owner.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
            {
                offset = new Vector2(0, -5);
            }
            Projectile.Center = owner.Center + new Vector2(offset.X * owner.direction, offset.Y * Math.Sign(owner.gravDir));
            if (owner.mount.Active)
            {
                Projectile.Center = owner.MountedCenter + new Vector2(offset.X * owner.direction, offset.Y * Math.Sign(owner.gravDir));
            }
            if (modowner.UsingGR)
            {
                Projectile.Kill();
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
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * (1 - owner.immuneAlpha / 255f), rot, tex.Size() / 2, Projectile.scale * scale, SP, 0);
            return false;
        }


    }
}