using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UIScrollViewHorizontal
{
    public class CollectionViewViewController : UICollectionViewController  
    {
        private static NSString animalCellId = new NSString("AnimalCell");
        static NSString headerId = new NSString("Header");

        private List<IEntity> animals;

        public CollectionViewViewController(UICollectionViewLayout layout)
            : base(layout)
        {
            animals = new List<IEntity>();
            for (int i = 0; i < 50; i++)
            {
                var newEntity = new Entity();
                newEntity.ImageSource = "RunThumb" + i%12 + ".jpg";;
                newEntity.Name = "Entity " + i;

                animals.Add(newEntity);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CollectionView.RegisterClassForCell(typeof (EntityCell), animalCellId);
            CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);
 
        }

        public override int NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            return animals.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView,
            MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var animalCell = (EntityCell) collectionView.DequeueReusableCell(animalCellId, indexPath);

            var animal = animals[indexPath.Row];

            animalCell.Image = animal.Image;

            return animalCell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
            headerView.Text = "Supplementary View";
            return headerView;
        }


        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = UIColor.Yellow;
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = UIColor.White;
        }

        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }



        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
            if (lineLayout != null)
            {
                if ((toInterfaceOrientation == UIInterfaceOrientation.Portrait) ||
                    (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
                    lineLayout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
                else
                    lineLayout.SectionInset = new UIEdgeInsets(0, 0.0f, 0, 0.0f);
            }
        }
    }

    public class Header : UICollectionReusableView
    {
        UILabel label;

        public string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
                SetNeedsDisplay();
            }
        }

        [Export("initWithFrame:")]
        public Header(System.Drawing.RectangleF frame)
            : base(frame)
        {
            label = new UILabel() { Frame = new System.Drawing.RectangleF(0, 0, 300, 50), BackgroundColor = UIColor.Yellow };
            AddSubview(label);
        }
    }

    public class EntityCell : UICollectionViewCell
    {
        UIImageView imageView;

        [Export ("initWithFrame:")]
        public EntityCell(System.Drawing.RectangleF frame) : base(frame)
        {
                BackgroundView = new UIView{BackgroundColor = UIColor.Orange};

                SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

                ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
                ContentView.Layer.BorderWidth = 2.0f;
                ContentView.BackgroundColor = UIColor.White;
                ContentView.Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);

                imageView = new UIImageView (UIImage.FromBundle ("RunThumb0.jpg"));
                imageView.Center = ContentView.Center;
                imageView.Transform = CGAffineTransform.MakeScale (0.7f, 0.7f);

                ContentView.AddSubview (imageView);
        }

        public UIImage Image {
                set {
                        imageView.Image = value;
                }
        }
    }

    public class LineLayout : UICollectionViewFlowLayout
    {
        public const float ITEM_SIZE = 70;
        public const int ACTIVE_DISTANCE = 200;
        public const float ZOOM_FACTOR = 0.3f;

        public LineLayout()
        {
            ItemSize = new SizeF(ITEM_SIZE, ITEM_SIZE);
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            MinimumLineSpacing = 50.0f;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(RectangleF rect)
        {
            var array = base.LayoutAttributesForElementsInRect(rect);
            var visibleRect = new RectangleF(CollectionView.ContentOffset, CollectionView.Bounds.Size);

            foreach (var attributes in array)
            {
                if (attributes.Frame.IntersectsWith(rect))
                {
                    float distance = visibleRect.GetMidX() - attributes.Center.X;
                    float normalizedDistance = distance / ACTIVE_DISTANCE;
                    if (Math.Abs(distance) < ACTIVE_DISTANCE)
                    {
                        float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs(normalizedDistance));
                        attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
                        attributes.ZIndex = 1;
                    }
                }
            }
            return array;
        }

        public override PointF TargetContentOffset(PointF proposedContentOffset, PointF scrollingVelocity)
        {
            float offSetAdjustment = float.MaxValue;
            float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
            RectangleF targetRect = new RectangleF(proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
            var array = base.LayoutAttributesForElementsInRect(targetRect);
            foreach (var layoutAttributes in array)
            {
                float itemHorizontalCenter = layoutAttributes.Center.X;
                if (Math.Abs(itemHorizontalCenter - horizontalCenter) < Math.Abs(offSetAdjustment))
                {
                    offSetAdjustment = itemHorizontalCenter - horizontalCenter;
                }
            }
            return new PointF(proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
        }

    }


    public interface IEntity
    {
        string Name { get; set; }
        UIImage Image { get; }
        string ImageSource { get; set; }
    }

    public class Entity : IEntity
    {
        public Entity()
        {
        }

        public string Name { get; set; }

        public UIImage Image
        {
            get
            {
                return UIImage.FromBundle(ImageSource);
            }
        }

        public string ImageSource { get; set; }
    }
}
