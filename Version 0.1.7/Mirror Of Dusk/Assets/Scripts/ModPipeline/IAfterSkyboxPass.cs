namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IAfterSkyboxPass
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle, RenderTargetHandle depthHandle);
    }
}