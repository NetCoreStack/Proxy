### Cross-Platform .NET Standard HTTP Base Flying Proxy

This project is demonstrating manage and consume distributed HTTP APIs and Micro Services
from different regions (hosts) with ease.

Flying Proxy allows the management of scalable applications, trigger many operations at the same time from your clients (Desktop, Web or Mobile App) and start to consume your new resources that you can simply add.

Flying Proxy aims to:
- Simple scalability
- Effective and type-safe management of distributed architecture
- Better performance
- Maintainability

## Sample Client (Web)

### Add ProxySettings section to the appsettings.json
```json
"ProxySettings": {
    "RegionKeys": {
        "Main": "http://localhost:5000/,http://localhost:5001/",
        "Authorization": "http://localhost:5002/",
        "Integrations": "http://localhost:5003/"
    }
}
```

### APIs Definitions
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

    [HttpPostMarker(ContentType = ContentType.MultipartFormData)]
    Task TaskActionBarMultipartFormData(Bar model);

    [HttpPostMarker(ContentType = ContentType.Xml)]
    Task TaskActionBarSimpleXml(BarSimple model);

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

### Startup ConfigureServices
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

### Proxy Usage (DI)
```csharp
public class HomeController : Controller
{
    private readonly IGuidelineApi _api;

    public TestController(IGuidelineApi api)
    {
        _api = api;
    }

    public async Task<IEnumerable<SampleModel>> GetModels()
    {
        var items = await _api.GetEnumerableModels();
        return items;
    }
}
```

## Sample Server

### API Implementation
```csharp
[Route("api/[controller]")]
public class GuidelineController : Controller, IGuidelineApi
{
    [HttpGet(nameof(GetEnumerableModels))]
    public Task<IEnumerable<SampleModel>> GetEnumerableModels()
    {
        ...
        return items;
    }

    [HttpPost(nameof(TaskComplexTypeModel))]
    public async Task TaskComplexTypeModel([FromBody]ComplexTypeModel model)
    {
        ...
    }

    [HttpPost(nameof(TaskActionBarMultipartFormData))]
    public Task TaskActionBarMultipartFormData(Bar model)
    {
        ...
    }

    [HttpPut("kv")]
    public async Task<bool> CreateOrUpdateKey(string key, Bar body)
    {
        ...
    }
}
```

### Unit Testing
Use [HttpLive](https://github.com/gencebay/httplive)

	httplive -p 5003,5004 -d test/NetCoreStack.Proxy.Tests/httplive.db

[Latest release on Nuget](https://www.nuget.org/packages/NetCoreStack.Proxy/)

### Prerequisites
> [ASP.NET Core](https://github.com/aspnet/Home)