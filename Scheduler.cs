using System.Diagnostics;

namespace DotEngine;

public class Scheduler
{
    public Stopwatch Timer { get; private set; } = new Stopwatch();

    public double FPS { get => 1000 / MilliSeconds; set => MilliSeconds = 1000 / value; }

    private double MilliSeconds { get; set; }

    private Action Routine { get; set; }

    public bool IsStopped { get; private set; } = false;
    public bool IsStopping { get; private set; } = false;

    public Action OnInitializing { get; set; }
    public Action OnInitialized { get; set; }
    public Action OnTerminating { get; set; }
    public Action OnTerminated { get; set; }

    public void Initialize(Action routine)
    {
        OnInitializing?.Invoke();

        Loger.Logging(Loger.Type.Info, "Intializing", writeOnFile: true);

        Routine = routine;

        OnInitialized?.Invoke();

        Loger.Logging(Loger.Type.Info, "Initialized", writeOnFile: true);
    }

    public async Task Terminate() =>
        await Task.Run(() =>
        {
            OnTerminating?.Invoke();

            Loger.Logging(Loger.Type.Info, "Terminating", writeOnFile: true);

            IsStopping = true;

            while (!IsStopped)
            {
                Task.Delay(1);
            }

            OnTerminated?.Invoke();

            Loger.Logging(Loger.Type.Info, "Terminated", writeOnFile: true);
        });

    public async Task StartLoop() =>
        await Task.Run(() =>
        {
            Loger.Logging(Loger.Type.Info, "Loop is function is called.", writeOnFile: true);

            Timer.Start();

            while (!IsStopping)
            {
                while (Timer.ElapsedMilliseconds < MilliSeconds)
                {
                    Task.Delay(1);
                }

                Timer.Restart();

                try
                {
                    Routine?.Invoke();
                }
                catch (Exception ex)
                {
                    Loger.Logging(Loger.Type.Error, ex.ToString(), writeOnFile: true);
                }
            }
            IsStopped = true;
        });
}
