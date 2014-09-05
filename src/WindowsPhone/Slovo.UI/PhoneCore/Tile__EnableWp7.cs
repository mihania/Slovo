#if EnableWp7

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slovo.Core
{
    using System;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using Microsoft.Phone.Shell;

    internal class Tile
    {
        private const string SourceTilePath = "/Slovo.UI;component/Images/Background.png";
        private const int TileLength = 173;

        internal static void UpdateLiveTile(Directions.DirectionArticle directionArticle)
        {
            var text = directionArticle.Sense;
            if (text.Length > 15)
            {
                text = text.Substring(0, 15);
                text += "...";
            }
            // Setup the font style for our tile.
            var fontFamily = new FontFamily("Segoe WP");
            var fontForeground = new SolidColorBrush(Colors.White);
            var tileSize = new Size(TileLength, TileLength);

            // Load our 'cloud' image.
            StreamResourceInfo sr = Application.GetResourceStream(new Uri("/Slovo;component/Images/Background.png", UriKind.Relative));
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(sr.Stream);

            // Create our image as a control, so it can be rendered to the WriteableBitmap.
            Image sourceTile = new Image();
            sourceTile.Source = bmp;
            sourceTile.Width = tileSize.Width;
            sourceTile.Height = tileSize.Height;

            // TextBlock for the text.
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = 20;
            textBlock.Foreground = fontForeground;
            textBlock.FontFamily = fontFamily;

            // Define the filename for our tile. Take note that a tile image *must* be saved in /Shared/ShellContent
            // or otherwise it won't display.
            string tileImage = "/Shared/ShellContent/SlovoDynamicTile.jpg";

            // Define the path to the isolatedstorage, so we can load our generated tile from there.
            string isoStoreTileImage = String.Format("isostore:{0}", tileImage);

            // Open the ISF store, 
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create our bitmap, in our selected dimension.
                WriteableBitmap bitmap = new WriteableBitmap((int)tileSize.Width, (int)tileSize.Height);

                // Render our cloud image
                bitmap.Render(sourceTile, new TranslateTransform()
                {
                    X = 0, // Left margin offset.
                    Y = 0 // Top margin offset.
                });

                // Render the time of the day text.
                bitmap.Render(textBlock, new TranslateTransform()
                {
                    X = 6,
                    Y = 6
                });

                // Create a stream to store our file in.
                var stream = store.CreateFile(tileImage);

                // Invalidate the bitmap to make it actually render.
                bitmap.Invalidate();

                // Save it to our stream.
                bitmap.SaveJpeg(stream, TileLength, TileLength, 0, 100);

                // Close the stream, and by that saving the file to the ISF.
                stream.Close();
            }

            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile tile = ShellTile.ActiveTiles.First();

            // Application should always be found
            if (tile != null)
            {

                // Define our tile data.
                StandardTileData tileData = new StandardTileData
                {
                    BackgroundImage = new Uri(isoStoreTileImage, UriKind.Absolute)
                };

                tile.Update(tileData);
            }
        }
    }
}
#endif