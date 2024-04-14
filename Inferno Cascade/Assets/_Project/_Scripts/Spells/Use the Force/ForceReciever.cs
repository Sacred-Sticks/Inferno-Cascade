using UnityEngine;

namespace Inferno_Cascade
{
    public class ForceReciever : MonoBehaviour, IForceReciever
    {
        [SerializeField] float FireForce;
        [SerializeField] float WaterForce;
        private Rigidbody rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void AddForceFromPosition(Vector3 explosionOrigin, SpellManager.SpellType spelltype)
        {
            float forceStrength = 0;

            if (spelltype == SpellManager.SpellType.Fire)
            {
                //doing this because only fire do damage
                float dist = Vector3.Distance(explosionOrigin, this.transform.position);
                //temp damage me tired
                float amount = -5;
                GetComponent<Health>().changeHealth(amount);

                //set force
                forceStrength = FireForce;
            }
            else if(spelltype == SpellManager.SpellType.Water)
            {
                forceStrength = WaterForce;
            }

            Vector3 direction = (transform.position - explosionOrigin).normalized;
            
            rb.AddForce(direction * forceStrength, ForceMode.Force);

            
            //water just push no need for damage calc
        }
    }
}
