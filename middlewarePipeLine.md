# 미들웨어 파이프라인

### 미들웨어(Middleware)는 서로 다른 애플리케이션이 서로 통신하는 데 사용되는 소프트웨어 구성요소이다.<br>

이렇게 미들웨어를 연결한 것을 요청 파이프라인이라고 하며, Controller, ApplicationCode, Request Pipeline, Middleware Pipeline 등은 동의어로 취급한다.
<br><br>

### [미들웨어 파이프라인의 구조.]<br>

브라우저 (HTTP)→ ASP.NET Core Module (Native Request)→ IISHTTPServer (Managed Request)→ \***\*MiddleWare Pipeline\*\*** (Http Context)→ Application Code<br><br>

여기서 ASP.NET Core Module은 IIS Server에, IISHttpServer, Middleware Pipeline, Application Code는 ASP.NET Core Application에 속한다.<br><br>

ASP.NET Core 애플리케이션에 대한 요청은 컨트롤러(Application Code) 에서 처리되기 전에 요청 파이프라인 (Middleware Pipeline) 에서 순차적으로 처리된다.<br><br>

### 파이프라인 구축

```
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

```

여기서 app 변수는 위 builder에 따라 WebApplication 클래스의 인스턴스고,<br>
WebApplication의 역할은<br>

1. 미들웨어 파이프라인 구축
2. 호스트 시작

<br>
이며, Use, Map, Run 등의 메서드를 이용하여 파이프라인을 구축해 나간다.

### 파이프라인 구축 방법

1. 인라인으로 정의<br>
   예를 들어 아래 코드와 같이 할 수 있다.<br>
   간단하고 빠르게 작성할 수 있지만, 프로젝트 규모가 커지면 코드가 지저분해지고 재사용이 어렵다.<br>

   ```
    app.Use(async (context, next) =>
    {
        // 요청 처리 전
        Console.WriteLine("요청처리");

        await next.Invoke(); // 다음 미들웨어로 전달

        // 응답 처리 후
        Console.WriteLine("응답완료");
    });
   ```

2. 재이용 가능한 클래스로서 정의<br>
   2-1. 내장 미들웨어<br>
   ASP.NET에서 기본적으로 제공해 주는 미들웨어로, 다양하게 있다.<br>
   예를들어, 아래와 같은 메서드가 전부 미들웨어를 추가하는 것들이다.<br>
   기본적으로 제공된 만큼, 안정적이고 동작이 검증되어 있다.

   ```
   app.UseRouting(); // RoutingMiddleware 클래스가 내부에 이미 구현되어 있음
   app.UseAuthentication(); // AuthenticationMiddleware 클래스가 이미 있음
   app.UseStaticFiles(); // StaticFileMiddleware 클래스가 이미 있음
   ```

   2-2. 커스텀 미들웨어<br>
   직접 만드는 **미들웨어 클래스**로, 내장 미들웨어에서 제공하지 않는 특별한 기능이 필요한 등의 상황에서 사용한다.<br>
   예를들어, 아래와 같은 코드가 있겠다.

   ```
   public class BusinessHoursMiddleware
   {
    private readonly RequestDelegate _next;

    public BusinessHoursMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var currentHour = DateTime.Now.Hour;

        // 영업시간은 오전 9시부터 오후 6시까지
        if (currentHour < 9 || currentHour >= 18)
        {
            // 영업시간이 아니면 403 에러 반환
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("영업시간은 오전 9시부터 오후 6시까지입니다.");
            return;
        }

        await _next(context);
    }
   }
   ```

   <br>

### 파이프라인 추가 순서 \***\*(매우 중요!)\*\***

[공식문서에도 아래와 같이 적혀 있다.](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#built-in-middleware)

> 미들웨어 구성 요소가 파일에 추가 `Program.cs` 되는 순서는 요청 시 미들웨어 구성 요소가 호출되는 순서와 응답의 역순을 정의합니다. 순서는 보안, 성능 및 기능에 **중요**합니다.

아래 코드를 실행하면 다음과 같이 처리가 될 것이다.

```
// 첫 번째 미들웨어
app.Use(async (context, next) =>
{
    Console.WriteLine("1번 미들웨어 - 요청 처리 전");
    await next(); // 다음 미들웨어로 진행
    Console.WriteLine("1번 미들웨어 - 응답 처리 후");
});

// 두 번째 미들웨어
app.Use(async (context, next) =>
{
    Console.WriteLine("2번 미들웨어 - 요청 처리 전");
    await next();
    Console.WriteLine("2번 미들웨어 - 응답 처리 후");
});

// 세 번째 미들웨어
app.Run(async (context) =>
{
    Console.WriteLine("3번 미들웨어 - 최종 처리");
    await context.Response.WriteAsync("응답");
});
```

```
1번 미들웨어 - 요청 처리 전
2번 미들웨어 - 요청 처리 전
3번 미들웨어 - 최종 처리
   (3번 미들웨어에서 HTTP Body Response)
2번 미들웨어 - 응답 처리 후
1번 미들웨어 - 응답 처리 후
```

즉, 미들웨어는 1, 2, 3번 순서대로 실행이 됐고, 3, 2, 1 순으로 응답하는 것이다.<br>
Authentication 부분을 예로 들어보자.

```
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
```

Authentication을 먼저 배치하고, Routing을 나중에 배치한다면, 인증 미들웨어가 실행될 때 어떤 엔드포인트로 가야할지 몰라 제대로 동작이 되지 않을 것이다.<br><br> \***\*또한 예외처리 미들웨어는 반드시 맨 앞쪽에 배치해야 한다.\*\***

```
app.UseExceptionHandler("/Error"); // 맨 앞에 배치
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### ASP.NET Core 공식 문서에서 권장하는 일반적인 순서는 다음과 같다.

```
app.UseExceptionHandler("/Error"); // 1. 예외 처리
app.UseHttpsRedirection(); // 2. HTTPS 리다이렉션
app.UseStaticFiles(); // 3. 정적 파일
app.UseRouting(); // 4. 라우팅
app.UseCors(); // 5. CORS
app.UseAuthentication(); // 6. 인증
app.UseAuthorization(); // 7. 권한 확인

// 커스텀 미들웨어

app.MapControllers(); // 8. 엔드포인트 매핑
```
