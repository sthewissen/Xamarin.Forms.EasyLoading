using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EasyLoadingSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {
        public bool IsFullscreenLoading { get; set; }
        public bool IsSkeletonLoading { get; set; }

        public ICommand FullscreenLoadingCommand { get; set; }
        public ICommand SkeletonCommand { get; set; }
        public ICommand RepeatingCommand { get; set; }

        public MainPageModel()
        {
            FullscreenLoadingCommand = new Command(async (x) =>
            {
                IsFullscreenLoading = true;
                await Task.Delay(3000);
                IsFullscreenLoading = false;
            });

            SkeletonCommand = new Command(async (x) =>
            {
                IsSkeletonLoading = true;
                await Task.Delay(3000);
                IsSkeletonLoading = false;
            });

            RepeatingCommand = new Command(async (x) =>
            {
                await CoreMethods.PushPageModel<ListViewPageModel>();
            });
        }
    }
}
