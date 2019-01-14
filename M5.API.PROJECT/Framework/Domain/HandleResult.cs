using System.Net;

namespace M5.API.PROJECT.Framework.Domain
{
    public class HandleResult
    {
        public HttpStatusCode StatusCode { get; private set; }
        public object Content { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        private HandleResult(HttpStatusCode statusCode, object content, string errorCode, string errorMessage)
        {
            StatusCode = statusCode;
            Content = content;
            ErrorCode = errorCode;
            ErrorMessage = ErrorMessage;
        }

        public static HandleResult Ok(object content = null, string errorCode = "", string errorMessage = "")
        {
            return new HandleResult(HttpStatusCode.OK, content, errorCode, errorMessage);
        }

        public static HandleResult NotFound(object content = null, string errorCode = "", string errorMessage = "")
        {
            return new HandleResult(HttpStatusCode.NotFound, content, errorCode, errorMessage);
        }

        public static HandleResult BadRequest(object content = null, string errorCode = "", string errorMessage = "")
        {
            return new HandleResult(HttpStatusCode.BadRequest, content, errorCode, errorMessage);
        }
    }
}
