using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Kickstarter.Bootstrapper
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            int totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = ActiveSceneGroup.Scenes[i];
                if (!reloadDupScenes && loadedScenes.Contains(sceneData.Name))
                    continue;

                var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                operationGroup.Operations.Add(operation);

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(TimeSpan.FromSeconds(0.1));
            }

            var activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
                SceneManager.SetActiveScene(activeScene);

            OnSceneGroupLoaded.Invoke();
            progress?.Report(1);
        }

        public async Task UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded)
                    continue;

                string sceneName = sceneAt.name;
                if (sceneName == "Bootstrapper")
                    continue;
                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);
            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null)
                    continue;
                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            while (!operationGroup.IsDone)
                await Task.Delay(100);
        }
    }
}

