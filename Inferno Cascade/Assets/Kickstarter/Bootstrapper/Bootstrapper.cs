using Kickstarter.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kickstarter.Bootstrapper
{
    public class Bootstrapper : PersistentSignleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SceneManager.LoadSceneAsync("Bootstrapper", mode: LoadSceneMode.Single);
        }
    }
}

