using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlurRenderer
{
    public class BlurPage : ContentPage
    {
        private BlurImage backgroundImage;

        public BlurPage()
        {
            RelativeLayout layout = new RelativeLayout();

            backgroundImage = new BlurImage();
            backgroundImage.Source = "cat_05.jpg";
            backgroundImage.VerticalOptions = LayoutOptions.CenterAndExpand;
            backgroundImage.HorizontalOptions = LayoutOptions.CenterAndExpand;
            backgroundImage.BlurMode = BlurModeEnum.None;

            layout.Children.Add(backgroundImage,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                heightConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));
             
            var btnChangeBlur = new Button();
            btnChangeBlur.Text = "Change Blur";
            btnChangeBlur.Clicked += (sender, args) =>
                               {
                                   switch (backgroundImage.BlurMode)
                                   {
                                       case BlurModeEnum.None:
                                           backgroundImage.BlurMode = BlurModeEnum.Light;
                                           break;
                                       case BlurModeEnum.Light:
                                           backgroundImage.BlurMode = BlurModeEnum.Dark;
                                           break;
                                       case BlurModeEnum.Dark:
                                           backgroundImage.BlurMode = BlurModeEnum.ExtraLight;
                                           break;
                                       case BlurModeEnum.ExtraLight:
                                           backgroundImage.BlurMode = BlurModeEnum.None;
                                           break;
                                       default:
                                           backgroundImage.BlurMode = BlurModeEnum.None;
                                           break;
                                   }
                               };

            layout.Children.Add(btnChangeBlur,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.RelativeToParent((parent) => { return parent.Height - 30; }),
                widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
                heightConstraint: Constraint.Constant(30));

            Content = layout;
        }
    }
}
