using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Services.Interfaces;

public interface ILaundryContactService
{
    Task AddMessage(LaundryContactDetailsDTO laundryContactDetailsDto); 
}