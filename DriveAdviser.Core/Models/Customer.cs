namespace DriveAdviser.Core.Models
{
    public class Customer
    {
        public string siTag { get; set; }
        public string CustomerID { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Zip4 { get; set; }
        public object PriPhone { get; set; }
        public object SecPhone { get; set; }
        public string PrimaryPhone { get; set; }
        public string HomePhone { get; set; }
        public object MobilePhone { get; set; }
        public object WorkPhone { get; set; }
        public string Email { get; set; }
        public object Email2 { get; set; }
        public object Email3 { get; set; }
        public string OnPromoList { get; set; }
        public string OnAlertList { get; set; }
    }
}