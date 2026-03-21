namespace Api.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; }

    /// <summary>Priorité (par défaut : normale).</summary>
    public ItemPriority Priority { get; set; } = ItemPriority.Normal;

    // Date d'échéance optionnelle (stockée en UTC).
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Catégorie optionnelle (user story « ajouter une catégorie »).</summary>
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
