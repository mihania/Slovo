namespace Slovo.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Extends ListBox to enable drag-and-drop reorder within the list.
    /// </summary>
    [TemplatePart(Name = ReorderListBox.DragIndicatorPart, Type = typeof(Image))]
    [TemplatePart(Name = ReorderListBox.DragInterceptorPart, Type = typeof(Canvas))]
    [TemplatePart(Name = ReorderListBox.ScrollViewerPart, Type = typeof(ScrollViewer))]
    public class ReorderListBox : ListBox
    {
        #region Template part name constants

        public const string DragIndicatorPart = "DragIndicator";
        public const string DragInterceptorPart = "DragInterceptor";
        public const string ScrollViewerPart = "ScrollViewer";

        #endregion

        private const string IsReorderEnabledPropertyName = "IsReorderEnabled";

        #region Private fields

        private double dragScrollDelta;
        private Panel itemsPanel;
        private ScrollViewer scrollViewer;
        private Canvas dragInterceptor;
        private Image dragIndicator;
        private object dragItem;
        private ReorderListBoxItem dragItemContainer;
        private Rect dragInterceptorRect;
        private int dropTargetIndex;

        #endregion

        /// <summary>
        /// Creates a new ReorderListBox and sets the default style key.
        /// The style key is used to locate the control template in Generic.xaml.
        /// </summary>
        public ReorderListBox()
        {
            this.DefaultStyleKey = typeof(ReorderListBox);
        }

        #region IsReorderEnabled DependencyProperty

        public static readonly DependencyProperty IsReorderEnabledProperty = DependencyProperty.Register(
            ReorderListBox.IsReorderEnabledPropertyName, typeof(bool), typeof(ReorderListBox),
            new PropertyMetadata(false, (d, e) => ((ReorderListBox)d).OnIsReorderEnabledChanged(e)));

        /// <summary>
        /// Gets or sets a value indicating whether reordering is enabled in the listbox.
        /// This also controls the visibility of the reorder drag-handle of each listbox item.
        /// </summary>
        public bool IsReorderEnabled
        {
            get
            {
                return (bool)this.GetValue(ReorderListBox.IsReorderEnabledProperty);
            }
            set
            {
                this.SetValue(ReorderListBox.IsReorderEnabledProperty, value);
            }
        }

        protected void OnIsReorderEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.dragInterceptor != null)
            {
                this.dragInterceptor.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }

            this.InvalidateArrange();
        }

        #endregion

        #region AutoScrollMargin DependencyProperty

        public static readonly DependencyProperty AutoScrollMarginProperty = DependencyProperty.Register(
            "AutoScrollMargin", typeof(int), typeof(ReorderListBox), new PropertyMetadata(32));

        /// <summary>
        /// Gets or sets the size of the region at the top and bottom of the list where dragging will
        /// cause the list to automatically scroll.
        /// </summary>
        public double AutoScrollMargin
        {
            get
            {
                return (int)this.GetValue(ReorderListBox.AutoScrollMarginProperty);
            }
            set
            {
                this.SetValue(ReorderListBox.AutoScrollMarginProperty, value);
            }
        }

        #endregion

        #region ItemsControl overrides

        /// <summary>
        /// Applies the control template, gets required template parts, and hooks up the drag events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.scrollViewer = (ScrollViewer)this.GetTemplateChild(ReorderListBox.ScrollViewerPart);
            this.dragInterceptor = this.GetTemplateChild(ReorderListBox.DragInterceptorPart) as Canvas;
            this.dragIndicator = this.GetTemplateChild(ReorderListBox.DragIndicatorPart) as Image;

            if (this.scrollViewer != null && this.dragInterceptor != null && this.dragIndicator != null)
            {
                this.dragInterceptor.Visibility = this.IsReorderEnabled ? Visibility.Visible : Visibility.Collapsed;

                this.dragInterceptor.ManipulationStarted += this.dragInterceptor_ManipulationStarted;
                this.dragInterceptor.ManipulationDelta += this.dragInterceptor_ManipulationDelta;
                this.dragInterceptor.ManipulationCompleted += this.dragInterceptor_ManipulationCompleted;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ReorderListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ReorderListBoxItem;
        }

        /// <summary>
        /// Ensures that a possibly-recycled item container (ReorderListBoxItem) is ready to display a list item.
        /// </summary>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ReorderListBoxItem itemContainer = (ReorderListBoxItem)element;
            itemContainer.ApplyTemplate();  // Loads visual states.

            // Set this state before binding to avoid showing the visual transition in this case.
            string reorderState = this.IsReorderEnabled ?
                ReorderListBoxItem.ReorderEnabledState : ReorderListBoxItem.ReorderDisabledState;
            VisualStateManager.GoToState(itemContainer, reorderState, false);

            itemContainer.SetBinding(ReorderListBoxItem.IsReorderEnabledProperty,
                new Binding(ReorderListBox.IsReorderEnabledPropertyName) { Source = this });

            if (item == this.dragItem)
            {
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.DraggingState, false);

                if (this.dropTargetIndex >= 0)
                {
                    // The item's dragIndicator is currently being moved, so the item itself is hidden. 
                    itemContainer.Visibility = Visibility.Collapsed;
                    this.dragItemContainer = itemContainer;
                }
                else
                {
                    // The item was just moved and dropped, so it needs to be transitioned back to not-dragging state.
                    VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NotDraggingState, true);
                    this.dragItem = null;
                }
            }
            else
            {
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NotDraggingState, false);
            }
        }

        /// <summary>
        /// Called when an item container (ReorderListBoxItem) is being removed from the list panel.
        /// This may be because the item was removed from the list or because the item is now outside
        /// the virtualization region (because ListBox uses a VirtualizingStackPanel as its items panel).
        /// </summary>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            ReorderListBoxItem itemContainer = (ReorderListBoxItem)element;
            if (itemContainer == this.dragItemContainer)
            {
                this.dragItemContainer.Visibility = Visibility.Visible;
                this.dragItemContainer = null;
            }
        }

        #endregion

        /// <summary>
        /// Called when the user presses down on the transparent drag-interceptor. Identifies the targed
        /// drag handle and list item and prepares for a drag operation.
        /// </summary>
        private void dragInterceptor_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (this.itemsPanel == null)
            {
                ItemsPresenter scrollItemsPresenter = (ItemsPresenter)this.scrollViewer.Content;
                this.itemsPanel = (Panel)VisualTreeHelper.GetChild(scrollItemsPresenter, 0);
            }

            GeneralTransform interceptorTransform = this.dragInterceptor.TransformToVisual(
                Application.Current.RootVisual);
            Point targetPoint = interceptorTransform.Transform(e.ManipulationOrigin);
            targetPoint = ReorderListBox.GetHostCoordinates(targetPoint);

            List<UIElement> targetElements = VisualTreeHelper.FindElementsInHostCoordinates(
                targetPoint, this.itemsPanel).ToList();
            ReorderListBoxItem targetItemContainer = targetElements.OfType<ReorderListBoxItem>().FirstOrDefault();
            if (targetItemContainer != null && targetElements.Contains(targetItemContainer.DragHandle))
            {
                VisualStateManager.GoToState(targetItemContainer, ReorderListBoxItem.DraggingState, true);

                GeneralTransform targetItemTransform = targetItemContainer.TransformToVisual(this.dragInterceptor);
                Point targetItemOrigin = targetItemTransform.Transform(new Point(0, 0));
                Canvas.SetLeft(this.dragIndicator, targetItemOrigin.X);
                Canvas.SetTop(this.dragIndicator, targetItemOrigin.Y);
                this.dragIndicator.Width = targetItemContainer.RenderSize.Width;
                this.dragIndicator.Height = targetItemContainer.RenderSize.Height;

                this.dragItemContainer = targetItemContainer;
                this.dragItem = this.dragItemContainer.Content;

                this.dragInterceptorRect = interceptorTransform.TransformBounds(
                    new Rect(new Point(0, 0), this.dragInterceptor.RenderSize));

                this.dropTargetIndex = -1;
            }
        }

        /// <summary>
        /// Called when the user drags on (or from) the transparent drag-interceptor.
        /// Moves the item (actually a rendered snapshot of the item) according to the drag delta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragInterceptor_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (this.Items.Count <= 1 || this.dragItem == null)
            {
                return;
            }

            if (this.dropTargetIndex == -1)
            {
                // When the drag actually starts, swap out the item for the drag-indicator image of the item.
                // This is necessary because the item itself may be removed from the virtualizing panel
                // if the drag causes a scroll of considerable distance.
                Size dragItemSize = this.dragItemContainer.RenderSize;
                WriteableBitmap writeableBitmap = new WriteableBitmap(
                    (int)dragItemSize.Width, (int)dragItemSize.Height);

                // Swap states to force the transition to complete.
                VisualStateManager.GoToState(this.dragItemContainer, ReorderListBoxItem.NotDraggingState, false);
                VisualStateManager.GoToState(this.dragItemContainer, ReorderListBoxItem.DraggingState, false);
                writeableBitmap.Render(this.dragItemContainer, null);

                writeableBitmap.Invalidate();
                this.dragIndicator.Source = writeableBitmap;

                this.dragIndicator.Visibility = Visibility.Visible;
                this.dragItemContainer.Visibility = Visibility.Collapsed;

                if (this.itemsPanel.Children.IndexOf(this.dragItemContainer) < this.itemsPanel.Children.Count - 1)
                {
                    this.UpdateDropTarget(Canvas.GetTop(this.dragIndicator) + this.dragIndicator.Height + 1, false);
                }
                else
                {
                    this.UpdateDropTarget(Canvas.GetTop(this.dragIndicator) - 1, false);
                }
            }

            double dragItemHeight = this.dragIndicator.Height;

            TranslateTransform translation = (TranslateTransform)this.dragIndicator.RenderTransform;
            double top = Canvas.GetTop(this.dragIndicator);
            translation.Y = Math.Max(-top - 0.01, Math.Min(e.CumulativeManipulation.Translation.Y,
                this.dragInterceptorRect.Height - top - dragItemHeight + 0.01));
            double y = top + translation.Y;
            this.UpdateDropTarget(y + dragItemHeight / 2, true);

            // Check if we're within the margin where auto-scroll needs to happen.
            bool scrolling = (this.dragScrollDelta != 0);
            double autoScrollMargin = this.AutoScrollMargin;
            if (autoScrollMargin > 0 && y < autoScrollMargin)
            {
                this.dragScrollDelta = y - autoScrollMargin;
                if (!scrolling)
                {
                    this.Dispatcher.BeginInvoke(() => this.DragScroll());
                    return;
                }
            }
            else if (autoScrollMargin > 0 && y + dragItemHeight > this.dragInterceptorRect.Height - autoScrollMargin)
            {
                this.dragScrollDelta = (y + dragItemHeight - (this.dragInterceptorRect.Height - autoScrollMargin));
                if (!scrolling)
                {
                    this.Dispatcher.BeginInvoke(() => this.DragScroll());
                    return;
                }
            }
            else
            {
                // We're not within the auto-scroll margin. This ensures any current scrolling is stopped.
                this.dragScrollDelta = 0;
            }
        }

        /// <summary>
        /// Called when the user releases a drag. Moves the item within the source list and then resets everything.
        /// </summary>
        private void dragInterceptor_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (this.dragItem == null)
            {
                return;
            }

            this.dragIndicator.Visibility = Visibility.Collapsed;
            ((TranslateTransform)this.dragIndicator.RenderTransform).Y = 0;
            this.dragIndicator.Source = null;

            if (this.dropTargetIndex >= 0)
            {
                this.MoveItem(this.dragItem, this.dropTargetIndex);
            }

            if (this.dragItemContainer != null)
            {
                this.dragItemContainer.Visibility = Visibility.Visible;
                VisualStateManager.GoToState(this.dragItemContainer, ReorderListBoxItem.NotDraggingState, true);
                this.dragItemContainer = null;
                this.dragItem = null;
            }

            this.dragScrollDelta = 0;
            this.dropTargetIndex = -1;
            this.ClearDropTarget();
        }

        /// <summary>
        /// Automatically scrolls for as long as the drag is held within the margin.
        /// The speed of the scroll is adjusted based on the depth into the margin.
        /// </summary>
        private void DragScroll()
        {
            if (this.dragScrollDelta != 0)
            {
                double scrollRatio = this.scrollViewer.ViewportHeight / this.scrollViewer.RenderSize.Height;
                double adjustedDelta = this.dragScrollDelta * scrollRatio;
                double newOffset = this.scrollViewer.VerticalOffset + adjustedDelta;
                this.scrollViewer.ScrollToVerticalOffset(newOffset);
                if (this.scrollViewer.VerticalOffset == newOffset)
                {
                    this.Dispatcher.BeginInvoke(() => this.DragScroll());
                }

                double dragItemOffset = Canvas.GetTop(this.dragIndicator) +
                    ((TranslateTransform)this.dragIndicator.RenderTransform).Y +
                    this.dragIndicator.Height / 2;
                this.UpdateDropTarget(dragItemOffset, true);
            }
        }

        /// <summary>
        /// Updates spacing (drop target indicators) surrounding the targeted region.
        /// </summary>
        /// <param name="dragItemOffset">Vertical offset into the items panel where the drag is currently targeting.</param>
        /// <param name="showTransition">True if the drop-indicator transitions should be shown.</param>
        private void UpdateDropTarget(double dragItemOffset, bool showTransition)
        {
            Point dragPoint = ReorderListBox.GetHostCoordinates(
                new Point(this.dragInterceptorRect.Left, this.dragInterceptorRect.Top + dragItemOffset));
            IEnumerable<UIElement> targetElements = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, this.itemsPanel);
            ReorderListBoxItem targetItem = targetElements.OfType<ReorderListBoxItem>().FirstOrDefault();
            if (targetItem != null)
            {
                GeneralTransform targetTransform = targetItem.DragHandle.TransformToVisual(this.dragInterceptor);
                Rect targetRect = targetTransform.TransformBounds(new Rect(new Point(0, 0), targetItem.DragHandle.RenderSize));
                double targetCenter = (targetRect.Top + targetRect.Bottom) / 2;

                int targetIndex = this.itemsPanel.Children.IndexOf(targetItem);
                int childrenCount = this.itemsPanel.Children.Count;
                bool after = dragItemOffset > targetCenter;

                ReorderListBoxItem indicatorItem = null;
                if (!after && targetIndex > 0)
                {
                    ReorderListBoxItem previousItem = (ReorderListBoxItem)this.itemsPanel.Children[targetIndex - 1];
                    if (previousItem.Tag as string == ReorderListBoxItem.DropAfterIndicatorState)
                    {
                        indicatorItem = previousItem;
                    }
                }
                else if (after && targetIndex < childrenCount - 1)
                {
                    ReorderListBoxItem nextItem = (ReorderListBoxItem)this.itemsPanel.Children[targetIndex + 1];
                    if (nextItem.Tag as string == ReorderListBoxItem.DropBeforeIndicatorState)
                    {
                        indicatorItem = nextItem;
                    }
                }
                if (indicatorItem == null)
                {
                    targetItem.DropIndicatorHeight = this.dragIndicator.Height;
                    string dropIndicatorState = after ?
                        ReorderListBoxItem.DropAfterIndicatorState : ReorderListBoxItem.DropBeforeIndicatorState;
                    VisualStateManager.GoToState(targetItem, dropIndicatorState, showTransition);
                    targetItem.Tag = dropIndicatorState;
                    indicatorItem = targetItem;
                }

                for (int i = targetIndex - 5; i <= targetIndex + 5; i++)
                {
                    if (i >= 0 && i < childrenCount)
                    {
                        ReorderListBoxItem nearbyItem = (ReorderListBoxItem)this.itemsPanel.Children[i];
                        if (nearbyItem != indicatorItem)
                        {
                            VisualStateManager.GoToState(nearbyItem, ReorderListBoxItem.NoDropIndicatorState, showTransition);
                            nearbyItem.Tag = ReorderListBoxItem.NoDropIndicatorState;
                        }
                    }
                }

                this.UpdateDropTargetIndex(targetItem, after);
            }
        }

        /// <summary>
        /// Updates the targeted index -- that is the index where the item will be moved to if dropped at this point.
        /// </summary>
        private void UpdateDropTargetIndex(ReorderListBoxItem targetItemContainer, bool after)
        {
            int dragItemIndex = this.Items.IndexOf(this.dragItem);
            int targetItemIndex = this.Items.IndexOf(targetItemContainer.Content);

            int newDropTargetIndex;
            if (targetItemIndex == dragItemIndex)
            {
                newDropTargetIndex = dragItemIndex;
            }
            else
            {
                newDropTargetIndex = targetItemIndex + (after ? 1 : 0) - (targetItemIndex >= dragItemIndex ? 1 : 0);
            }

            if (newDropTargetIndex != this.dropTargetIndex)
            {
                this.dropTargetIndex = newDropTargetIndex;
            }
        }

        /// <summary>
        /// Hides any drop-indicators that are currently visible.
        /// </summary>
        private void ClearDropTarget()
        {
            foreach (ReorderListBoxItem itemContainer in this.itemsPanel.Children)
            {
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NoDropIndicatorState, false);
                itemContainer.Tag = null;
            }
        }

        /// <summary>
        /// Moves an item to a specified index in the source list.
        /// </summary>
        private bool MoveItem(object item, int toIndex)
        {
            object itemsSource = this.ItemsSource;

            System.Collections.IList sourceList = itemsSource as System.Collections.IList;
            if (!(sourceList is System.Collections.Specialized.INotifyCollectionChanged))
            {
                // If the source does not implement INotifyCollectionChanged, then there's no point in
                // changing the source because changes to it will not be synchronized with the list items.
                // So, just change the ListBox's view of the items.
                sourceList = this.Items;
            }

            int fromIndex = sourceList.IndexOf(item);
            if (fromIndex != toIndex)
            {
                sourceList.RemoveAt(fromIndex);
                sourceList.Insert(toIndex, item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets host coordinates, adjusting for orientation. This is helpful when identifying what
        /// controls are under a point.
        /// </summary>
        private static Point GetHostCoordinates(Point point)
        {
            PhoneApplicationFrame frame = (PhoneApplicationFrame)Application.Current.RootVisual;
            switch (frame.Orientation)
            {
                case PageOrientation.LandscapeLeft: return new Point(frame.RenderSize.Width - point.Y, point.X);
                case PageOrientation.LandscapeRight: return new Point(point.Y, frame.RenderSize.Height - point.X);
                default: return point;
            }
        }
    }
}
