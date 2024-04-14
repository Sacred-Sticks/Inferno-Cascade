using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kickstarter.DependencyInjection;

namespace Inferno_Cascade
{
    public class hpLocator : MonoBehaviour, IDependencyProvider
    {
        private Image hpBar;
        [Provide] hpLocator hpLoC => this;

        public void Start()
        {
            hpBar = GetComponent<Image>();
        }

        public void SetHPBar(float hp, float MaxHp)
        {
            hpBar.fillAmount = hp / MaxHp;
        }
    }
}
