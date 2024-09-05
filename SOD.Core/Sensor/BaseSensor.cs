using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Disposables;

namespace SOD.Core.Sensor
{
    public abstract class BaseSensor<T> : IObservable<T>
    {
        private object locker = new object();
        private List<IObserver<T>> observers = new List<IObserver<T>>();
        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock(locker)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
                return Disposable.Create(() =>
                {
                    observers.Remove(observer);
                });
            }
            
        }
        public void Notify(T value)
        {
            lock (locker)
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(value);
                }
            }
        }
    }
}
