using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hyperdrive.Core.Interfaces;

namespace Hyperdrive.Core.Security
{
    public class LicenseUtil : ILicenseInfo
    {
        private int OnlineLicenseStatus = -1; // -1 = Doesn't exist, 0 = Inactive, 1 = Active 
        private string LocalLicenseFilePath = @"C:\ProgramData\Displace\license.lic";
        private string OnlineLicenseFilePath = "http://www.displace.international/hyperdrive.lic";
        private bool _LicenseActive = false;
        private int _DaysLeftInLocalLicense = -1;

        public bool LicenseActive { get { return _LicenseActive; } }
        public int DaysLeftInLicense { get { return _DaysLeftInLocalLicense; } }

        public event EventHandler LicenseInfoComplete;

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
            CheckOnlineLicense();
            CheckLocalLicense();

            // If online is deactivated
            if (OnlineLicenseStatus == 0)
            {
                _LicenseActive = false;
                SetLicenseDays(-2);
            }

            // If online is active and local was previously deactivated
            else if (OnlineLicenseStatus == 1 && _DaysLeftInLocalLicense == -2)
            {
                _LicenseActive = true;
                SetLicenseDays(6);
            }

            // If both licenses don't exist or can't be found
            else if (OnlineLicenseStatus == -1 && _DaysLeftInLocalLicense == -1)
            {
                _LicenseActive = false;
                SetLicenseDays(-1);
            }

            // If online is active but local not created
            else if (OnlineLicenseStatus == 1 && _DaysLeftInLocalLicense == -1)
            {
                _LicenseActive = true;
                CreateLocalLicense();
            }
                
            // If online is active and local exists
            else if (OnlineLicenseStatus == 1 && _DaysLeftInLocalLicense >= 0)
            {
                _LicenseActive = true;
                SetLicenseDays(6);
            }

            // If online can't be found and local exists
            else if (OnlineLicenseStatus == -1 && _DaysLeftInLocalLicense >= 0)
            {
                _LicenseActive = false;
                ReduceLicenseDays();
            }

            LicenseInfoComplete(this, EventArgs.Empty);

        }

        private void CheckLocalLicense()
        {
            if (File.Exists(LocalLicenseFilePath))
            {
                _DaysLeftInLocalLicense = Int32.Parse(File.ReadAllText(LocalLicenseFilePath));
            }
            else
            {
                _DaysLeftInLocalLicense = -1;
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

                OnlineLicenseStatus = content == "Active" ?  1 : 0;
            }
            catch (Exception)
            {
                OnlineLicenseStatus = -1;
            }

        }

        private void CreateLocalLicense()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LocalLicenseFilePath));

                File.WriteAllText(LocalLicenseFilePath, "6");
                _DaysLeftInLocalLicense = 6;
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to create local license");
            }

        }

        private void SetLicenseDays(int days)
        {
            _DaysLeftInLocalLicense = days;
            if (File.Exists(LocalLicenseFilePath))
            {
                File.WriteAllText(LocalLicenseFilePath, days.ToString());

            }
        }

        private void ReduceLicenseDays()
        {
            if (File.Exists(LocalLicenseFilePath))
            {
                int days = Int32.Parse(File.ReadAllText(LocalLicenseFilePath));
                if (days > 0)
                    days--;
                else
                    days = 0;

                File.WriteAllText(LocalLicenseFilePath, days.ToString());
                _DaysLeftInLocalLicense = days;
            }
        }

    }
}
