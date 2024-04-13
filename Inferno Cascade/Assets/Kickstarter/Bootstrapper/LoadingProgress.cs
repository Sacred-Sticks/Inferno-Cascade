using System;

namespace Kickstarter.Bootstrapper
{
    public class LoadingProgress : IProgress<float>
    {
        public LoadingProgress(ILoadingBar loadingBar)
        {
            Progressed += loadingBar.SetProgress;
        }

        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}

