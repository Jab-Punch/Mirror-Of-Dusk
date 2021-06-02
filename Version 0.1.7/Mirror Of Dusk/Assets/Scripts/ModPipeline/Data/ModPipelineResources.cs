using UnityEngine.Serialization;

namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    public class ModPipelineResources : ScriptableObject
    {
        [FormerlySerializedAs("BlitShader"), SerializeField] Shader m_BlitShader = null;
        [FormerlySerializedAs("CopyDepthShader"), SerializeField] Shader m_CopyDepthShader = null;
        [FormerlySerializedAs("ScreenSpaceShadowShader"), SerializeField] Shader m_ScreenSpaceShadowShader = null;
        [FormerlySerializedAs("SamplingShader"), SerializeField] Shader m_SamplingShader = null;
        [FormerlySerializedAs("HsbShader"), SerializeField] Shader m_HsbShader = null;

        public Shader blitShader
        {
            get { return m_BlitShader; }
        }

        public Shader copyDepthShader
        {
            get { return m_CopyDepthShader; }
        }

        public Shader screenSpaceShadowShader
        {
            get { return m_ScreenSpaceShadowShader; }
        }

        public Shader samplingShader
        {
            get { return m_SamplingShader; }
        }

        public Shader hsbShader
        {
            get { return m_HsbShader; }
        }
    }
}
