The main functionality as
1. Add RSS feed (parameters: feed url)
2. Get all active RSS feeds
3. Get all unread news from some date (parameters: date)
4. Set news as read
5. Registration and Login using JWT Token and Bearer authentication
6. Recreating the SQL database upon launch of the program

In addition, Postman collection of reqests was made.

During development, it was concluded that building stronger connections between objects would be beneficial. One user has a structured list with read news and other information, rather than creating unnecessary connections from subscriptions to news. Additionally, there was no strong need to create a separate "News" table. It would be more efficient to have a list of all news from the RSS feed inside the "Subscription" object.
