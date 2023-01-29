namespace API.Enteties;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }

    public string pubDate { get; set; }
    public bool IsRead { get; set; }
    
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; }
    
}