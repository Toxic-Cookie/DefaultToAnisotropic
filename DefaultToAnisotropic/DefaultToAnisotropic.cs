using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using NeosModLoader;
using FrooxEngine;

namespace LogixScript.Core
{
    class DefaultToAnisotropic : NeosMod
    {
        public override string Name => "DefaultToAnisotropic";
        public override string Author => "Toxic_Cookie";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/Toxic-Cookie/DefaultToAnisotropic";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.Toxic_Cookie.DefaultToAnisotropic");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ImageImporter), nameof(ImageImporter.SetupTextureProxyComponents))]
        static class Patch
        {
            static void Postfix(IAssetProvider<Texture2D> texture)
            {
                StaticTexture2D textureprovider = (StaticTexture2D)texture;
                textureprovider.FilterMode.Value = TextureFilterMode.Anisotropic;
            }
        }
    }
}
