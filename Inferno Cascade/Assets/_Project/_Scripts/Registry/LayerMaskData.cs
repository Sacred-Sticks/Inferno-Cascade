using UnityEngine;

namespace Inferno_Cascade
{
    [CreateAssetMenu(fileName = "LayerMaskData", menuName = "Inferno Cascade/LayerMaskData")]
    public class LayerMaskData : ScriptableObject
    {
        [SerializeField] private LayerMask layerMask;
        public LayerMask LayerMask => layerMask;
    }
}
