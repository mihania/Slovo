namespace Slovo
{
    using System.Reflection;
    using System;
    using System.Net.NetworkInformation;

    internal class Settings
    {
        internal static bool IsTrial
        {
            get
            {
                return false;
            }
        }

        internal static Version AssemblyVersion
        {
            get
            {
                var CurrentAssembly = new Settings().GetType().GetTypeInfo().Assembly.FullName;
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