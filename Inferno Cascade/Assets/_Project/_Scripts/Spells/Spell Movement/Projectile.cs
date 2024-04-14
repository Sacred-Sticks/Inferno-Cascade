using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float Speed;
        void Start()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
        }
    }
}
