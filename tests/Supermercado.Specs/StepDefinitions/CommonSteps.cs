using Supermercado.Specs.Support;
using Reqnroll;

namespace Supermercado.Specs.StepDefinitions;

[Binding]
public sealed class CommonSteps(ApiContext api)
{
    [Then("la respuesta debe tener el código {int}")]
    public async Task ThenStatusCodeIs(int expected)
    {
        var actual = (int)api.Response.StatusCode;
        if (actual != expected)
        {
            var body = await api.Response.Content.ReadAsStringAsync();
            Assert.Fail($"Se esperaba {expected} pero llegó {actual}. Cuerpo: {body}");
        }
    }

    [Then("la respuesta debe contener el mensaje {string}")]
    public async Task ThenResponseContainsMessage(string message)
    {
        var messages = await api.ReadErrorMessagesAsync();
        Assert.Contains(message, messages);
    }
}
