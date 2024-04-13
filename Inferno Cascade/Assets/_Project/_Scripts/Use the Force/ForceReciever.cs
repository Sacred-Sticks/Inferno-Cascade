using UnityEngine;

namespace Inferno_Cascade
{
    public class ForceReciever : MonoBehaviour, IForceReciever
    {
        [SerializeField] float forceStrength;
        private Rigidbody rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void AddForceFromPosition(Vector3 explosionOrigin, SpellType spelltype)
        {
            Vector3 direction = (transform.position - explosionOrigin).normalized;
            
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
    }
}
