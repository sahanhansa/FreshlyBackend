
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Services.Interfaces;

namespace FreshlyBackendNew.Services.Implementations
{
    public class LaundryContactService : ILaundryContactService

    {
    private readonly ApplicationDbContext _context;

    public LaundryContactService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddMessage(LaundryContactDetailsDTO laundryContactDetailsDto)
    {
        var message = new Feedback
        {
            Description = laundryContactDetailsDto.Message,
            SubmittedByType = "Laundry",
            InquiryType = laundryContactDetailsDto.SelectedSubject,
            UserId = laundryContactDetailsDto.LaundryID


        };

        _context.Feedbacks.Add(message);
        await _context.SaveChangesAsync();
    }

    }
}