using Microsoft.EntityFrameworkCore;
using Editari.Data;
using System;
using System.Linq;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=eDitariDb1;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");
        
        using (var db = new AppDbContext(optionsBuilder.Options))
        {
            try
            {
                var parentId = db.Parents.Select(p => p.ParentId).FirstOrDefault();
                if (parentId > 0)
                {
                    Console.WriteLine("Trying to delete Parent " + parentId);
                    await db.RefreshTokens.Where(rt => rt.ParentId == parentId).ExecuteDeleteAsync();
                    await db.StudentParents.Where(sp => sp.ParentId == parentId).ExecuteDeleteAsync();
                    
                    var parent = new Editari.Models.Parent { ParentId = parentId };
                    db.Parents.Remove(parent);
                    await db.SaveChangesAsync();
                    Console.WriteLine("SUCCESS!");
                }
                else Console.WriteLine("No Parent to delete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
            
            try
            {
                var studentId = db.Students.Select(s => s.StudentId).FirstOrDefault();
                if (studentId > 0)
                {
                    Console.WriteLine("Trying to delete Student " + studentId);
                    await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Grades WHERE StudentId = {studentId}");
                    await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM [Attendances] WHERE StudentId = {studentId}");
                    await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Comments WHERE StudentId = {studentId}");
                    await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM StudentParent WHERE StudentId = {studentId}");
                    await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Students WHERE StudentId = {studentId}");
                    Console.WriteLine("SUCCESS!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
        }
    }
}
