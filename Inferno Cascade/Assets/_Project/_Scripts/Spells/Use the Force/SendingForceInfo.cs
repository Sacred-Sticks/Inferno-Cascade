using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class SendingForceInfo : MonoBehaviour
    {
        [SerializeField] float radius;
        [SerializeField] SpellManager.SpellType spelltype;

        private void OnCollisionEnter(Collision collision)
        {
            System.Action action = spelltype switch
            {
                SpellManager.SpellType.Fire => FireCollision,
                SpellManager.SpellType.Water => () => WaterCollision(collision),
                _ => null
            };
            action?.Invoke();
            Destroy(gameObject);
        }

        private void FireCollision()
        {
            var colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var collider in colliders)
                collider.GetComponentInParent<IForceReciever>()?.AddForceFromPosition(transform.position, spelltype);
        }

        private void WaterCollision(Collision collision)
        {
            collision.transform.GetComponentInParent<IForceReciever>()?.AddForceFromPosition(transform.position, spelltype);
        }
    }
}

