using Microsoft.EntityFrameworkCore;
namespace Part04_01_EfCore.Data;
public sealed class CampusDb(DbContextOptions<CampusDb> options) : DbContext(options)
{
  public DbSet<Student> Students => Set<Student>();
  public DbSet<Product> Products => Set<Product>();
  public DbSet<Category> Categories => Set<Category>();
  protected override void OnModelCreating(ModelBuilder b)
  {
    b.Entity<Student>(e=>{ e.HasIndex(x=>x.StudentNumber).IsUnique(); e.Property(x=>x.FullName).HasMaxLength(200); });
    b.Entity<Product>(e=>{ e.HasIndex(x=>x.Sku).IsUnique(); e.Property(x=>x.Price).HasPrecision(18,2);
      e.Property(x=>x.RowVersion).IsConcurrencyToken();
      e.HasQueryFilter(x=>!x.IsDeleted); // soft delete demo / tenant-like filter pattern
    });
    b.Entity<Category>().HasMany(c=>c.Products).WithOne(p=>p.Category!).HasForeignKey(p=>p.CategoryId);
  }
}
public class Student { public int Id{get;set;} public string StudentNumber{get;set;}=""; public string FullName{get;set;}=""; public string Major{get;set;}=""; public int EnrollmentYear{get;set;} }
public class Category { public int Id{get;set;} public string Name{get;set;}=""; public List<Product> Products{get;set;}=new(); }
public class Product { public int Id{get;set;} public string Sku{get;set;}=""; public string Name{get;set;}=""; public decimal Price{get;set;} public int Stock{get;set;} public int CategoryId{get;set;} public Category? Category{get;set;} public uint RowVersion{get;set;} public bool IsDeleted{get;set;} }
