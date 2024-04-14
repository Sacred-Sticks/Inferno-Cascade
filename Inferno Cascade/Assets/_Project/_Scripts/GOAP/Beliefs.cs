using System;
using UnityEngine;

namespace Inferno_Cascade
{
    public class AgentBelief
    {
        private AgentBelief(string name)
        {
            Name = name;
        }

        public string Name { get; }
    
        private Func<bool> condition = () => false;
        private Func<Vector3> observedLocation = () => Vector3.zero;
        
        public Vector3 Location => observedLocation();
        public bool Evaluate() => condition();

        public class Builder
        {
            private readonly AgentBelief _belief;

            public Builder(string name)
            {
                _belief = new AgentBelief(name);
            }

            public Builder WithCondition(Func<bool> condition)
            {
                _belief.condition = condition;
                return this;
            }

            public Builder WithLocation(Func<Vector3> location)
            {
                _belief.observedLocation = location;
                return this;
            }

            public AgentBelief Build() => _belief;
        }
    }
}
