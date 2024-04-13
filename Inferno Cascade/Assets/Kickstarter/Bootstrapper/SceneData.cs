using Eflatun.SceneReference;
using System;

namespace Kickstarter.Bootstrapper
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType Type;
    }
}

