namespace eDitari.Dtos
{
    public class StudentParentDto
    {
        public int StudentId { get; set; }
        public int ParentId { get; set; }

        // Përdoret kur bëjmë .Include për të shfaqur emrin e studentit
        public string? StudentName { get; set; }

        // Përdoret kur bëjmë .Include për të shfaqur emrin e prindit
        public string? ParentName { get; set; }

        // Fusha shtesë që mund të përdoren nga UI ose për identifikim të brendshëm
        public object? Id { get; internal set; }
        public object? Emri { get; internal set; }
    }
}
