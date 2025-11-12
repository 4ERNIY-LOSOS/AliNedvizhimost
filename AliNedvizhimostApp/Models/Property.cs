namespace AliNedvizhimostApp.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public double Area { get; set; }
        public int Rooms { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }
}
