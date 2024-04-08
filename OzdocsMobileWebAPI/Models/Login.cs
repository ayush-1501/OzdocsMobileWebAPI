namespace OzdocsMobileWebAPI.Models
{
    public class Login
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string OfficeId { get; set; }
        public string CompanyName { get; set; }
        public string Email_Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
