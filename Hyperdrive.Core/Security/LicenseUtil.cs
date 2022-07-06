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
        private string LocalLicenseFilePath = @"C:\ProgramData\Displace\hyperdrive.lic";
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
                Console.WriteLine("License has been deactivated and set local license to 0");

            if (OnlineLicenseStatus == -1 && LocalLicenseStatus == -1)
                Console.WriteLine("Neither license exists so close program");

            if (OnlineLicenseStatus == 1 && LocalLicenseStatus == -1)
                CreateLocalLicense();

            if (OnlineLicenseStatus == -1 && LocalLicenseStatus > 0);

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
            LocalLicenseStatus--;

            if (LocalLicenseStatus == 0)
            {
                Console.WriteLine("License is over and needs to connect to server");
            }
            else
            {
                Console.WriteLine("Display number of uses left");
            }
        }
    }
}
