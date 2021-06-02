namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IAfterDepthPrePass
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle);
    }
}
