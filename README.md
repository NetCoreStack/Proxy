### Cross-Platform .NET Core HTTP Base Flying Proxy

This project is demonstrating manage and consume distributed API - Micro Service 
from different regions (endpoints) with ease. 

This project aims to:
- Simple scalability
- Effective and type-safe management of distributed architecture
- Better performance
- Maintainability
- Provide updated API or SDK usage

[Latest release on Nuget](https://www.nuget.org/packages/NetCoreStack.Proxy/)

### Usage for Client Side

#### Startup ConfigureServices (ASP.NET Core method gets called by the runtime.)
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add NetCoreProxy Dependencies and Configuration
    services.AddNetCoreProxy(Configuration, options =>
    {
        // Register the API to use as Proxy
        options.Register<IGuidelineApi>();
    });

    // Add framework services.
    services.AddMvc();
}
```

#### Example Dependency Injection
```csharp
public class TestController : Controller
{
    private readonly IGuidelineApi _api;

    public TestController(IGuidelineApi api)
    {
        _api = api;
    }

    public async Task<IActionResult> GetPostsAsync()
    {
        var items = await _api.GetPostsAsync();
        return Json(items);
    }
}
```

#### Add Configuration Section to the appsettings.json file (or your configuration file)
##### Example for development environment:
```json
"ProxySettings": {
    "RegionKeys": {
      "Main": "http://localhost:5000/,http://localhost:5001/",
      "Authorization": "http://localhost:5002/",
      "Integrations": "http://localhost:5003/"
    }
  }
```

### Usage for Contracts (Common) Side

#### API Contract Definition (Default HttpMethod is HttpGet)
```csharp
[ApiRoute("api/[controller]", regionKey: "Main")]
public interface IGuidelineApi : IApiContract
{
    void VoidOperation();

    int PrimitiveReturn(int i, string s, long l, DateTime dt);

    Task TaskOperation();

    Task<IEnumerable<Post>> GetPostsAsync();

    Task GetWithReferenceType(SimpleModel model);

    [HttpPost]
    Task TaskActionPost(SimpleModel model);
}
```

### Usage for Backend - Server Side
#### API Contract Implementation
```csharp
[Route("api/[controller]")]
public class GuidelineController : Controller, IGuidelineApi
{
    private readonly ILoggerFactory _loggerFactory;

    protected ILogger Logger { get; }

    public GuidelineController(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        Logger = _loggerFactory.CreateLogger<GuidelineController>();
    }

    [HttpGet(nameof(GetPostsAsync))]
    public async Task<IEnumerable<Post>> GetPostsAsync()
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jsonplaceholder.typicode.com/posts"));
        var response = await Factory.Client.SendAsync(httpRequest);
        var content = await response.Content.ReadAsStringAsync();
        var items = JsonConvert.DeserializeObject<List<Post>>(content);
        Logger.LogDebug($"{nameof(GetPostsAsync)}, PostsCount:{items.Count}");
        return items;
    }

    [HttpGet(nameof(GetWithReferenceType))]
    public async Task GetWithReferenceType([FromQuery]SimpleModel model)
    {
        var serializedModel = JsonConvert.SerializeObject(model);
        Logger.LogDebug($"{nameof(GetWithReferenceType)}, Model: {serializedModel}");
        await Task.Delay(900);
    }

    [HttpGet(nameof(PrimitiveReturn))]
    public int PrimitiveReturn(int i, string s, long l, DateTime dt)
    {
        Logger.LogDebug($"{nameof(PrimitiveReturn)}, i:{i}, s:{s}, l:{l}, dt:{dt}");
        return i + 10;
    }

    [HttpPost(nameof(TaskActionPost))]
    public async Task TaskActionPost([FromBody]SimpleModel model)
    {
        var serializedModel = JsonConvert.SerializeObject(model);
        Logger.LogDebug($"{nameof(TaskActionPost)}, Model: {serializedModel}");
        await Task.Delay(900);
    }

    [HttpGet(nameof(TaskOperation))]
    public async Task TaskOperation()
    {
        await Task.Delay(2000);
        Logger.LogDebug($"{nameof(TaskOperation)}, long running process completed!");
    }

    [HttpGet(nameof(VoidOperation))]
    public void VoidOperation()
    {
        var str = "Hello World!";
        Logger.LogDebug($"{nameof(VoidOperation)}, {str}");
    }
}
```


### Prerequisites
> [ASP.NET Core](https://github.com/aspnet/Home)

### Installation
> dotnet restore