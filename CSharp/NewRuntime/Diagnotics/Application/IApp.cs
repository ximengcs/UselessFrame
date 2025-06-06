
using System;

namespace Core.Application
{
    public interface IApp
    {
        bool Disposed { get; }

        void Update();

        void OnGUI(Action handler);
        void OnUpdate(Action<float> handler);

        public static IApp Create(string title, int width, int height)
        {
            return new XApp(title, width, height);
        }
    }
}
