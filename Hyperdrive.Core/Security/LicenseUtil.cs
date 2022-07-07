using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.Security
{
    public class LicenseUtil
    {
        private int LocalLicenseStatus = -1;
        private int OnlineLicenseStatus = -1; // -1 = Doesn't exist, 0 = Inactive, 1 = Active 
        private string LocalLicenseFilePath = @"C:\ProgramData\Displace\license.lic";
        private string OnlineLicenseFilePath = "http://www.displace.international/hyperdrive.lic";

        public async void StartLicenseCheck()
        {
            try
            {
                await Task.Run(() => LicenseCheck());
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async Task LicenseCheck()
        {
            CheckLocalLicense();
            CheckOnlineLicense();

            if (OnlineLicenseStatus == 0)
                DeactivateLocalLicense();

            if (OnlineLicenseStatus == -1 && LocalLicenseStatus == -1)
                LicenseDoesNotExist();

            if (OnlineLicenseStatus == 1 && LocalLicenseStatus == -1)
                CreateLocalLicense();

            if (OnlineLicenseStatus == 1 && LocalLicenseStatus >= 0)
                RunNormal();

            if (OnlineLicenseStatus == -1 && LocalLicenseStatus >= 0)
                RunTimeLimitMode();

        }

        private void CheckLocalLicense()
        {
            if (File.Exists(LocalLicenseFilePath))
            {
                Console.WriteLine("Local license found");
                LocalLicenseStatus = Int32.Parse(File.ReadAllText(LocalLicenseFilePath));
                Console.WriteLine("Local license status: " + LocalLicenseStatus);
            }
            else
            {
                Console.WriteLine("Local license not found");
                LocalLicenseStatus = -1;
                Console.WriteLine("Local license status: " + LocalLicenseStatus);
            }
        }

        private void CheckOnlineLicense()
        {
            WebClient client = new WebClient();
            String content;

            try
            {
                Stream stream = client.OpenRead(OnlineLicenseFilePath);
                StreamReader reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                Console.WriteLine("Online license found");
                OnlineLicenseStatus = content == "Active" ?  1 : 0;
                Console.WriteLine("Online license status: " + OnlineLicenseStatus);
            }
            catch (Exception)
            {
                OnlineLicenseStatus = -1;
                Console.WriteLine("Online license not found");
                Console.WriteLine("Online license status: " + OnlineLicenseStatus);
            }

        }

        private void CreateLocalLicense()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LocalLicenseFilePath));

            File.WriteAllText(LocalLicenseFilePath, "6");
            LocalLicenseStatus = Int32.Parse(File.ReadAllText(LocalLicenseFilePath));
            Console.WriteLine("Local license status: " + LocalLicenseStatus);
        }

        private void RunTimeLimitMode()
        {
            

            if (LocalLicenseStatus == 0 || LocalLicenseStatus == 1)
            {
                File.WriteAllText(LocalLicenseFilePath, "0");
                LocalLicenseStatus = 0;
                Console.WriteLine("Temporary license is over and needs to connect to license server");
            }
            else
            {
                LocalLicenseStatus--;
                File.WriteAllText(LocalLicenseFilePath, LocalLicenseStatus.ToString());
                Console.WriteLine("Display number of uses left");
            }
        }

        private void RunNormal()
        {
            File.WriteAllText(LocalLicenseFilePath, "6");
            LocalLicenseStatus = 6;
            Console.WriteLine("Run program as normal");
        }

        private void DeactivateLocalLicense()
        {
            File.WriteAllText(LocalLicenseFilePath, "0");
            LocalLicenseStatus = 0;
            Console.WriteLine("License has been deactivated and will no longer run.");
        }

        private void LicenseDoesNotExist()
        {
            Console.WriteLine("License has not been activated and we cannot reach the license server.");
        }
    }
}
