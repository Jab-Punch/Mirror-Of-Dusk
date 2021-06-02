namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IAfterTransparentPass
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle, RenderTargetHandle depthHandle);
    }
}