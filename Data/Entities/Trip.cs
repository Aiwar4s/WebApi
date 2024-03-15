namespace WebApi.Data.Entities;

public class Trip
{
    public int Id { get; set; }
    public string Departure { get; set; }
    public string Destination { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public int Seats { get; set; }
    public double Price { get; set; }
}