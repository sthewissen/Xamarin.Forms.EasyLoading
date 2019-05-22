using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasyLoadingSample.Controls
{
    public class AnimatedBoxView: BoxView
    {
        public AnimatedBoxView()
        {
            var smoothAnimation = new Animation();
            smoothAnimation.WithConcurrent((f) => this.Opacity = f, 0.3, 0.5, Xamarin.Forms.Easing.Linear);
            smoothAnimation.WithConcurrent((f) => this.Opacity = f, 0.8, 0.8, Xamarin.Forms.Easing.Linear);
            smoothAnimation.WithConcurrent((f) => this.Opacity = f, 0.8, 1, Xamarin.Forms.Easing.Linear);
            smoothAnimation.Commit(this, "smoothAnimation", 0, 300, Easing.Linear, null, () => true);

            //Task.Delay(2000);
        }
    }
}
