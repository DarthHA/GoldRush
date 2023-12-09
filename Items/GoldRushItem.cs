using GoldRush.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GoldRush.Items
{
    public class GoldRushItem : ModItem
    {
        public static int UseLoop = 0;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ItemSlot.ShiftInUse)
            {
                string description = string.Format(Language.GetTextValue("Mods.GoldRush.ItemTooltipExtra2.GoldRushItem"), ModContent.ItemType<GreedDiamond>());
                tooltips.Add(new TooltipLine(Mod, "tooltip", description));
            }
            else
            {
                string description = Language.GetTextValue("Mods.GoldRush.ItemTooltipExtra1.GoldRushItem");
                tooltips.Add(new TooltipLine(Mod, "tooltip", description));
            }
        }

        public override void SetDefaults()
        {
            Item.damage = 1500;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.crit = 25;
            Item.knockBack = 15f;
            Item.value = Item.sellPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<GoldRushPen1>();
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.GetModPlayer<GoldRushPlayer>().UsingGR)
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
            else
            {
                if (player.GetModPlayer<GoldRushPlayer>().GRCD > 0)
                {
                    return false;
                }
                if (GRPortalUtils.PortalCount(player.whoAmI) > 0)
                {
                    if (Main.projectile[GRPortalUtils.GetPortal(player.whoAmI, (int)GRPortalUtils.GetMaxID(player.whoAmI))].Distance(Main.MouseWorld) > 1500)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/Ready"), player.Center);
                int count = 5;
                if (player.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
                {
                    count = 1;
                }
                if (GRPortalUtils.PortalCount(player.whoAmI) >= count)
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
                int protmp = Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<GoldPortal>(), 0, 0, player.whoAmI, GRPortalUtils.GetMaxID(player.whoAmI) + 1);

                Main.projectile[protmp].localAI[0] = dir;
            }
            else
            {
                if (GRPortalUtils.PortalCount(player.whoAmI) > 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/Ready"), player.Center);
                    damage = (int)(damage * GoldRush.DashModifier);
                    if (player.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
                    {
                        damage *= 4;
                    }
                    else
                    {
                        damage += (int)(damage * 0.1f * GRPortalUtils.PortalCount(player.whoAmI));
                    }
                    GoldRushPlayer.InitDash(player);
                    Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushDash>(), 0, 0, player.whoAmI);
                    Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushDashGlow>(), damage, knockback, player.whoAmI);

                }
                else
                {
                    if (player.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Greed)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/Atk2"), player.Center);
                        damage = (int)(damage * GoldRush.GreedDamageModifier);
                        UseLoop++;
                        if (UseLoop > 2)
                        {
                            UseLoop = 0;
                            Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushSlash2>(), damage, knockback, player.whoAmI);
                        }
                        else
                        {
                            Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushPen2>(), damage, knockback, player.whoAmI);
                        }
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("GoldRush/Sounds/Atk1"), player.Center);
                        UseLoop++;
                        if (UseLoop > 3)
                        {
                            UseLoop = 0;
                            Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushSlash1>(), damage, knockback, player.whoAmI);
                        }
                        else
                        {
                            Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<GoldRushPen1>(), damage, knockback, player.whoAmI);
                        }
                    }
                }
            }
            return false;
        }


        public override void HoldItem(Player player)
        {
            bool HasPortalFake = false;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<GoldPortalFake>() && proj.owner == player.whoAmI)
                {
                    HasPortalFake = true;
                    break;
                }
            }
            if (!HasPortalFake && GRPortalUtils.PortalCount(player.whoAmI) > 0) 
            {
                Projectile.NewProjectile(null,player.Center + new Vector2(player.direction * 100, 0), Vector2.Zero, ModContent.ProjectileType<GoldPortalFake>(), 0, 0, player.whoAmI);
            }
            if (GoldRushPlayer.IsSpare(player))
            {
                bool HasProj = false;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<GoldRushReady>() && proj.owner == player.whoAmI)
                    {
                        HasProj = true;
                    }
                }
                if (!HasProj)
                {
                    Projectile.NewProjectile(null, player.Center + new Vector2(player.direction * 10, 0), Vector2.Zero, ModContent.ProjectileType<GoldRushReady>(), 0, 0, player.whoAmI);
                }
            }

            //player.mount.Dismount(player);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Lighting.AddLight(Item.position, 0.4f, 0.4f, 0.1f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Amber, 100);
            recipe.AddIngredient(ItemID.PlatinumBar, 100);
            recipe.AddIngredient(ItemID.GoldBar, 100);
            recipe.AddIngredient(ItemID.PlatinumCoin, 100);
            recipe.AddIngredient(ItemID.GoldCoin, 100);
            recipe.AddIngredient(ItemID.LunarBar, 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}