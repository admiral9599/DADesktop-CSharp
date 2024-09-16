using Newtonsoft.Json;

namespace DriveAdviser.Core.Models
{
    public class ServerJsonResult
    {
        [JsonProperty]
        public bool Success { get; set; }

        [JsonProperty]
        public object Id { get; set; }

        [JsonProperty]
        public string Msg { get; set; }

        //[JsonConverter(typeof(InfoConverter))]
        public All All { get; set; }

        [JsonConverter(typeof(InfoConverter))]
        public IdComputer Id_Computer { get; set; }
    }

    public class All
    {
        public bool Success { get; set; }
        public string Id { get; set; }
        public string Msg { get; set; }
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
        public string id_computer { get; set; }
        public string wkey { get; set; }
        public string computerName { get; set; }
        public string active { get; set; }
        public string dateCreated { get; set; }
        public string id_computerDetail { get; set; }
        public string id_computerSchrockSystem { get; set; }
        public string id_User { get; set; }
    }
    public class ComputerDetail
    {
        public bool Success { get; set; }
        public string Id { get; set; }
        public string Msg { get; set; }
        public All all { get; set; }
    }

    public class IdComputer
    {
        public bool Success { get; set; }
        public string Id { get; set; }
        public string Msg { get; set; }
        public All ComputerInfo { get; set; }
    }
}