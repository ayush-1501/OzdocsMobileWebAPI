namespace OzdocsMobileWebAPI.Models
{
    public class Master
    {
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string ReferenceId { get; set; }
        public string OfficeId { get; set; } 
		public string Exporter { get; set; }
		public string Consignee { get; set; }
        public string Buyer { get; set; }
        public decimal InvoiceValue { get; set; }
        public string Currency { get; set; }        
		public string InvoiceNo { get; set; }
		public DateTime InvoiceDate { get; set; }
        public string DocumentStatus { get; set; }
        public string EDNStatus { get; set; }
        public string EDN { get; set; }
        public string Details { get; set; }
        public string ExporterRef { get; set; }
        public string BuyerRef { get; set; }
        public string MUserId { get; set; }  
    }
}
