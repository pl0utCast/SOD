using MemBus;
using SOD.App.Benches;
using SOD.App.Messages.Test;
using SOD.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SOD.LocalizationService;
using SOD.App.Benches.SODBench;
using ReactiveUI.Validation.Helpers;

namespace SOD.ViewModels.Testing.ManualCommandsBench.Test.Dialog
{
    public class ManualValidateTestViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly ILocalizationService _localizationService;

        public ManualValidateTestViewModel(Bench bench, IBus bus, IDialogService dialogService, ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            this.WhenActivated(dis =>
            {
                foreach (var post in bench.Posts)
                {
                    if (post.IsEnable)
                    {
                        var vm = new ValidatePostViewModel() { Id = post.Id };
                        vm.Name = _localizationService["Testing.ManualCommandsBench.Post"] + " " + post.Id;

                        vm.Activator.Activate().DisposeWith(dis);
                        vm.IsValid().Subscribe(v =>
                        {
                            foreach (var post in Posts)
                            {
                                CanClose &= post.ValidationContext.IsValid;
                            }
                        })
                        .DisposeWith(dis);
                        Posts.Add(vm);
                    }
                }

                if (Posts.Count <= 3) // Верстаем количество колонок по количеству постов
                    PostsColumn = Posts.Count;
                else // Верстаем количество колонок по количеству постов деленное на 2 и округленное в большую сторону
                    PostsColumn = (int)Math.Ceiling(Posts.Count / 2.0);
            });

            Close = ReactiveCommand.Create(() =>
            {
                var response = new ManualValidateTestResponse();

                foreach (var post in Posts)
                {
                    var status = post.Valid ? PostStatus.Valid : PostStatus.UnValid;
                    response.Status.Add(new Tuple<int, PostStatus>(post.Id, status));
                }

                bus.Publish(response);
                dialogService.Close();
            });
        }
        [Reactive]
        public bool CanClose { get; set; }
        public ReactiveCommand<Unit, Unit> Close { get; set; }
        public int PostsColumn { get; private set; }
        public ObservableCollection<ValidatePostViewModel> Posts { get; set; } = new ObservableCollection<ValidatePostViewModel>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }

    public class ValidatePostViewModel : ReactiveValidationObject, IActivatableViewModel
    {
        public ValidatePostViewModel()
        {
            //this.WhenActivated(dis =>
            //{
            //    /*this.WhenAnyValue(x => x.Valid)
            //        .Throttle(TimeSpan.FromSeconds(1))
            //        .Subscribe(v =>
            //        {
            //            UnValid = !v;
            //        })
            //        .DisposeWith(dis);
            //    this.WhenAnyValue(x => x.UnValid)
            //        .Throttle(TimeSpan.FromSeconds(1))
            //        .Subscribe(uv =>
            //        {
            //            Valid = !uv;
            //        })
            //        .DisposeWith(dis);*/
            //    this.ValidationRule(x => x.Valid, v => (v && !UnValid) || (!v && UnValid), "error").DisposeWith(dis);
            //});
        }
        public int Id { get; set; }
        public string Name { get; set; }
        [Reactive]
        public bool Valid { get; set; }
        [Reactive]
        public bool UnValid { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}
