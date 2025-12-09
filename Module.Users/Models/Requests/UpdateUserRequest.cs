namespace Module.Users.Models.Requests
{
    public class UpdateUserRequest
    {
        public bool IsActived { get; set; }
        public string? LockReason { get; set; }
        public string? Note { get; set; }
    }
}