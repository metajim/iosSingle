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
	public partial class PersonalEditPage : ContentPage
	{
        public List<ProfileRecordFormatted> m_RecordList = new List<ProfileRecordFormatted>();
        private DateTime m_ClickTime;
        private ItemEdit m_ItemPage;
        
		public PersonalEditPage ()
		{
			InitializeComponent ();
            m_ClickTime = DateTime.Now;
            m_ItemPage = null;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if(m_ItemPage!=null)
            {
                //get details
                //need to adjust pre and post event when start/end times change
                bool bStartChange = false;
                bool bEndChange = false;
                bool bContinue = true;

                //make sure incoming start comes before incoming end
                if (m_ItemPage.m_StartView.CompareTo(m_ItemPage.m_EndView)>0)
                    bContinue = false;

                if(m_ItemPage.m_Index < (m_RecordList.Count - 1) && m_ItemPage.m_StartView != m_RecordList.Where(x=>x.IndexID==m_ItemPage.m_Index).First().StartView)
                {
                    //check to make sure start doesn't go past previous start value
                    //check next index
                    bStartChange = true;
                    if (m_ItemPage.m_StartView.CompareTo(m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index + 1).First().StartView) < 0)
                        bContinue = false;
                }

                if (m_ItemPage.m_Index > 0 && m_ItemPage.m_EndView != m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index).First().EndView)
                {
                    //check to make sure end doesn't go beyond next end
                    //check previous index
                    bEndChange = true;
                    if (m_ItemPage.m_EndView.CompareTo(m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index - 1).First().EndView) > 0)
                        bContinue = false;
                }


                if (bContinue)
                {
                    //m_RecordList.Where(s => s.IndexID == m_ItemPage.m_Index).First().EndView = m_ItemPage.m_EndView;
                    m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index).First().StartView = m_ItemPage.m_StartView;                    
                    m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index).First().EndView = m_ItemPage.m_EndView;

                    if (bStartChange)
                    {
                        //adjust previous item
                        m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index+1).First().EndView = m_ItemPage.m_StartView;
                    }
                    if(bEndChange)
                    {
                        m_RecordList.Where(x => x.IndexID == m_ItemPage.m_Index-1).First().StartView = m_ItemPage.m_EndView;
                    }
                }
                m_ItemPage = null;
            }
            lstMainView.ItemsSource = null;
            lstMainView.ItemsSource = m_RecordList;
        }

        private void lstMainView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {            
            m_ClickTime = DateTime.Now;
        }

        private void lstMainView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            TimeSpan diff = DateTime.Now - m_ClickTime;
            if (diff.TotalMilliseconds<10000)
            {

                if (e.Item != null)
                {
                    m_ItemPage = new ItemEdit();

                    ProfileRecordFormatted pr = e.Item as ProfileRecordFormatted;


                    m_ItemPage.m_StartView = pr.StartView;
                    m_ItemPage.m_EndView = pr.EndView;
                    m_ItemPage.m_Profile = pr.Profile;
                    m_ItemPage.m_Index = pr.IndexID;

                    Navigation.PushAsync(m_ItemPage);
                }
            }
            
            m_ClickTime = DateTime.Now;
        }
    }
}