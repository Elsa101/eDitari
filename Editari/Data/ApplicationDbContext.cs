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

            // ---------------- SEED TEST DATA ----------------
            
            // 1. School Classes
            modelBuilder.Entity<SchoolClass>().HasData(
                new SchoolClass { ClassId = 10, ClassName = "10" },
                new SchoolClass { ClassId = 11, ClassName = "11" }
            );

            // 2. Teachers
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TeacherId = 10, Name = "Arben", Surname = "Gashi", Email = "arben.gashi@shkolla.com", Username = "arben.gashi", PasswordHash = adminHash, Phone = "044111222" },
                new Teacher { TeacherId = 11, Name = "Blerina", Surname = "Krasniqi", Email = "blerina.k@shkolla.com", Username = "blerina.k", PasswordHash = adminHash, Phone = "044333444" }
            );

            // 3. Parents
            modelBuilder.Entity<Parent>().HasData(
                new Parent { ParentId = 10, Name = "Besnik", Surname = "Dervishi", Email = "besnik.d@prindi.al", PasswordHash = adminHash, Phone = "049555666", IsActive = true, CreatedAt = DateTime.Parse("2024-01-01") },
                new Parent { ParentId = 11, Name = "Liridona", Surname = "Berisha", Email = "liridona.b@prindi.al", PasswordHash = adminHash, Phone = "049777888", IsActive = true, CreatedAt = DateTime.Parse("2024-01-01") }
            );

            // 4. Students
            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 10, Name = "Driton", Surname = "Dervishi", Email = "driton@nxenes.al", TeacherId = 10, ClassId = 10, DateOfBirth = DateTime.Parse("2008-05-15"), LinkCode = "DRITON123" },
                new Student { StudentId = 11, Name = "Elira", Surname = "Berisha", Email = "elira@nxenes.al", TeacherId = 11, ClassId = 11, DateOfBirth = DateTime.Parse("2009-09-20"), LinkCode = "ELIRA456" }
            );

            // 5. StudentParents (Junction)
            modelBuilder.Entity<StudentParent>().HasData(
                new StudentParent { StudentId = 10, ParentId = 10 },
                new StudentParent { StudentId = 11, ParentId = 11 }
            );
        }
    }
}
