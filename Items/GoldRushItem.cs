using GoldRush.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Items
{
    public class GoldRushItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold Rush");
            DisplayName.AddTranslation(GameCulture.Chinese, "闪金冲锋");
            Tooltip.SetDefault("\"The weapon she used to brandish in her prime, before the greed solidified and became what it is now.\n" +
                "One can release their primal desires and strike enemies with full force; technical skill is unneeded.\n" +
                "Each clout with the weapon gives a satisfying sensation to the wielder, but one must be careful not to get carried away.\"\n\n" +
                "Right click to create up to six pairs of portals and left click to attack.\n" +
                "Every hit has a chance to grant a buff that increases your melee damage by 1.5% and decreases your defence by 2.\n" +
                "When switching to other weapons, this buff will disappear.\n" +
                "When you hit enemies enough times, Gold Rush will undergo a qualitative change.");
            Tooltip.AddTranslation(GameCulture.Chinese, "“这支拳套从一处充斥着无尽贪婪的时空而来，它能够令你全力抨击你的敌人。\n" +
                "它是基于人类最原始的欲望而运作的，所以操作它不需要特别的技巧。\n" +
                "使用这只武器时的感觉十分轻松自如。\n" +
                "但是你也很容易被这只拳套的力量带动身体。”\n\n" +
                "右键能制造出至多6对传送门，左键攻击\n" +
                "每击中一次都有几率得到一个效果，该效果会使你近战伤害增加1.5%，但防御减少2\n" +
                "当切换武器时该效果消失。\n" +
                "当击中敌人足够次数后，闪金冲锋将会发生质变。");
        }

        public override void SetDefaults()
        {
            item.damage = 1400;
            item.melee = true;
            item.width = 50;
            item.height = 32;
            item.scale = 0.5f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 35;
            item.value = Item.sellPrice(0, 20, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item44;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<GoldRushProj>();
            item.holdStyle = ItemHoldStyleID.HoldingOut;
            
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.GetModPlayer<GoldRushPlayer>().GRCD > 0)
            {
                return false;
            }
            if (player.altFunctionUse != 2)
            {
                if (GRPortalUtils.PortalCount(player.whoAmI) <= 0 || player.GetModPlayer<GoldRushPlayer>().UseGR)
                {
                    return false;
                }

                if (GRPortalUtils.PortalCount(player.whoAmI) > 0)
                {
                    if (Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, (int)GRPortalUtils.GetMaxID(player.whoAmI))].Distance(player.Center) > 1500)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (GRPortalUtils.PortalCount(player.whoAmI) > 0)
                {
                    if (Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, (int)GRPortalUtils.GetMaxID(player.whoAmI))].Distance(Main.MouseWorld) > 1500)
                    {
                        return false;
                    }
                }
                if (GRPortalUtils.PortalCount(player.whoAmI) >= 6)
                {
                    Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, 0)].ai[1] = 1;
                    GRPortalUtils.PullProjID(player.whoAmI);
                }
                int dir;
                if (GRPortalUtils.PortalCount(player.whoAmI) <= 0)
                {
                    dir = Math.Sign(Main.MouseWorld.X - player.Center.X);
                }
                else
                {
                    dir = Math.Sign(Main.MouseWorld.X - Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, (int)GRPortalUtils.GetMaxID(player.whoAmI))].Center.X);
                }
                if (dir == 0)
                {
                    dir = Main.rand.Next(2) * 2 - 1;
                }
                int protmp = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<GoldPortal>(), 0, 0, player.whoAmI, GRPortalUtils.GetMaxID(player.whoAmI) + 1);
                
                Main.projectile[protmp].localAI[0] = dir;
            }
            else
            {
                player.GetModPlayer<GoldRushPlayer>().ScreenPos = player.Center;
                player.GetModPlayer<GoldRushPlayer>().CurrentPortal = 0;
                player.GetModPlayer<GoldRushPlayer>().CurrentState = 0;
                player.GetModPlayer<GoldRushPlayer>().UseGR = true;
                player.GetModPlayer<GoldRushPlayer>().ScreenLocked = 120;
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushProj>(), 0, 0, player.whoAmI);
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushGlow>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            bool HasPortalFake = false;
            bool HasGRHeld = false;
            foreach(Projectile proj in Main.projectile)
            {
                if(proj.active && proj.type == ModContent.ProjectileType<GoldPortalFake>() && proj.owner == player.whoAmI)
                {
                    HasPortalFake = true;
                }
                if (proj.active && proj.type == ModContent.ProjectileType<GoldRushHeld>() && proj.owner == player.whoAmI)
                {
                    HasGRHeld = true;
                }
            }
            if (!HasPortalFake && GRPortalUtils.PortalCount(player.whoAmI) > 0) 
            {
                Projectile.NewProjectile(player.Center + new Vector2(player.direction * 100, 0), Vector2.Zero, ModContent.ProjectileType<GoldPortalFake>(), 0, 0, player.whoAmI);
            }
            if (!HasGRHeld && !player.GetModPlayer<GoldRushPlayer>().UseGR)
            {
                Projectile.NewProjectile(player.Center + new Vector2(player.direction * 10, 0), Vector2.Zero, ModContent.ProjectileType<GoldRushHeld>(), 0, 0, player.whoAmI);
            }

            if (player.mount.Active && player.mount._data.Minecart)
            {
                player.mount.Dismount(player);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Amber, 100);
            recipe.AddIngredient(ItemID.PlatinumBar, 100);
            recipe.AddIngredient(ItemID.GoldBar, 100);
            recipe.AddIngredient(ItemID.PlatinumCoin, 100);
            recipe.AddIngredient(ItemID.GoldCoin, 100);
            recipe.AddIngredient(ItemID.LunarBar, 100);
            recipe.SetResult(this);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddRecipe();
        }
    }
}