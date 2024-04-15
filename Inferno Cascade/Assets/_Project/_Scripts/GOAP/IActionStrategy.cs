namespace Inferno_Cascade
{
    public interface IActionStrategy
    {
        public bool CanPerform { get; }
        public bool Complete { get; }

        public void Start()
        {
            // noop
        }

        public void Update(float deltaTime)
        {
            // noop
        }

        public void Stop()
        {
            // noop
        }
    }
}
