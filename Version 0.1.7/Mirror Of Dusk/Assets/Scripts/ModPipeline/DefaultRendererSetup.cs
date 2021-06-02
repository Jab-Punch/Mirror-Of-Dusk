﻿using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    internal class DefaultRendererSetup : IRendererSetup
    {
         private DepthOnlyPass m_DepthOnlyPass;
         private MainLightShadowCasterPass m_MainLightShadowCasterPass;
         private AdditionalLightsShadowCasterPass m_AdditionalLightsShadowCasterPass;
         private SetupForwardRenderingPass m_SetupForwardRenderingPass;
         //private ScreenSpaceShadowResolvePass m_ScreenSpaceShadowResolvePass;
         private CreateModRenderTexturesPass m_CreateModRenderTexturesPass;
         private BeginXRRenderingPass m_BeginXrRenderingPass;
         private SetupModConstanstPass m_SetupModConstants;
         private RenderOpaqueForwardPass m_RenderOpaqueForwardPass;
         //private OpaquePostProcessPass m_OpaquePostProcessPass;
         private DrawSkyboxPass m_DrawSkyboxPass;
         private CopyDepthPass m_CopyDepthPass;
         private CopyColorPass m_CopyColorPass;
         private HSBPass m_CopyHSBPass;
         private RenderTransparentForwardPass m_RenderTransparentForwardPass;
         private TransparentPostProcessPass m_TransparentPostProcessPass;
         private FinalBlitPass m_FinalBlitPass;
         //private EndXRRenderingPass m_EndXrRenderingPass;

#if UNITY_EDITOR
        private SceneViewDepthCopyPass m_SceneViewDepthCopyPass;
#endif

        private RenderTargetHandle ColorAttachment;
        private RenderTargetHandle DepthAttachment;
        private RenderTargetHandle DepthTexture;
        private RenderTargetHandle OpaqueColor;
        private RenderTargetHandle MainLightShadowmap;
        private RenderTargetHandle AdditionalLightsShadowmap;
        private RenderTargetHandle ScreenSpaceShadowmap;

        [NonSerialized]
        private bool m_Initialized = false;

        private void Init()
        {
            if (m_Initialized)
                return;

            m_DepthOnlyPass = new DepthOnlyPass();
            m_MainLightShadowCasterPass = new MainLightShadowCasterPass();
            //m_AdditionalLightsShadowCasterPass = new AdditionalLightsShadowCasterPass();
            m_SetupForwardRenderingPass = new SetupForwardRenderingPass();
            //m_ScreenSpaceShadowResolvePass = new ScreenSpaceShadowResolvePass();
            m_CreateModRenderTexturesPass = new CreateModRenderTexturesPass();
            m_BeginXrRenderingPass = new BeginXRRenderingPass();
            m_SetupModConstants = new SetupModConstanstPass();
            m_RenderOpaqueForwardPass = new RenderOpaqueForwardPass();
            //m_OpaquePostProcessPass = new OpaquePostProcessPass();
            m_DrawSkyboxPass = new DrawSkyboxPass();
            m_CopyDepthPass = new CopyDepthPass();
            m_CopyColorPass = new CopyColorPass();
            m_CopyHSBPass = new HSBPass();
            m_RenderTransparentForwardPass = new RenderTransparentForwardPass();
            m_TransparentPostProcessPass = new TransparentPostProcessPass();
            m_FinalBlitPass = new FinalBlitPass();
            //m_EndXrRenderingPass = new EndXRRenderingPass();


#if UNITY_EDITOR
            m_SceneViewDepthCopyPass = new SceneViewDepthCopyPass();
#endif

            // RenderTexture format depends on camera and pipeline (HDR, non HDR, etc)
            // Samples (MSAA) depend on camera and pipeline
            ColorAttachment.Init("_CameraColorTexture");
            DepthAttachment.Init("_CameraDepthAttachment");
            DepthTexture.Init("_CameraDepthTexture");
            OpaqueColor.Init("_CameraOpaqueTexture");
            MainLightShadowmap.Init("_MainLightShadowmapTexture");
            AdditionalLightsShadowmap.Init("_AdditionalLightsShadowmapTexture");
            ScreenSpaceShadowmap.Init("_ScreenSpaceShadowmapTexture");

            m_Initialized = true;
        }

        public void Setup(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Init();

            Camera camera = renderingData.cameraData.camera;

            renderer.SetupPerObjectLightIndices(ref renderingData.cullResults, ref renderingData.lightData);
            RenderTextureDescriptor baseDescriptor = ScriptableRenderer.CreateRenderTextureDescriptor(ref renderingData.cameraData);
            RenderTextureDescriptor shadowDescriptor = baseDescriptor;
            shadowDescriptor.dimension = TextureDimension.Tex2D;

            bool mainLightShadows = false;
            if (renderingData.shadowData.supportsMainLightShadows)
            {
                mainLightShadows = m_MainLightShadowCasterPass.Setup(MainLightShadowmap, ref renderingData);
                if (mainLightShadows)
                    renderer.EnqueuePass(m_MainLightShadowCasterPass);
            }

            if (renderingData.shadowData.supportsAdditionalLightShadows)
            {
                bool additionalLightShadows = m_AdditionalLightsShadowCasterPass.Setup(AdditionalLightsShadowmap, ref renderingData, renderer.maxVisibleAdditionalLights);
                if (additionalLightShadows)
                    renderer.EnqueuePass(m_AdditionalLightsShadowCasterPass);
            }

            bool resolveShadowsInScreenSpace = mainLightShadows && renderingData.shadowData.requiresScreenSpaceShadowResolve;
            bool requiresDepthPrepass = resolveShadowsInScreenSpace || renderingData.cameraData.isSceneViewCamera ||
                                        (renderingData.cameraData.requiresDepthTexture && (!CanCopyDepth(ref renderingData.cameraData) || renderingData.cameraData.isOffscreenRender));


            requiresDepthPrepass |= renderingData.cameraData.isStereoEnabled;

            renderer.EnqueuePass(m_SetupForwardRenderingPass);

            if (requiresDepthPrepass)
            {
                m_DepthOnlyPass.Setup(baseDescriptor, DepthTexture, SampleCount.One);
                renderer.EnqueuePass(m_DepthOnlyPass);

                foreach (var pass in camera.GetComponents<IAfterDepthPrePass>())
                    renderer.EnqueuePass(pass.GetPassToEnqueue(m_DepthOnlyPass.descriptor, DepthTexture));
            }

            /*if (resolveShadowsInScreenSpace)
            {
                m_ScreenSpaceShadowResolvePass.Setup(baseDescriptor, ScreenSpaceShadowmap);
                renderer.EnqueuePass(m_ScreenSpaceShadowResolvePass);
            }*/

            bool requiresRenderToTexture = ScriptableRenderer.RequiresIntermediateColorTexture(ref renderingData.cameraData, baseDescriptor);

            RenderTargetHandle colorHandle = RenderTargetHandle.CameraTarget;
            RenderTargetHandle depthHandle = RenderTargetHandle.CameraTarget;

            if (requiresRenderToTexture)
            {
                colorHandle = ColorAttachment;
                depthHandle = DepthAttachment;

                var sampleCount = (SampleCount)renderingData.cameraData.msaaSamples;
                m_CreateModRenderTexturesPass.Setup(baseDescriptor, colorHandle, depthHandle, sampleCount);
                renderer.EnqueuePass(m_CreateModRenderTexturesPass);
            }

            /*if (renderingData.cameraData.isStereoEnabled)
                renderer.EnqueuePass(m_BeginXrRenderingPass);*/

            RendererConfiguration rendererConfiguration = ScriptableRenderer.GetRendererConfiguration(renderingData.lightData.additionalLightsCount);

            m_SetupModConstants.Setup(renderer.maxVisibleAdditionalLights, renderer.perObjectLightIndices);
            renderer.EnqueuePass(m_SetupModConstants);

            m_RenderOpaqueForwardPass.Setup(baseDescriptor, colorHandle, depthHandle, ScriptableRenderer.GetCameraClearFlag(camera), camera.backgroundColor, rendererConfiguration);
            renderer.EnqueuePass(m_RenderOpaqueForwardPass);
            foreach (var pass in camera.GetComponents<IAfterOpaquePass>())
                renderer.EnqueuePass(pass.GetPassToEnqueue(baseDescriptor, colorHandle, depthHandle));

            /*if (renderingData.cameraData.postProcessEnabled &&
                renderingData.cameraData.postProcessLayer.HasOpaqueOnlyEffects(renderer.postProcessingContext))
            {
                m_OpaquePostProcessPass.Setup(baseDescriptor, colorHandle);
                renderer.EnqueuePass(m_OpaquePostProcessPass);

                foreach (var pass in camera.GetComponents<IAfterOpaquePostProcess>())
                    renderer.EnqueuePass(pass.GetPassToEnqueue(baseDescriptor, colorHandle, depthHandle));
            }*/

            if (camera.clearFlags == CameraClearFlags.Skybox)
            {
                m_DrawSkyboxPass.Setup(colorHandle, depthHandle);
                renderer.EnqueuePass(m_DrawSkyboxPass);
            }

            foreach (var pass in camera.GetComponents<IAfterSkyboxPass>())
                renderer.EnqueuePass(pass.GetPassToEnqueue(baseDescriptor, colorHandle, depthHandle));

            if (renderingData.cameraData.requiresDepthTexture && !requiresDepthPrepass)
            {
                m_CopyDepthPass.Setup(depthHandle, DepthTexture);
                renderer.EnqueuePass(m_CopyDepthPass);
            }

            if (renderingData.cameraData.requiresOpaqueTexture)
            {
                m_CopyColorPass.Setup(colorHandle, OpaqueColor);
                renderer.EnqueuePass(m_CopyColorPass);
            }

            m_CopyHSBPass.Setup(colorHandle, OpaqueColor);
            renderer.EnqueuePass(m_CopyHSBPass);

            m_RenderTransparentForwardPass.Setup(baseDescriptor, colorHandle, depthHandle, rendererConfiguration);
            renderer.EnqueuePass(m_RenderTransparentForwardPass);

            foreach (var pass in camera.GetComponents<IAfterTransparentPass>())
                renderer.EnqueuePass(pass.GetPassToEnqueue(baseDescriptor, colorHandle, depthHandle));

            if (!renderingData.cameraData.isStereoEnabled && renderingData.cameraData.postProcessEnabled)
            {
                m_TransparentPostProcessPass.Setup(baseDescriptor, colorHandle, BuiltinRenderTextureType.CameraTarget);
                renderer.EnqueuePass(m_TransparentPostProcessPass);
            }
            else if (!renderingData.cameraData.isOffscreenRender && colorHandle != RenderTargetHandle.CameraTarget)
            {
                m_FinalBlitPass.Setup(baseDescriptor, colorHandle);
                renderer.EnqueuePass(m_FinalBlitPass);
            }

            foreach (var pass in camera.GetComponents<IAfterRender>())
                renderer.EnqueuePass(pass.GetPassToEnqueue());

            /*if (renderingData.cameraData.isStereoEnabled)
            {
                renderer.EnqueuePass(m_EndXrRenderingPass);
            }*/

#if UNITY_EDITOR
            if (renderingData.cameraData.isSceneViewCamera)
            {
                m_SceneViewDepthCopyPass.Setup(DepthTexture);
                renderer.EnqueuePass(m_SceneViewDepthCopyPass);
            }
#endif
        }

        bool CanCopyDepth(ref CameraData cameraData)
        {
            bool msaaEnabledForCamera = (int)cameraData.msaaSamples > 1;
            bool supportsTextureCopy = SystemInfo.copyTextureSupport != CopyTextureSupport.None;
            bool supportsDepthTarget = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
            bool supportsDepthCopy = !msaaEnabledForCamera && (supportsDepthTarget || supportsTextureCopy);
            
            bool msaaDepthResolve = false;
            return supportsDepthCopy || msaaDepthResolve;
        }
    }
}