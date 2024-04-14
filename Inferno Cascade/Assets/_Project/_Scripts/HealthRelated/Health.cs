using UnityEngine;
using Kickstarter.Observer;
using Kickstarter.DependencyInjection;

namespace Inferno_Cascade
{
    public interface IOnDeath
    {
        public void OnDeath();
    }

    public class Health : Observable
    {
        [SerializeField] float maxHP;
        [SerializeField] bool IsPlayer;
        
        [SerializeField] private float hp;

        public float HealthPercentage => hp / maxHP;

        [Inject] private hpLocator hpLoc;

        private void Start()
        {
            hp = maxHP;
        }

        public void ChangeHealth(float amount)
        {
            hp += amount;
            if (hp > maxHP) { hp = maxHP; }
            NotifyObservers(new HealthChange(hp));

            if(IsPlayer)
            {
                hpLoc.SetHPBar(hp, maxHP);
            }

            if (hp <= 0)
            {
                GetComponent<IOnDeath>().OnDeath();
            }
        }

        public struct HealthChange : INotification
        {
            public HealthChange(float health)
            {
                Health = health;
            }

            public float Health { get; }
        }
    }
}
