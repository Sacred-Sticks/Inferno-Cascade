using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Inferno_Cascade
{
    public class ForceReciever : MonoBehaviour, IForceReciever
    {
        [SerializeField] float FireForce;
        [SerializeField] float WaterForce;
        [SerializeField] bool isEnemy = true;
        private Rigidbody rb;

        Coroutine routine;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void AddForceFromPosition(Vector3 explosionOrigin, SpellManager.SpellType spelltype)
        {
            if (isEnemy)
            {
                var nav = GetComponent<NavMeshAgent>();
                nav.enabled = false;
                GetComponent<GoapAgent>().enabled = false;
                rb.isKinematic = false;

                if (routine != null)
                {
                    StopCoroutine(routine);
                }
                routine = StartCoroutine(EnableNavMesh());
            }
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

        private IEnumerator EnableNavMesh()
        {
            yield return new WaitForSeconds(2);
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<GoapAgent>().enabled = true;
            rb.isKinematic = true;
        }

        private void FireCalculation(Vector3 explosionOrigin)
        {
            //doing this because only fire do damage
            float dist = Vector3.Distance(explosionOrigin, this.transform.position);
            //temp damage me tired
            float amount = -5;
            GetComponent<Health>().ChangeHealth(amount);

            Vector3 direction = (transform.position - explosionOrigin).normalized;
            StartCoroutine(AddForce(direction, FireForce));
        }

        private void WaterCalculation()
        {
            var playerBody = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody);
            var direction = transform.position - playerBody.position;
            StartCoroutine(AddForce(direction, WaterForce));
        }

        private IEnumerator AddForce(Vector3 direction, float forceStrength)
        {
            yield return new WaitForEndOfFrame();
            rb.AddForce(direction * forceStrength, ForceMode.Force);
        }
    }
}
