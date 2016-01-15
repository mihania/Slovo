namespace Slovo
{
    using Microsoft.Phone.Marketplace;
    using System.Reflection;
    using System;
    using System.Net.NetworkInformation;

    internal class Settings
    {
        internal static Version AssemblyVersion
        {
            get
            {
                var CurrentAssembly = Assembly.GetExecutingAssembly().FullName;
                return new Version(CurrentAssembly.Split('=')[1].Split(',')[0]);
            }
        }

        private static bool isNetworkAvailable;
        internal static bool IsNetworkAvailable
        {
            get
            {
                return isNetworkAvailable;
            }
            set
            {
                isNetworkAvailable = value;
            }
        }

        internal static void Init()
        {
            isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
