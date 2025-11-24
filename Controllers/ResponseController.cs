using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace aspCoreApiPractice.Controllers
{
    // getter, setter을 쓰고,
    // 필드가 대문자로 시작하면, JSON으로 변환을 해서 보내준다.
    public struct TestResponse
    {
        public string Buldak { get; set; }
        public double price { get; set; }
    }
    [Route("Responses")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        [Route("hello")]
        [HttpGet]
        public TestResponse Test()
        {
            var response = new TestResponse();
            response.Buldak = "볶음면";
            response.price = 3000;

            return response;
        }


        // 비동기 핸들링: Task
        [Route("asynchronous")]
        [HttpGet]
        public async Task<TestResponse> asyncTest()
        {
            var response = new TestResponse();
            response.Buldak = "뽂음면";
            response.price = 3500;

            await Task.Delay(10000); // 10초 뒤 return됨

            return response;
        }
    }
}