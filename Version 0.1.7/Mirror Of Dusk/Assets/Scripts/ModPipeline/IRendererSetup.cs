namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IRendererSetup
    {
        void Setup(ScriptableRenderer renderer, ref RenderingData renderingData);
    }
}