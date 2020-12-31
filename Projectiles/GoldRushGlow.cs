using GoldRush.Buffs;
using GoldRush.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace GoldRush.Projectiles
{
    public class GoldRushGlow : ModProjectile
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
            projectile.friendly = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 120;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
            projectile.light = 0.3f;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            projectile.penetrate = -1;
            projectile.timeLeft = 99999;
            projectile.friendly = true;
            projectile.hostile = false;
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
            projectile.Center = owner.Center + new Vector2(OffSet.X * owner.direction, OffSet.Y);
            if (modowner.UseGR)
            {
                if (modowner.CurrentState == 0)
                {
                    projectile.hide = true;
                    projectile.alpha = 255;
                }
                else
                {
                    projectile.alpha = 0;
                    projectile.hide = false;
                }
            }
            else
            {
                projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = Main.player[projectile.owner].direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Yellow * projectile.Opacity * 0.3f, 0, tex.Size() / 2, projectile.scale * 2, SP, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(3) == 1 && !target.immortal)
            {
                Main.player[projectile.owner].AddBuff(ModContent.BuffType<GreedyPower>(), 540);
            }
        }
        

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.rand.Next(3) == 1 && !Main.player[projectile.owner].HasBuff(ModContent.BuffType<BlissPower>()))
            {
                Main.player[projectile.owner].AddBuff(ModContent.BuffType<GreedyPower>(), 99999999);
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!crit)
            {
                crit = Main.rand.NextBool();
            }
        }

        public override bool CanDamage()
        {
            return !projectile.hide;
        }
    }
}