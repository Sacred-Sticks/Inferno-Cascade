using System;
using System.Collections.Generic;
using System.Linq;

namespace Kickstarter.Bootstrapper
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType type) => Scenes.FirstOrDefault(Scene => Scene.Type == type)?.Name;
    }
}

