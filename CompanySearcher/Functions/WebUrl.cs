using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySearcher
{
    public class WebUrl
    {
        public static string getSearchedCompanyListJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/MainServlet.json?sendMessageList&className=QuerySummary&Q=";
        public static string getSearchedCompanyListJsonCenter = "&Page=";
        public static string getSearchedCompanyListJsonEnd = "&Limit=";

        public static string getAbnormalCompanyListJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/JZGSServlet.json?queryYCML=true&Q=";
        public static string getAbnormalCompanyListJsonCenter = "&Page=";
        public static string getAbnormalCompanyListJsonEnd = "&Limit=";

        public static string getCheckCompanyListJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/JZGSServlet.json?queryCCJC=true&Q=";
        public static string getCheckCompanyListJsonCenter1 = "&Page=";
        public static string getCheckCompanyListJsonCenter2 = "&Item=";
        public static string getCheckCompanyListJsonEnd = "&Limit=";

        public static string getCompanyBasicInfoJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/MainServlet.json?sendAllMessage&EntId=";
        public static string getCompanyBasicInfoJsonCenter = "&EntNo=";
        public static string getCompanyBasicInfoJsonEnd = "&Info=";

        public static string getCompanyReportListJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/MainServlet.json?queryBusiness&EntId=";
        public static string getCompanyReportListJsonCenter = "&EntNo=";
        public static string getCompanyReportListJsonEnd = "&Info=";

        public static string getReportBasicInfoJsonHead = "http://www.jsgsj.gov.cn:58888/XhsPhone/MainServlet.json?queryNB&EntId=";
        public static string getReportBasicInfoJsonCenter = "&EntNo=";
        public static string getReportBasicInfoJsonEnd = "&Id=";
    }
}
