using CodeX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System.Reflection;

[assembly: AssemblyTitle("DefaultToAnisotropic")]
[assembly: AssemblyProduct("DefaultToAnisotropic")]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]

class DefaultToAnisotropic : NeosMod
{
    public override string Name => "DefaultToAnisotropic";
    public override string Author => "Toxic_Cookie";
    public override string Version => "1.1.0";
    public override string Link => "https://github.com/Toxic-Cookie/DefaultToAnisotropic";

    // configurations for StaticTextureProvider<Texture2D, Bitmap2D, BitmapMetadata, Texture2DVariantDescriptor>
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<TextureFilterMode?> FilterMode = new("FilterMode", "The type of filtering to use", () => TextureFilterMode.Anisotropic);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<int?> AnisotropicLevel = new("AnisotropicLevel", "The level of anisotropy to use when FilterMode is set to Anisotropic", () => 16);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> Uncompressed = new("Uncompressed", "Do not use texture compression", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> DirectLoad = new("DirectLoad", "Bypass variant generation system and just decode the texture and generate mipmaps on the fly", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> ForceExactVariant = new("ForceExactVariant", "Force texture to load specifically requested variant, without using a fallback version", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<TextureCompression?> PreferredFormat = new("PreferredFormat", "Allows overriding the preferred asset variant to load", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<float?> MipMapBias = new("MipMapBias", "Offset to the MipMap calculation", () => null);

    // configurations for StaticTexture2D
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> IsNormalMap = new("IsNormalMap", "The texture should be interpreted as a Normal Map", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<TextureWrapMode?> WrapModeU = new("WrapModeU", "How to handle U values outside of the range [0.0,1.0] ", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<TextureWrapMode?> WrapModeV = new("WrapModeV", "How to handle V values outisde of the range [0.0,1.0] ", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<float?> PowerOfTwoAlignThreshold = new("PowerOfTwoAlignThreshold", "This will automatically align the requested size to nearest power of two if it's within a given percentange (default 5%)", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> CrunchCompressed = new("CrunchCompressed", "Enable crunch compression", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<int?> MaxSize = new("MaxSize", "Maximum size to request from the asset variant system", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> MipMaps = new("MipMaps", "Enable mipmaps", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<Filtering?> MipMapFilter = new("MipMapFilter", "Algorithm used to generate mipmap levels", () => null);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool?> Readable = new("Readable", "Store texture in a format that supports CPU reads", () => null);

    private static ModConfiguration? config;

    public override void OnEngineInit()
    {
        config = GetConfiguration();
        Harmony harmony = new Harmony("net.Toxic_Cookie.DefaultToAnisotropic");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(ImageImporter), nameof(ImageImporter.SetupTextureProxyComponents))]
    private static class Patch
    {
        private static void Postfix(IAssetProvider<Texture2D> texture)
        {
            StaticTexture2D textureprovider = (StaticTexture2D)texture;

            ApplySetting(FilterMode, textureprovider.FilterMode);
            ApplySetting(AnisotropicLevel, textureprovider.AnisotropicLevel);
            ApplySetting(Uncompressed, textureprovider.Uncompressed);
            ApplySetting(DirectLoad, textureprovider.DirectLoad);
            ApplySetting(ForceExactVariant, textureprovider.ForceExactVariant);
            ApplySetting(PreferredFormat, textureprovider.PreferredFormat);
            ApplySetting(MipMapBias, textureprovider.MipMapBias);

            ApplySetting(IsNormalMap, textureprovider.IsNormalMap);
            ApplySetting(WrapModeU, textureprovider.WrapModeU);
            ApplySetting(WrapModeV, textureprovider.WrapModeV);
            ApplySetting(PowerOfTwoAlignThreshold, textureprovider.PowerOfTwoAlignThreshold);
            ApplySetting(CrunchCompressed, textureprovider.CrunchCompressed);
            ApplySetting(MaxSize, textureprovider.MaxSize);
            ApplySetting(MipMaps, textureprovider.MipMaps);
            ApplySetting(MipMapFilter, textureprovider.MipMapFilter);
            ApplySetting(Readable, textureprovider.Readable);
        }
    }

    // handle when the sync is non-nullable
    private static void ApplySetting<T>(ModConfigurationKey<T?> configKey, Sync<T> sync) where T : struct
    {
        if (config!.GetValue(configKey) is T setting)
        {
            sync.Value = setting;
        }
    }

    // handle when the sync is nullable
    private static void ApplySetting<T>(ModConfigurationKey<T?> configKey, Sync<T?> sync) where T : struct
    {
        if (config!.GetValue(configKey) is T setting)
        {
            sync.Value = setting;
        }
    }
}
