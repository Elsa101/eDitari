using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eDitari.Dtos;
using Editari.Data;
using Microsoft.EntityFrameworkCore;
using eDitari.Services.Interfaces;
using Editari.Models;


public class SubjectService : ISubjectService
{
    private readonly AppDbContext _context;

    public SubjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> GetAllSubjectsAsync()
    {
        return await _context.Subjects
            .Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                TeacherId = s.TeacherId
            })
            .ToListAsync();
    }

   public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
{
    var subject = await _context.Subjects.FindAsync(id);
    if (subject == null)
        return null;

    return new SubjectDto
    {
        SubjectId = subject.SubjectId,
        Name = subject.Name,
        TeacherId = subject.TeacherId
    };
}


    public async Task<SubjectDto> CreateSubjectAsync(SubjectDto subjectDto)
    {
        // Rregulli 1: Verifiko nëse ekziston mësimdhënësi
        var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == subjectDto.TeacherId);
        if (!teacherExists)
            throw new Exception("Mësimdhënësi me këtë ID nuk ekziston.");

        // Rregulli 2: Mos lejo emra të njëjtë të lëndëve
        var subjectExists = await _context.Subjects.AnyAsync(s => s.Name == subjectDto.Name);
        if (subjectExists)
            throw new Exception("Një lëndë me këtë emër tashmë ekziston.");

        // Rregulli 3: Kufizo numrin e lëndëve për mësimdhënës (maksimum 5)
        var subjectCount = await _context.Subjects.CountAsync(s => s.TeacherId == subjectDto.TeacherId);
        if (subjectCount >= 5)
            throw new Exception("Një mësimdhënës mund të ketë maksimumi 5 lëndë.");

        var subject = new Subject
        {
            Name = subjectDto.Name,
            TeacherId = subjectDto.TeacherId
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        subjectDto.SubjectId = subject.SubjectId;
        return subjectDto;
    }

    public async Task<SubjectDto> UpdateSubjectAsync(int id, SubjectDto subjectDto)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
            throw new Exception("Lënda nuk u gjet.");

        // Rregulli: Verifiko mësimdhënësin e ri
        var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == subjectDto.TeacherId);
        if (!teacherExists)
            throw new Exception("Mësimdhënësi me këtë ID nuk ekziston.");

        // Rregulli: Mos lejo emër të njëjtë për lëndë të tjera
        var duplicateName = await _context.Subjects
            .AnyAsync(s => s.Name == subjectDto.Name && s.SubjectId != id);
        if (duplicateName)
            throw new Exception("Një lëndë tjetër me këtë emër tashmë ekziston.");

        // Rregulli: Kontrollo limitin e lëndëve për mësimdhënësin (pa e llogaritur këtë lëndë)
        var subjectCount = await _context.Subjects
            .CountAsync(s => s.TeacherId == subjectDto.TeacherId && s.SubjectId != id);
        if (subjectCount >= 5)
            throw new Exception("Një mësimdhënës mund të ketë maksimumi 5 lëndë.");

        subject.Name = subjectDto.Name;
        subject.TeacherId = subjectDto.TeacherId;

        await _context.SaveChangesAsync();

        return subjectDto;
    }

    public async Task<bool> DeleteSubjectAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
            throw new Exception("Lënda nuk u gjet.");

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        return true;
    }

    Task<IEnumerable<SubjectDto>> ISubjectService.GetAllSubjectsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto subjectDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSubjectAsync(int id, CreateSubjectDto subjectDto)
    {
        throw new NotImplementedException();
    }
}
