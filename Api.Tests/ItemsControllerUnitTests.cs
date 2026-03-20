using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Api.Data;
using Api.Models;
using Xunit;

namespace Api.Tests;

public class ItemsControllerUnitTests
{
    private static ItemsController CreateController(AppDbContext db)
    {
        return new ItemsController(db);
    }

    [Fact]
    public async Task GetAll_RetourneListeVide_QuandAucunItem()
    {
        await using var db = TestDbContext.CreateInMemory(nameof(GetAll_RetourneListeVide_QuandAucunItem));
        var controller = CreateController(db);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<ActionResult<List<Item>>>(result);
        var list = Assert.IsType<List<Item>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetById_RetourneNotFound_QuandItemInexistant()
    {
        await using var db = TestDbContext.CreateInMemory(nameof(GetById_RetourneNotFound_QuandItemInexistant));
        var controller = CreateController(db);

        var result = await controller.GetById(99999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_AjouteItem_EtRetourneCreatedAtAction()
    {
        await using var db = TestDbContext.CreateInMemory(nameof(Create_AjouteItem_EtRetourneCreatedAtAction));
        var controller = CreateController(db);
        var item = new Item { Name = "UnitTest", Description = "Desc" };

        var result = await controller.Create(item, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdItem = Assert.IsType<Item>(created.Value);
        Assert.True(createdItem.Id > 0);
        Assert.Equal("UnitTest", createdItem.Name);
        Assert.Single(db.Items);
    }

    [Fact]
    public async Task Delete_RetourneNoContent_QuandItemSupprime()
    {
        await using var db = TestDbContext.CreateInMemory(nameof(Delete_RetourneNoContent_QuandItemSupprime));
        db.Items.Add(new Item { Name = "X" });
        await db.SaveChangesAsync();
        var controller = CreateController(db);
        var id = db.Items.Single().Id;

        var result = await controller.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(db.Items);
    }
}
