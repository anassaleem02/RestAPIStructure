using System.Net;

namespace CommonDataLayer.Model.ResponseModels
{
    public class ApiResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int? Count { get; set; }
        public object Data { get; set; }
        public object Error { get; set; }
    }
}
