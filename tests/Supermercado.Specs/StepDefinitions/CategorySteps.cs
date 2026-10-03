using System.Net;
using System.Net.Http.Json;
using Supermercado.Api.Contracts;
using Supermercado.Specs.Support;
using Reqnroll;

namespace Supermercado.Specs.StepDefinitions;

[Binding]
public sealed class CategorySteps(ApiContext api)
{
    private const string Url = "api/categories";

    [Given("que existe la categoría {string}")]
    public Task GivenCategoryExists(string name) => CreateExistingAsync(name, null);

    [Given("que existe la categoría {string} con descripción {string}")]
    public Task GivenCategoryExistsWithDescription(string name, string description) =>
        CreateExistingAsync(name, description);

    [When("creo una categoría con nombre {string} y descripción {string}")]
    public Task WhenCreateCategory(string name, string description) => CreateAsync(new { name, description });

    [When("creo una categoría con nombre {string} sin descripción")]
    public Task WhenCreateCategoryWithoutDescription(string name) => CreateAsync(new { name });

    [When("creo una categoría sin nombre")]
    public Task WhenCreateCategoryWithoutName() => CreateAsync(new { description = "Sin nombre" });

    [When("edito la categoría {string} con nombre {string} y descripción {string}")]
    public Task WhenEditCategory(string current, string name, string description) =>
        UpdateAsync(api.CategoryIds[current], new { name, description });

    [When("edito la categoría {string} sin nombre")]
    public Task WhenEditCategoryWithoutName(string current) =>
        UpdateAsync(api.CategoryIds[current], new { description = "Sin nombre" });

    [When("edito una categoría que no existe")]
    public Task WhenEditMissingCategory() => UpdateAsync(ApiContext.MissingId, new { name = "Fantasma" });

    [When("consulto el listado de categorías")]
    public async Task WhenGetAllCategories() => api.LastResponse = await api.Client.GetAsync(Url);

    [When("consulto la categoría {string}")]
    public async Task WhenGetCategory(string name) =>
        api.LastResponse = await api.Client.GetAsync($"{Url}/{api.CategoryIds[name]}");

    [When("consulto una categoría que no existe")]
    public async Task WhenGetMissingCategory() =>
        api.LastResponse = await api.Client.GetAsync($"{Url}/{ApiContext.MissingId}");

    [When("elimino la categoría {string}")]
    public async Task WhenDeleteCategory(string name) =>
        api.LastResponse = await api.Client.DeleteAsync($"{Url}/{api.CategoryIds[name]}");

    [When("elimino una categoría que no existe")]
    public async Task WhenDeleteMissingCategory() =>
        api.LastResponse = await api.Client.DeleteAsync($"{Url}/{ApiContext.MissingId}");

    [Then("la categoría debe tener nombre {string} y descripción {string}")]
    public async Task ThenCategoryHas(string name, string description)
    {
        var category = await api.ReadAsync<CategoryResponse>();
        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
    }

    [Then("la categoría debe tener nombre {string} sin descripción")]
    public async Task ThenCategoryHasNoDescription(string name)
    {
        var category = await api.ReadAsync<CategoryResponse>();
        Assert.Equal(name, category.Name);
        Assert.Null(category.Description);
    }

    [Then("el listado de categorías debe ser:")]
    public async Task ThenCategoryListIs(DataTable table)
    {
        var categories = await api.ReadAsync<List<CategoryResponse>>();
        var expected = table.Rows.Select(r => (r["Nombre"], r["Descripción"]));
        var actual = categories.Select(c => (c.Name, c.Description ?? ""));
        Assert.Equal(expected, actual);
    }

    [Then("la categoría debe mostrar los productos:")]
    public async Task ThenCategoryShowsProducts(DataTable table)
    {
        var category = await api.ReadAsync<CategoryDetailResponse>();
        Assert.Equal(table.Rows.Select(r => r["Nombre"]), category.Products.Select(p => p.Name));
    }

    [Then("la categoría {string} ya no debe existir")]
    public async Task ThenCategoryDoesNotExist(string name)
    {
        using var response = await api.Client.GetAsync($"{Url}/{api.CategoryIds[name]}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task CreateExistingAsync(string name, string? description)
    {
        await CreateAsync(new { name, description });
        api.Response.EnsureSuccessStatusCode();
    }

    private async Task CreateAsync(object body)
    {
        api.LastResponse = await api.Client.PostAsJsonAsync(Url, body);
        if (api.LastResponse.IsSuccessStatusCode)
        {
            var created = await api.ReadAsync<CategoryResponse>();
            api.CategoryIds[created.Name] = created.Id;
        }
    }

    private async Task UpdateAsync(int id, object body) =>
        api.LastResponse = await api.Client.PutAsJsonAsync($"{Url}/{id}", body);
}
