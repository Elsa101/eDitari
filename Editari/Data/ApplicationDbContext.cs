using Microsoft.EntityFrameworkCore;
using Editari.Models;


namespace Editari.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
         
        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Editari.Models.Parent> Parents { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // StudentParent: Many-to-many me key të përbërë
            modelBuilder.Entity<StudentParent>()
                .HasKey(sp => new { sp.StudentId, sp.ParentId });

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId);

            // ---------------- SEED ADMINISTRATORS ----------------
            // Valid 60-character BCrypt hash for 'Admin123!' (Cost 11)
            var adminHash = "$2a$11$fLHLUcoQ7nexmqL29WN1huACCmrjvptuJjk67IOgAWWVOA8XqlIN6"; 
            
            modelBuilder.Entity<Staff>().HasData(
                new Staff { StaffId = 1, Name = "Admin 1", Username = "admin1@editari.com", PasswordHash = adminHash, Role = "Admin" },
                new Staff { StaffId = 2, Name = "Admin 2", Username = "admin2@editari.com", PasswordHash = adminHash, Role = "Admin" },
                new Staff { StaffId = 3, Name = "Admin 3", Username = "admin3@editari.com", PasswordHash = adminHash, Role = "Admin" }
            );
        }
    }
}
