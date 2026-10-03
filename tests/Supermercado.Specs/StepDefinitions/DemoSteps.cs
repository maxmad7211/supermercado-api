using Supermercado.Api.Contracts;
using Supermercado.Specs.Support;
using Reqnroll;

namespace Supermercado.Specs.StepDefinitions;

[Binding]
public sealed class DemoSteps(ApiContext api)
{
    [When("reinicio los datos de demostración")]
    public async Task WhenResetDemoData() =>
        api.LastResponse = await api.Client.PostAsync("api/demo/reset", null);

    [Then("el listado de categorías debe estar vacío")]
    public async Task ThenCategoriesEmpty()
    {
        using var response = await api.Client.GetAsync("api/categories");
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
    }

    [Then("el listado de productos debe estar vacío")]
    public async Task ThenProductsEmpty()
    {
        using var response = await api.Client.GetAsync("api/products");
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
    }

    [Then("la categoría creada debe tener el id {int}")]
    public async Task ThenCreatedCategoryHasId(int id)
    {
        var category = await api.ReadAsync<CategoryResponse>();
        Assert.Equal(id, category.Id);
    }
}
