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
            switch (spelltype)
            {
                case SpellManager.SpellType.Fire:
                    FireCalculation(explosionOrigin);
                    break;
                case SpellManager.SpellType.Water:
                    WaterCalculation();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            
            //water just push no need for damage calc
        }

        private void FireCalculation(Vector3 explosionOrigin)
        {
            //doing this because only fire do damage
            float dist = Vector3.Distance(explosionOrigin, this.transform.position);
            //temp damage me tired
            float amount = -5;
            GetComponent<Health>().changeHealth(amount);

            Vector3 direction = (transform.position - explosionOrigin).normalized;
            AddForce(direction, FireForce);
        }

        private void WaterCalculation()
        {
            var playerBody = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody);
            var direction = transform.position - playerBody.position;
            AddForce(direction, WaterForce);
        }

        private void AddForce(Vector3 direction, float forceStrength)
        {
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
    }
}
