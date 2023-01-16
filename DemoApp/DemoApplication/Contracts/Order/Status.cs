namespace DemoApplication.Contracts.Order
{
    public enum Status
    {
        Created = 1,
        Confirmed = 2,
        Rejected = 3,
        Sended = 4,
        Completed = 5,
    }
    public static class StatusStatusCode
    {
        public static string GetStatusCode(this Status status)
        {
            switch (status)
            {
                case Status.Created:
                    return "Created";
                case Status.Confirmed:
                    return "Confirmed";
                case Status.Rejected:
                    return "Rejected";
                case Status.Sended:
                    return "Sended";
                case Status.Completed:
                    return "Completed";
                default:
                    throw new Exception("This status code not found");

            }
        }
    }
}
