using Microsoft.EntityFrameworkCore;
using LibraryCatalog.Models;

namespace LibraryCatalog.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    // EF Core will automatically configure TPH (Table-Per-Hierarchy) 
    // and create a 'Discriminator' column for CatalogItems
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Magazine> Magazines => Set<Magazine>();
}