using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kickstarter.Observer;
using UnityEngine.UI;

namespace Inferno_Cascade
{
    public interface IOnDeath
    {
        public void OnDeath();
    }

    public class Health : Observable
    {
        [SerializeField] float hp,Maxhp;
        [SerializeField] Image hpBar;
        [SerializeField] bool IsPlayer;


        public void Start()
        {
            if(IsPlayer)
            {
                hpBar=FindObjectOfType<hpLocator>().gameObject.GetComponent<Image>();
            }
        }

        public void changeHealth(float amount)
        {
            hp += amount;
            if (hp > Maxhp) { hp = Maxhp; }
            NotifyObservers(new HealthChange(hp));

            if(hpBar !=null) { hpBar.fillAmount = hp / Maxhp; }

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
