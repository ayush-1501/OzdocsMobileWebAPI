using Microsoft.Extensions.Configuration;
using System.IO;

namespace OzdocsMobileWebAPI.BusinessLayer
{
    public class BusinessTier
    {
        private readonly IConfiguration _configuration;        

        private string EDNFolder = "EDNTransmission";
        //private string PRAFolder = "PRATransmission";
        //private string AQISFolder = "AQISTransmission";

        public BusinessTier(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public enum FileType
        {
            IN,
            OUT            
        }
        public class FileData
        {
            public int RecordId { get; set; }
            public string OfficeId { get; set; }
            public string RefId { get; set; }
            public int Version { get; set; }
            public FileType Type { get; set; }
            public string FileContent { get; set; }

        }
        public void SaveEDIFile(FileData oFileData)
        {
            string _TransmissionFolder = _configuration.GetValue<string>("TransmissionFolder"), _EDNTransmissionLocation = _TransmissionFolder + EDNFolder + "\\" + oFileData.OfficeId;
            //_EDNTransmissionLocation =  "D:\\VDF_Work\\Transmission\\" + "EDNTransmission" + "\\" + "ANZCO"

            string _EDNTransmissionFile = string.Empty;

            if (!Directory.Exists(_EDNTransmissionLocation))
            {
                Directory.CreateDirectory(_EDNTransmissionLocation);
            }

            if (oFileData.Type== FileType.IN)
            {
                _EDNTransmissionFile = _EDNTransmissionLocation + "\\" + oFileData.RefId + "(" + oFileData.Version + ")_In_" + oFileData.RecordId.ToString() + ".edi";
            }
            else
            {
                _EDNTransmissionFile = _EDNTransmissionLocation + "\\" + oFileData.RefId + "(" + oFileData.Version + ")_Out_" + oFileData.RecordId.ToString() + ".edi";
            }

            StreamWriter sw = new StreamWriter(_EDNTransmissionFile, false);
            sw.WriteLine(oFileData.FileContent);
            sw.Flush();
            sw.Close();
        }
    }
}
