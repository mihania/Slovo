namespace Slovo.Controls
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Extends ListBoxItem to support a styleable drag handle, drop-indicator spacing,
    /// and visual states and transitions for dragging/dropping and enabling/disabling the reorder capability.
    /// </summary>
    [TemplatePart(Name = ReorderListBoxItem.DragHandlePart, Type = typeof(Windows.UI.Xaml.Controls.ContentPresenter))]
    [TemplatePart(Name = ReorderListBoxItem.DropBeforeSpacePart, Type = typeof(Windows.UI.Xaml.UIElement))]
    [TemplatePart(Name = ReorderListBoxItem.DropAfterSpacePart, Type = typeof(Windows.UI.Xaml.UIElement))]
    [TemplateVisualState(Name = ReorderListBoxItem.ReorderDisabledState, GroupName = ReorderListBoxItem.ReorderEnabledStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.ReorderEnabledState, GroupName = ReorderListBoxItem.ReorderEnabledStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.NotDraggingState, GroupName = ReorderListBoxItem.DraggingStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.DraggingState, GroupName = ReorderListBoxItem.DraggingStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.NoDropIndicatorState, GroupName = ReorderListBoxItem.DropIndicatorStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.DropBeforeIndicatorState, GroupName = ReorderListBoxItem.DropIndicatorStateGroup)]
    [TemplateVisualState(Name = ReorderListBoxItem.DropAfterIndicatorState, GroupName = ReorderListBoxItem.DropIndicatorStateGroup)]
    public class ReorderListBoxItem : Windows.UI.Xaml.Controls.ListBoxItem
    {
        #region Template part name constants
        public const string DragHandlePart = "DragHandle";
        public const string DropBeforeSpacePart = "DropBeforeSpace";
        public const string DropAfterSpacePart = "DropAfterSpace";
        #endregion
        #region Visual state name constants
        public const string ReorderEnabledStateGroup = "ReorderEnabledStates";
        public const string ReorderDisabledState = "ReorderDisabled";
        public const string ReorderEnabledState = "ReorderEnabled";
        public const string DraggingStateGroup = "DraggingStates";
        public const string NotDraggingState = "NotDragging";
        public const string DraggingState = "Dragging";
        public const string DropIndicatorStateGroup = "DropIndicatorStates";
        public const string NoDropIndicatorState = "NoDropIndicator";
        public const string DropBeforeIndicatorState = "DropBeforeIndicator";
        public const string DropAfterIndicatorState = "DropAfterIndicator";
        #endregion

        /// <summary>
        /// Creates a new ReorderListBoxItem and sets the default style key.
        /// The style key is used to locate the control template in Generic.xaml.
        /// </summary>
        public ReorderListBoxItem()
        {
            this.DefaultStyleKey = typeof(ReorderListBoxItem);
        }
        #region DropIndicatorHeight DependencyProperty
        public static readonly Windows.UI.Xaml.DependencyProperty DropIndicatorHeightProperty = Windows.UI.Xaml.DependencyProperty.Register("DropIndicatorHeight", typeof(double), typeof(ReorderListBoxItem), new PropertyMetadata(0.0, (d, e) => ((ReorderListBoxItem)d).OnDropIndicatorHeightChanged(e)));

        /// <summary>
        /// Gets or sets the height of the drop-before and drop-after indicators.
        /// The drop-indicator visual states and transitions are automatically updated to use this height.
        /// </summary>
        public double DropIndicatorHeight
        {
            get
            {
                return (int)this.GetValue(ReorderListBoxItem.DropIndicatorHeightProperty);
            }
            set
            {
                this.SetValue(ReorderListBoxItem.DropIndicatorHeightProperty, value);
            }
        }

        /// <summary>
        /// Updates the drop-indicator height value for visual state and transition animations.
        /// </summary>
        /// <remarks>
        /// This is a workaround for the inability of visual states and transitions to do template binding
        /// in Silverlight 3. In SL4, they could bind directly to the DropIndicatorHeight property instead.
        /// </remarks>
        protected void OnDropIndicatorHeightChanged(Windows.UI.Xaml.DependencyPropertyChangedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Panel rootPanel = (Windows.UI.Xaml.Controls.Panel)Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(this, 0);
            Windows.UI.Xaml.VisualStateGroup vsg = ReorderListBoxItem.GetVisualStateGroup(rootPanel, ReorderListBoxItem.DropIndicatorStateGroup);
            if (vsg != null)
            {
                foreach (Windows.UI.Xaml.VisualState vs in vsg.States)
                {
                    foreach (Windows.UI.Xaml.Media.Animation.Timeline animation in vs.Storyboard.Children)
                    {
                        this.UpdateDropIndicatorAnimationHeight((double)e.NewValue, animation);
                    }
                }
                foreach (VisualTransition vt in vsg.Transitions)
                {
                    foreach (Windows.UI.Xaml.Media.Animation.Timeline animation in vt.Storyboard.Children)
                    {
                        this.UpdateDropIndicatorAnimationHeight((double)e.NewValue, animation);
                    }
                }
            }
        }

        /// <summary>
        /// Helper for the UpdateDropIndicatorAnimationHeight method.
        /// </summary>
        private void UpdateDropIndicatorAnimationHeight(double height, Windows.UI.Xaml.Media.Animation.Timeline animation)
        {
            Windows.UI.Xaml.Media.Animation.DoubleAnimation da = animation as Windows.UI.Xaml.Media.Animation.DoubleAnimation;
            if (da != null)
            {
                string targetName = Windows.UI.Xaml.Media.Animation.Storyboard.GetTargetName(da);

                // ToDo: Verify this change I did it during migration to UWP
                var targetPath = Storyboard.GetTargetProperty(da);
                if ((targetName == ReorderListBoxItem.DropBeforeSpacePart || targetName == ReorderListBoxItem.DropAfterSpacePart) && targetPath != null && targetPath == "Height")
                {
                    if (da.From > 0 && da.From != height)
                    {
                        da.From = height;
                    }
                    if (da.To > 0 && da.To != height)
                    {
                        da.To = height;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a named VisualStateGroup for a framework element.
        /// </summary>
        private static Windows.UI.Xaml.VisualStateGroup GetVisualStateGroup(Windows.UI.Xaml.FrameworkElement element, string groupName)
        {
            Windows.UI.Xaml.VisualStateGroup result = null;
            var groups = Windows.UI.Xaml.VisualStateManager.GetVisualStateGroups(element);
            if (groups != null)
            {
                foreach (Windows.UI.Xaml.VisualStateGroup group in groups)
                {
                    if (group.Name == groupName)
                    {
                        result = group;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region IsReorderEnabled DependencyProperty
        public static readonly Windows.UI.Xaml.DependencyProperty IsReorderEnabledProperty = Windows.UI.Xaml.DependencyProperty.Register("IsReorderEnabled", typeof(bool), typeof(ReorderListBoxItem), new PropertyMetadata(false, (d, e) => ((ReorderListBoxItem)d).OnIsReorderEnabledChanged(e)));

        /// <summary>
        /// Gets or sets a value indicating whether the drag handle should be shown.
        /// </summary>
        public bool IsReorderEnabled
        {
            get
            {
                return (bool)this.GetValue(ReorderListBoxItem.IsReorderEnabledProperty);
            }
            set
            {
                this.SetValue(ReorderListBoxItem.IsReorderEnabledProperty, value);
            }
        }

        protected void OnIsReorderEnabledChanged(Windows.UI.Xaml.DependencyPropertyChangedEventArgs e)
        {
            string visualState = (bool)e.NewValue ? ReorderListBoxItem.ReorderEnabledState : ReorderListBoxItem.ReorderDisabledState;
            Windows.UI.Xaml.VisualStateManager.GoToState(this, visualState, true);
        }
        #endregion

        #region DragHandleTemplate DependencyProperty
        public static readonly Windows.UI.Xaml.DependencyProperty DragHandleTemplateProperty = Windows.UI.Xaml.DependencyProperty.Register("DragHandleTemplate", typeof(DataTemplate), typeof(ReorderListBoxItem), null);

        /// <summary>
        /// Gets or sets the template for the drag handle.
        /// </summary>
        public DataTemplate DragHandleTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ReorderListBoxItem.DragHandleTemplateProperty);
            }
            set
            {
                this.SetValue(ReorderListBoxItem.DragHandleTemplateProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Gets the element (control template part) that serves as a handle for dragging the item. 
        /// </summary>
        public Windows.UI.Xaml.Controls.ContentPresenter DragHandle { get; private set; }

        /// <summary>
        /// Applies the control template, checks for required template parts, and initializes visual states.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DragHandle = this.GetTemplateChild(ReorderListBoxItem.DragHandlePart) as Windows.UI.Xaml.Controls.ContentPresenter;
            if (this.DragHandle == null)
            {
                throw new InvalidOperationException("ReorderListBoxItem must have a DragHandle ContentPresenter part.");
            }
            Windows.UI.Xaml.VisualStateManager.GoToState(this, ReorderListBoxItem.ReorderDisabledState, false);
            Windows.UI.Xaml.VisualStateManager.GoToState(this, ReorderListBoxItem.NotDraggingState, false);
            Windows.UI.Xaml.VisualStateManager.GoToState(this, ReorderListBoxItem.NoDropIndicatorState, false);
        }

    }

}