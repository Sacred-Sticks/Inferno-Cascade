using UnityEngine;

namespace Inferno_Cascade
{
    public interface IForceReciever
    {
        public void AddForceFromPosition(Vector3 explosionOrigin, SpellManager.SpellType spelltype);
    }
}
