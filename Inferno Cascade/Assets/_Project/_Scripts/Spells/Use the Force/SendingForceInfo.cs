using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class SendingForceInfo : MonoBehaviour
    {
        [SerializeField] float radius;
        private Collider[] colliders= new Collider[10];
        [SerializeField] SpellManager.SpellType spelltype;

        private void OnCollisionEnter(Collision collision)
        {
            //fire ball calc
            if(spelltype == SpellManager.SpellType.Fire)
            {
                int size = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
                for (int i = 0; i < size; i++)
                {
                    var go = colliders[i].gameObject;
                    go.transform.root.GetComponent<IForceReciever>()?.AddForceFromPosition(transform.position, spelltype);
                }
            }
            else if(spelltype ==SpellManager.SpellType.Water)
            {
                //water only affects the target it hits
                collision.transform.root.GetComponent<IForceReciever>()?.AddForceFromPosition(transform.position, spelltype);
            }
            Destroy(this.gameObject);
        }
    }
}

