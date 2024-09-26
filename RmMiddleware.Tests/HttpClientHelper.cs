using Target = RmMiddleware.Helpers;

namespace RmMiddleware.Tests;

public class HttpClientHelper
{
    [Fact]
    public async Task GetReturnJObject()
    {
        var jObject = await Target.HttpClientHelper.GetReturnJObject(new Uri("http://localhost:5079/api/Test"), null);
        await Target.HttpClientHelper.PostStringBodyReturnJObject(new Uri("http://localhost:5079/api/Test"),
            jObject?.ToString(), null);
        Assert.True(true);
    }
}