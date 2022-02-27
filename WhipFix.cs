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
            IL.Terraria.Projectile.AI_165_Whip += Projectile_AI_165_Whip;
        }

        // tML�����itemAnimationΪ0��1ʱ���ж���һ�γ��֣������������ӣ��պ�ʹ���е�SmartSelectLookup��ʱ��û��itemAnimationΪ0�Ŀյ����ˣ��б�Ҳ��������
        private void Projectile_AI_165_Whip(ILContext il) {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, i => i.MatchLdloc(0));
            c.GotoNext(MoveType.After, i => i.MatchLdfld(typeof(Player), nameof(Player.itemAnimation)));
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<int, Player, int>>((returnValue, player) => {
                if (returnValue == 1) {
                    player.itemAnimation = 0; // ��Ϊ0�Ż���SmartSelectLookup���л�selectedItem
                    return 0;
                }
                return returnValue;
            });
        }
    }
}