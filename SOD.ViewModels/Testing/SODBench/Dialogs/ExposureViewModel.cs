using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SOD.ViewModels.Testing.SODBench.Dialogs
{
    public class ExposureViewModel : ReactiveObject
    {
        public int ExposureNumber { get; set; }
        [Reactive]
        public bool Valid { get; set; } = true;
        [Reactive]
        public bool UnValid { get; set; }
        public string StartPressure { get; set; }
        public string StopPressure { get; set; }
        public string Leakage { get; set; }
        public string Drops { get; set; }
        public string Result { get; set; }
    }
}