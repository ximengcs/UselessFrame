using Veldrid;
using Veldrid.Sdl2;
using System.Diagnostics;
using Veldrid.StartupUtilities;
using System;
using System.Threading;

namespace Core.Application
{
    internal class XApp : IApp
    {
        private int _width;
        private int _height;

        private Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private ResourceFactory _resourceFactory;
        private CommandList _cmdList;
        private ImGuiRenderer _render;
        private Action _onGUIHandler;
        private Action<float> _onUpdateHandler;
        private bool _closeTaskSource;
        private Stopwatch _sw;

        public bool Disposed => _closeTaskSource;

        public XApp(string title, int width, int height)
        {
            _width = width;
            _height = height;
            WindowCreateInfo windowInfo = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = _width,
                WindowHeight = _height,
                WindowTitle = title
            };
            _window = VeldridStartup.CreateWindow(ref windowInfo);
            _window.Resized += WindowSizeChangeHandler;

            GraphicsDeviceOptions options = new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true);
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options);
            _resourceFactory = _graphicsDevice.ResourceFactory;
            _cmdList = _resourceFactory.CreateCommandList();

            OutputDescription option = _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription;
            _render = new ImGuiRenderer(_graphicsDevice, option, windowInfo.WindowWidth, windowInfo.WindowHeight);
            _onGUIHandler = null;
        }

        public void OnUpdate(Action<float> updateHandler)
        {
            _onUpdateHandler += updateHandler;
        }

        public void Update()
        {
            if (_sw == null)
                _sw = Stopwatch.StartNew();

            float deltaTime = _sw.ElapsedTicks / (float)Stopwatch.Frequency;
            if (_window.Exists && !_closeTaskSource)
            {
                InputSnapshot input = _window.PumpEvents();
                _render.Update(deltaTime, input);
                _onGUIHandler?.Invoke();
                _cmdList.Begin();
                _cmdList.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
                _cmdList.ClearColorTarget(0, new RgbaFloat(0.2f, 0.2f, 0.2f, 1f));
                _render.Render(_graphicsDevice, _cmdList);
                _cmdList.End();
                _graphicsDevice.SubmitCommands(_cmdList);
                _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
                Thread.Sleep((int)(deltaTime * 1000));
                _sw.Restart();
            }
            else
            {
                _render.Dispose();
                _cmdList.Dispose();
                _graphicsDevice.Dispose();
                _closeTaskSource = true;
            }

            _onUpdateHandler?.Invoke(deltaTime);
        }

        public void OnGUI(Action handler)
        {
            _onGUIHandler += handler;
        }

        private void WindowSizeChangeHandler()
        {
            _width = _window.Width;
            _height = _window.Height;
            _graphicsDevice.MainSwapchain.Resize((uint)_width, (uint)_height);
            _render.WindowResized(_width, _height);
        }
    }
}
