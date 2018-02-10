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
	public partial class CollectData : ContentPage
	{
        Options m_Options;
        bool m_IsLoggedIn;
        bool m_IsBreak;
        bool m_IsLunch;
        bool m_EmployeeFound;

        bool m_SelectFlag = false;

        List<BadgeType> m_BadgeList;
        List<Levels> m_LevelList;
        List<ProfileRecord> m_ProfileEventList;
        int PickerStatus;

        BadgeType[] m_ActiveProfile;
        BadgeType m_ActiveEmployee;
        PersonalEditPage m_EditScreen;

		public CollectData ()
		{
			InitializeComponent ();

            cmdEdit.IsEnabled = false;

            //not that we really need these
            m_Options = new Options();
            m_Options.LoadOptionsFromDB();

            SetScreen();
            m_BadgeList = new List<BadgeType>();
            m_BadgeList = DataAccess.GetBadgeTable();

            m_LevelList = new List<Levels>();
            m_LevelList = DataAccess.GetLevelsTable();
            m_ActiveProfile = new BadgeType[12];
            for (int Count = 0; Count < 12; Count++)
                m_ActiveProfile[Count] = new BadgeType() { BadgeID = "" };

            m_ProfileEventList = new List<ProfileRecord>();
            m_ActiveEmployee = new BadgeType();

            m_EmployeeFound = false;
            m_IsLoggedIn = false;
            m_IsLunch = false;
            m_IsBreak = false;

            //ToolbarItems.Add(new ToolbarItem("Search", "search.png", () => { }));
            
        }
        public void SetScreen()
        {
            viewScan.IsVisible = false; 
            viewLunch.IsVisible = false;
            viewBreak.IsVisible = false;


            if(m_EmployeeFound)
            {
                viewScan.IsVisible = true;
            }
            if(m_IsLoggedIn)
            {
                
                viewLunch.IsVisible = true;
                viewBreak.IsVisible = true;
            }

            
        }
        public void UpdateEmployeeName()
        {
            lblEmployeeName.Text = string.Format("{0} ({1})", m_ActiveEmployee.Description, m_ActiveEmployee.Misc);
        }
        public void LoginEmp_Clicked(object sender, EventArgs e)
        {
            //validate emp badge, set active employee
            BadgeType bt = new BadgeType();
            string UseBadge = txtEmployee.Text;
            while (UseBadge.Length < 6)
                UseBadge = "0" + UseBadge;

            if (m_BadgeList.Where(s => s.BadgeID == UseBadge).Count() > 0)
            {
                bt = m_BadgeList.Where(s => s.BadgeID == UseBadge).First();

                if(bt.Type1.ToUpper()=="EMPLOYEE" && bt.BadgeID==UseBadge)
                {
                    if (m_ActiveEmployee.BadgeID != bt.BadgeID)
                    {
                        m_EmployeeFound = true;
                        m_IsLoggedIn = false;
                        txtScanIn.Text = "--:--";
                        txtScanOut.Text = "--:--";
                        txtBreakIn.Text = "--:--";
                        txtBreakOut.Text = "--:--";
                        txtLunchIn.Text = "--:--";
                        txtLunchOut.Text = "--:--";
                        m_ActiveEmployee = bt;

                        UpdateEmployeeName();
                    }

                }
            }

            SetScreen();

            if (m_EmployeeFound)
            {
                
                PickerStatus = 0;
                RunPicker();
            }
        }
        public void SelectProfile_Clicked(object sender, EventArgs e)
        {
            PickerStatus = 0;
            RunPicker();
        }
        
        private void RunPicker()
        {
            bool bContinue = true;
            PickerStatus++;

            if (m_LevelList.Where(s => s.Order == PickerStatus).Count() == 0)
                bContinue = false;

            if (bContinue)
            {
                string UseLevel = m_LevelList.Where(s => s.Order == PickerStatus).First().Level;


                List<BadgeType> curList = m_BadgeList.Where(s => s.Misc == UseLevel && s.Type1 == "Profile" && s.Type2 == "Element").ToList();

                Picker ppNew = new Picker();


                ppNew.ItemsSource = curList;
                ppNew.ItemDisplayBinding = new Binding("Description");

                if (m_ActiveProfile[PickerStatus - 1] != null)
                    ppNew.SelectedItem = m_ActiveProfile[PickerStatus - 1];
                else
                    ppNew.SelectedItem = curList.First();

                ppNew.SelectedIndexChanged += pickProfile_SelectedIndexChanged;
                ppNew.Unfocused += PpNew_Unfocused;

                m_SelectFlag = true;

                viewMain.Children.Add(ppNew);
                ppNew.IsVisible = true;
                ppNew.Focus();
            }
            else
            {
                UpdateActiveProfileText();
                cmdEdit.IsEnabled = true;
            }
        }

        private void PpNew_Unfocused(object sender, FocusEventArgs e)
        {
            
            if (m_SelectFlag)
            {
                Picker ppCur = sender as Picker;
                m_SelectFlag = false;
                if (ppCur.SelectedItem != null)
                {

                    BadgeType bt = ppCur.SelectedItem as BadgeType;

                    if (PickerStatus <= 12 && PickerStatus > 0)
                        m_ActiveProfile[PickerStatus - 1] = bt;

                    ppCur.IsVisible = false;
                    lblProfile.Text = bt.Description;

                    ppCur.Unfocus();

                    viewMain.Children.RemoveAt(viewMain.Children.Count - 1);

                    RunPicker();
                }
            }
        }

        private void UpdateActiveProfileText()
        {
            string result = "";
            foreach(BadgeType bt in m_ActiveProfile)
            {
                if(bt!=null)
                {
                    result += string.Format("{0}, ", bt.Description);

                }
            }
            result = result.Trim(new char[] { ' ', ',' });

            lblProfile.Text = result;
        }

        private void pickProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker ppCur = sender as Picker;
            m_SelectFlag = false;
            if (ppCur.SelectedItem!=null)
            {
                
                BadgeType bt = ppCur.SelectedItem as BadgeType;

                if(PickerStatus<=12 && PickerStatus>0)
                    m_ActiveProfile[PickerStatus - 1] = bt;

                ppCur.IsVisible = false;
                lblProfile.Text = bt.Description;

                ppCur.Unfocus();

                viewMain.Children.RemoveAt(viewMain.Children.Count - 1);

                RunPicker();
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if(m_EditScreen!=null)
            {
                //load from record list
                UpdateProfileEventFromEdits();
                m_EditScreen = null;
            }
            else
                LoadProfileEventList();


            SetScreen();
        }
        private void UpdateProfileEventFromEdits()
        {
         
            foreach(ProfileRecordFormatted prf in m_EditScreen.m_RecordList)
            {
                if (prf.Profile == "-DELETED-")
                {
                    m_ProfileEventList[prf.IndexID].Badge1 = "[DELETED]";
                }                
                else
                {
                    m_ProfileEventList[prf.IndexID].StartTime = prf.StartView.Replace(":", "");
                    m_ProfileEventList[prf.IndexID].EndTime = prf.EndView.Replace(":", "");
                }

                
            }
            for (int Count = 0; Count < m_ProfileEventList.Count; Count++)
            {
                if (m_ProfileEventList[Count].Badge1 == "[DELETED]")
                {
                    m_ProfileEventList.RemoveAt(Count);
                    Count--;
                }
            }

            DataAccess.UpdateProfileRecordTable(m_ProfileEventList, DataTrack.GetTimeStamp().Substring(0, 8));


        }
        private void LoadProfileEventList()
        {
            m_EmployeeFound = false;
            m_IsLoggedIn = false;
            m_IsLunch = false;
            m_IsBreak = false;
            //load event list for today
            //set active profile
            m_ProfileEventList =DataAccess.GetProfileTable(DataTrack.GetTimeStamp().Substring(0,8));

            if (m_ProfileEventList.Count > 0)
            {
                //set active profile
                //get employee from top profile
                BadgeType bt = new BadgeType();
                bt = m_BadgeList.Where(s => s.Type1 == "Employee" && s.Misc == m_ProfileEventList[0].EmployeeID).First();
                m_ActiveEmployee = bt;

                txtEmployee.Text = bt.BadgeID;
                lblEmployeeName.Text = string.Format("{0} ({1})", m_ActiveEmployee.Description, m_ActiveEmployee.Misc);
                m_EmployeeFound = true;
                bool bProfileFound = false;


                if(m_ProfileEventList[0].Badge1=="[LUNCH]")
                {
                    if(m_ProfileEventList[0].EndTime=="")
                    {
                        m_IsLunch = true;
                    }
                }
                else if (m_ProfileEventList[0].Badge1 == "[BREAK]")
                {
                    if (m_ProfileEventList[0].EndTime == "")
                    {
                        m_IsBreak = true;
                    }
                }
                else if(m_ProfileEventList[0].StartTime!="")
                {
                    m_IsLoggedIn = true;
                }

                for (int Count=0; Count<m_ProfileEventList.Count; Count++)
                {
                    if(m_ProfileEventList[Count].Badge1!="[BREAK]" && m_ProfileEventList[Count].Badge1 != "[LUNCH]")
                    {

                        if (!bProfileFound)
                        {
                            //set active profile
                            for(int Count2=0; Count2<12; Count2++)
                            {
                                //if (m_ProfileEventList[Count].GetBadgeAt(Count2 + 1) != "")
                                if(m_BadgeList.Where(s => s.BadgeID == m_ProfileEventList[Count].GetBadgeAt(Count2 + 1)).Count()>0)
                                    m_ActiveProfile[Count2] = m_BadgeList.Where(s => s.BadgeID == m_ProfileEventList[Count].GetBadgeAt(Count2 + 1)).First();
                                else
                                    m_ActiveProfile[Count2] = new BadgeType();

                            }
                            if(m_ProfileEventList[Count].EndTime != "")
                                txtScanOut.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].EndTime);

                        }
                        if(m_ProfileEventList[Count].StartTime!="")
                            txtScanIn.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].StartTime);
                        


                        bProfileFound = true;

                    }
                    else if(m_ProfileEventList[Count].Badge1 == "[BREAK]")
                    {
                        if (m_ProfileEventList[Count].EndTime != "")
                            txtBreakOut.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].EndTime);

                    
                        if (m_ProfileEventList[Count].StartTime != "")
                            txtBreakIn.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].StartTime);
                    }
                    else if (m_ProfileEventList[Count].Badge1 == "[LUNCH]")
                    {
                        if (m_ProfileEventList[Count].EndTime != "")
                            txtLunchOut.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].EndTime);


                        if (m_ProfileEventList[Count].StartTime != "")
                            txtLunchIn.Text = DataTrack.GetDisplayTime(m_ProfileEventList[Count].StartTime);
                    }
                }

                if (bProfileFound)
                {
                    UpdateActiveProfileText();
                    cmdEdit.IsEnabled = true;
                }

            }

            
        }

        private ProfileRecord UpdateProfile(ProfileRecord UseProfile)
        {
            ProfileRecord result = UseProfile;

            for (int Count = 0; Count < 12; Count++)
            {
                if (m_ActiveProfile[Count].BadgeID == null)
                    result.SetBadgeAt(Count + 1, "");
                else
                    result.SetBadgeAt(Count + 1, m_ActiveProfile[Count].BadgeID);
            }
            return result;
        }

        private void Scan_Clicked(object sender, EventArgs e)
        {
            //set logged in
            //set start time or end time
            //send scan to data
            if (m_IsBreak || m_IsLunch)
                return;


            if (m_ProfileEventList.Count == 0)
            {
                m_ProfileEventList.Add(UpdateProfile(new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge=m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0,8), OriginTime=DataTrack.GetTimeStamp() }));

            }
            else if(!m_IsLoggedIn && txtScanIn.Text == "--:--")
            {
                m_ProfileEventList.Insert(0, UpdateProfile(new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge = m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0, 8), OriginTime = DataTrack.GetTimeStamp() }));
            }


            if (!m_IsLoggedIn && txtScanIn.Text == "--:--")
            {
                //start time
                txtScanIn.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                m_IsLoggedIn = true;

                m_ProfileEventList[0].StartTime = DataTrack.GetTimeStamp().Substring(8);
            }
            else
            {
                // will always extend logout time if start time exists
                txtScanOut.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                m_IsLoggedIn = false;
                m_ProfileEventList[0].EndTime = DataTrack.GetTimeStamp().Substring(8);
                DataAccess.AddRecord("SGLOUT");
            }
            DataAccess.AddRecord(m_ActiveEmployee.BadgeID);
            DataAccess.UpdateProfileRecordTable(m_ProfileEventList, DataTrack.GetTimeStamp().Substring(0, 8));
            SetScreen(); //update in case we're logging in

        }
        private void Lunch_Clicked(object sender, EventArgs e)
        {
            //flag in or out from lunch
            //set start time or end time
            //send scan to data
            if (m_IsBreak)
                return;

            if(!m_IsLunch)
            {
                m_IsLunch = true;
                txtLunchIn.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                DataAccess.AddRecord("LCHBEG");

                //leave activity
                m_ProfileEventList[0].EndTime = DataTrack.GetTimeStamp().Substring(8);

                m_ProfileEventList.Insert(0, new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge = m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0, 8), OriginTime = DataTrack.GetTimeStamp() });
                m_ProfileEventList[0].SetBadgeAt(1, "[LUNCH]");

                m_ProfileEventList[0].StartTime = DataTrack.GetTimeStamp().Substring(8);
            }
            else
            {
                m_IsLunch = false;
                txtLunchOut.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                DataAccess.AddRecord("LCHEND");
                //end lunch
                m_ProfileEventList[0].EndTime = DataTrack.GetTimeStamp().Substring(8);

                //restart profile, set new start time and profile to active profile
                m_ProfileEventList.Insert(0, UpdateProfile(new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge = m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0, 8), OriginTime = DataTrack.GetTimeStamp() }));
                m_ProfileEventList[0].StartTime = DataTrack.GetTimeStamp().Substring(8);

            }
            DataAccess.UpdateProfileRecordTable(m_ProfileEventList, DataTrack.GetTimeStamp().Substring(0, 8));
            DataAccess.AddRecord(m_ActiveEmployee.BadgeID);
        }
        private void Break_Clicked(object sender, EventArgs e)
        {
            //flag in or out from lunch
            //set start time or end time
            //send scan to data

            if (m_IsLunch)
                return;

            if (!m_IsBreak)
            {
                m_IsBreak = true;
                txtBreakIn.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                DataAccess.AddRecord("BRKBEG");

                //leave activity
                m_ProfileEventList[0].EndTime = DataTrack.GetTimeStamp().Substring(8);

                m_ProfileEventList.Insert(0, new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge = m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0, 8), OriginTime = DataTrack.GetTimeStamp() });
                m_ProfileEventList[0].SetBadgeAt(1, "[BREAK]");

                m_ProfileEventList[0].StartTime = DataTrack.GetTimeStamp().Substring(8);
            }
            else
            {
                m_IsBreak = false;
                txtBreakOut.Text = DataTrack.GetDisplayTime(DataTrack.GetTimeStamp().Substring(8));
                DataAccess.AddRecord("BRKEND");

                //end break
                m_ProfileEventList[0].EndTime = DataTrack.GetTimeStamp().Substring(8);

                //restart profile, set new start time and profile to active profile
                m_ProfileEventList.Insert(0, UpdateProfile(new ProfileRecord() { EmployeeID = m_ActiveEmployee.Misc, EmployeeBadge = m_ActiveEmployee.BadgeID, FileDate = DataTrack.GetTimeStamp().Substring(0, 8), OriginTime = DataTrack.GetTimeStamp() }));
                m_ProfileEventList[0].StartTime = DataTrack.GetTimeStamp().Substring(8);
            }
            DataAccess.UpdateProfileRecordTable(m_ProfileEventList, DataTrack.GetTimeStamp().Substring(0, 8));
            DataAccess.AddRecord(m_ActiveEmployee.BadgeID);
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            m_EditScreen = new PersonalEditPage();
            //pass profilerecord list
            //for(int _index=0; _index<m_ProfileEventList.Count; _index++)
            //reversing, so that early events are on top
            for(int _index=m_ProfileEventList.Count-1; _index>=0; _index--)
            {
                string _profile = "";
                if (m_ProfileEventList[_index].EmployeeID == m_ActiveEmployee.Misc)
                {
                    if (m_ProfileEventList[_index].Badge1 == "[BREAK]" || m_ProfileEventList[_index].Badge1 == "[LUNCH]")
                    {
                        if (m_ProfileEventList[_index].Badge1 == "[BREAK]")
                            _profile = "Break Event";
                        else
                            _profile = "Lunch Event";
                    }
                    else
                    {
                        for (int Count = 1; Count <= 12; Count++)
                        {
                            if (m_ProfileEventList[_index].GetBadgeAt(Count) != "")
                            {
                                BadgeType bt = m_BadgeList.Where(b => b.BadgeID == m_ProfileEventList[_index].GetBadgeAt(Count)).First();
                                if (bt.Description != "")
                                    _profile += bt.Description + ", ";
                            }

                        }  //end profile loop
                    }
                    m_EditScreen.m_RecordList.Add(new ProfileRecordFormatted()
                    {
                        IndexID = _index,
                        Profile = _profile,
                        
                        StartView = DataTrack.GetLongDisplayTime(m_ProfileEventList[_index].StartTime),
                        EndView = DataTrack.GetLongDisplayTime(m_ProfileEventList[_index].EndTime)

                    });
                }
            }
            Navigation.PushAsync(m_EditScreen);
        }
    }
}