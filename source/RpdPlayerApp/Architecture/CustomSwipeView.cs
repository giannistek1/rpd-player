using Syncfusion.Maui.Core.Internals;
using Syncfusion.Maui.ListView;
using Microsoft.Maui.Controls;
#if IOS
using UIKit;
#endif
#if ANDROID
using Android.Views;
#endif

namespace RpdPlayerApp.Architecture
{
    public class CustomSwipeView : SwipeView
#if ANDROID
    , ITapGestureListener
#endif
    {
        public CustomSwipeView()
        {
#if ANDROID
            this.AddGestureListener(this);
#endif
#if IOS
            var tapGestureRecognizer = new UITapGestureRecognizer(OnTapped);
            (this.Handler?.PlatformView as UIView)?.AddGestureRecognizer(tapGestureRecognizer);
#endif
        }

        protected override void OnHandlerChanged()
        {
            if (this.Handler is not null)
            {
#if ANDROID
                (this.Handler.PlatformView as Android.Views.View)!.Touch += CustomSwipeView_Touch;
#endif
#if IOS
                var uiView = this.Handler.PlatformView as UIView;
                if (uiView != null)
                {
                    var tapGestureRecognizer = new UITapGestureRecognizer(OnTapped);
                    uiView.AddGestureRecognizer(tapGestureRecognizer);
                }
#endif
            }
            base.OnHandlerChanged();
        }

#if ANDROID
        private void CustomSwipeView_Touch(object? sender, Android.Views.View.TouchEventArgs e)
        {
            e.Handled = false;
        }
#endif

#if IOS
        private void OnTapped(UITapGestureRecognizer recognizer)
        {
            // Get the location of the tap (in the view's coordinate space)
            var locationInView = recognizer.LocationInView(recognizer.View);

            // Create a TapEventArgs using the location and tap count (1 for single tap)
            var tapArgs = new TapEventArgs(new Point(locationInView.X, locationInView.Y), (int)recognizer.NumberOfTapsRequired);

            // Call the OnTap method with the TapEventArgs
            OnTap(tapArgs);
        }
#endif

#if ANDROID
        public void OnTap(TapEventArgs e)
        {
            var parent = this.GetParent(this);

            if (parent is not ListViewGroupHeaderItem) { return; }

            ((ListViewGroupHeaderItem)parent).OnTap(e);
        }
#else
        public void OnTap(TapEventArgs e)
        {
            var parent = this.GetParent(this);

            if (parent is not ListViewGroupHeaderItem) { return; }

            ((ListViewGroupHeaderItem)parent).OnTap(e);
        }
#endif

        internal object GetParent(object currentView)
        {
            while (currentView is not null && currentView.GetType() != typeof(ListViewGroupHeaderItem))
            {
                return this.GetParent((currentView as Element).Parent);
            }
            return currentView;
        }
    }
}