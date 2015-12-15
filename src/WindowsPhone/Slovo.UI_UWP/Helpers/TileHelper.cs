using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace WindowsPhoneUWP.UpgradeHelpers
{
    public abstract class TileHelper
    {      
        /// <summary>
        /// Use for create a Badge 
        /// </summary>
        public  int? Count { get; set; }
        public  string  Title { get; set; }
        public Windows.UI.Color BackgroundColor { get; set; }
        public abstract TileNotification GetNotificacion();
        public abstract BadgeNotification GetBadge();
    }
}
