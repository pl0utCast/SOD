using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using ReactiveUI;
using System.Linq;
using System.Windows.Controls;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace SOD.Navigation
{
    public class NavigationService : ReactiveObject, INavigationService
    {
        private readonly Func<Type, object> _viewModelResolver;

        private NavigationContext<FrameworkElement> currentNavigationContext = null;
        private Dictionary<string, NavigationContext<FrameworkElement>> dictionary = new Dictionary<string, NavigationContext<FrameworkElement>>();
        private Dictionary<string, NavigationContext<FrameworkElement>> saveContextDictonary = new Dictionary<string, NavigationContext<FrameworkElement>>();
        private Dictionary<string, List<NavigationContext<FrameworkElement>>> historyDictonary = new Dictionary<string, List<NavigationContext<FrameworkElement>>>();
        private string lastRootViewName = string.Empty;
        private List<IObserver<FrameworkElement>> observers = new List<IObserver<FrameworkElement>>();

        private static NavigationService navigationService;

        public NavigationService(Func<Type, object> viewModelResolver)
        {
            _viewModelResolver = viewModelResolver;
            navigationService = this;
        }
        public void GoBack(bool isDeactivate=true)
        {
            var history = GetHistory(lastRootViewName);
            if (history.Count < 2) return;
            NavigationContext<FrameworkElement> prevContext = null;
            prevContext = history[history.Count - 2];
            if (prevContext != null)
            {
                if (CurrentView.DataContext is IActivatableViewModel supportsDeactivationViewModel && isDeactivate)
                {
                    supportsDeactivationViewModel.Activator.Deactivate();
                }

                history.Remove(history.Last());
                currentNavigationContext = history.Last();
                var view = prevContext.View;
                //view.DataContext = kernel.Get(prevContext.ViewModelType);
                CurrentView = view;
                Notify(CurrentView);
                if (CurrentView != null && CurrentView.DataContext != null)
                {
                    var title = CurrentView.DataContext.GetType().GetProperty("ViewTitle")?.GetValue(CurrentView.DataContext);
                    if (title != null) ViewTitle = (string)title;
                    if (CurrentView.DataContext is IActivatableViewModel supportsActivationViewModel)
                    {
                        supportsActivationViewModel.Activator.Activate();
                    }
                }

            }
        }

        public void NavigateTo(string viewKey)
        {
            NavigateTo(viewKey, null);
        }

        public void NavigateTo(string viewKey, object dataContext)
        {
            if (string.IsNullOrWhiteSpace(viewKey)) throw new ArgumentException();
            //if (history.Count > 0 && viewKey.Equals(history.Last().RootKey)) return;
            if (lastRootViewName.Equals(viewKey)) return;

            // переход между root секциями если это возможно
            if (saveContextDictonary.TryGetValue(viewKey, out var savedContext))
            {
                // сохраняем текущий контекст что-бы была возможность вернуться
                if (currentNavigationContext != null)
                {
                    if (saveContextDictonary.ContainsKey(currentNavigationContext.RootKey))
                    {
                        saveContextDictonary[currentNavigationContext.RootKey] = currentNavigationContext;
                    }
                }
                lastRootViewName = viewKey;
                // востановаливаем сохранёный контекст
                currentNavigationContext = savedContext;
                CurrentView = savedContext.View;
                Notify(CurrentView);
            }

            // переход на новый view в рамках root секции
            if (dictionary.TryGetValue(viewKey, out var navigationContext))
            {
                var history = GetHistory(currentNavigationContext.RootKey);
                if (history.Count == 0 || !history.Last().Equals(currentNavigationContext)) AddHistory(currentNavigationContext);
                var view = navigationContext.View;
                if (dataContext == null)
                {
                    dataContext = _viewModelResolver(navigationContext.ViewModelType);
                }
                if (typeof(IViewFor).IsAssignableFrom(view.GetType()))
                {
                    ((IViewFor)view).ViewModel = dataContext;
                }
                view.DataContext = dataContext;

                CurrentView = view;
                Notify(CurrentView);
                currentNavigationContext = navigationContext;
                AddHistory(currentNavigationContext);
            }

            if (CurrentView != null && CurrentView.DataContext != null)
            {
                var title = CurrentView.DataContext.GetType().GetProperty("ViewTitle")?.GetValue(CurrentView.DataContext);
                if (title != null) ViewTitle = (string)title;
                if (CurrentView.DataContext is IActivatableViewModel supportsActivationViewModel)
                {
                    supportsActivationViewModel.Activator.Activate();
                }
            }
        }

        public void RegisterRoot(string rootName, string viewKey, FrameworkElement view, Type viewModelType)
        {
            if (viewModelType!=null)
            {
                view.DataContext = _viewModelResolver(viewModelType);
            }
            
            if (!saveContextDictonary.ContainsKey(rootName)) saveContextDictonary.Add(rootName, new NavigationContext<FrameworkElement>()
            {
                RootKey = rootName,
                ViewName = viewKey,
                View = view,
                ViewModelType = viewModelType
            });
        }

        public void RegisterView(string rootKey, string viewKey, FrameworkElement view, Type viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewKey)) throw new ArgumentException();

            if (!dictionary.ContainsKey(viewKey))
            {
                var context = new NavigationContext<FrameworkElement>() { RootKey = rootKey, ViewName = viewKey, View = view, ViewModelType = viewModel };
                dictionary.Add(viewKey, context);
            }
            else
            {
                throw new ArgumentException("Данный ключ уже существует");
            }
        }

        private void AddHistory(NavigationContext<FrameworkElement> navigationContext)
        {
            if (!historyDictonary.ContainsKey(navigationContext.RootKey))
            {
                var history = new List<NavigationContext<FrameworkElement>>();
                history.Add(navigationContext);
                historyDictonary.Add(navigationContext.RootKey, history);
            }
            else
            {
                historyDictonary[navigationContext.RootKey].Add(navigationContext);
            }
        }

        private List<NavigationContext<FrameworkElement>> GetHistory(string viewKey)
        {
            if (historyDictonary.ContainsKey(viewKey)) return historyDictonary[viewKey];
            return new List<NavigationContext<FrameworkElement>>();
        }

        public IDisposable Subscribe(IObserver<FrameworkElement> observer)
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

        public void Notify(FrameworkElement value)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(value);
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        [Reactive]
        public FrameworkElement CurrentView { get; set; }

        [Reactive]
        public string ViewTitle { get; set; }

        public static INavigationService Instance => navigationService;
    }
}
