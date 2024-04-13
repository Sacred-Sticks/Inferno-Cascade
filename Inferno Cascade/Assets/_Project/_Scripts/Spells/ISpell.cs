using UnityEngine;

namespace Inferno_Cascade
{
    public interface ISpell
    {
        public static void BeginSpell(ISpell spell, Rigidbody body = null)
        {
            if (spell.RequiresBody)
            {
                spell.BeginSpell(body);
                return;
            }
            spell.BeginSpell();
        }
        public bool RequiresBody { get; }

        public void BeginSpell()
        {

        }
        public void BeginSpell(Rigidbody body)
        {

        }
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
        public bool RequiresBody => false;

        public void BeginSpell()
        {
            Debug.Log($"{Name} Used");
        }
    }
}
