using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UIScrollViewHorizontal
{
    class CollectionViewSample   : UIViewController
    {

        private UserSource _userSource;
        private UICollectionView _collectionViewUser;
        private UIImageView _preview;


    public override void ViewDidLoad()
    {
        base.ViewDidLoad();


        var lineLayout = new ImageListLineLayout()
            {
                MinimumLineSpacing = 1f,
                ScrollDirection = UICollectionViewScrollDirection.Horizontal
            };

        Title = "Collection";

        _collectionViewUser = new UICollectionView(new RectangleF(new PointF(0,View.Bounds.Height - 75), new SizeF(View.Bounds.Width,75)), lineLayout);

        _collectionViewUser.ContentInset = new UIEdgeInsets(0,View.Bounds.Width/2, 0, View.Bounds.Width/2);
        _collectionViewUser.BackgroundColor = UIColor.DarkGray;

        _preview = new UIImageView(new RectangleF(new PointF(0,50), new SizeF(View.Bounds.Width, View.Bounds.Height-(75 + 50))));
        _preview.ContentMode = UIViewContentMode.ScaleAspectFit;
        View.AddSubview(_preview);




        View.AddSubview(_collectionViewUser);
        _userSource = new UserSource();
        _userSource.FontSize = 11f;
        _userSource.ImageViewSize = new SizeF(53.0f, 73.0f);

        _collectionViewUser.RegisterClassForCell(typeof(UserCell), UserCell.CellID);
        _collectionViewUser.ShowsHorizontalScrollIndicator = true;
        _collectionViewUser.Source = _userSource;
        _userSource.ScrollImageChanged += userSource_ScrollImageChanged;
        _userSource.LoadMoarRequest += userSource_LoadMoarRequest;


        UIButton first = UIButton.FromType(UIButtonType.System);
        first.SetTitle("first", UIControlState.Normal);
        first.BackgroundColor = UIColor.LightGray;
        first.Frame = new RectangleF(new PointF(0,20),new SizeF(50,30));
        first.TouchUpInside += first_TouchUpInside;
        View.AddSubview(first);

        UIButton last = UIButton.FromType(UIButtonType.System);
        last.SetTitle("last", UIControlState.Normal);
        last.BackgroundColor = UIColor.LightGray;
        last.Frame = new RectangleF(new PointF(View.Bounds.Width-50, 20), new SizeF(50, 30));
        last.TouchUpInside += last_TouchUpInside;
        View.AddSubview(last);



        for (int i = 0; i < 35; i++)
        {
            _userSource.Rows.Add(new UserElement(i.ToString(), "RunThumb" + i%12 + ".jpg","run" + i%12 + ".jpg", null));
            
        }

        //userSource.Rows.Add(new UserElement("Name 1", UIImage.FromBundle("RunThumb1.jpg"), () => elementTapped("Name 1")));
        //userSource.Rows.Add(new UserElement("Name 2", UIImage.FromBundle("RunThumb2.jpg"), () => elementTapped("Name 2")));
        //userSource.Rows.Add(new UserElement("Name 3", UIImage.FromBundle("RunThumb3.jpg"), () => elementTapped("Name 3")));
        //userSource.Rows.Add(new UserElement("Name 1", UIImage.FromBundle("RunThumb1.jpg"), () => elementTapped("Name 1")));
        //userSource.Rows.Add(new UserElement("Name 2", UIImage.FromBundle("RunThumb2.jpg"), () => elementTapped("Name 2")));
        //userSource.Rows.Add(new UserElement("Name 3", UIImage.FromBundle("RunThumb3.jpg"), () => elementTapped("Name 3")));
        //userSource.Rows.Add(new UserElement("Name 1", UIImage.FromBundle("RunThumb1.jpg"), () => elementTapped("Name 1")));
        //userSource.Rows.Add(new UserElement("Name 2", UIImage.FromBundle("RunThumb2.jpg"), () => elementTapped("Name 2")));
        //userSource.Rows.Add(new UserElement("Name 3", UIImage.FromBundle("RunThumb3.jpg"), () => elementTapped("Name 3")));
        //userSource.Rows.Add(new UserElement("Name 1", UIImage.FromBundle("RunThumb1.jpg"), () => elementTapped("Name 1")));
        //userSource.Rows.Add(new UserElement("Name 2", UIImage.FromBundle("RunThumb2.jpg"), () => elementTapped("Name 2")));
        //userSource.Rows.Add(new UserElement("Name 3", UIImage.FromBundle("RunThumb3.jpg"), () => elementTapped("Name 3")));
        //userSource.Rows.Add(new UserElement("Name 1", UIImage.FromBundle("RunThumb1.jpg"), () => elementTapped("Name 1")));
        //userSource.Rows.Add(new UserElement("Name 2", UIImage.FromBundle("RunThumb2.jpg"), () => elementTapped("Name 2")));
        //userSource.Rows.Add(new UserElement("Name 3", UIImage.FromBundle("RunThumb3.jpg"), () => elementTapped("Name 3")));

        _collectionViewUser.ReloadData();
    }

    void userSource_LoadMoarRequest()
    {
        for (int i = 0; i < 12; i++)
        {
            _userSource.Rows.Add(new UserElement(_userSource.Rows.Count.ToString(), "RunThumb" + i % 12 + ".jpg", "run" + i % 12 + ".jpg", null));
            _collectionViewUser.InsertItems(new NSIndexPath[] { NSIndexPath.FromItemSection((int) (_userSource.Rows.Count-1),0)});

        }
        
        
    }

    void first_TouchUpInside(object sender, EventArgs e)
    {

        _collectionViewUser.SetContentOffset(new PointF(0 - _collectionViewUser.ContentInset.Left,0), true );

    }

    void last_TouchUpInside(object sender, EventArgs e)
    {

        _collectionViewUser.SetContentOffset(new PointF(_collectionViewUser.ContentSize.Width + _collectionViewUser.ContentInset.Right, 0), true);

    }


    void userSource_ScrollImageChanged(int obj)
    {
        _preview.Image = _userSource.Rows[obj].PreviewImage;
    }



    private void elementTapped(String title)
    {
        new UIAlertView("Tapped", title, null, "OK", null).Show();
    }
}



    




























    public class UserSource : UICollectionViewSource
{
    public UserSource()
    {
        Rows = new List<UserElement>();
    }

    public int _lastScrollImage = -1;

    public List<UserElement> Rows { get; private set; }

    public Single FontSize { get; set; }

    public SizeF ImageViewSize { get; set; }

    public event Action<int> ScrollImageChanged;

    protected virtual void OnScrollImageChanged(int scrollInfo)
    {
        Action<int> handler = ScrollImageChanged;
        if (handler != null) handler(scrollInfo);
    }

    public event Action LoadMoarRequest;
    protected virtual void OnLoadMoarRequest()
    {
        Action handler = LoadMoarRequest;
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
                var cv = scrollView as UICollectionView;

                if (scrollView.ContentOffset.X >= (scrollView.ContentSize.Width - 400))
                {
                    LoadMoarRequest();
                }

                //var centerPoint = new PointF(cv.Center.X + cv.ContentOffset.X,
                //    cv.Center.Y + cv.ContentOffset.Y);

                var centerPoint = new PointF(cv.Center.X + cv.ContentOffset.X,
                    20);

                var index = cv.IndexPathForItemAtPoint(centerPoint);
                if (index != null)
                {
                    if (index.Item != _lastScrollImage)
                    {
                        OnScrollImageChanged(index.Item);
                        _lastScrollImage = index.Item;
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

    public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var cell = (UserCell) collectionView.CellForItem(indexPath);
        cell.ImageView.Alpha = 1;

        UserElement row = Rows[indexPath.Row];
        //row.Tapped.Invoke();
    }

    public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var cell = (UserCell) collectionView.DequeueReusableCell(UserCell.CellID, indexPath);

        UserElement row = Rows[indexPath.Row];

        cell.UpdateRow(row, FontSize, ImageViewSize);

        return cell;
    }
}

public class UserElement
{
    public UserElement(String title, string thumbImageSource, string previewImageSource, NSAction tapped)
    {
        Title = title;
        Tapped = tapped;
        _thumbImageSource = thumbImageSource;
        _previewImageSource = previewImageSource;
    }

    private string _thumbImageSource;
    private string _previewImageSource;

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
    public UserCell(RectangleF frame)
        : base(frame)
    {
        ImageView = new UIImageView();
        //ImageView.Layer.BorderColor = UIColor.DarkGray.CGColor;
        //ImageView.Layer.BorderWidth = 1f;
        //ImageView.Layer.CornerRadius = 3f;
        //ImageView.Layer.MasksToBounds = true;
        //ImageView.ContentMode = UIViewContentMode.ScaleToFill;

        ContentView.AddSubview(ImageView);

        LabelView = new UILabel();
        LabelView.BackgroundColor = UIColor.Clear;
        LabelView.TextColor = UIColor.Red;
        LabelView.TextAlignment = UITextAlignment.Center;

        ContentView.AddSubview(LabelView);
    }

    public UIImageView ImageView { get; private set; }

    public UILabel LabelView { get; private set; }

    public void UpdateRow(UserElement element, Single fontSize, SizeF imageViewSize)
    {
        LabelView.Text = element.Title;
        ImageView.Image = element.ThumbnailImage;

        //LabelView.Font = UIFont.FromName("HelveticaNeue-Bold", fontSize);

        ImageView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
        LabelView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
    }
}


public class ImageListLineLayout : UICollectionViewFlowLayout
{
    //public const float ITEM_SIZE = 70;
    public const int ACTIVE_DISTANCE = 70;
    public const float ZOOM_FACTOR = 0.3f;

    public ImageListLineLayout()
    {
        //ItemSize = new SizeF(ITEM_SIZE, ITEM_SIZE);
        ItemSize = new SizeF(53.0f, 73.0f);
        ScrollDirection = UICollectionViewScrollDirection.Horizontal;
        SectionInset = new UIEdgeInsets(0, 0, 0, 0);
        MinimumLineSpacing = 50f;
        

    }

    public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
    {
        return true;
    }

    //public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(RectangleF rect)
    //{
    //    var array = base.LayoutAttributesForElementsInRect(rect);
    //    var visibleRect = new RectangleF(CollectionView.ContentOffset, CollectionView.Bounds.Size);

    //    foreach (var attributes in array)
    //    {
    //        if (attributes.Frame.IntersectsWith(rect))
    //        {
    //            float distance = visibleRect.GetMidX() - attributes.Center.X;
    //            float normalizedDistance = distance / ACTIVE_DISTANCE;
    //            if (Math.Abs(distance) < ACTIVE_DISTANCE)
    //            {
    //                float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs(normalizedDistance));
    //                attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
    //                attributes.ZIndex = 1;
    //            }
    //        }
    //    }
    //    return array;
    //}

    //public override PointF TargetContentOffset(PointF proposedContentOffset, PointF scrollingVelocity)
    //{
    //    float offSetAdjustment = float.MaxValue;
    //    float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
    //    RectangleF targetRect = new RectangleF(proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
    //    var array = base.LayoutAttributesForElementsInRect(targetRect);
    //    foreach (var layoutAttributes in array)
    //    {
    //        float itemHorizontalCenter = layoutAttributes.Center.X;
    //        if (Math.Abs(itemHorizontalCenter - horizontalCenter) < Math.Abs(offSetAdjustment))
    //        {
    //            offSetAdjustment = itemHorizontalCenter - horizontalCenter;
    //        }
    //    }
    //    return new PointF(proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
    //}

}


}


