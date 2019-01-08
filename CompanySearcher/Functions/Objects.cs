using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace CompanySearcher
{
    public class Objects
    {
    }

    public class SearcherColors
    {
        public static Color Cyan = Color.FromArgb(255, 36, 81, 128);
        public static Color ShieldRed = Color.FromArgb(255, 229, 20, 0);
        public static Color LightRed = Color.FromArgb(255, 229, 141, 141);
        public static Color DarkRed = Color.FromArgb(255, 190, 34, 34);

        public static string CyanString = "#245180";
        public static string ShieldRedString = "#E51400";
        public static string LightRedString = "#E58D8D";
        public static string DarkRedString = "#BE2222";
    }

    public class SplitListItem
    {
        public SplitListItem(string title, string symbol)
        {
            Title = title;
            Symbol = symbol;
        }
        public string Title { get; set; }
        public string Symbol { get; set; }
    }

    public class SearchedCompanyListItem
    {
        public SearchedCompanyListItem(string id, string regNo, string name, string status, string type, string estDate, string legPerson, string regOrg, string rN, string recColor, string nameColor, string summaryColor)
        {
            Id = id;
            RegNo = regNo;
            Name = name;
            Status = status;
            Type = type;
            EstDate = estDate;
            LegPerson = legPerson;
            RegOrg = regOrg;
            RN = rN;
            RecColor = recColor;
            NameColor = nameColor;
            SummaryColor = summaryColor;
        }
        public string Id { get; set; }
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string EstDate { get; set; }
        public string LegPerson { get; set; }
        public string RegOrg { get; set; }
        public string RN { get; set; }
        public string RecColor { get; set; }
        public string NameColor { get; set; }
        public string SummaryColor { get; set; }
    }

    public class AbnormalCompanyListItem
    {
        public AbnormalCompanyListItem(string id, string regNo, string name, string date)
        {
            Id = id;
            RegNo = regNo;
            Name = name;
            Date = date;
        }
        public string Id { get; set; }
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class CheckCompanyListItem
    {
        public CheckCompanyListItem(string id, string regNo, string name, string date, string result, string recColor)
        {
            Id = id;
            RegNo = regNo;
            Name = name;
            Date = date;
            Result = result;
            RecColor = recColor;
        }
        public string Id { get; set; }
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Result { get; set; }
        public string RecColor { get; set; }
    }

    public class CompanyShareholderInfoListItem
    {
        public CompanyShareholderInfoListItem(string name, string regionType, string cerType, string avatarIcon)
        {
            Name = name;
            RegionType = regionType;
            CerType = cerType;
            AvatarIcon = avatarIcon;
        }
        public string Name { get; set; }
        public string RegionType { get; set; }
        public string CerType { get; set; }
        public string AvatarIcon { get; set; }
    }

    public class CompanyChangeInfoListItem
    {
        public CompanyChangeInfoListItem(string title, string date, string contentBefore, string contentAfter)
        {
            Title = title;
            Date = date;
            ContentBefore = contentBefore;
            ContentAfter = contentAfter;
        }
        public string Title { get; set; }
        public string Date { get; set; }
        public string ContentBefore { get; set; }
        public string ContentAfter { get; set; }
    }

    public class CompanyCheckInfoListItem
    {
        public CompanyCheckInfoListItem(string date, string type, string checkOrg, string result)
        {
            Date = date;
            Type = type;
            CheckOrg = checkOrg;
            Result = result;
        }
        public string Date { get; set; }
        public string Type { get; set; }
        public string CheckOrg { get; set; }
        public string Result { get; set; }
    }

    public class SearchedCompanyClipboardItem
    {
        public SearchedCompanyClipboardItem(string regNo, string name, string estDate, string legPerson, string regOrg)
        {
            RegNo = regNo;
            Name = name;
            EstDate = estDate;
            LegPerson = legPerson;
            RegOrg = regOrg;
        }
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string EstDate { get; set; }
        public string LegPerson { get; set; }
        public string RegOrg { get; set; }
    }

    //public class CompanyBasicInfo
    //{
    //    public CompanyBasicInfo(string id, string regNo, string name, string state, string capital, string legPerson, string estDate, string preDate, string beginDate, string endDate, string type, string address, string regOrg, string scope)
    //    {
    //        Id = id;
    //        RegNo = regNo;
    //        Name = name;
    //        State = state;
    //        Capital = capital;
    //        LegPerson = legPerson;
    //        EstDate = estDate;
    //        PreDate = preDate;
    //        BeginDate = beginDate;
    //        EndDate = endDate;
    //        Type = type;
    //        Address = address;
    //        Scope = scope;
    //    }
    //    public string Id { get; set; }
    //    public string RegNo { get; set; }
    //    public string Name { get; set; }
    //    public string State { get; set; }
    //    public string Capital { get; set; }
    //    public string LegPerson { get; set; }
    //    public string EstDate { get; set; }
    //    public string PreDate { get; set; }
    //    public string BeginDate { get; set; }
    //    public string EndDate { get; set; }
    //    public string Type { get; set; }
    //    public string Address { get; set; }
    //    public string RegOrg { get; set; }
    //    public string Scope { get; set; }
    //}
}
