using System.Security.Claims;
using System.Xml;
using API.Data;
using API.Enteties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class SubscriptionsController: BaseApiConroller
{
    private readonly DataContext _context;
    //private string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public SubscriptionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
    {
        
        return await _context.Subscriptions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Subscription>> GetSubscription(int id)
    {
        return await _context.Subscriptions.FindAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<Subscription>> AddRSSFeed(string feedUrl)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        XmlDocument doc = new XmlDocument();
        doc.Load(feedUrl);
        XmlNodeList nodes = doc.GetElementsByTagName("item");

        var news = new List<News>();
            
        foreach (XmlNode node in nodes)
        {
            var message = new News
            {
                Title = node["title"].InnerText,
                Description = node["description"].InnerText,
                Url = node["link"].InnerText,
                IsRead = false,
                SubscriptionId = _context.Subscriptions.Last().Id + 1
            };
            _context.News.Add(message);
        }
       

        var subscription = new Subscription
        {
            feedUrl = feedUrl,
            News = news,
            UserId = Convert.ToInt32(userId)
        };
        
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return subscription; 
    }
    
    // [HttpPut("{id}")]
    // public async Task<IActionResult> SetNewsAsRead(int id)
    // {
    //     
    // }

} 