namespace Entities.Models;

public class ShapedEntity
{
    public ShapedEntity() 
    {
        Entity = [];
    }

    public Entity Entity { get; set; }
    public Guid Id { get; set; }
}