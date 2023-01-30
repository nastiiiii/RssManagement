using System.Xml;

namespace API.Enteties;

public class Subscription
{
    public int Id { get; set; }
    
    public string feedUrl { get; set; }

    public List<News> News { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    
}