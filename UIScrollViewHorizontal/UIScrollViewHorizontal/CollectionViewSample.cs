using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UIScrollViewHorizontal
{
    internal class CollectionViewSample : UIViewController
    {
        private UICollectionView _collectionViewUser;
        private UIImageView _preview;
        private UserSource _userSource;
        private UIButton _animateButton;
        private const int MaxElementsToAdd = 500;
        private System.Timers.Timer t = new System.Timers.Timer();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Collection";

            const int collectionViewHeight = 75;

            // defines the layout of the collectionView
            var lineLayout = new HorizontalLineLayout
                             {
                                 MinimumLineSpacing = 1f,
                                 ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                             };

            _collectionViewUser =
                new UICollectionView(
                    new RectangleF(
                        new PointF(0, View.Bounds.Height - collectionViewHeight), 
                        new SizeF(View.Bounds.Width, collectionViewHeight)), 
                    lineLayout);
            _collectionViewUser.ContentInset = new UIEdgeInsets(0, View.Bounds.Width/2, 0, View.Bounds.Width/2);
            _collectionViewUser.BackgroundColor = UIColor.White;
            View.AddSubview(_collectionViewUser);

            _userSource = new UserSource();
            _userSource.ImageViewSize = new SizeF(53.0f, 73.0f);


            _collectionViewUser.RegisterClassForCell(typeof(UserCell), UserCell.CellID);
            _collectionViewUser.ShowsHorizontalScrollIndicator = true;

            _collectionViewUser.Source = _userSource;
            _userSource.ScrollImageChanged += UserSourceScrollImageChanged;
            _userSource.LoadMoreRequest += UserSourceLoadMoreRequest;

            _preview = new UIImageView(new RectangleF(new PointF(0, 60), new SizeF(View.Bounds.Width, View.Bounds.Height - (75 + 60))));
            _preview.ContentMode = UIViewContentMode.ScaleAspectFit;
            View.AddSubview(_preview);

            UIButton first = UIButton.FromType(UIButtonType.System);
            first.SetTitle("rewind", UIControlState.Normal);
            first.BackgroundColor = UIColor.White;
            first.Frame = new RectangleF(new PointF(0, 20), new SizeF(100, 40));
            first.TouchUpInside += FirstTouchUpInside;
            View.AddSubview(first);

            _animateButton = UIButton.FromType(UIButtonType.System);
            _animateButton.SetTitle("Animate", UIControlState.Normal);
            _animateButton.BackgroundColor = UIColor.White;
            _animateButton.Frame = new RectangleF(new PointF(View.Bounds.Width - 100, 20), new SizeF(100, 40));
            _animateButton.TouchUpInside += LastTouchUpInside;
            View.AddSubview(_animateButton);


            // preload with some data 
            for (int i = 0; i < 12; i++)
            {
                var elementIndex = _userSource.Rows.Count;
                _userSource.Rows.Add(new UserElement(
                    title: elementIndex.ToString(CultureInfo.InvariantCulture),
                    thumbImageSource: "RunThumb" + elementIndex % 12 + ".jpg",
                    previewImageSource: "run" + elementIndex % 12 + ".jpg",
                    tapped: () => ElementTapped(elementIndex)));
            }
            _collectionViewUser.ReloadData();

            t.Elapsed += t_Elapsed;
            t.Interval = 10;

        }

        private void UserSourceLoadMoreRequest()
        {
            // only add up to the maximum number we have specified.
            // technically our only limitation is the number of 
            // UserSource rows we want to hold.  The actual UI controls
            if (_userSource.Rows.Count >= MaxElementsToAdd)
                return;

            for (var i = 0; i < 12; i++)
            {
                AddImageToCollection(i%12);
            }
        }

        private void AddImageToCollection(int imageIndex)
        {
            var elementIndex = _userSource.Rows.Count;
            _userSource.Rows.Add(new UserElement(
                title: elementIndex.ToString(CultureInfo.InvariantCulture), 
                thumbImageSource: "RunThumb" + imageIndex + ".jpg", 
                previewImageSource: "run" + imageIndex + ".jpg",
                tapped: () => ElementTapped(elementIndex)));

            var collectionIndex = _userSource.Rows.Count <= 0 ? 0 : _userSource.Rows.Count - 1;

            _collectionViewUser.InsertItems(new[] { NSIndexPath.FromItemSection(item: collectionIndex, section: 0) });
        }

        private void FirstTouchUpInside(object sender, EventArgs e)
        {
            _collectionViewUser.SetContentOffset(new PointF(0 - _collectionViewUser.ContentInset.Left, 0), true);
        }

        private void LastTouchUpInside(object sender, EventArgs e)
        {
            if (t.Enabled)
            {
                _animateButton.SetTitle("Animate", UIControlState.Normal);
                t.Stop();
            }
            else
            {
                _animateButton.SetTitle("Stop", UIControlState.Normal);
                t.Start();
            }
        }

        private void UserSourceScrollImageChanged(int obj)
        {
            _preview.Image = _userSource.Rows[obj].PreviewImage;
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            InvokeOnMainThread(delegate
                               {
                                   var offset = _collectionViewUser.ContentOffset;
                                   var max = _collectionViewUser.ContentSize;

                                   if (offset.X < max.Width)
                                       _collectionViewUser.SetContentOffset(new PointF(offset.X + 5.3f, 0), false);
                                   else
                                   {
                                       t.Stop();

                                   }
                               });
        }

        private void ElementTapped(int userElementId)
        {
            _preview.Image = _userSource.Rows[userElementId].PreviewImage;
        }
    }


    public class UserSource : UICollectionViewSource
    {

        public UserSource()
        {
            Rows = new List<UserElement>();
        }

        public int LastScrollImage = -1;

        public List<UserElement> Rows { get; private set; }

        public SizeF ImageViewSize { get; set; }

        public event Action<int> ScrollImageChanged;

        protected virtual void OnScrollImageChanged(int scrollInfo)
        {
            var handler = ScrollImageChanged;
            if (handler != null) handler(scrollInfo);
        }

        public event Action LoadMoreRequest;

        protected virtual void OnLoadMoreRequest()
        {
            var handler = LoadMoreRequest;
            if (handler != null) handler();
        }

        public override Int32 NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }


        public override void Scrolled(UIScrollView scrollView)
        {
            if (scrollView is UICollectionView)
            {
                var collectionView = scrollView as UICollectionView;

                // when we get a certain distance towards the end request more
                if (scrollView.ContentOffset.X >= (scrollView.ContentSize.Width - 400))
                {
                    LoadMoreRequest();
                }

                var centerPoint = new PointF(collectionView.Center.X + collectionView.ContentOffset.X, 20);

                NSIndexPath index = collectionView.IndexPathForItemAtPoint(centerPoint);
                
                if (index != null)
                {
                    if (index.Item != LastScrollImage)
                    {
                        OnScrollImageChanged(index.Item);
                        LastScrollImage = index.Item;
                    }
                }
            }
        }


        public override Int32 GetItemsCount(UICollectionView collectionView, Int32 section)
        {
            return Rows.Count;
        }

        public override Boolean ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (UserCell) collectionView.CellForItem(indexPath);
            cell.ImageView.Alpha = 0.5f;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UserElement row = Rows[indexPath.Row];
            if (row.Tapped != null)
                row.Tapped.Invoke();
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (UserCell) collectionView.CellForItem(indexPath);
            cell.ImageView.Alpha = 1;

        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (UserCell) collectionView.DequeueReusableCell(UserCell.CellID, indexPath);

            UserElement row = Rows[indexPath.Row];

            cell.UpdateRow(row, ImageViewSize);

            return cell;
        }
    }

    public class UserElement
    {
        private readonly string _previewImageSource;
        private readonly string _thumbImageSource;

        public UserElement(String title, string thumbImageSource, string previewImageSource, NSAction tapped)
        {
            Title = title;
            Tapped = tapped;
            _thumbImageSource = thumbImageSource;
            _previewImageSource = previewImageSource;
        }

        public String Title { get; set; }

        public UIImage ThumbnailImage
        {
            get { return UIImage.FromBundle(_thumbImageSource); }
        }

        public UIImage PreviewImage
        {
            get { return UIImage.FromBundle(_previewImageSource); }
        }

        public NSAction Tapped { get; set; }
    }

    public class UserCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("UserSource");

        [Export("initWithFrame:")]
        public UserCell(RectangleF frame) : base(frame)
        {
            ImageView = new UIImageView();
            ContentView.AddSubview(ImageView);

            LabelView = new UILabel();
            LabelView.BackgroundColor = UIColor.Clear;
            LabelView.TextColor = UIColor.Red;
            LabelView.TextAlignment = UITextAlignment.Center;
            ContentView.AddSubview(LabelView);
        }

        public UIImageView ImageView { get; private set; }

        public UILabel LabelView { get; private set; }

        public void UpdateRow(UserElement element, SizeF imageViewSize)
        {
            LabelView.Text = element.Title;
            ImageView.Image = element.ThumbnailImage;

            ImageView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
            LabelView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
        }
    }

    public class HorizontalLineLayout : UICollectionViewFlowLayout
    {
        public HorizontalLineLayout()
        {
            ItemSize = new SizeF(53.0f, 73.0f);
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            MinimumLineSpacing = 50f;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
        {
            return true;
        }
    }
}