using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SOD.Navigation
{
    public interface INavigationService : IObservable<FrameworkElement>
    {
        /// <summary>
        /// Регистрация View к которму возможно перейти через сервис навигации
        /// </summary>
        /// <param name="viewKey">Имя view для перехода</param>
        /// <param name="view">view instance</param>
        void RegisterView(string rootKey, string viewKey, FrameworkElement view, Type viewModel);

        void RegisterRoot(string rootName, string viewKey, FrameworkElement view, Type viewModelType);
        /// <summary>
        /// Переход к view
        /// </summary>
        /// <param name="viewKey">Имя view к которому необходимо перейти</param>
        void NavigateTo(string viewKey);
        void NavigateTo(string viewKey, object dataContext);
        /// <summary>
        /// Вернуться к передыдущему view
        /// </summary>
        void GoBack(bool isDiactivate=true);
        /// <summary>
        /// Текущий view 
        /// </summary>
        FrameworkElement CurrentView { get; }

        void Clear();

        string ViewTitle { get; }
    }
}
