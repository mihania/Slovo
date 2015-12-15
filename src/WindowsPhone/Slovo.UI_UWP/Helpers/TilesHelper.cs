using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using System.Threading.Tasks;

namespace WindowsPhoneUWP.UpgradeHelpers
{
    public static class TilesHelper
    {
        /// <summary>
        ///   This methods ensures that the SecondaryTile instance is initialized as expected.
        /// 
        ///   In contrast to WP8 Tiles,  UWP Tiles require having a value for the following properties:
        ///    - TileId
        ///    - DisplayName
        ///    - Arguments
        ///    - VisualElements.Square150x150Logo
        /// 
        ///  See https://msdn.microsoft.com/en-us/library/windows/apps/br242213.aspx for more details.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="tileUri"></param>
        /// <returns></returns>
        public static SecondaryTile FinishInitialization(this SecondaryTile tile, Uri tileUri)
        {
            var tileId = tileUri.AbsolutePath.Replace('/', '_');

            if (string.IsNullOrWhiteSpace(tile.TileId))
            {
                tile.TileId = tileId;
            }
            if (string.IsNullOrWhiteSpace(tile.DisplayName))
            {
                tile.DisplayName = "-NO DISPLAY NAME-";
            }
            else
            {
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            }
            if (string.IsNullOrWhiteSpace(tile.Arguments))
            {
                var argsFromUri = (tileUri.Query ?? "").TrimStart('?');
                if (string.IsNullOrEmpty(argsFromUri))
                {
                    tile.Arguments = "arg=defaultArg";
                }
                else
                {
                    tile.Arguments = argsFromUri;
                }
            }

            if (tile.VisualElements.Square150x150Logo.AbsolutePath == "/")
            {
                tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            }

            return tile;
        }

        public static Uri GetAsNavigationUri(this SecondaryTile tile)
        {
	    if(tile.TileId == "____dummyApplicationTile")
            {
                return new Uri(tile.TileId, UriKind.Relative);
            }
            string navigation = tile.TileId.Replace('_', '/');
            navigation = navigation.Replace("+", ":");
            navigation = navigation.Replace("%", "?");
            try
            {
                return new Uri(navigation, UriKind.Relative);
            }
            catch (Exception)
            {
                return new Uri(navigation, UriKind.Absolute);
            }
        }


        public static async System.Threading.Tasks.Task<IEnumerable<Windows.UI.StartScreen.SecondaryTile>> GetActiveTiles()
        {          

            var tiles = (System.Collections.Generic.IEnumerable<Windows.UI.StartScreen.SecondaryTile>)await Windows.UI.StartScreen.SecondaryTile.FindAllForPackageAsync();
            return new[] { new Windows.UI.StartScreen.SecondaryTile("____dummyApplicationTile") }.Concat(tiles);
        }

        public static async Task UpdateHelper(SecondaryTile tile, TileHelper notification)
        {           
            if (tile.TileId == "____dummyApplicationTile")
            {
                Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(notification.GetNotificacion());
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(notification.GetBadge());
                               
            }
            else
            {
                tile.VisualElements.BackgroundColor = notification.BackgroundColor != null ? notification.BackgroundColor : tile.VisualElements.BackgroundColor;
                tile.DisplayName = (notification.Title != null && notification.Title != "") ? notification.Title : tile.DisplayName;
                Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(notification.GetNotificacion());
                BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tile.TileId).Update(notification.GetBadge());
                await tile.UpdateAsync();
            }
        }

        public static async Task CreateHelper(Uri navigationUri, TileHelper xmlstring)
        {
            string tileId = SetNavigationUri(navigationUri);
            string shortName = xmlstring.Title;
            string arguments = "arg=defaultArg";

            if (string.IsNullOrWhiteSpace(xmlstring.Title))
            {
                shortName = "-NO DISPLAY NAME-";
            }
            if (navigationUri != null)
            {
                char[] Sign = "%".ToCharArray();
                var argsFromUri = tileId.Substring(tileId.IndexOf(Sign[0]) + 1, tileId.Length - (tileId.IndexOf(Sign[0]) + 1));
                if (tileId.IndexOf(Sign[0]) > 0 && string.IsNullOrEmpty(argsFromUri))
                {
                    arguments = "arg=defaultArg";
                }
                else
                {
                    arguments = argsFromUri;
                }
            }
            Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            Uri logoReference = new Uri("ms-appx:///Assets/ApplicationIcon.png");
            SecondaryTile Tile = new SecondaryTile(tileId);
            Tile.Arguments = arguments;
            Tile.DisplayName = (xmlstring.Title != null && xmlstring.Title != "") ? xmlstring.Title : "-NO DISPLAY NAME-";
            Tile.VisualElements.Square150x150Logo = square150x150Logo;
            Tile.VisualElements.BackgroundColor = xmlstring.BackgroundColor != null ? xmlstring.BackgroundColor : Tile.VisualElements.BackgroundColor;

            await Tile.RequestCreateAsync();
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForSecondaryTile(Tile.TileId).Update(xmlstring.GetNotificacion());
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(Tile.TileId).Update(xmlstring.GetBadge());
        }

        public static async Task CreateHelper(Uri navigationUri, TileHelper xmlstring, bool wideSupport)
        {
            string tileId = SetNavigationUri(navigationUri); 
            string shortName = xmlstring.Title;
            string arguments = "arg=defaultArg";

            if (string.IsNullOrWhiteSpace(xmlstring.Title))
            {
                shortName = "-NO DISPLAY NAME-";
            }
            if (navigationUri != null)
            {
                char[] Sign = "%".ToCharArray();
                var argsFromUri = tileId.Substring(tileId.IndexOf(Sign[0]) + 1, tileId.Length - (tileId.IndexOf(Sign[0]) + 1));
                if (tileId.IndexOf(Sign[0]) > 0 && string.IsNullOrEmpty(argsFromUri))
                {
                    arguments = "arg=defaultArg";
                }
                else
                {
                    arguments = argsFromUri;
                }
            }
            Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            Uri logoReference = new Uri("ms-appx:///Assets/ApplicationIcon.png");
            Uri wideLogoReference =square150x150Logo;
            SecondaryTile Tile = new SecondaryTile(tileId);
            Tile.VisualElements.BackgroundColor = xmlstring.BackgroundColor != null ? xmlstring.BackgroundColor : Tile.VisualElements.BackgroundColor;
            Tile.Arguments = arguments;
            Tile.DisplayName = (xmlstring.Title != null && xmlstring.Title != "") ? xmlstring.Title : "-NO DISPLAY NAME-";
            Tile.VisualElements.Square150x150Logo = square150x150Logo;
            Tile.VisualElements.Wide310x150Logo = wideSupport ? wideLogoReference : Tile.VisualElements.Wide310x150Logo;

            await Tile.RequestCreateAsync();
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(Tile.TileId).EnableNotificationQueue(true);
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForSecondaryTile(Tile.TileId).Update(xmlstring.GetNotificacion());            
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(Tile.TileId).Update(xmlstring.GetBadge());
            
        }

        private static string SetNavigationUri(Uri navigationUri)
        {
            string navigation = navigationUri.OriginalString.Replace('/', '_');
            navigation = navigation.Replace(":", "+");
            navigation = navigation.Replace("?", "%");
            return navigation;
        }
        
    }
}
