using GoldRush.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Buffs
{
    public class BlissPower : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Power of Bliss");
            DisplayName.AddTranslation(GameCulture.Chinese, "幸福之力");
            Description.SetDefault("You hold the bliss tightly..." +
                "\nMelee damage increased, remove defense reduction effect.\n" +
                "You melee attack will heal yourself.\n" +
                "Gold Rush cooldown decreased.");
            Description.AddTranslation(GameCulture.Chinese, "你紧握着手中的幸福...\n" +
                "近战伤害增加，移除防御力降低效果。\n" +
                "你的近战攻击会治疗自己。\n" +
                "闪金冲锋冷却时间降低。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
            longerExpertDebuff = false;

        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Yellow;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.meleeDamage += 0.75f;  
        }



    }
}
