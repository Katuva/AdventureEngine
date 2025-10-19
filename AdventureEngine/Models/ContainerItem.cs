namespace AdventureEngine.Models;

/// <summary>
/// Junction table linking items to containers they're stored in
/// </summary>
public class ContainerItem
{
    public int Id { get; set; }
    public int ContainerId { get; set; }
    public Container Container { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
}
