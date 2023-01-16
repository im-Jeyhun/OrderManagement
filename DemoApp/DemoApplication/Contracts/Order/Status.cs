using DemoApplication.Database.Models;

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

    public static class StatusMessage
    {
        public static string GetStatusMessage(this Status status , User user , string orderNumber)
        {
            switch (status)
            {
                case Status.Created:
                    return $"Hörmətli {user.FirstName} {user.LastName}, sizin {orderNumber} yaradildi.";
                case Status.Confirmed:
                    return $"Hörmətli {user.FirstName} {user.LastName}, sizin {orderNumber} tesdiqlendi.";
                case Status.Rejected:
                    return $"Hörmətli {user.FirstName} {user.LastName}, sizin {orderNumber} tesdiqlenmedi.";
                case Status.Sended:
                    return $"Hörmətli {user.FirstName} {user.LastName}, sizin {orderNumber} göndərildi, kuryer sizinlə əlaqə saxlayacaq..";
                case Status.Completed:
                    return $"Hörmətli {user.FirstName} {user.LastName}, sizin {orderNumber} kuryer tərəfindən təhvil verildi..";
                default:
                    throw new Exception("This status code not found");

            }
        }
    }
}