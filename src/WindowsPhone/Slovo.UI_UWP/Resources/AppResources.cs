//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Slovo.Resources
{
   using System;

   /// <summary>
   ///   A strongly-typed resource class, for looking up localized strings, etc.
   /// </summary>
   // This class was auto-generated by the StronglyTypedResourceBuilder
   // class via a tool like ResGen or Visual Studio.
   // To add or remove a member, edit your .ResX file then rerun ResGen
   // with the /str option, or rebuild your VS project.
   internal class AppResources
   {

      internal AppResources()
      {
      }

      /// <summary>
      ///   Looks up a localized string similar to [Your feedback here]
      ///
      ///
      ///---------------------------------
      ///Device Name: {0}
      ///Device Manufacturer: {1}
      ///Device Firmware Version: {2}
      ///Device Hardware Version: {3}
      ///Application Version: {4}.
      /// </summary>
      internal static string FeedbackBody
      {
         get
         {
            return GetString("FeedbackBody");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to Sorry to hear you didn&apos;t want to rate Slovo.
      ///
      ///Tell us about your experience or suggest how we can make it even better..
      /// </summary>
      internal static string FeedbackMessage1
      {
         get
         {
            return GetString("FeedbackMessage1");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to no thanks.
      /// </summary>
      internal static string FeedbackNo
      {
         get
         {
            return GetString("FeedbackNo");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to Slovo Feedback.
      /// </summary>
      internal static string FeedbackSubject
      {
         get
         {
            return GetString("FeedbackSubject");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to Can we make it better?.
      /// </summary>
      internal static string FeedbackTitle
      {
         get
         {
            return GetString("FeedbackTitle");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to mihania@gmail.com.
      /// </summary>
      internal static string FeedbackTo
      {
         get
         {
            return GetString("FeedbackTo");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to give feedback.
      /// </summary>
      internal static string FeedbackYes
      {
         get
         {
            return GetString("FeedbackYes");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to We&apos;d love you to rate our app 5 stars
      ///
      ///Showing us some love on the store helps us to continue to work on the app and make things even better!.
      /// </summary>
      internal static string RatingMessage1
      {
         get
         {
            return GetString("RatingMessage1");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to You look to be getting a lot of use out of Slovo!
      ///
      ///Do you want to give the application a 5 star rating to show your appreciation?.
      /// </summary>
      internal static string RatingMessage2
      {
         get
         {
            return GetString("RatingMessage2");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to no thanks.
      /// </summary>
      internal static string RatingNo
      {
         get
         {
            return GetString("RatingNo");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to Enjoying Slovo?.
      /// </summary>
      internal static string RatingTitle
      {
         get
         {
            return GetString("RatingTitle");
         }
      }

      /// <summary>
      ///   Looks up a localized string similar to rate 5 stars.
      /// </summary>
      internal static string RatingYes
      {
         get
         {
            return GetString("RatingYes");
         }
      }

      private static string GetString(string key)
      {
         Windows.ApplicationModel.Resources.ResourceLoader loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("AppResources");
         return loader.GetString(key);
      }

   }

}