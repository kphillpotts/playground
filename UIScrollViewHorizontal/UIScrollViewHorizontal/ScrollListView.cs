using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace UIScrollViewHorizontal
{
    public class ScrollLIstView : UIViewController
    {
        private UIScrollView _scrollView;
        private List<UIImageView> _visibleThumbnails;
        private UIImageView _preview;
        private UILabel _debugLabel;
        private List<ListElement> _entries;

        // Thumbnail sizes
        private static readonly SizeF ThumbnailSize = new SizeF(53.0f, 73.0f);

        public ScrollLIstView()
        {
            _visibleThumbnails = new List<UIImageView>();
            _entries = new List<ListElement>();
        }

        private float PlaceNewThumbnailOnRight(float position, ListElement listElement)
        {
            var newImageView = new UIImageView();
            newImageView.Frame = new RectangleF(position, 0, ThumbnailSize.Width, ThumbnailSize.Height);
            newImageView.Image = listElement.GetThumbnailImage();
            
            _visibleThumbnails.Add(newImageView);

            return newImageView.Frame.GetMaxX();
        }

        private float PlaceNewThumbnailOnLeft(float position, ListElement listElement)
        {
            var newImageView = new UIImageView();
            newImageView.Frame = new RectangleF(position-ThumbnailSize.Width, 0, ThumbnailSize.Width, ThumbnailSize.Height);
            newImageView.Image = listElement.GetThumbnailImage();
            
            _visibleThumbnails.Insert(0, newImageView);

            return newImageView.Frame.GetMinX();
        }


        private int ScrollOffsetToIndex(float scrollOffset)
        {
            var index = scrollOffset/ThumbnailSize.Width;
            if (index < 0) return 0;
            return (int) index;
        }





        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // how many thumbnails wide is the screen
            var thumbnailCount = View.Bounds.Width/ThumbnailSize.Width;


            View.BackgroundColor = UIColor.Red;
            //float w = 80.0f;
            float padding = 0.0f;
            int n = 100;

            var height = ThumbnailSize.Height + 2 * padding;

            _scrollView = new UIScrollView
            {
                Frame = new RectangleF(0, View.Bounds.Height - height, View.Frame.Width, height),
                ContentSize = new SizeF((ThumbnailSize.Width + padding) * n, ThumbnailSize.Height),
                BackgroundColor = UIColor.DarkGray,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                ContentInset = new UIEdgeInsets(0, View.Bounds.Width / 2, 0, View.Bounds.Width / 2)
            };

            for (int i = 0; i < n; i++)
            {
                int step = i % 12;

                var entry = new ListElement
                              {
                                  Index = i,
                                  PreviewImage = "run" + step + ".jpg",
                                  ThumbImage = "RunThumb" + step + ".jpg"
                              };

                var thumbImageView = new UIImageView(UIImage.FromFile(entry.ThumbImage));
                thumbImageView.Frame = new RectangleF(padding * (i + 1) + (i * ThumbnailSize.Width), padding, ThumbnailSize.Width, ThumbnailSize.Height);
                _scrollView.AddSubview(thumbImageView);
                _visibleThumbnails.Add(thumbImageView);
                _entries.Add(entry);
            }

            _debugLabel = new UILabel(new RectangleF(0, 20, View.Bounds.Width, 20));
            _debugLabel.TextColor = UIColor.Blue;
            _debugLabel.TextAlignment = UITextAlignment.Center;
            View.AddSubview(_debugLabel);

            _preview = new UIImageView(new RectangleF(new PointF(0, 0), new SizeF(View.Bounds.Width, View.Bounds.Height - height)));
            View.AddSubview(_preview);
            _scrollView.ShowsHorizontalScrollIndicator = false;

            var scrollDelegate = new ScrollDelegate(ThumbnailSize.Width, View.Bounds.Width);
            scrollDelegate.ScrollImageChanged += ScrollImageChanged;
            _scrollView.Delegate = scrollDelegate;

            View.AddSubview(_scrollView);
        }

        void ScrollImageChanged(ScrollInfo info)
        {
            var calcIndex = ScrollOffsetToIndex(info.ScrollOffset);

            _debugLabel.Text = "Index " + info.ImageIndex + " Offset " + info.ScrollOffset + " CalcOffset " + calcIndex;

            if ((info.ImageIndex >= 0) && (info.ImageIndex <= _visibleThumbnails.Count - 1))
            {
                _preview.Image = UIImage.FromFile(_entries[info.ImageIndex].PreviewImage);
            }
        }



    }

    class ListElement
    {
        public int Index { get; set; }
        public string ThumbImage { get; set; }
        public string PreviewImage { get; set; }
        public float LogicalXPos { get; set; }


        public UIImage GetThumbnailImage()
        {
            return UIImage.FromFile(ThumbImage);
        }

        public UIImage GetPreviewImage()
        {
            return UIImage.FromFile(PreviewImage);
        }
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

        }
    }

}
