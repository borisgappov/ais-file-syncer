using System;
using System.Timers;

namespace AisFileSyncer.Infrastructure.Utils
{
    public class SyncTimer : IDisposable
    {
        private Timer _timer;

        public SyncTimer(Action OnTimer = null, double? interval = null)
        {
            if (OnTimer != null && interval != null)
            {
                Start(OnTimer, interval.Value);
            }
        }

        public void Start(Action OnTimer, double interval)
        {
            if (_timer?.Enabled ?? false)
            {
                Stop();
            }
            _timer = new Timer(interval);
            _timer.Elapsed += (s, e) =>
            {
                OnTimer?.Invoke();
            };
            _timer.Enabled = true;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
