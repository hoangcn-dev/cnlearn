namespace API.Models
{
    public class ApiResponse
    {
        public static SuccessResponse Success()
        {
            return new SuccessResponse
            {
                Message = null,
                Data = null
            };
        }

        public static SuccessResponse Success(string message)
        {
            return new SuccessResponse
            {
                Message = message,
                Data = null
            };
        }

        public static SuccessResponse Success(object data)
        {
            return new SuccessResponse
            {
                Message = null,
                Data = data
            };
        }

        public static ErrorResponse Error()
        {
            return new ErrorResponse
            {
                ErrorMessage = null,
            };
        }

        public static ErrorResponse Error(string error)
        {
            return new ErrorResponse
            {
                ErrorMessage = error,
            };
        }
    }
}
