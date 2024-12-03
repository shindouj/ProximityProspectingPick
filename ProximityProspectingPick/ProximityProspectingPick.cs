using HarmonyLib;
using ProximityProspectingPick.ModConfiguration;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace ProximityProspectingPick;

public class ProximityProspectingPick : ModSystem {
    private const string PropickFixId = "propickfix";
    private const string ToolModesKey = "proPickToolModes";
    private const string ProspectingPickNodeRadiusKey = "propickNodeSearchRadius";
    private Harmony? _harmony;

    public const string ModId = "proximityprospectingpick";
    public static ICoreAPI Api = null!;
    public static ModConfig ModConfig = null!;
    public static List<SkillItem> SkillItems = new();
    
    public override double ExecuteOrder() => 0.0000001;

    public override void Start(ICoreAPI api) {
        base.Start(api);
        Api = api;

        if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
            _harmony = new Harmony(Mod.Info.ModID);
            _harmony.PatchAll();
        }

        ModConfig = api.LoadModConfig<ModConfig>("proximityProspectingPick.json");

        if (ModConfig == null) {
            ModConfig = new ModConfig();
            api.StoreModConfig(ModConfig, "proximityProspectingPick.json");
        }
    }

    public override void AssetsLoaded(ICoreAPI api) {
        ObjectCacheUtil.Delete(api, ToolModesKey);
        SkillItems = GetSkillItems(api);
        ObjectCacheUtil.GetOrCreate(api, ToolModesKey, () => SkillItems.ToArray());
    }

    private List<SkillItem> GetSkillItems(ICoreAPI api) {
        ICoreClientAPI? capi = api as ICoreClientAPI;
        var retVal = new List<SkillItem>();

        retVal.Add(GetDensitySkillItem(capi));
        if (api.ModLoader.GetMod(PropickFixId) != null) {
            retVal.Add(GetPropickFixSkillItem(capi));
        }
        if (api.World.Config.GetString(ProspectingPickNodeRadiusKey).ToInt() > 0) {
            retVal.Add(GetNodeSkillItem(capi));
        }
        retVal.Add(GetProximitySkillItem(capi));

        return retVal;
    }

    private SkillItem GetDensitySkillItem(ICoreClientAPI? capi) => 
        GetSkillItem(capi, code: "density", name: Lang.Get("Density Search Mode (Long range, chance based search)"), texturePath: "textures/icons/heatmap.svg");

    private SkillItem GetPropickFixSkillItem(ICoreClientAPI? capi) =>
        GetSkillItem(capi, code: "core", name: Lang.Get("Core Sample Mode (Searches in a straight line)"), assetLocation: PropickFixId, texturePath: "textures/icons/coresample.svg");
    
    private SkillItem GetNodeSkillItem(ICoreClientAPI? capi) =>
        GetSkillItem(capi, code: "node", name: Lang.Get("Node Search Mode (Short range, exact search)"), texturePath: "textures/icons/rocks.svg");

    private SkillItem GetProximitySkillItem(ICoreClientAPI? capi) => 
        GetSkillItem(capi, code: "proximity", name: Lang.Get(ModId + ":search-mode"), texturePath: "textures/icons/worldmap/spiral.svg");

    private static SkillItem GetSkillItem(ICoreClientAPI? capi, string code, string name, string texturePath, string? assetLocation = null) {
        return new SkillItem() {
            Code = new AssetLocation(code),
            Name = Lang.Get(name)
        }.Also(
            it => {
                it.WithIcon(capi, capi?.Gui?.LoadSvgWithPadding(
                    loc: assetLocation != null ? new AssetLocation(assetLocation, texturePath) : new AssetLocation(texturePath),
                    textureWidth: 48,
                    textureHeight: 48,
                    padding: 5,
                    color: ColorUtil.WhiteArgb)
                );
                it.TexturePremultipliedAlpha = false;
            });
    }
}