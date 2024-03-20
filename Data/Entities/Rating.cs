namespace WebApi.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public Trip Trip { get; set; }
}