using Syncfusion.Maui.Core.Internals;
using Syncfusion.Maui.ListView;

namespace RpdPlayerApp.Architecture
{
#if ANDROID
    public class CustomSwipeView : SwipeView,ITapGestureListener
#else
    public class CustomSwipeView : SwipeView
#endif
    {
        public CustomSwipeView()
        {
#if ANDROID
            this.AddGestureListener(this);
#endif
        }

        protected override void OnHandlerChanged()
        {
            if (this.Handler is not null)
            {
#if ANDROID
               (this.Handler.PlatformView as Android.Views.View)!.Touch += CustomSwipeView_Touch;
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

        public void OnTap(TapEventArgs e)
        {
            var parent = this.GetParent(this);

            if (parent is not ListViewGroupHeaderItem) { return; }

            ((ListViewGroupHeaderItem)parent).OnTap(e);
        }

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
