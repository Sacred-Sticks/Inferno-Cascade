using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float Speed;
        [SerializeField] private bool dealDamage = false;
        [SerializeField] private bool useGravity = false;

        private readonly float damage = 50;

        void Start()
        {
            var body = GetComponent<Rigidbody>();
            body.useGravity = useGravity;
            body.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!dealDamage)
                return;
            var health = collision.gameObject.GetComponent<Health>();
            health?.ChangeHealth(-damage);
            Destroy(gameObject);
        }
    }
}
