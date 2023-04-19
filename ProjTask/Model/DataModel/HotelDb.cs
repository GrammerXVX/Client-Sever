using Microsoft.EntityFrameworkCore;
using MinAPI.Model.Dto;
using MinAPI.Model.Entity;

namespace MinAPI.Model.DataModel;
//Scaffold-DbContext "Name=SqlServer" Microsoft.EntityFrameworkCore.SqlServer
public partial class HotelDb : DbContext
{
    public HotelDb()
    {
    }

    public HotelDb(DbContextOptions<HotelDb> options)
        : base(options)
    {
    }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelRoom> HotelRooms { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }
    public virtual DbSet<RoomDto> RoomDto{ get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoomDto>()
        .HasNoKey();
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hotels__3214EC2755811783");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HotelName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HotelRoo__3214EC27B0B6F963");

            entity.ToTable("HotelRoom");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HotelId).HasColumnName("HotelID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__HotelRoom__Hotel__671F4F74");

            entity.HasOne(d => d.Room).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__HotelRoom__RoomI__681373AD");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC27E580A864");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Rooms__Type__3E1D39E1");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomType__3214EC27355369D4");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
