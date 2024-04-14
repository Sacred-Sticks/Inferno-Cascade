using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class EnemyDeath : MonoBehaviour, IOnDeath
    {
        public void OnDeath()
        {
            Destroy(this.gameObject);
        }
    }
}
