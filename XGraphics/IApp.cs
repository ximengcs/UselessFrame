
using System;

namespace Core.Application
{
    public interface IApp
    {
        float DeltaTime { get; }
        bool Disposed { get; }

        void Update();
        void Resize(int width, int height);
        void OnGUI(Action handler);
        void OnUpdate(Action<float> handler);

        public static IApp Create(string title, int width, int height)
        {
            return new XApp(title, width, height);
        }
    }
}
