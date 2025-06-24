
namespace Core.Application
{
    public interface IApp
    {
        float DeltaTime { get; }
        bool Disposed { get; }

        CancellationToken DisposeToken { get; }

        void Update(float deltaTime);
        void Resize(int width, int height);
        void OnGUI(Action handler);
        void OnUpdate(Action<float> handler);

        public static IApp Create(string title, int width, int height)
        {
            return new XApp(title, width, height);
        }
    }
}
