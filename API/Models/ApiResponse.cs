using Core.Utilities;

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

        public static SuccessResponse UpdateSuccess(object id)
        {
            return new SuccessResponse
            {
                Message = StringUtil.ApiMessages.Updated,
                Data = new UpdatedResponse
                {
                    UpdatedId = id
                }
            };
        }

        public static SuccessResponse DeleteSuccess(object id)
        {
            return new SuccessResponse
            {
                Message = StringUtil.ApiMessages.Deleted,
                Data = new DeletedResponse
                {
                    DeletedId = id
                }
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
