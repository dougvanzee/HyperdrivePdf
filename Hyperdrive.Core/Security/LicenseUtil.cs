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
        private bool bLocalLicenseExists = false;
        private int LicenseCountdown = -1;
        private int bOnlineLicenseStatus = -1; // -1 = Doesn't exist, 0 = Inactive, 1 = Active 

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
        }

        private void CheckLocalLicense()
        {
            if (File.Exists(@"C:\ProgramData\Displace\hyperdrive.lic"))
            {
                Console.WriteLine("Local license found");
            }
            else
            {
                Console.WriteLine("Local license not found");
            }
        }

        private void CheckOnlineLicense()
        {
            WebClient client = new WebClient();
            String content;

            try
            {
                Stream stream = client.OpenRead("http://www.displace.international/hyperdrive.lic");
                StreamReader reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                Console.WriteLine("Online license found");
            }
            catch (Exception)
            {
                Console.WriteLine("Online license not found");
            }

        }
    }
}
