#if EnableWp7
#else
namespace Slovo.Core
{
    using Microsoft.Phone.Shell;

    internal class Tile
    {
        /// <summary>
        /// Updates live tile on shell
        /// </summary>
        /// <param name="directionArticle">Direction Article</param>
        /// <remarks>
        /// Do not use WP7 technique on WP8 with WriteableBitmap, it causes application to fail on resume
        /// http://stackoverflow.com/questions/10221516/wp7-bringing-back-app-from-a-tombstone-state-causes-app-to-crash-using-a-write
        /// </remarks>
        internal static void UpdateLiveTile(Directions.DirectionArticle directionArticle)
        {
            // Loop through any existing Tiles that are pinned to Start.
            foreach (var tileToUpdate in ShellTile.ActiveTiles)
            {
                FlipTileData tileData = new FlipTileData();

                // the text that displays on the front side of the medium and wide tile sizes
                tileData.Title = Slovo.Resources.CommonResources.ApplicationName;

                // the title to display at the bottom of the back of the Tile
                tileData.BackTitle = directionArticle.Sense;

                // the text to display on the back of the Tile, above the title.
                tileData.BackContent = directionArticle.ShortDefinition;

                // the text that displays above the title, on the back-side of the wide Tile size
                tileData.WideBackContent = directionArticle.ShortDefinition;

                // Invoke the new version of ShellTile.Update.
                tileToUpdate.Update(tileData);
            }
        }
    }
}
#endif