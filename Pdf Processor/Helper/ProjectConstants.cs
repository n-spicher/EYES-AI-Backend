using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Helper
{
    public class ProjectConstants
    {
        public static string AuthKey = "this is a key for creating jwt token.it is a temporary key and will be replaced by your original key. this can any string containing any character";

        public static string smtpEmail = "";
        public static string smtpPassword = "";
        public static int smtpPort = 465;
        public static string smptpHost = "";

        public static string mailFrom = "";
    }
}
