namespace OzdocsMobileWebAPI.Models
{
    public class FIDocument
    {
        public int  Id { get; set; }
        public string FidocsId { get; set; }
        public int Version { get; set; }
        public string Status { get; set; }
        public string OfficeId { get; set; }
        public DateTime SDateTime { get; set; }
        public DateTime RDateTime { get; set; }
        public string ControlAck {  get; set; }
        public string Flag { get; set; }
        public string Temp { get; set; }
        public string StatusDesc { get; set; }
        public float ControlRef { get; set; }
        public string FiRef { get; set;}
        public string BillRef { get; set; }
        public string IsLastLine { get; set;}
        public string Info { get; set; }
        public string Error1 { get; set; }
        public string Error2 { get; set; }
        public string Error3 { get; set; }
        public string Error4 { get; set; }
        public string Error5 { get; set; }
        public string Error6 { get; set; }
        public string Error7 { get; set; }
        public string Error8 { get; set; }
        public string Error9 { get; set; }
        public string Error10 { get; set; }
        public string Error11 { get; set; }
        public string Error12 { get; set; }
        public string Error13 { get; set; }
        public string Error14 { get; set; }
        public string Error15 { get; set; }
        public string Error16 { get; set; }
        public string Error17 { get; set; }
        public string Error18 { get; set; }
        public string Error19 { get; set; }
        public string Error20 { get; set; }
        public string File_In_Name { get; set; }
        public string File_In_Content { get; set; }
        public string File_Out_Name { get; set; }
        public string File_Out_Content { get; set; }


        public int IsNew { get; set; }
    }
}
