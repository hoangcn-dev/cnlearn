namespace HoangCN.LearnMS.Enums
{
    /// <summary>
    /// Quyền truy cập của câu hỏi trắc nghiệm
    /// </summary>
    public enum QuestionAccessType
    {
        /// <summary>
        /// Công khai (Mọi người đều có thể xem và làm)
        /// </summary>
        Public = 0,

        /// <summary>
        /// Riêng tư (Chỉ chủ sở hữu câu hỏi mới xem được)
        /// </summary>
        Private = 1
    }
}

