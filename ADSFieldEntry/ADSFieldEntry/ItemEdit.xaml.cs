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
	public partial class ItemEdit : ContentPage
	{
        public string m_Profile { get; set; }
        public string m_StartView { get; set; }
        public string m_EndView { get; set; }
        public int m_Index { get; set; }


        public bool m_SaveOK = false;

		public ItemEdit ()
		{
			InitializeComponent ();

            
        }

        private string ValidateEntry(string UseValue)
        {
         
            
            string strValue = UseValue;
            string strResult;
            string strPart;

            //start
            strValue += ":::";
            strResult = "";


            //hours
            strPart = strValue.Substring(0, strValue.IndexOf(":"));
            strValue = strValue.Substring(strValue.IndexOf(":") + 1);
            if (IsNumeric(strPart))
            {
                while (strPart.Length < 0)
                    strPart = "0" + strPart;                
            }
            else
                strPart = "00";

            strResult = strPart + ":";

            //minutes
            strPart = strValue.Substring(0, strValue.IndexOf(":"));
            strValue = strValue.Substring(strValue.IndexOf(":") + 1);
            if (IsNumeric(strPart))
            {
                while (strPart.Length < 0)
                    strPart = "0" + strPart;
            }
            else
                strPart = "00";

            strResult += strPart + ":";

            //seconds
            strPart = strValue.Substring(0, strValue.IndexOf(":"));
            strValue = strValue.Substring(strValue.IndexOf(":") + 1);
            if (IsNumeric(strPart))
            {
                while (strPart.Length < 0)
                    strPart = "0" + strPart;
            }
            else
                strPart = "00";

            strResult += strPart;


            return strResult;
        }
        private void Save_Click(object sender, EventArgs e)
        {
            txtStart.Text = ValidateEntry(txtStart.Text);
            txtEnd.Text = ValidateEntry(txtEnd.Text);

            if (txtStart.Text != "00:00:00" && txtEnd.Text != "00:00:00")
            {
                m_SaveOK = true;
                m_Profile = txtProfile.Text;
                m_StartView = txtStart.Text;
                m_EndView = txtEnd.Text;

                Navigation.PopAsync();
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            
            txtProfile.Text = m_Profile;
            txtStart.Text = m_StartView;
            txtEnd.Text = m_EndView;

           
        }
        private bool IsNumeric(string UseValue)
        {
            bool result = true;
            foreach (char c in UseValue.ToCharArray())
            {
                if (c < '0' || c > '9')
                {
                    result = false;
                    break;
                    
                }
            }
            return result;
        }
        private bool IsTimeNumeric(string UseValue)
        {
            bool result = true;
            foreach (char c in UseValue.ToCharArray())
            {
                if(c<'0' || c>'9')
                {
                    if(c!=':')
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
        private void txtStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTimeNumeric(e.NewTextValue))
                txtStart.Text = e.OldTextValue;
        }
    }
}