using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.IO;
using System.Reactive;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using SQLite;
using System.Collections.Specialized;

namespace ADSFieldEntry
{
    public class EmployeeType
    {

    }
    public class Levels
    {
        public string Level { get; set; }
        public int Order { get; set; }
    }

    public class BadgeType
    {
        public string Type1 { get; set; }
        public string Type2 { get; set; }

        public string Description { get; set; }
        public string Misc { get; set; }

        public string BadgeID { get; set; }
        public string ButtonID { get; set; }

        public string Details { get; set; }



    }
    public class Options
    {
        public string CompanyID { get; set; }
        public string WebFolder { get; set; }
        public string EmpName { get; set; }
        public string CompanyName { get; set; }
        public string PreviousEmployee { get; set; }
        public string ProbeName { get; set; }

        public Options()
        {
            CompanyID = "";
            WebFolder = "";
            EmpName = "";
            CompanyName = "";
            ProbeName = "";
            PreviousEmployee = "";
        }
        public void LoadOptionsFromDB()
        {
            
            Options oo = DataAccess.GetOptions();

            CompanyID = oo.CompanyID;
            WebFolder = oo.WebFolder;
            EmpName = oo.EmpName;
            CompanyName = oo.CompanyName;
            PreviousEmployee = oo.PreviousEmployee;
            ProbeName = oo.ProbeName;

        }
    }

    public class AccountInfo
    {
        public int Status { get; set; }
        public DateTime LastCheckTime { get; set; }

