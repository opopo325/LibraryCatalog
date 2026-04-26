using Microsoft.EntityFrameworkCore;
using LibraryCatalog.Data;
using LibraryCatalog.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add database context (SQLite)
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite("Data Source=library.db"));

// 2. Add CORS policy for the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 3. Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// --- API ENDPOINTS ---

// 1. ADD: Add a new book
app.MapPost("/api/books", async (Book book, LibraryContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
});

// 1. ADD: Add a new magazine
app.MapPost("/api/magazines", async (Magazine magazine, LibraryContext db) =>
{
    db.Magazines.Add(magazine);
    await db.SaveChangesAsync();
    return Results.Created($"/api/magazines/{magazine.Id}", magazine);
});

// 2. DELETE: Delete any literature by Id
app.MapDelete("/api/items/{id}", async (int id, LibraryContext db) =>
{
    var item = await db.CatalogItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.CatalogItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 3. EDIT: Edit book
app.MapPut("/api/books/{id}", async (int id, Book updatedBook, LibraryContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    // Update fields
    book.Title = updatedBook.Title;
    book.Theme = updatedBook.Theme;
    book.Year = updatedBook.Year;
    book.Language = updatedBook.Language;
    book.PagesCount = updatedBook.PagesCount;
    book.Author = updatedBook.Author;
    book.Publisher = updatedBook.Publisher;
    book.Price = updatedBook.Price;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 3. EDIT: Edit magazine
app.MapPut("/api/magazines/{id}", async (int id, Magazine updatedMagazine, LibraryContext db) =>
{
    var magazine = await db.Magazines.FindAsync(id);
    if (magazine is null) return Results.NotFound();

    // Update fields
    magazine.Title = updatedMagazine.Title;
    magazine.Theme = updatedMagazine.Theme;
    magazine.Year = updatedMagazine.Year;
    magazine.Language = updatedMagazine.Language;
    magazine.PagesCount = updatedMagazine.PagesCount;
    magazine.IssuesPerYear = updatedMagazine.IssuesPerYear;
    magazine.IssueNumber = updatedMagazine.IssueNumber;
    magazine.AnnualSubscriptionPrice = updatedMagazine.AnnualSubscriptionPrice;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 4. GET: View book titles sorted alphabetically
app.MapGet("/api/books/sorted-titles", async (LibraryContext db) =>
{
    var titles = await db.Books
        .OrderBy(b => b.Title)
        .Select(b => b.Title)
        .ToListAsync();
    return Results.Ok(titles);
});

// 5. GET: Filter magazines by computer theme
app.MapGet("/api/magazines/computers", async (LibraryContext db) =>
{
    // We check both English and Ukrainian common terms just in case
    var computersTheme = await db.Magazines
        .Where(m => m.Theme.ToLower().Contains("комп'ютер") || m.Theme.ToLower().Contains("computer"))
        .ToListAsync();
    return Results.Ok(computersTheme);
});

// 6. GET: Select ONLY magazines
app.MapGet("/api/magazines", async (LibraryContext db) =>
{
    var magazines = await db.Magazines.ToListAsync();
    return Results.Ok(magazines);
});

// 7. GET: Search magazine by issue number
app.MapGet("/api/magazines/issue/{number}", async (int number, LibraryContext db) =>
{
    var magazines = await db.Magazines
        .Where(m => m.IssueNumber == number)
        .ToListAsync();
    return Results.Ok(magazines);
});

// 8. GET: Search books by author
app.MapGet("/api/books/author/{authorName}", async (string authorName, LibraryContext db) =>
{
    // Using EF.Functions.Like for case-insensitive search in SQLite (if needed) or simple Contains
    var books = await db.Books
        .Where(b => b.Author.ToLower().Contains(authorName.ToLower()))
        .ToListAsync();
    return Results.Ok(books);
});

// 9. GET: Calculate the price of a single magazine issue
app.MapGet("/api/magazines/{id}/issue-price", async (int id, LibraryContext db) =>
{
    var magazine = await db.Magazines.FindAsync(id);
    if (magazine is null) return Results.NotFound();

    // Calling the virtual method from the assignment requirement
    var pricePerIssue = magazine.CalculateItemPrice();
    
    return Results.Ok(new { 
        MagazineId = magazine.Id, 
        Title = magazine.Title, 
        PricePerIssue = pricePerIssue 
    });
});

// GET: View ALL literature (Books + Magazines together)
app.MapGet("/api/items", async (LibraryContext db) =>
{
    var items = await db.CatalogItems.ToListAsync();
    return Results.Ok(items);
});

// GET: View ALL books (full data, not just titles)
app.MapGet("/api/books", async (LibraryContext db) =>
{
    var books = await db.Books.ToListAsync();
    return Results.Ok(books);
});

app.Run();