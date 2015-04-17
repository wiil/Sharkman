using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightCalendar.SilverlightCalendarService;

namespace SilverlightCalendar
{
    public delegate void ItemContextMenuOpeningEventHandler(object sender, Item item, ContextMenu contextMenu);
    public delegate void CalendarItemContextMenuOpeningEventHandler(object sender, CalendarItem item, ContextMenu contextMenu);
}