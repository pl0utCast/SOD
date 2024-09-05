using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SOD.Dialogs
{
    public class DialogService : ReactiveObject,  IDialogService
    {
        private Dictionary<string, ContentControl> dialogs = new Dictionary<string, ContentControl>();
        private bool isDialogRunning = false;
        private object dialogResult;
        private readonly Action<object> _showMessageAction;

        public DialogService(Action<object> showMessageAction)
        {
            Instance = this;
            _showMessageAction = showMessageAction;
        }

        public void Close()
        {
            if (IsOpen)
            {
                if (DialogView.DataContext is IActivatableViewModel activatableViewModel)
                {
                    activatableViewModel.Activator.Deactivate();
                }
                IsOpen = false;
            }
        }

        public void CloseAsync(object result)
        {
            dialogResult = result;
            isDialogRunning = false;
            if (IsOpen)
            {
                if (DialogView.DataContext is IActivatableViewModel activatableViewModel)
                {
                    activatableViewModel.Activator.Deactivate();
                }
                IsOpen = false;
                
            }
        }

        public void RegisterDialog(string dialogName, ContentControl view)
        {
            if (view == null || dialogName == null) throw new ArgumentNullException();

            if (!dialogs.ContainsKey(dialogName))
            {
                dialogs.Add(dialogName, view);
            }
        }

        public void ShowDialog(string name, object viewModel)
        {
            if (viewModel == null) return;

            if (dialogs.TryGetValue(name, out var view))
            {
                if (viewModel is IActivatableViewModel activatableViewModel)
                {
                    view.DataContext = activatableViewModel;
                    activatableViewModel.Activator.Activate();
                }
                else
                {
                    view.DataContext = viewModel;
                }

                DialogView = view;
                IsOpen = true;
            }
        }

        public async Task<object> ShowDialogAsync(string dialogName, object viewModel)
        {
            if (viewModel == null) return null;

            if (dialogs.TryGetValue(dialogName, out var view))
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    if (viewModel is IActivatableViewModel activatableViewModel)
                    {
                        view.DataContext = activatableViewModel;
                        activatableViewModel.Activator.Activate();
                    }
                    else
                    {
                        view.DataContext = viewModel;
                    }
                    DialogView = view;
                    IsOpen = true;
                    isDialogRunning = true;

                });
                return await Task.Run(() => Waiting());
            }
            return null;
        }

        private object Waiting()
        {
            while (isDialogRunning)
            {
                Thread.Sleep(300);
            }
            return dialogResult;
        }

        public void ShowMessage(string message)
        {
            _showMessageAction?.Invoke(message);
        }

        public static DialogService Instance { get; private set; }

        [Reactive]
        public ContentControl DialogView { get; private set; }
        [Reactive]
        public bool IsOpen { get; set; }
    }
}
