namespace Slovo
{
    using Microsoft.Phone.Marketplace;
    using System.Reflection;
    using System;
    using System.Net.NetworkInformation;

    internal class Settings
    {
        private static bool? isTrial;

        internal static bool IsTrial
        {
            get
            {
//                if (!isTrial.HasValue)
//                {
//                    LicenseInformation li = new LicenseInformation();
//                    isTrial = li.IsTrial();
//#if DEBUG
//                    // isTrial = true;
//#endif
//                }

//                return isTrial.Value;

                // since 2.0 Slovo is free of charge
                return false;
            }
        }

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
