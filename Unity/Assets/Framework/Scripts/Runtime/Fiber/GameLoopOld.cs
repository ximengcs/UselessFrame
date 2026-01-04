
//namespace UselessFrame.NewRuntime.Fiber
//{
//    public enum GameLoopState
//    {
//        STOPPED,
//        PAUSED,
//        RUNNING
//    }

//    public class GameLoop
//    {
//        private GameLoopState _state;
//        private float _step;
//        private int _maxUpdates;

//        private float _timing;

//        public GameLoop() {
//    _state = GameLoopState.STOPPED;
//            _step = 1000 / 60f;
//            _maxUpdates = 300;
//    }
//    bool isStopped => this._state == GameLoopState.STOPPED;
//        bool isPaused => this._state == GameLoopState.PAUSED;
//        bool isRunning() => this._state == GameLoopState.RUNNING;
//        public void start()
//    {
//        if (this.isStopped)
//        {
//            this._state = GameLoopState.RUNNING;

//            const lag = 0;
//            const delta = 0;
//            const total = 0;
//            const last = null;

//            this.timing = { last, total, delta, lag }
//            ;
//            this.frame = requestAnimationFrame(this.tick);
//        }
//    }
//    stop()
//    {
//        if (this.isRunning || this.isPaused)
//        {
//            this.state = STOPPED;
//            cancelAnimationFrame(this.frame);
//        }
//    }
//    pause()
//    {
//        if (this.isRunning)
//        {
//            this.state = PAUSED;
//            cancelAnimationFrame(this.frame);
//        }
//    }
//    resume()
//    {
//        if (this.isPaused)
//        {
//            this.state = RUNNING;
//            this.frame = requestAnimationFrame(this.tick);
//        }
//    }
//    tick(time)
//    {
//        if (this.timing.last === null) this.timing.last = time;
//        this.timing.delta = time - this.timing.last;
//        this.timing.total += this.timing.delta;
//        this.timing.lag += this.timing.delta;
//        this.timing.last = time;

//        let numberOfUpdates = 0;

//        while (this.timing.lag >= this.options.step)
//        {
//            this.timing.lag -= this.options.step;
//            this.onUpdate(this.options.step, this.timing.total);
//            this.numberOfUpdates++;
//            if (this.numberOfUpdates >= this.options.maxUpdates)
//            {
//                this.onPanic();
//                break;
//            }
//        }

//        this.onRender(this.timing.lag / this.options.step);

//        this.frame = requestAnimationFrame(this.tick);
//    }
//}
//}
