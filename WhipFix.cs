using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;

namespace WhipFix
{
    public class WhipFix : Mod
    {
        public override void Load() {
            base.Load();
            // 判断代码都是一样的，直接用同一个IL编辑
            IL.Terraria.Projectile.AI_165_Whip += WhipPatch;
            IL.Terraria.Projectile.AI_019_Spears += WhipPatch;
        }

        // tML大概是itemAnimation为0和1时各判断了一次出手，丢了两个鞭子，刚好使运行到SmartSelectLookup的时候没有itemAnimation为0的空挡期了，切鞭也不能用了
        private void WhipPatch(ILContext il) {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, i => i.MatchLdloc(0));
            c.GotoNext(MoveType.After, i => i.MatchLdfld(typeof(Player), nameof(Player.itemAnimation)));
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<int, Player, int>>((returnValue, player) => {
                if (returnValue == 1) {
                    player.itemAnimation = 0; // 设为0才会在SmartSelectLookup里切换selectedItem
                    return 0;
                }
                return returnValue;
            });
        }
    }
}