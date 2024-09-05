using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SOD.Dialogs
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        void ShowDialog(string name, object viewModel);
        Task<object> ShowDialogAsync(string dialogName, object viewModel);
        void CloseAsync(object result);
        void Close();
        void RegisterDialog(string dialogName, ContentControl view);
        ContentControl DialogView { get; }
        bool IsOpen { get; set; }
    }
}
