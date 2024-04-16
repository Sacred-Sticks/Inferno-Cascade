using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Inferno_Cascade
{
    public interface ISpell
    {
        /// <summary>
        /// Called to begin or activate the spell
        /// </summary>
        public void BeginSpell();

        /// <summary>
        /// Called to end the spell (only implement when necessary)
        /// </summary>
        public void EndSpell()
        {

        }

        public float ManaCost { get; }
    }

    public class ExampleSpell : ISpell
    {
        public ExampleSpell(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void BeginSpell()
        {
            // if you need to use a Rigidbody, you can get it from the Registry like so...
            // you can declare it as Rigidbody if you want, I just use var for brevity, it looks cleaner to me
            // commented out because the rigidbody isn't registered yet, so it would throw an error
            Debug.Log($"{Name} Used");
            // var body = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody);
            // Rigidbody rb = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody);
        }

        public void EndSpell()
        {
            // Only implement EndSpell if you need to do something when the spell ends
            Debug.Log($"{Name} Ended");
        }

        public float ManaCost => 10;
    }

    public class ProjectileSpell : ISpell
    {
        public ProjectileSpell(string pathway, float manaCost)
        {
            this.pathway = pathway;
            ManaCost = manaCost;

            mask = Resources.Load<LayerMaskData>("Objects/Masks/Spell-Ignore").LayerMask;
        }

        private static float forwardOffset = 0.5f;
        private static float verticalOffset = 1.35f;
        private string pathway { get; }
        private LayerMask mask { get; }

        public void BeginSpell()
        {
            var body = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody);
            Vector3 offset = body.transform.forward * forwardOffset + body.transform.up * verticalOffset; 
            var position = body.position + offset;

            Transform camTran = Camera.main.transform;
            var projectile = Resources.Load<GameObject>(pathway);
            var ray = new Ray(position, camTran.forward);

            if (!Physics.Raycast(ray, out var hit, mask))
            {
                UnityEngine.Object.Instantiate(projectile, position, camTran.rotation);
                return;
            }
            var direction = hit.point - position;
            UnityEngine.Object.Instantiate(projectile, position, Quaternion.LookRotation(direction));
        }

        public float ManaCost { get; }
    }

    public class HealSpell : ISpell
    {
        private float healAmount { get; }
        private float healCooldown { get; }

        private bool canHeal;

        public HealSpell(float healAmount, float healCooldown, float manaCost)
        {
            this.healAmount = healAmount;
            this.healCooldown = healCooldown;
            ManaCost = manaCost;
        }

        public void BeginSpell()
        {
            var health = Registry.Get<Rigidbody>(RegistryStrings.PlayerRigidbody).GetComponent<Health>();
            Heal(health);
        }

        public void EndSpell()
        {
            canHeal = false;
        }

        public float ManaCost { get; }

        private async void Heal(Health health)
        {
            canHeal = true;
            while (canHeal)
            {
                health.ChangeHealth(healAmount);
                await Task.Delay(TimeSpan.FromSeconds(healCooldown));
            }
        }
    }
}
