using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using SOD.App.Benches.SODBench;
using SOD.Core.Valves;
using SOD.App.Benches;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using MemBus;
using SOD.App.Messages;

namespace SOD.ViewModels.Testing.ManualCommandsBench
{
    public class PostViewModel : ReactiveObject, IActivatableViewModel
    {
        private Post _post;
        private readonly IBus _bus;
        public PostViewModel(IBus bus, Post post, IEnumerable<object> serials)
        {
            _bus = bus;
            _post = post;
            //this.WhenActivated(dis =>
            //{
            //    valve.Subscribe(v=>
            //    {
            //        if (v==null)
            //        {
            //            Serials = null;
            //        }
            //        else
            //        {
            //            Serials = (IEnumerable<object>)v?.GetValveProperty("serial_numbers")?.Value;
            //        }
            //    })
            //    .DisposeWith(dis);
            //});
            Serials = serials;
            IsEnable = _post.IsEnable;
            Serial = post.SerialNumber;
            Name = post.Name;
        }

        public void Save()
        {
            _post.IsEnable = IsEnable;
            _post.SerialNumber = Serial;
            _bus.Publish(new EnablePostMessage(_post.Id, IsEnable));
        }


        [Reactive]
        public bool IsEnable { get; set; }
        [Reactive]
        public string Serial { get; set; }
        [Reactive]
        public IEnumerable<object> Serials { get; set; }
        public string Name { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
