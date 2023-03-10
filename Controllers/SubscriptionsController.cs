using System.Globalization;
using System.Security.Claims;
using System.Xml;
using API.Data;
using API.Enteties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class SubscriptionsController : BaseApiConroller
{
    private readonly DataContext _context;

    public SubscriptionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetSubscriptions()
    {
        return await  _context.Subscriptions.Where(s => s.UserId == GetUserId())
            .Select(x => x.feedUrl).ToListAsync();
    }
    
    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<News>>> GetUnreadNews(string date)
    {
        string format = "ddd, dd MMM yyyy HH:mm";
        DateTime date1 = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
        return await _context.News.Where(n => 
           DateTime.ParseExact(n.pubDate, format, CultureInfo.InvariantCulture) >= date1 && !n.IsRead)
            .ToListAsync();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> SetNewsAsRead(int id)
    {
        var news = await _context.News.FindAsync(id);

        if (news == null)
        {
            return NotFound();
        }

        news.IsRead = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpPost]
    public async Task<ActionResult<Subscription>> AddRSSFeed(string feedUrl)
    {
        var news = new List<News>();
       

        var subscription = new Subscription
        {
            feedUrl = feedUrl,
            News = news,
            UserId = GetUserId()
        };
        
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        
        var lastId = _context.Subscriptions.Max(x => x.Id);
       

        _context.Subscriptions.FirstOrDefault(x => x.Id == lastId).News = createNewsList(feedUrl, lastId);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    

    private List<News> createNewsList(string feedUrl, int lastId)
    {
        var news = new List<News>();
        var doc = new XmlDocument();
        doc.Load(feedUrl);
        var nodes = doc.GetElementsByTagName("item");

        foreach (XmlNode node in nodes)
        {
            
            var message = new News
            {
                Title = node["title"].InnerText,
                Description = node["description"].InnerText,
                Url = node["link"].InnerText,
                IsRead = false,
                pubDate = node["pubDate"].InnerText,
                SubscriptionId = lastId
            };
            news.Add(message);
            _context.News.Add(message);
        }

        return news;
    }

    private int GetUserId()
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user =  _context.Users.FirstOrDefault(x => x.UserName == username);
        return user.Id;
    }
    
}