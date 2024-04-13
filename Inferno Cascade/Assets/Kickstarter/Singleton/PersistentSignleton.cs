using UnityEngine;

namespace Kickstarter.Singleton
{
    public class PersistentSignleton<T> : Singleton<T> where T : Component
    {
        protected override void Awake()
        {
            base.Awake();
            if (instance == null)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
