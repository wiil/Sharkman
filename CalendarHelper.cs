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
using System.Collections.Generic;
using SilverlightCalendar.Model;

namespace SilverlightCalendar
{
    public class CalendarHelper
    {
        public static SolidColorBrush NormalCalendarItemBrush = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
        public static SolidColorBrush TeamMemberCalendarItemBrush = new SolidColorBrush(Color.FromArgb(255, 0, 204, 122));
        public static SolidColorBrush SelectedCalendarItemBrush = new SolidColorBrush(Color.FromArgb(255, 255, 128, 64));
        public static SolidColorBrush AbsentCalendarItemBrush = new SolidColorBrush(Color.FromArgb(255, 158, 43, 62));
        public static SolidColorBrush CalendarItemSumBrush = new SolidColorBrush(Color.FromArgb(255, 111, 153, 53));
        public static SolidColorBrush InvalidCalendarItemBrush = new SolidColorBrush(Color.FromArgb(255, 7, 94, 148));
        public static SolidColorBrush CalendarDataGridHolidayBrush = new SolidColorBrush(Color.FromArgb(136, 136, 255, 0));
        public static SolidColorBrush CalendarDataGridWeekendBrush = new SolidColorBrush(Color.FromArgb(40, 51, 180, 255));
        public static SolidColorBrush CalendarDataGridMainBackground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        
        public static SolidColorBrush DefaultItemBrush = new SolidColorBrush(Color.FromArgb(255, 70, 130, 180));
        public static SolidColorBrush KrankItemBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 255));

        public static Dictionary<int, SolidColorBrush> AuftragsartenColors { get; set; }
        public static BaseDataObject BaseData = new BaseDataObject();
        public static FilterObject CurrentCalendarItemFilter { get; set; }
        public static Dictionary<int, string> AuftragsArtenBezeichung { get; set; }   

        public static SolidColorBrush IsValidOpacity(SolidColorBrush brush, bool valid)
        {
            if (brush != null)
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = brush.Color;
                if (valid)
                { solidColorBrush.Opacity = 1.0; }
                else
                { solidColorBrush.Opacity = 0.3; }
                return solidColorBrush;
            }

            return null;
        }

        public static SolidColorBrush GetCalendarItemBackground(object parameter)
        {
            string status = parameter.ToString();
            if (status == "DefaultCalendarItem")
            {
                return NormalCalendarItemBrush;
            }
            else if (status == "TeamMember")
            {
                return TeamMemberCalendarItemBrush;
            }
            else if (status == "DefaultItem")
            {
                return DefaultItemBrush;
            }
            else if (status == "Krank")
            {
                return KrankItemBrush;
            }
            else
            {
                return null;
            }
        }

        public static SolidColorBrush GetItemBackground(int auftragsArtId)
        {
            if (CalendarHelper.AuftragsartenColors.ContainsKey(auftragsArtId))
            {
                return AuftragsartenColors[auftragsArtId];
            }
            else
            {
                return DefaultItemBrush;
            }
        }

        public static Uri GetImageUri(string relativeImagePath)
        {
            CalendarHelper moduleHelper = new CalendarHelper();
            Uri imageUri = new Uri("/" + moduleHelper.GetType().Assembly.ToString().Split(',')[0] + ";component/" + relativeImagePath, UriKind.Relative);

            return imageUri;
        }
    }
}
