using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Items
{
    public class GreedDiamond : ModItem
    {
        public int PickTime = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brilliant Bliss");
            //DisplayName.AddTranslation(GameCulture.Chinese, "幸福的碎片");
            /* Tooltip.SetDefault(
@"You may shouldn't see this
If you see it, drop and pick it up again"); */
            //Tooltip.AddTranslation(GameCulture.Chinese, 
//@"你可能不该看到这个
//如果你看到了，扔掉它然后再捡起来");
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 11;
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = 1;
        }

        public override bool OnPickup(Player player)
        {
           SoundEngine.PlaySound(SoundID.Item29, Item.Center);
            GoldRushPlayer.HeadLife(player, player.statLifeMax2 / 4, -1);
            if (player.GetModPlayer<GoldRushPlayer>().FinalState == GoldRushPlayer.FType.Empty)
            {
                player.GetModPlayer<GoldRushPlayer>().BlissCount++;
                if (player.GetModPlayer<GoldRushPlayer>().BlissCount > 4)
                {
                    player.GetModPlayer<GoldRushPlayer>().BlissCount = 4;
                }
            }
            return false;
        }

        public override bool CanPickup(Player player)
        {
            if (PickTime <= 60)
            {
                return false;
            }
            if (player.GetModPlayer<GoldRushPlayer>().UsingGR)
            {
                return false;
            }
            if (player.GetModPlayer<GoldRushPlayer>().ScreenLocked > 0)
            {
                return false;
            }
            return true;
        }

        public override bool ItemSpace(Player player)
        {
            return true;
        }
        public override void UpdateInventory(Player player)
        {
            PickTime = 0;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Lighting.AddLight(Item.position, 0.4f, 0.4f, 0.1f);
            gravity = 0f;
            float Dist = 16 * 6f;
            int me = -1;

            if (++PickTime > 60)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead && player.Distance(Item.Center) < Dist && CanPickup(player))
                    {
                        Dist = player.Distance(Item.Center);
                        me = i;
                    }
                }
            }
            if (me != -1)
            {
                Item.velocity = Vector2.Lerp(Item.velocity, Item.DirectionTo(Main.player[me].Center) * 7f, 0.05f);
                Item.beingGrabbed = true;
            }
            else
            {
                Item.velocity *= 0.95f;
            }
        }
    }
}