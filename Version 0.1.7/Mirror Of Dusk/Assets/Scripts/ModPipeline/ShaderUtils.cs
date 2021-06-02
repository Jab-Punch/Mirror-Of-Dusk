namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public enum ShaderPathID
    {
        PhysicallyBased,
        SimpleLit,
        Unlit,
        TerrainPhysicallyBased,
        ParticlesPhysicallyBased,
        ParticlesSimpleLit,
        ParticlesUnlit,
        Count
    }

    public static class ShaderUtils
    {
        static readonly string[] s_ShaderPaths =
        {
            "Mod Render Pipeline/Lit",
            "Mod Render Pipeline/Simple Lit",
            "Mod Render Pipeline/Unlit",
            "Mod Render Pipeline/Terrain/Lit",
            "Mod Render Pipeline/Particles/Lit",
            "Mod Render Pipeline/Particles/Simple Lit",
            "Mod Render Pipeline/Particles/Unlit",
        };

        public static string GetShaderPath(ShaderPathID id)
        {
            int index = (int)id;
            if (index < 0 && index >= (int)ShaderPathID.Count)
            {
                Debug.LogError("Trying to access Mod shader path out of bounds");
                return "";
            }

            return s_ShaderPaths[index];
        }
    }
}
