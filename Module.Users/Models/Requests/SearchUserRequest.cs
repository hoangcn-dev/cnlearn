using Core.Models;

namespace Module.Users.Models.Requests
{
    public class SearchUserRequest : PagingRequest
    {
        public string? Email { get; set; }
    }
}
