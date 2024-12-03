using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace ProximityProspectingPick;

[HarmonyPatch]
public class ProspectingPickPatch {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemProspectingPick), "OnBlockBrokenWith")]
    // ReSharper disable twice InconsistentNaming
    public static bool OnBlockBrokenWith(ItemProspectingPick __instance, ref bool __result, IWorldAccessor world, Entity byEntity, ItemSlot itemslot,
        BlockSelection blockSel, float dropQuantityMultiplier) {
        var toolMode = __instance.GetToolMode(itemslot, (byEntity as EntityPlayer)?.Player, blockSel);
        
        if (toolMode != ProximityProspectingPick.SkillItems.Count - 1) {
            return true;
        }

        ProbeBlockProximityMode(world, byEntity, blockSel, ProximityProspectingPick.ModConfig.PropickProximitySearchRadius);
        __result = true;
        return false;
    }

    private static void ProbeBlockProximityMode(IWorldAccessor world, Entity byEntity, BlockSelection blockSel, int radius) {
        IPlayer? byPlayer = null;
        
        if (byEntity is EntityPlayer) {
            byPlayer = world.PlayerByUid(((EntityPlayer) byEntity).PlayerUID);
        }

        Block block = world.BlockAccessor.GetBlock(blockSel.Position);
        block.OnBlockBroken(world, blockSel.Position, byPlayer, 0);
        
        IServerPlayer? serverPlayer = byPlayer as IServerPlayer;
        
        if (!block.IsPropickable() || serverPlayer == null) {
            return;
        }

        BlockPos pos = blockSel.Position.Copy();
        int closestOre = -1;

        ProximityProspectingPick.Api.World.BlockAccessor.WalkBlocks(pos.AddCopy(radius, radius, radius),
            pos.AddCopy(-radius, -radius, -radius), (nblock, x, y, z) => {
                if (nblock.BlockMaterial == EnumBlockMaterial.Ore && nblock.Variant.ContainsKey("type")) {
                    var distanceTo = (int)Math.Round(pos.DistanceTo(x, y, z));

                    if (closestOre == -1 || closestOre > distanceTo) {
                        closestOre = distanceTo;
                    }
                }
            });

        if (closestOre != -1) {
            serverPlayer.SendMessage(
                GlobalConstants.InfoLogChatGroup,
                Lang.GetL(serverPlayer.LanguageCode, ProximityProspectingPick.ModId + ":ore-found", closestOre),
                EnumChatType.Notification
            );
        } else {
            serverPlayer.SendMessage(
                GlobalConstants.InfoLogChatGroup,
                Lang.GetL(serverPlayer.LanguageCode, ProximityProspectingPick.ModId + ":ore-not-found", radius),
                EnumChatType.Notification
            );
        }
    }
}