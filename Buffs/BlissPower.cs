using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GoldRush.Buffs
{
    public class BlissPower : ModBuff
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
            rare = ItemRarityID.Yellow;
        }
    }
}
