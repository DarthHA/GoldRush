using GoldRush.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Buffs
{
    public class GreedyPower : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Power of Greedy");
            DisplayName.AddTranslation(GameCulture.Chinese, "贪婪之力");
            Description.SetDefault("Your desire will never get satisfied...\n" +
                "Melee damage increased, but defence decreased.");
            Description.AddTranslation(GameCulture.Chinese, "你的欲望永远不会得到满足...\n" +
                "近战伤害增加，但防御力降低。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
            longerExpertDebuff = false;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            GoldRushPlayer modplayer = player.GetModPlayer<GoldRushPlayer>();
            player.meleeDamage += 0.015f * modplayer.GreedyPowerLevel;
            player.statDefense -= 2 * modplayer.GreedyPowerLevel;
        }
        
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            GoldRushPlayer modplayer = player.GetModPlayer<GoldRushPlayer>();
            modplayer.GreedyPowerLevel++;
            if (modplayer.GreedyPowerLevel >= 50)
            {
                player.AddBuff(ModContent.BuffType<BlissPower>(), 99999999);
            }
            return false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            GoldRushPlayer modplayer = Main.LocalPlayer.GetModPlayer<GoldRushPlayer>();
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                tip += "\n当前伤害加成：" + 1.5f * modplayer.GreedyPowerLevel + "%。\n当前防御降低：" + (-2) * modplayer.GreedyPowerLevel + "。";
            }
            else
            {
                tip += "\nCurrent damage bonus: " + 1.5f * modplayer.GreedyPowerLevel + "%.\nCurrent defence reduction: " + (-2) * modplayer.GreedyPowerLevel + ".";
            }
            if (modplayer.GreedyPowerLevel > 25)
            {
                rare = ItemRarityID.Yellow;
            }
        }
    }
}
