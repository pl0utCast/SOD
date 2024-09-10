using Akavache;
using ReactiveUI;
using SOD.ViewModels.Reports.Dialogs;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SOD.View.Reports.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ExportReportDialogView.xaml
    /// </summary>
    public partial class ExportReportDialogView : UserControl, IViewFor<ExportReportDialogViewModel>
    {
        private const string EXPORT_PATH = "ExportReportPath";
        public ExportReportDialogView()
        {
            InitializeComponent();
            this.DataContextChanged += (s, e) => { ViewModel = (ExportReportDialogViewModel)e.NewValue; };
            this.WhenActivated(dis =>
            {
                selectPathBtn.Events()
                             .PreviewMouseLeftButtonDown
                             .Subscribe(args =>
                             {
                                 var dialog = new System.Windows.Forms.FolderBrowserDialog();
                                 if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                 {
                                     ViewModel.Path = dialog.SelectedPath;
                                     Akavache.BlobCache.UserAccount.InsertObject(EXPORT_PATH, dialog.SelectedPath);
                                 }
                             })
                            .DisposeWith(dis);

                Akavache.BlobCache.UserAccount
                        .GetObject<string>(EXPORT_PATH)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Catch(Observable.Return<string>(null, RxApp.MainThreadScheduler))
                        .BindTo(this, x=>x.ViewModel.Path)
                        .DisposeWith(dis);
            });
            
        }

        public ExportReportDialogViewModel ViewModel { get; set; }
        object IViewFor.ViewModel 
        { 
            get
            {
                return ViewModel;
            }
            set
            {
                ViewModel = (ExportReportDialogViewModel)value;
            }
        }
    }
}
