using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.EasyLoading;

namespace EasyLoadingSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {
        public bool IsFullscreenLoading { get; set; }
        public bool IsSkeletonLoading { get; set; }

        public LoadingState LoadingState { get; set; } = LoadingState.Success;
        public bool IsStateLoading { get; set; }

        public ICommand FullscreenLoadingCommand { get; set; }
        public ICommand SkeletonCommand { get; set; }
        public ICommand RepeatingCommand { get; set; }
        public ICommand StateCommand { get; set; }

        public MainPageModel()
        {
            FullscreenLoadingCommand = new Command(async (x) =>
            {
                IsFullscreenLoading = true;
                await Task.Delay(2000);
                IsFullscreenLoading = false;
            });

            SkeletonCommand = new Command(async (x) =>
            {
                IsSkeletonLoading = true;
                await Task.Delay(2000);
                IsSkeletonLoading = false;
            });

            RepeatingCommand = new Command(async (x) =>
            {
                await CoreMethods.PushPageModel<ListViewPageModel>();
            });

            StateCommand = new Command(async (x) =>
            {
                try
                {
                    IsStateLoading = true;
                    LoadingState = LoadingState.Loading;
                    await Task.Delay(2000);
                    throw new Exception();
                }
                catch
                {
                    LoadingState = LoadingState.Error;
                    await Task.Delay(2000);
                }
                finally
                {
                    LoadingState = LoadingState.Success;
                    IsStateLoading = false;
                }
            });
        }
    }
}
