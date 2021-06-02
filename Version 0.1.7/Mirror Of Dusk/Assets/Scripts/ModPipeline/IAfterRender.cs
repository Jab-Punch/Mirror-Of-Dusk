namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IAfterRender
    {
        ScriptableRenderPass GetPassToEnqueue();
    }
}
