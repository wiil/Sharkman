using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.DragDrop;
using SilverlightCalendar.SilverlightCalendarService;
using System.Collections.ObjectModel;
using SilverlightCalendar.Model;
using Reisewitz.Core.Framework;

namespace SilverlightCalendar
{
    public partial class MainPage : UserControl
    {

        public MainPage()
        {
            InitializeComponent();

            Framework.Current = new CalendarFramework();

            Calendar.ViewModel = new CalendarControlViewModel(); ;
            Calendar.ItemContextMenuOpening += new ItemContextMenuOpeningEventHandler(Calendar_ItemContextMenuOpening);
            Calendar.CalendarItemContextMenuOpening += new CalendarItemContextMenuOpeningEventHandler(Calendar_CalendarItemContextMenuOpening);
            //RadDragAndDropManager.AddDragQueryHandler(TestListBox, OnDragQueryTest);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            
            SilverlightCalendarService.SilverlightCalendarServiceClient client = new SilverlightCalendarService.SilverlightCalendarServiceClient();
            ServiceCalendarItemFilter filter = new ServiceCalendarItemFilter();
            filter.DateMin = DateTime.Today.AddDays(-6);
            filter.DateMax = DateTime.Today.AddDays(14);
            filter.LoadItems = true;
            Calendar.ViewModel.LoadByFilter(filter);
            Calendar.ViewModel.LoadBaseData();
        }

        void Calendar_CalendarItemContextMenuOpening(object sender, CalendarItem item, ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new MenuItem() { Header = "abc" });
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Calendar.ViewModel.Rows != null)
            //{
            //    foreach (var x in Calendar.ViewModel.Rows.FirstOrDefault().Items.FirstOrDefault().Items)
            //    {
            //        TestListBox.Items.Add(x);
            //    }
            //}
        }

        void Calendar_ItemContextMenuOpening(object sender, Item item, ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new MenuItem() { Header = "xyz" });
            contextMenu.Items.Add(new MenuItem() { Header = "123" });
        }       

        private void OnDragQueryTest(object sender, DragDropQueryEventArgs e)
        {
            var item = (e.Source as ListBox).SelectedItem;
            if (e.Options.Status == DragStatus.DragQuery && item != null)
            {
                e.Options.ArrowCue = RadDragAndDropManager.GenerateArrowCue();
                e.Options.Payload = item;
                ContentControl cc = new ContentControl();
                cc.Content = item;
                e.Options.DragCue = cc;
            }

            e.QueryResult = true;
            e.Handled = true;
        }

        private void OnDragInfoTest(object sender, DragDropEventArgs e)
        {

        }
    }
}