        public static string GetDeviceID()
        {
            string result = "";
#if __IOS__
                
            result = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
#endif

#if __ANDROID__

            result = Android.OS.Build.Serial;


#endif


            return result;
        }

    }
    public class RawDataRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ScanValue { get; set; }
        public string Flags { get; set; }
        public int Status { get; set; }
    }
    public class ProfileRecordFormatted
    {
        public string Profile { get; set; }
        public string StartView { get; set; }
        public string EndView { get; set; }
        public int IndexID { get; set; }

    }

    public class ProfileRecord
    {
        //for each event
        public string FileDate { get; set; }
        public string OriginTime { get; set; }
        public string EmployeeBadge { get; set; }
        public string EmployeeID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }        
        public string ProbeName { get; set; }
        public string KeypadValue { get; set; }
        public double Count1 { get; set; }
        public double Count2 { get; set; }
        public double Count3 { get; set; }

        public int Status { get; set; }
        
        public string Badge1 { get; set; }
        public string Badge2 { get; set; }
        public string Badge3 { get; set; }
        public string Badge4 { get; set; }
        public string Badge5 { get; set; }
        public string Badge6 { get; set; }
        public string Badge7 { get; set; }
        public string Badge8 { get; set; }
        public string Badge9 { get; set; }
        public string Badge10 { get; set; }
        public string Badge11 { get; set; }
        public string Badge12 { get; set; }
        public string GetBadgeAt(int Index)
        {
            string result = "";
            switch(Index)
            {
                case 1:
                    result = Badge1;break;
                case 2:
                    result = Badge2; break;
                case 3:
                    result = Badge3; break;
                case 4:
                    result = Badge4; break;
                case 5:
                    result = Badge5; break;
                case 6:
                    result = Badge6; break;
                case 7:
                    result = Badge7; break;
                case 8:
                    result = Badge8; break;
                case 9:
                    result = Badge9; break;
                case 10:
                    result = Badge10; break;
                case 11:
                    result = Badge11; break;
                case 12:
                    result = Badge12; break;
            };

            return result;
        }
        public void SetBadgeAt(int Index, string BadgeValue)
        {
            switch(Index)
            {
                case 1:
                    Badge1 = BadgeValue;break;
                case 2:
                    Badge2 = BadgeValue; break;
                case 3:
                    Badge3 = BadgeValue; break;
                case 4:
                    Badge4 = BadgeValue; break;
                case 5:
                    Badge5 = BadgeValue; break;
                case 6:
                    Badge6 = BadgeValue; break;
                case 7:
                    Badge7 = BadgeValue; break;
                case 8:
                    Badge8 = BadgeValue; break;
                case 9:
                    Badge9 = BadgeValue; break;
                case 10:
                    Badge10 = BadgeValue; break;
                case 11:
                    Badge11 = BadgeValue; break;
                case 12:
                    Badge12 = BadgeValue; break;
            };
        }
        public ProfileRecord()
        {
            Count1 = 0;
            Count2 = 0;
            Count3 = 0;
            Status = 0;
            
        }
    }

    public class DataTrack
    {
        public static DateTime GetDateTimeFromTimeStamp(string UseTimeStamp)
        {
            DateTime result = DateTime.Now;
            try
            {
                if (UseTimeStamp.Length >= 14)
                {
                    result = new DateTime(Int32.Parse(UseTimeStamp.Substring(0, 4)), Int32.Parse(UseTimeStamp.Substring(4, 2)), Int32.Parse(UseTimeStamp.Substring(6, 2)), Int32.Parse(UseTimeStamp.Substring(8, 2)), Int32.Parse(UseTimeStamp.Substring(10, 2)), Int32.Parse(UseTimeStamp.Substring(12, 2)));
                    
                }
            }
            catch (Exception ee)
            {

            }
            return result;
        }
        public bool ReMaskLongTimeStamp(string UseValue)
        {
            bool result = false;
            if(UseValue.Length==8)
            {
                if(UseValue.Substring(2,1)==":" && UseValue.Substring(5,1)==":")
                {
                    result = true;
                }
            }
            if(!result)
            {
                
            }

            return result;
        }
        
        public static string GetLongDisplayTime(string UseTimeStamp)
        {
            string result = "";
            int h, m, s;
            h = 0;
            m = 0;
            s = 0;

            if (UseTimeStamp == null)
                return "00:00:00";
            if (UseTimeStamp == "")
                return "00:00:00";

            Int32.TryParse(UseTimeStamp.Substring(0, 2), out h);
            Int32.TryParse(UseTimeStamp.Substring(2, 2), out m);
            Int32.TryParse(UseTimeStamp.Substring(4, 2), out s);

            

            result = string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            return result;
        }
        public static string GetDisplayTime(string UseTimeStamp)
        {
            string result = "";
            int h, m, s;
            h = 0;
            m = 0;
            s = 0;

            if (UseTimeStamp == null)
                return "--:--";
            if (UseTimeStamp == "")
                return "--:--";


            Int32.TryParse(UseTimeStamp.Substring(0, 2), out h);
            Int32.TryParse(UseTimeStamp.Substring(2, 2), out m);
            Int32.TryParse(UseTimeStamp.Substring(4, 2), out s);

            if (s >= 30)
                m++;

            result = string.Format("{0:00}:{1:00}", h, m);
            return result;
        }
        public static string GetRawFileHeader(string UseProbeName)
        {
            string result = string.Format("H {0} 00 {1}\n", GetTimeStamp(), UseProbeName);
            return result;

        }
        public static string GetTimeStampMinusTwo(string UseTimeStamp)
        {
            DateTime _result = new DateTime();
            string result = UseTimeStamp;
            try
            {
                if (UseTimeStamp.Length >= 14)
                {
                    _result = new DateTime(Int32.Parse(UseTimeStamp.Substring(0, 4)), Int32.Parse(UseTimeStamp.Substring(4, 2)), Int32.Parse(UseTimeStamp.Substring(6, 2)), Int32.Parse(UseTimeStamp.Substring(8, 2)), Int32.Parse(UseTimeStamp.Substring(10, 2)), Int32.Parse(UseTimeStamp.Substring(12, 2)));
                    _result = _result.AddSeconds(-2);
                    result = GetTimeStamp(_result);
                }
            }
            catch(Exception ee)
            {

            }

            return result;
        }

        public static string GetTimeStamp()
        {
            return string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
        public static string GetTimeStamp(DateTime dt)
        {
            return string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
        public static List<BadgeType> GetBadgeList()
        {
            return DataAccess.GetBadgeTable();
        }
        public static List<Levels> GetLevelList()
        {

            return DataAccess.GetLevelsTable();
        }
        public static void LoadProfileLevelsFromFile()
        {
            string FileName = FileInteraction.CreateDirectory("DEF") + "PROFILE.DEF";
            List<Levels> LevelList = new List<Levels>();
            try
            {
                FileInfo fi = new FileInfo(FileName);
                if (fi.Exists)
                {
                    string StopString = "THIS line MUST remain";
                    string FileData = FileInteraction.ReadCompleteFile("DEF", "PROFILE.DEF");

                    if (FileData != "")
                    {
                        string[] FileInfo = FileData.Split('\n');

                        if(FileInfo.Length>2)
                        {
                            int OrderValue = 0;
                            string curLevel = "";
                            string FileLevel = FileInfo[1].Trim();
                            int OffSet = 73;
                            if (FileLevel.Length >= 8)
                                OffSet += 10;

                            while(curLevel.IndexOf(StopString)==-1)
                            {
                                curLevel = FileInfo[OffSet+OrderValue].Trim();
                                if(curLevel.IndexOf(StopString)==-1)
                                {
                                    Levels ll = new Levels();
                                    OrderValue++;

                                    ll.Order = OrderValue;
                                    ll.Level = curLevel;
                                    LevelList.Add(ll);

                                }
                            }
                        }




                        DataAccess.UpdateLevelsTable(LevelList);

                    }


                }
            }
            catch (Exception ee)
            {

            }
        }
    
        public static void LoadBadgesFromFile()
        {
            string FileName = FileInteraction.CreateDirectory("DEF") + "BADGE.DEF";
            List<BadgeType> BadgeList = new List<BadgeType>();
            try
            {
                FileInfo fi = new FileInfo(FileName);
                if(fi.Exists)
                {

                    string FileData = FileInteraction.ReadCompleteFile("DEF", "BADGE.DEF");

                    if(FileData!="")
                    {
                        string[] FileInfo = FileData.Split('\n');

                        for(int Count=2; Count<FileInfo.Length-1; Count+=2)
                        {
                            string[] strItems = FileInfo[Count].Split(';');
                            BadgeType bt = new BadgeType();
                            bt.Type1 = strItems[1];
                            bt.Type2 = strItems[2];

                            bt.BadgeID = strItems[3];
                            bt.Description = strItems[6];
                            bt.Misc = strItems[7];
                            bt.ButtonID = strItems[5];
                            bt.Details = "";

                            BadgeList.Add(bt);

                        }


                        DataAccess.UpdateBadgeTable(BadgeList);

                    }


                }
            }
            catch(Exception ee)
            {

            }
        }

        
    }
    public class FileInteraction
    {
        public static string CreateDirectory(string UseFolder)
        {
            string result = "";
#if __IOS__
            result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/";
#endif
#if __ANDROID__
            result = Android.OS.Environment.ExternalStorageDirectory.ToString() + "/";
#endif

            try
            {
                result += UseFolder + "/";
                Directory.CreateDirectory(result);

            }
            catch(Exception ee)
            {

            }

            return result;
        }

        public static void WriteCompleteFile(string UseFolder, string UseFileName, string UseFileData)
        {
            try
            {
                string fName = CreateDirectory(UseFolder) + UseFileName;
                File.WriteAllText(fName, UseFileData);
            }
            catch (Exception ee)
            {
                string xxy = ee.Message;
                xxy += "hi";
            }
        }
        public static string ReadCompleteFile(string UseFolder, string UseFileName)
        {
            string result = "";
            try
            {
                string fName = CreateDirectory(UseFolder) + UseFileName;
                result = File.ReadAllText(fName);
            }
            catch (Exception ee)
            {
                string xxy = ee.Message;
                xxy += "hi";
            }
            return result;
            
        }
    }
    public class DataAccess
    {
        public static string m_DBFile;
        public static SQLiteConnection m_Conn;

        public static void WipeDatabase()
        {
            //m_Conn.CreateTable<ProfileRecord>();
            //m_Conn.CreateTable<RawDataRecord>();

            m_Conn.Execute("DELETE from ProfileRecord");
            m_Conn.Execute("DELETE from RawDataRecord");


        }

        public static void OpenDatabase()
        {
            m_DBFile = FileInteraction.CreateDirectory("ADSFE") + "ADS_DB";

            try
            {
                //FileInfo fi = new FileInfo(m_DBFile);
                //fi.Delete();

                m_Conn = new SQLiteConnection(m_DBFile);
                
                if(m_Conn !=null)
                {
                    m_Conn.CreateTable<ProfileRecord>();
                    m_Conn.CreateTable<RawDataRecord>();
                    m_Conn.CreateTable<Options>();
                    m_Conn.CreateTable<BadgeType>();
                    m_Conn.CreateTable<Levels>();



                    if(m_Conn.Table<Options>().Count()==0)
                    {
                        
                        m_Conn.Insert(new Options(), typeof(Options));

                        
                    }
                }

            }
            catch(Exception ee)
            {

            }
             
        }

        public static List<ProfileRecord> GetProfileTable(string UseFileName)
        {
            if (m_Conn != null)
            {
                List<ProfileRecord> mm = m_Conn.Query<ProfileRecord>("Select * from ProfileRecord order by OriginTime DESC");

                return mm.Where(s => s.FileDate == UseFileName).ToList();
            }
            else
                return new List<ProfileRecord>();

        }
        public static void UpdateProfileRecordTable(List<ProfileRecord> UseList, string UseFileName)
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("DELETE from ProfileRecord WHERE FileDate='" + UseFileName + "'");
                m_Conn.InsertAll(UseList, typeof(ProfileRecord));
            }
        }
        public static List<BadgeType> GetBadgeTable()
        {
            if (m_Conn != null)
            {
                List<BadgeType> mm = m_Conn.Query<BadgeType>("Select * from BadgeType");

                return mm;
            }
            else
                return new List<BadgeType>();
        }
        public static List<Levels> GetLevelsTable()
        {
            if (m_Conn != null)
            {
                List<Levels> mm = m_Conn.Query<Levels>("Select * from Levels order by [Order]");

                return mm;
            }
            else
                return new List<Levels>();
        }
        public static void UpdateBadgeTable(List<BadgeType> UseList)
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("delete from BadgeType");
                m_Conn.InsertAll(UseList, typeof(BadgeType));
            }
            
        }
        public static void UpdateLevelsTable(List<Levels> UseList)
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("delete from Levels");
                m_Conn.InsertAll(UseList, typeof(Levels));
            }
        }
        public static void SaveOptions(Options UseOption)
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("delete from Options");
                m_Conn.Insert(UseOption, typeof(Options));
            }
        }
        public static Options GetOptions()
        {
            Options oo = new Options();
            if (m_Conn != null)
            {
                oo = m_Conn.Table<Options>().First();
            }
            return oo;
        }

        public static void AddRecord(string UseValue)
        {
            if (m_Conn != null)
            {
                AddRecord(DateTime.Now, UseValue);
            }
        }
        public static void AddRecord(DateTime UseTime, string UseValue)
        {
            if (m_Conn != null)
            {
                RawDataRecord rw = new RawDataRecord();
                rw.Flags = "";
                rw.ScanValue = UseValue;
                rw.TimeStamp = UseTime;
                rw.Status = 0;

                AddRecord(rw);
            }
        }
        public static void AddRecord(RawDataRecord UseRecord)
        {
            if (m_Conn != null)
            {
                m_Conn.Insert(UseRecord, typeof(RawDataRecord));
            }
        }
        public static List<RawDataRecord> GetRecordTable(bool GetAllData)
        {
            
            List<RawDataRecord> result = new List<RawDataRecord>();
            if (m_Conn != null)
            {
                string strQuery = "Select * from RawDataRecord";
                if (!GetAllData)
                {
                    strQuery += " WHERE Status=0";
                }
                strQuery += " Order By ID";
                result = m_Conn.Query<RawDataRecord>(strQuery);

            }
            return result;
        }
        public static void UpdateRecordTable()
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("Update RawDataRecord set Status=1");
            }
        }
        public static void UpdateProfileRecordStatus()
        {
            if (m_Conn != null)
            {
                m_Conn.Execute("Update ProfileRecord set Status=1");
            }
        }
        public static List<ProfileRecord> GetPendingRecords()
        {
            //return all pending records
            if (m_Conn != null)
            {
                List<ProfileRecord> mm = m_Conn.Query<ProfileRecord>("Select * from ProfileRecord where Status=0 order by EmployeeID, FileDate, StartTime");

                return mm;
            }
            else
                return new List<ProfileRecord>();
        }
        public static List<RawDataRecord> GetPendingData()
        {
            List<ProfileRecord> _events = GetPendingRecords();
            List<RawDataRecord> resultList = new List<RawDataRecord>();
            int Count, Count2;
            RawDataRecord rc;
            string LastDayFile = "0000000";
            string LastEmployee = "";
            string LastProfile = "";
            string CurrentProfile = "";
            bool bIsSpecialEvent;
            bool bStartTimeSet;

            bool bSaveStart, bSaveEnd;

            for (Count = 0; Count < _events.Count; Count++)
            {
                if (_events[Count].Badge1 != "[BREAK]" && _events[Count].Badge1 != "[LUNCH]")
                {
                    //leave alone if break or lunch event, to pass through to next event
                    CurrentProfile = "";
                    bIsSpecialEvent = false;
                    for (Count2 = 0; Count2 < 12; Count2++)
                    {
                        CurrentProfile += _events[Count].GetBadgeAt(Count2 + 1);
                    }
                }
                else
                    bIsSpecialEvent = true;



                if (LastEmployee != _events[Count].EmployeeID || LastDayFile!=_events[Count].FileDate)
                {
                    LastProfile = "";  //reset profile 
                    bStartTimeSet = false;
                }

                if (CurrentProfile != LastProfile)
                {
                    //save profile
                    for (Count2 = 0; Count2 < 12; Count2++)
                    {
                        if (_events[Count].GetBadgeAt(Count2 + 1) == "")
                            break;
                        else
                        {
                            rc = new RawDataRecord();

                            rc.Flags = "";
                            rc.TimeStamp = DataTrack.GetDateTimeFromTimeStamp(_events[Count].FileDate+_events[Count].StartTime).AddSeconds(-2);
                            rc.ScanValue = _events[Count].GetBadgeAt(Count2 + 1);
                            resultList.Add(rc);
                        }
                    }
                }

                //if is last event of day for employee AND start time has already been done, only endtime                
                //if is special event, both times
                //if first event, always start time

                //check file/emp resets 
                bSaveStart = false;
                bSaveEnd = false;

                if(Count== _events.Count-1)
                {
                    //last event of the list
                    bSaveStart = false;
                    bSaveEnd = true;
                    if (_events.Count == 1)
                    {
                        bSaveStart = true;
                    }

                }
                else if(Count==0)
                {
                    //very first event
                    bSaveStart = true;
                    bSaveEnd = false;
                    if(_events.Count==1)
                    {
                        bSaveEnd = true;
                    }
                }
                else if(_events[Count+1].EmployeeID!=_events[Count].EmployeeID || _events[Count + 1].FileDate != _events[Count].FileDate)
                {
                    //last event for employee/day
                    bSaveStart = false;
                    bSaveEnd = true;
                }
                else if(bIsSpecialEvent)
                {
                    //break/lunch always save
                    bSaveStart = true;
                    bSaveEnd = true;
                }
                else if (CurrentProfile!=LastProfile)
                {
                    //profile change, need start time again
                    //if profile hasnt changed, and are just returning from break/lunch, dont need start time
                    bSaveStart = true;
                    bSaveEnd = false;
                }
                else if (_events[Count - 1].EmployeeID != _events[Count].EmployeeID || _events[Count - 1].FileDate != _events[Count].FileDate)
                {
                    //if first scan for this employee/day
                    bSaveStart = true;
                    bSaveEnd = false;
                }


                if (bSaveStart)
                {
                    if(_events[Count].Badge1=="[BREAK]" || _events[Count].Badge1 == "[LUNCH]")
                    {
                        rc = new RawDataRecord();
                        rc.Flags = "";
                        rc.TimeStamp = DataTrack.GetDateTimeFromTimeStamp(_events[Count].FileDate+_events[Count].StartTime);
                        rc.ScanValue = _events[Count].Badge1 == "[BREAK]" ? "SBREAK" : "SLUNCH";
                        resultList.Add(rc);
                    }
                    rc = new RawDataRecord();

                    rc.Flags = "";
                    rc.TimeStamp = DataTrack.GetDateTimeFromTimeStamp(_events[Count].FileDate + _events[Count].StartTime);
                    rc.ScanValue = _events[Count].EmployeeBadge;
                    resultList.Add(rc);
                }
                if (bSaveEnd)
                {
                    if (_events[Count].Badge1 == "[BREAK]" || _events[Count].Badge1 == "[LUNCH]")
                    {
                        rc = new RawDataRecord();
                        rc.Flags = "";
                        rc.TimeStamp = DataTrack.GetDateTimeFromTimeStamp(_events[Count].FileDate + _events[Count].EndTime);
                        rc.ScanValue = _events[Count].Badge1 == "[BREAK]" ? "EBREAK" : "ELUNCH";
                        resultList.Add(rc);

                    }
                    rc = new RawDataRecord();

                    rc.Flags = "";
                    rc.TimeStamp = DataTrack.GetDateTimeFromTimeStamp(_events[Count].FileDate + _events[Count].EndTime);
                    rc.ScanValue = _events[Count].EmployeeBadge;
                    resultList.Add(rc);
                }

                LastProfile = CurrentProfile;
                LastEmployee = _events[Count].EmployeeID;
                LastDayFile = _events[Count].FileDate;

            }

            return resultList;
        }
        
    }
    public class AccountInformation
    {
        public static string GetAccountInformationString()
        {
            /*
            JsonObject jo = new JsonObject();
            JsonObject jp = new JsonObject();

            jo.Add("interface", "ADSFPRestAPI");
            jo.Add("method", "getAccountInformation");
            jp.Add("prmAccountID", AccountInfo.GetDeviceID());
            jo.Add("parameters", jp);

            
            return jo.ToString();*/

            Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();
            Newtonsoft.Json.Linq.JObject jp = new Newtonsoft.Json.Linq.JObject();

            jo.Add("interface", "ADSFPRestAPI");
            jo.Add("method", "getAccountInformation");
            jp.Add("prmAccountID", AccountInfo.GetDeviceID());
            jo.Add("parameters", jp);

            return jo.ToString();
        }

        public static string GetRegisterNewAccountString(string EmployeeName, string CompanyName)
        {
            Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();
            Newtonsoft.Json.Linq.JObject jp = new Newtonsoft.Json.Linq.JObject();

            jo.Add("interface", "ADSFPRestAPI");
            jo.Add("method", "registerAccountNew");

            jp.Add("prmAccountID", AccountInfo.GetDeviceID());
            jp.Add("prmCompanyName", CompanyName);
            jp.Add("prmEmployeeName", EmployeeName);
            jp.Add("parameters", jp);

            jo.Add("parameters", jp);

            return jo.ToString();
        }
    }
    public class WebInteraction
    {
        public static string m_LicenseURL = "http://www.agriculturaldatasystems.com/ADSFP-RestAPI/Handler1.ashx";
        public static string m_FileSyncURL = "http://www.touchmemory.net/Interface/runfiles.php";
        public static string m_HttpResult;

        public static string SendHttpPost(string strLocation, string strData)
        {

            WebClient _client = new WebClient();
            string result = "";
            try
            {
                
                result = _client.UploadString(strLocation, strData);
            }
            catch (Exception ee)
            {

            }
            return result;

        }
        public static string SendHttpPostValues(string strLocation, NameValueCollection UseItems)
        {

            WebClient _client = new WebClient();
            System.Collections.Specialized.NameValueCollection items = new System.Collections.Specialized.NameValueCollection();
            string result = "";
            try
            {
                /*items.Add("fname", "BADGE.DEF");
                items.Add("action", "download");
                items.Add("CompanyID", "221238");
                items.Add("folder", "JIMCREEK");*/

                Byte[] bList =  _client.UploadValues(strLocation, UseItems);


                result = System.Text.Encoding.UTF8.GetString(bList);
            }
            catch (Exception ee)
            {

            }
            return result;

        }
        public static async void SendHttpJSON(string strLocation, string serPort, string strJSON)
        {
            //string result = "";
            HttpClient _client = new HttpClient();
            //int result = 0;
            try
            {
                
                var response = await _client.PostAsync(new Uri(strLocation), new StringContent(strJSON));
                var responseString = await response.Content.ReadAsStringAsync();

                m_HttpResult = responseString;
               // result = 1;
                //result = responseString;
            }
            catch(Exception ee)
            {
                string xx = ee.Message;
                xx += "";
               // result = -1;
            }
            _client.Dispose();
            //return result;
            
            
        }
        public static NameValueCollection GetRawFileSendValues(string UseCompanyID, string UseWebFolder, string UseProbeName)
        {
            NameValueCollection result = new NameValueCollection();
            result.Add("CompanyID", UseCompanyID);
            result.Add("folder", UseWebFolder);
            result.Add("fname", string.Format("{0}_DATA_{1}.RAW", DataTrack.GetTimeStamp(), UseProbeName));
            result.Add("action", "upload");



            return result;
        }
        public static NameValueCollection GenerateDefFileRequest(string UseCompanyID, string UseWebFolder, string UseFile)
        {
            NameValueCollection result = new NameValueCollection();

            //result = string.Format("fname={2}&action=download&folder={1}&type=1&CompanyID={0}", UseCompanyID, UseWebFolder, UseFile);

            result.Add("fname", UseFile);
            result.Add("action", "download");
            result.Add("CompanyID", UseCompanyID);
            result.Add("folder", UseWebFolder);


            return result;

        }
       

    }

}
