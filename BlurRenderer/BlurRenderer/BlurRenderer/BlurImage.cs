using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlurRenderer
{
    public enum BlurModeEnum
    {
        None = 0,
        Light = 1,
        Dark = 2,
        ExtraLight = 3
    }

    public class BlurImage : Image
    {
        public static readonly BindableProperty BlurModeProperty =
            BindableProperty.Create<BlurImage, BlurModeEnum>(image => image.BlurMode, BlurModeEnum.None);

        public BlurModeEnum BlurMode
        {
            get { return (BlurModeEnum) GetValue(BlurModeProperty); }
            set { SetValue(BlurModeProperty, value); }
        }
    }
}
