namespace Slovo.UI
{
   using Slovo.Resources;

   /// <summary>
   /// A class with the property exposing localization resources
   /// </summary>
   public sealed class LocalizedStrings
   {

      public LocalizedStrings()
      {
      }
      private static CommonResources commonResources = new CommonResources();

      /// <summary>
      /// Gets a reference to localization resources 
      /// </summary>
      public CommonResources CommonResources
      {
         get
         {
            return commonResources;
         }
      }

   }

}