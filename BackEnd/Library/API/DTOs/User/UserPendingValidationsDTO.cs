namespace API.DTOs.User
{
    public class UserPendingValidationsDTO
    {
        public bool EmailIsPending { get; set; }
        public bool PhoneNumberIsPending { get; set; }
    }
}
