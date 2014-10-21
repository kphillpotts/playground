using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.UIKit;

namespace UIScrollViewHorizontal
{
    public class ScrollLIstView : UIViewController
    {
        private UIScrollView _scrollView;
        private List<UIButton> _buttons;
        private List<UIImageView> _images;
        private float _thumbWidth = 53.0f;
        private UIImageView _preview;
        private UILabel _debugLabel;

        private List<Entry> _entries;

        public ScrollLIstView()
        {
            _buttons = new List<UIButton>();
            _images = new List<UIImageView>();
            _entries = new List<Entry>();

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Red;
            float h = 73.0f;
            //float w = 80.0f;
            float padding = 0.0f;
            int n = 100;

            var height = h + 2 * padding;

            _scrollView = new UIScrollView
            {

                Frame = new RectangleF(0, View.Bounds.Height - height, View.Frame.Width, height),

                //Frame = new RectangleF(0, 0, View.Frame.Width, h + 2 * padding),
                ContentSize = new SizeF((_thumbWidth + padding) * n, h),
                BackgroundColor = UIColor.DarkGray,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                ContentInset = new UIEdgeInsets(0, View.Bounds.Width / 2, 0, View.Bounds.Width / 2)
            };

            for (int i = 0; i < n; i++)
            {
                int step = i % 12;

                Entry entry = new Entry();
                entry.Index = i;
                entry.PreviewImage = "run" + step + ".jpg";
                entry.ThumbImage = "RunThumb" + step + ".jpg";

                //var image = UIImage.FromFile("site.jpg");
                var iv = new UIImageView(UIImage.FromFile(entry.ThumbImage));

                iv.Frame = new RectangleF(padding * (i + 1) + (i * _thumbWidth), padding, _thumbWidth, h);
                _scrollView.AddSubview(iv);
                _images.Add(iv);
                _entries.Add(entry);

                //var button = UIButton.FromType(UIButtonType.RoundedRect);
                //button.SetTitle(i.ToString(), UIControlState.Normal);
                //button.Frame = new RectangleF(padding * (i + 1) + (i * w),
                //        padding, w, h);
                //_scrollView.AddSubview(button);
                //_buttons.Add(button);
            }

            _debugLabel = new UILabel(new RectangleF(0, 20, View.Bounds.Width, 20));
            _debugLabel.TextColor = UIColor.Blue;
            _debugLabel.TextAlignment = UITextAlignment.Center;
            View.AddSubview(_debugLabel);

            _preview = new UIImageView(new RectangleF(new PointF(0, 0), new SizeF(View.Bounds.Width, View.Bounds.Height - height)));
            View.AddSubview(_preview);

            var sd = new ScrollDelegate(_thumbWidth, View.Bounds.Width);
            sd.ScrollImageChanged += sd_ScrollImageChanged;

            _scrollView.Delegate = sd;

            View.AddSubview(_scrollView);
        }

        void sd_ScrollImageChanged(ScrollInfo info)
        {
            _debugLabel.Text = "Index " + info.ImageIndex + " Offset " + info.ScrollOffset;

            if ((info.ImageIndex >= 0) && (info.ImageIndex <= _images.Count - 1))
            {
                _preview.Image = UIImage.FromFile(_entries[info.ImageIndex].PreviewImage);
            }
        }



    }

    class Entry
    {
        public int Index { get; set; }
        public string ThumbImage { get; set; }
        public string PreviewImage { get; set; }
    }

    class ScrollInfo
    {
        public int ImageIndex { get; set; }
        public int ScrollOffset { get; set; }
    }

    class ScrollDelegate : UIScrollViewDelegate
    {

        public event Action<ScrollInfo> ScrollImageChanged;

        protected virtual void OnScrollImageChanged(ScrollInfo scrollInfo)
        {
            Action<ScrollInfo> handler = ScrollImageChanged;
            if (handler != null) handler(scrollInfo);
        }

        private float _thumbWidth;
        private float _width;

        public ScrollDelegate(float thumbWidth, float width)
        {
            _thumbWidth = thumbWidth;
            _width = width;

        }

        public override void Scrolled(UIScrollView scrollView)
        {
            //base.Scrolled(scrollView);
            var off = scrollView.ContentOffset;
            //   Debug.WriteLine(off.ToString());

            var size = scrollView.ContentSize;

            var index = Math.Floor(off.X / _thumbWidth);




            OnScrollImageChanged(new ScrollInfo() { ImageIndex = (int)index, ScrollOffset = (int)off.X });


            //  Debug.WriteLine("index " + Math.Floor(index));

            //if (off.X > (size.Width - 500))
            //{
            //    Debug.WriteLine("TimeToLoad");
            //}

        }
    }

}
