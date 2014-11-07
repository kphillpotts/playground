using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using BlurRenderer;
using BlurRenderer.iOS.NativeRenderer;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BlurImage), typeof(BlurImageRenderer))]
namespace BlurRenderer.iOS.NativeRenderer
{
    public class BlurImageRenderer : ImageRenderer
    {
        UIVisualEffectView _blurView;
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            // kill existing blurview if it exists
            if (_blurView != null)
            {
                _blurView.RemoveFromSuperview();
                _blurView = null;
            }

            var b = this.Element as BlurImage;

            switch (b.BlurMode)
            {
                case BlurModeEnum.None:
                    break;
                case BlurModeEnum.Light:
                    _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Light));
                    break;
                case BlurModeEnum.Dark:
                    _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
                    break;
                case BlurModeEnum.ExtraLight:
                    _blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight));
                    break;
                default:
                    break;
            }

            // add a new blurview if we have one set
            if (_blurView != null)
            {
                _blurView.Frame = new RectangleF(rect.Location, rect.Size);
                NativeView.AddSubview(_blurView);
            }
        }


        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null) return;

            if (e.PropertyName == "BlurMode")
            {
                SetNeedsDisplay();
            }
        }
    }
}