namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public interface IAfterOpaquePass
    {
        ScriptableRenderPass GetPassToEnqueue(
            RenderTextureDescriptor baseDescriptor,
            RenderTargetHandle colorAttachmentHandle,
            RenderTargetHandle depthAttachmentHandle);
    }
}