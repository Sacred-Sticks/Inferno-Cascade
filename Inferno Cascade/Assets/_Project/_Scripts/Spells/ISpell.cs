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
    }

    public struct ExampleSpell : ISpell
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
    }

    public struct FireBall : ISpell
    {
        public void BeginSpell()
        {
            Transform camTran = Camera.main.transform;
            GameObject fb = Resources.Load<GameObject>("Assests/Prefabs/SpellPrefabs/FireBall");
            var ball = Object.Instantiate(fb, camTran.position, camTran.rotation);
        }
    }
}
