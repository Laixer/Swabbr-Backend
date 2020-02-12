using Newtonsoft.Json;

namespace Swabbr.Api.Errors
{

    public class ErrorMessage
    {
        public ErrorMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}