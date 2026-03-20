using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api.Data;
using Api.Models;
using Xunit;

namespace Api.Tests;

public class ItemsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ItemsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));
                if (descriptor != null) services.Remove(descriptor);
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestItems_" + Guid.NewGuid()));
            });
        });
        _client = _factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_RetourneListeVide_QuandAucunItem()
    {
        var response = await _client.GetAsync("/api/items");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<Item>>();
        Assert.NotNull(items);
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetById_Retourne404_QuandItemInexistant()
    {
        var response = await _client.GetAsync("/api/items/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_PuisGetAll_RetourneLeItemCree()
    {
        var item = new Item { Name = "Test", Description = "Desc" };
        var createResponse = await _client.PostAsJsonAsync("/api/items", item);
        createResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Test", created.Name);
        Assert.Equal("Desc", created.Description);

        var listResponse = await _client.GetAsync("/api/items");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<List<Item>>();
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal(created.Id, list[0].Id);
    }

    [Fact]
    public async Task GetById_RetourneItem_QuandExiste()
    {
        var item = new Item { Name = "GetById", Description = null };
        var createResponse = await _client.PostAsJsonAsync("/api/items", item);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/items/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var got = await getResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(got);
        Assert.Equal(created.Id, got.Id);
        Assert.Equal("GetById", got.Name);
    }

    [Fact]
    public async Task Update_ModifieItem_EtRetourne200()
    {
        var item = new Item { Name = "Avant" };
        var createResponse = await _client.PostAsJsonAsync("/api/items", item);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);

        created.Name = "Apres";
        created.Description = "Nouvelle desc";
        var updateResponse = await _client.PutAsJsonAsync($"/api/items/{created.Id}", created);
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(updated);
        Assert.Equal("Apres", updated.Name);
        Assert.Equal("Nouvelle desc", updated.Description);
    }

    [Fact]
    public async Task Update_Retourne404_QuandItemInexistant()
    {
        var item = new Item { Id = 99999, Name = "X" };
        var response = await _client.PutAsJsonAsync("/api/items/99999", item);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Retourne400_QuandIdIncoherent()
    {
        var item = new Item { Id = 1, Name = "X" };
        var response = await _client.PutAsJsonAsync("/api/items/2", item);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_SupprimeItem_EtRetourne204()
    {
        var item = new Item { Name = "ToDelete" };
        var createResponse = await _client.PostAsJsonAsync("/api/items", item);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Item>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Retourne404_QuandItemInexistant()
    {
        var response = await _client.DeleteAsync("/api/items/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
