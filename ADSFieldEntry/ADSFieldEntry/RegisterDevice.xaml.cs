using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ADSFieldEntry
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterDevice : ContentPage
	{
        Options m_Options;
		public RegisterDevice ()
		{
			InitializeComponent ();

            txtDeviceID.Text = AccountInfo.GetDeviceID();

            m_Options = new Options();
            m_Options.LoadOptionsFromDB();

            txtEmployee.Text = m_Options.EmpName;
            txtCompany.Text = m_Options.CompanyName;
        }

        public void Register_Clicked(object sender, EventArgs e)
        {
            if (txtCompany.Text == "" || txtEmployee.Text == "")
            {
                if (txtCompany.Text == "")
                    txtCompany.BackgroundColor = Color.Pink;
                else
                    txtCompany.BackgroundColor = Color.White;


                if (txtEmployee.Text == "")
                    txtEmployee.BackgroundColor = Color.Pink;
                else
                    txtEmployee.BackgroundColor = Color.White;

                return;
            }

            string UseMe = AccountInformation.GetRegisterNewAccountString(txtEmployee.Text, txtCompany.Text);
            WebInteraction.SendHttpPost(WebInteraction.m_LicenseURL, UseMe);

            Navigation.PopAsync();
        }
	}
}