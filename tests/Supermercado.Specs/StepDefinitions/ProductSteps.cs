using System.Net;
using System.Net.Http.Json;
using Supermercado.Api.Contracts;
using Supermercado.Specs.Support;
using Reqnroll;

namespace Supermercado.Specs.StepDefinitions;

[Binding]
public sealed class ProductSteps(ApiContext api)
{
    private const string Url = "api/products";

    [Given("que existe el producto {string} en la categoría {string}")]
    public async Task GivenProductExists(string name, string category)
    {
        await CreateAsync(new { name, categoryId = api.CategoryIds[category] });
        api.Response.EnsureSuccessStatusCode();
    }

    [When("creo un producto con nombre {string}, descripción {string} y categoría {string}")]
    public Task WhenCreateProduct(string name, string description, string category) =>
        CreateAsync(new { name, description, categoryId = api.CategoryIds[category] });

    [When("creo un producto con nombre {string} sin descripción en la categoría {string}")]
    public Task WhenCreateProductWithoutDescription(string name, string category) =>
        CreateAsync(new { name, categoryId = api.CategoryIds[category] });

    [When("creo un producto sin nombre en la categoría {string}")]
    public Task WhenCreateProductWithoutName(string category) =>
        CreateAsync(new { description = "Sin nombre", categoryId = api.CategoryIds[category] });

    [When("creo un producto con nombre {string} sin categoría")]
    public Task WhenCreateProductWithoutCategory(string name) => CreateAsync(new { name });

    [When("creo un producto con nombre {string} en una categoría que no existe")]
    public Task WhenCreateProductInMissingCategory(string name) =>
        CreateAsync(new { name, categoryId = ApiContext.MissingId });

    [When("edito el producto {string} con nombre {string}, descripción {string} y categoría {string}")]
    public Task WhenEditProduct(string current, string name, string description, string category) =>
        UpdateAsync(api.ProductIds[current], new { name, description, categoryId = api.CategoryIds[category] });

    [When("edito el producto {string} sin nombre")]
    public Task WhenEditProductWithoutName(string current) =>
        UpdateAsync(api.ProductIds[current], new { categoryId = api.CategoryIds.Values.First() });

    [When("edito el producto {string} sin categoría")]
    public Task WhenEditProductWithoutCategory(string current) =>
        UpdateAsync(api.ProductIds[current], new { name = current });

    [When("edito un producto que no existe")]
    public Task WhenEditMissingProduct() =>
        UpdateAsync(ApiContext.MissingId, new { name = "Fantasma", categoryId = api.CategoryIds.Values.First() });

    [When("consulto el producto {string}")]
    public async Task WhenGetProduct(string name) =>
        api.LastResponse = await api.Client.GetAsync($"{Url}/{api.ProductIds[name]}");

    [When("consulto un producto que no existe")]
    public async Task WhenGetMissingProduct() =>
        api.LastResponse = await api.Client.GetAsync($"{Url}/{ApiContext.MissingId}");

    [When("consulto el listado de productos")]
    public async Task WhenGetAllProducts() => api.LastResponse = await api.Client.GetAsync(Url);

    [When("elimino el producto {string}")]
    public async Task WhenDeleteProduct(string name) =>
        api.LastResponse = await api.Client.DeleteAsync($"{Url}/{api.ProductIds[name]}");

    [When("elimino un producto que no existe")]
    public async Task WhenDeleteMissingProduct() =>
        api.LastResponse = await api.Client.DeleteAsync($"{Url}/{ApiContext.MissingId}");

    [Then("el producto debe tener nombre {string} y descripción {string}")]
    public async Task ThenProductHas(string name, string description)
    {
        var product = await api.ReadAsync<ProductResponse>();
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
    }

    [Then("el producto debe tener nombre {string} sin descripción")]
    public async Task ThenProductHasNoDescription(string name)
    {
        var product = await api.ReadAsync<ProductResponse>();
        Assert.Equal(name, product.Name);
        Assert.Null(product.Description);
    }

    [Then("el producto debe pertenecer a la categoría {string}")]
    public async Task ThenProductBelongsTo(string category)
    {
        var product = await api.ReadAsync<ProductResponse>();
        Assert.Equal(api.CategoryIds[category], product.Category.Id);
        Assert.Equal(category, product.Category.Name);
    }

    [Then("el listado de productos debe ser:")]
    public async Task ThenProductListIs(DataTable table)
    {
        var products = await api.ReadAsync<List<ProductResponse>>();
        var expected = table.Rows.Select(r => (r["Nombre"], r["Categoría"]));
        var actual = products.Select(p => (p.Name, p.Category.Name));
        Assert.Equal(expected, actual);
    }

    [Then("el producto {string} ya no debe existir")]
    public async Task ThenProductDoesNotExist(string name)
    {
        using var response = await api.Client.GetAsync($"{Url}/{api.ProductIds[name]}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task CreateAsync(object body)
    {
        api.LastResponse = await api.Client.PostAsJsonAsync(Url, body);
        if (api.LastResponse.IsSuccessStatusCode)
        {
            var created = await api.ReadAsync<ProductResponse>();
            api.ProductIds[created.Name] = created.Id;
        }
    }

    private async Task UpdateAsync(int id, object body) =>
        api.LastResponse = await api.Client.PutAsJsonAsync($"{Url}/{id}", body);
}
