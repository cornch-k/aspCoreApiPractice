using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace aspCoreApiPractice.Controllers
{
    public struct CreateProductRequestDto
    {
        // [Required]를 쓰면, Id만 필수필드.
        // 나머지도 똑같이 적용하고 싶다면 전부 어노테이션을 붙이기
        // [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    [Route("/APIExample")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("Hello")] // /APIExample/Hello?name=Cornch
        [HttpGet]
        public string Hello(string? name)
        {
            return $"HELLO, {name}"; // Hello, Cornch
        }
        // [HTTPGet]을 빼고, public string GetHello()로 작성하는 것도 되나,
        // swagger gen이 깨져버림.
        // body string 전체를 받고 싶으면, public string Hello([FromBody] string? name)


        // 기본적으로 Body Param은 Optional 설정이 기본
        // 필수로 하고싶다면, 구조체 필드 위에 [Required] 어노테이션 쓰기
        [Route("Product")]
        [HttpPost]
        public string CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            return $"Product: {dto.Id}, {dto.Name}, {dto.Price}";
        }

        [Route("Header")]
        [HttpGet]
        public string HelloHeader([FromHeader(Name = "user-agent")] string userAgent, [FromQuery] string name)
        {
            return $"{userAgent}: Hello, {name}";
        }

        [Route("IActionResult")]
        [HttpGet]
        // IActionResult ActionResultTest()를 해도 되지만,
        // 문서화가 힘들기 때문에 redirect를 받는게 아니라면 ActionResult<T>를 쓰는게 더 좋다.
        public ActionResult<TestResponse> ActionResultTest()
        {
            var response = new TestResponse();
            response.Buldak = "bokkeummyeon";
            response.price = 13000;

            return Ok(response);
        }

        [Route("StatudCode")]
        [HttpGet]
        public ActionResult<TestResponse> StatusCodeTest()
        {
            var response = new TestResponse();
            response.Buldak = "bokkeummyeon";
            response.price = 13000;

            // return StatusCode(Code, return값)을 하면 커스텀 상태 코드를 반환가능.
            return StatusCode(404, response);
        }

        [Route("Redirect")]
        [HttpGet]
        // Redirect를 할 때에는 IActionResult를 써서 어떤거든 return 받을 수 있게 하는게 편함.
        public IActionResult RedirectTest()
        {
            var response = new TestResponse();
            response.Buldak = "samyang";
            response.price = 130000;

            return Redirect("/APIExample/Hello?name=test");
            // https://naver.com으로 하니까 200은 뜨지만 TypeError: Load failed를 반환한다.
            // 공부중인 로컬호스트 주소로 바꾸니까 제대로 동작함
        }
    }
}