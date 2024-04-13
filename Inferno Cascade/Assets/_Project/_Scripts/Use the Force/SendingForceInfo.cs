using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class SendingForceInfo : MonoBehaviour
    {
        [SerializeField] float radius;
        private Collider[] colliders= new Collider[10];
        [SerializeField] SpellType spelltype;

        private void OnCollisionEnter(Collision collision)
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
            for (int i = 0; i < size; i++)
            {
                var go = colliders[i].gameObject;
                go.GetComponent<IForceReciever>()?.AddForceFromPosition(transform.position, spelltype);

                //following is what tutorial used

            }
        }
    }
    public enum SpellType {Fire,Water}
}

