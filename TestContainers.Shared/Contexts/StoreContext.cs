namespace TestContainers.Shared.Contexts;

using Microsoft.EntityFrameworkCore;
using TestContainers.Shared.Models;

public partial class StoreContext : DbContext
{
    public StoreContext()
    {
    }

    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; } = null!;
    public virtual DbSet<Item> Items { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Account>(entity =>
        {
            _ = entity.HasKey(e => e.UserId)
                .HasName("accounts_pkey");

            _ = entity.ToTable("accounts");

            _ = entity.HasIndex(e => e.Email, "accounts_email_key")
                .IsUnique();

            _ = entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .UseIdentityAlwaysColumn();

            _ = entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");

            _ = entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");

            _ = entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");

            _ = entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");

            _ = entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");

            _ = entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        _ = modelBuilder.Entity<Item>(entity =>
        {
            _ = entity.ToTable("items");

            _ = entity.Property(e => e.ItemId)
                .HasColumnName("item_id")
                .UseIdentityAlwaysColumn();

            _ = entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");

            _ = entity.Property(e => e.Price).HasColumnName("price");

            _ = entity.Property(e => e.Stock).HasColumnName("stock");
        });

        _ = modelBuilder.Entity<Order>(entity =>
        {
            _ = entity.ToTable("orders");

            _ = entity.Property(e => e.OrderId)
                .HasColumnName("order_id")
                .UseIdentityAlwaysColumn();

            _ = entity.Property(e => e.ItemId).HasColumnName("item_id");

            _ = entity.Property(e => e.NumberOfItems).HasColumnName("number_of_items");

            _ = entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");

            _ = entity.Property(e => e.UserId).HasColumnName("user_id");

            _ = entity.HasOne(d => d.Item)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("orders_item_id_fkey");

            _ = entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
