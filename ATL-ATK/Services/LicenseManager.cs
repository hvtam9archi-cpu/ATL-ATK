using System;
using System.Management;

namespace ATL_ATK.Services
{
    public class LicenseManager
    {
        private static LicenseManager _instance;
        public static LicenseManager Instance => _instance ?? (_instance = new LicenseManager());

        public bool IsValid { get; private set; }

        private LicenseManager()
        {
            // Simple validation logic for demonstration
            IsValid = ValidateLicense();
        }

        private bool ValidateLicense()
        {
            try
            {
                // In a real scenario, check HWID against a server or token
                string hwid = GetHardwareId();
                return !string.IsNullOrEmpty(hwid);
            }
            catch
            {
                return false;
            }
        }

        private string GetHardwareId()
        {
            try
            {
                string cpuInfo = string.Empty;
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (cpuInfo == string.Empty)
                    {
                        cpuInfo = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                }
                return cpuInfo;
            }
            catch
            {
                return "UNKNOWN_HWID";
            }
        }
    }
}
