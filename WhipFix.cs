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
            // �жϴ��붼��һ���ģ�ֱ����ͬһ��IL�༭
            IL.Terraria.Projectile.AI_165_Whip += WhipPatch;
            IL.Terraria.Projectile.AI_019_Spears += WhipPatch;
        }

        // tML�����itemAnimationΪ0��1ʱ���ж���һ�γ��֣������������ӣ��պ�ʹ���е�SmartSelectLookup��ʱ��û��itemAnimationΪ0�Ŀյ����ˣ��б�Ҳ��������
        private void WhipPatch(ILContext il) {
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