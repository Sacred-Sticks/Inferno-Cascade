using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class PlayerDeath : MonoBehaviour, IOnDeath
    {
        public void OnDeath()
        {
            //put up loss screen
            Debug.Log("you lost bitch");
        }
    }
}
