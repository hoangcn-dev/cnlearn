using Core.Models;

namespace Module.Users.Models.Requests
{
    public class SearchUserLogRequest : PagingRequest
    {
        public Guid? UserId { get; set; }
    }
}