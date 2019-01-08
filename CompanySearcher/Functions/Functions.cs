using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CompanySearcher
{
    public class Functions
    {
        public static string tryGetValueFromNavigation(string contents, string valueName)
        {
            try
            {
                if (!contents.Contains(valueName))
                    return "";

                int valueBegin = contents.IndexOf(valueName) + valueName.Length + 1;
                int valueEnd = contents.IndexOf("&", valueBegin);
                string result;
                if (valueEnd > 0)
                    result = contents.Substring(valueBegin, valueEnd - valueBegin);
                else
                    result = contents.Substring(valueBegin, contents.Length - valueBegin);
                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string convertStringToBase64(string contents)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(contents)).Replace("+", "%242").Replace("=", "%241");
            }
            catch
            {
                return "";
            }
        }

        public static void loadUIEImg(Image uie, string imgUrl)
        {
            try
            {
                Uri imageUri;
                BitmapImage bitMapImage = new BitmapImage();
                imageUri = new Uri(new Uri("ms-appx://"), imgUrl);
                bitMapImage.UriSource = imageUri;
                uie.Source = bitMapImage;
            }
            catch
            {
            }
        }
    }
}
