using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace ADSFieldEntry
{
	public partial class MainPage : ContentPage
	{
        Options m_Options;
		public MainPage()
		{
			InitializeComponent();
            string iconName = "";
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var res in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
                if(res.EndsWith("ADS_title.png"))
                {
                    iconName = res;
                }
            }

            if(iconName!="")
            {
                imgHead.Source = ImageSource.FromResource(iconName);
                imgHead.Aspect = Aspect.AspectFit;
            }

            DataAccess.OpenDatabase();
            m_Options = new Options();
            m_Options.LoadOptionsFromDB();



        }
        public void Register_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterDevice());
        }
        public void Sync_Clicked(object sender, EventArgs e)
        {
            bool bBadgeReceived = false;
            bool bProfileReceived = false;
            //send data
            //get badge.def
            //get profile.def

            //List<RawDataRecord> RawList = DataAccess.GetRecordTable(false);
            List<RawDataRecord> RawList = DataAccess.GetPendingData();
            string RawData = DataTrack.GetRawFileHeader(m_Options.ProbeName);
            for(int Count=0; Count<RawList.Count; Count++)
            {
                RawData += string.Format("{0}0 HH {1,-12}{2}\r\n", DataTrack.GetTimeStamp(RawList[Count].TimeStamp), RawList[Count].ScanValue, RawList[Count].Flags);
            }
            RawData += string.Format("T 000\r\n");
            RawData += string.Format("S {0}\r\n", AccountInfo.GetDeviceID());
            

            NameValueCollection RawValues = WebInteraction.GetRawFileSendValues(m_Options.CompanyID, m_Options.WebFolder, m_Options.ProbeName);
            RawValues.Add("filedata", RawData);
            RawValues.Add("FileLen", RawData.Length.ToString());

            string result = WebInteraction.SendHttpPostValues(WebInteraction.m_FileSyncURL, RawValues);
            if(result.IndexOf("<adsdelimiter>")>=0)
            {
                result += "<adsdelimiter>";
                string Temp = result.Substring(0, result.IndexOf("<adsdelimiter>"));
                result = result.Substring(result.IndexOf("<adsdelimiter>") + 14);
                if (Temp == RawValues["fname"])
                {
                    Temp = result.Substring(0, result.IndexOf("<adsdelimiter>"));
                    if (Temp == RawValues["FileLen"])
                    {
                        //DataAccess.UpdateRecordTable();


                        //DataAccess.UpdateProfileRecordStatus();
                    }
                }
            }


            NameValueCollection UseMe = WebInteraction.GenerateDefFileRequest(m_Options.CompanyID, m_Options.WebFolder, "BADGE.DEF");
            result = WebInteraction.SendHttpPostValues(WebInteraction.m_FileSyncURL, UseMe);
            if(result!="")
            {
                //process badge.def
                FileInteraction.WriteCompleteFile("DEF", "BADGE.DEF", result);
                bBadgeReceived = true;
            }



            UseMe = WebInteraction.GenerateDefFileRequest(m_Options.CompanyID, m_Options.WebFolder, "PROFILE.DEF");
            result = WebInteraction.SendHttpPostValues(WebInteraction.m_FileSyncURL, UseMe);
            if (result != "")
            {
                //process badge.def
                FileInteraction.WriteCompleteFile("DEF", "PROFILE.DEF", result);
                bProfileReceived = true;
            }

            if(bBadgeReceived)
            {
                DataTrack.LoadBadgesFromFile();
            }
            if (bProfileReceived)
            {
                DataTrack.LoadProfileLevelsFromFile();
            }

        }
        public void Settings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Settings());
        }
        public void CollectData_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CollectData());

        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            cmdCollectData.IsVisible = false;
            cmdSettings.IsVisible = false;
            cmdRegister.IsVisible = false;
            cmdSync.IsVisible = false;

            string UseMe = AccountInformation.GetAccountInformationString();
            //WebInteraction.SendHttpJSON(WebInteraction.m_LicenseLocation, "80", UseMe);
            string OnlineResponse = WebInteraction.SendHttpPost(WebInteraction.m_LicenseURL, UseMe);
            if(OnlineResponse!="")
            {
                //process json
                Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();
                jo = Newtonsoft.Json.Linq.JObject.Parse(OnlineResponse);
                string vv = jo.GetValue("Value").ToString();
                if (vv.IndexOf("1")==0)
                {
                    cmdCollectData.IsVisible = true;
                    cmdSettings.IsVisible = true;
                    cmdSync.IsVisible = true;

                }
                else
                {
                    cmdRegister.IsVisible = true;
                }
            }
            m_Options.LoadOptionsFromDB();
            //if license ok, make buttons visible
                
        }
        
    }
}
