### Cross-Platform .NET Standard HTTP Base Flying Proxy

This project is demonstrating manage and consume distributed HTTP APIs and Micro Services
from different regions (hosts) with ease.

Flying Proxy allows the management of scalable applications, trigger many operations at the same time from your clients (Desktop, Web or Mobile App) and start to consume your new resources that you can simply add.

Flying Proxy aims to:
- Simple scalability
- Effective and type-safe management of distributed architecture
- Better performance
- Maintainability

### Sample Client (Web)

#### Add ProxySettings section to the appsettings.json
```json
"ProxySettings": {
    "RegionKeys": {
        "Main": "http://localhost:5000/,http://localhost:5001/",
        "Authorization": "http://localhost:5002/",
        "Integrations": "http://localhost:5003/"
    }
}
```

#### APIs Definitions
```csharp
// This API expose methods from localhost:5000 and localhost:5001 as configured on ProxySettings
[ApiRoute("api/[controller]", regionKey: "Main")]
public interface IGuidelineApi : IApiContract
{
    [HttpHeaders("X-Method-Header: Some Value")]
    Task TaskOperation();

    int PrimitiveReturn(int i, string s, long l, DateTime dt);

    Task<IEnumerable<SampleModel>> GetEnumerableModels();

    Task GetWithReferenceType(SimpleModel model);

    /// <summary>
    /// Default Content-Type is ModelAware.
    /// If the any parameter(s) has FormFile type property that will be MultipartFormData 
    /// if not will be JSON
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPostMarker]
    Task TaskActionPost(ComplexTypeModel model);

    /// <summary>
    /// Template and parameter usage, key parameter will be part of the request Url 
    /// and extracting it as api/guideline/kv/<key>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPutMarker(Template = "kv/{key}")]
    Task<bool> CreateOrUpdateKey(string key, Bar body);
}
```

#### Startup ConfigureServices
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddNetCoreProxy(Configuration, options =>
    {
        // Register the API to use as a Proxy
        options.Register<IGuidelineApi>();
    });

    // Add framework services.
    services.AddMvc();
}
```

#### Proxy Usage (DI)
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

### Backend - Server Side
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

#### Multipart form data:
Proxy sends all POST methods as JSON but if the method parameter model contains IFormFile type property it converts the content-type to multipart/form-data. In this case, use any model to POST multipart/form-data to API without [FromBody] attribute on action parameter. For example:

```csharp
// Interface
[HttpPostMarker]
Task<AlbumViewModel> SaveAlbumSubmitAsync(AlbumViewModelSubmit model)    
```

```csharp
// API Controller
[HttpPost(nameof(SaveAlbumSubmitAsync))]
public async Task<AlbumViewModel> SaveAlbumSubmitAsync(AlbumViewModelSubmit model)  
```

#### Unit Testing
Use [HttpLive](https://github.com/gencebay/httplive)

	httplive -p 5003,5004 -d test/NetCoreStack.Proxy.Tests/httplive.db

[Latest release on Nuget](https://www.nuget.org/packages/NetCoreStack.Proxy/)

### Prerequisites
> [ASP.NET Core](https://github.com/aspnet/Home)