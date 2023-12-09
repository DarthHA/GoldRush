using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Buffs
{
    public class VictoryPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;

        }




        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            GoldRushPlayer modplayer = Main.LocalPlayer.GetModPlayer<GoldRushPlayer>();
            tip += string.Format(Language.GetTextValue("Mods.GoldRush.Misc.VictoryPowerExtra"), modplayer.VictoryPowerLevel * 2f, modplayer.BlissCount);


            if (modplayer.BlissCount >= 4)
            {
                rare = ItemRarityID.Yellow;
            }
            else if (modplayer.VictoryPowerLevel > 25)
            {
                rare = ItemRarityID.LightRed;
            }
        }
    }
}
