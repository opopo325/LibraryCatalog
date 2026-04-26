namespace LibraryCatalog.Models;

// Base abstract class for all catalog items
public abstract class CatalogItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Language { get; set; } = string.Empty;
    public int PagesCount { get; set; }

    // Virtual method to calculate price as per requirements
    public abstract decimal CalculateItemPrice();
}

// Derived class 1: Books
public class Book : CatalogItem
{
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // The price of a book is simply its set price
    public override decimal CalculateItemPrice() => Price;
}

// Derived class 2: Magazines
public class Magazine : CatalogItem
{
    public int IssuesPerYear { get; set; }
    public int IssueNumber { get; set; }
    public decimal AnnualSubscriptionPrice { get; set; }

    // Price of 1 issue: annual price / number of issues
    public override decimal CalculateItemPrice()
    {
        if (IssuesPerYear == 0) return 0;
        return AnnualSubscriptionPrice / IssuesPerYear;
    }
}