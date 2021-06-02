namespace UnityEngine.Experimental.Rendering.ModPipeline
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class ModPipelineAdditionalLightData : MonoBehaviour
    {
        [Tooltip("Controls the usage of pipeline settings.")]
        [SerializeField] bool m_UsePipelineSettings = true;

        public bool usePipelineSettings
        {
            get { return m_UsePipelineSettings; }
            set { m_UsePipelineSettings = value; }
        }
    }
}
