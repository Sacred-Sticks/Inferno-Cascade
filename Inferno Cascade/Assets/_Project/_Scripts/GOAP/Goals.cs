using System.Collections.Generic;

namespace Inferno_Cascade
{
    public class AgentGoal
    {
        public string Name { get; }
        public float Priority { get; private set; }
        public HashSet<AgentBelief> DesiredEffects { get; } = new HashSet<AgentBelief>();

        private AgentGoal(string name)
        {
            Name = name;
        }

        public class Builder
        {
            private readonly AgentGoal goal;

            public Builder(string name)
            {
                goal = new AgentGoal(name)
                {
                    Priority = 1
                };
            }

            public Builder WithPriority(float priority)
            {
                goal.Priority = priority;
                return this;
            }

            public Builder WithDesiredEffect(AgentBelief belief)
            {
                goal.DesiredEffects.Add(belief);
                return this;
            }

            public AgentGoal Build() => goal;
        }
    }
}
