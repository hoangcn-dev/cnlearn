using HoangCN.Core.Common.Attributes;
using HoangCN.LearnMS.Entities;
using System.Text.Json.Serialization;

namespace HoangCN.LearnMS.Requests
{
    /// <summary>
    /// Lưu/Bỏ lưu câu hỏi
    /// </summary>
    public class ToggleUserSavedRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
        public bool IsSaved { get; set; }
    }
}
