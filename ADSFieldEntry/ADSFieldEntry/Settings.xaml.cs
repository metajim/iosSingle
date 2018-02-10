using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ADSFieldEntry
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Settings : ContentPage
	{
        int m_WipeIncrement=0;
        Options m_Options;
		public Settings ()
		{
			InitializeComponent ();

            m_Options = new Options();
            m_Options.LoadOptionsFromDB();

            txtCompanyID.Text = m_Options.CompanyID;
            txtWebFolder.Text = m_Options.WebFolder;
            txtProbeName.Text = m_Options.ProbeName;

          
		}

        private void OK_Clicked(object sender, EventArgs e)
        {
            if (txtPassword.Text.ToUpper() == "ADSOK")
            {
                viewPass.IsVisible = false;
                viewSettings.IsVisible = true;
            }
            else if (txtPassword.Text.ToUpper() == "TOUCHMEMORY")
            {
                m_WipeIncrement++;

                if (m_WipeIncrement == 1)
                    lblWipe.Text = "Press OK 2 more times to wipe database";
                else if (m_WipeIncrement == 2)
                    lblWipe.Text = "Press OK 1 more times to wipe database";
                else if (m_WipeIncrement == 3)
                {
                    DataAccess.WipeDatabase();
                    lblWipe.Text = "Collected Data Wiped!  Press OK again to wipe badges";
                }
                else if(m_WipeIncrement>=4)
                {
                    DataAccess.WipeDatabase();
                    lblWipe.Text = "Definition Data Wiped!";
                }
            }
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            m_Options.WebFolder = txtWebFolder.Text;
            m_Options.CompanyID = txtCompanyID.Text;
            m_Options.ProbeName = txtProbeName.Text;

            DataAccess.SaveOptions(m_Options);

            Navigation.PopAsync();
        }

    }
}