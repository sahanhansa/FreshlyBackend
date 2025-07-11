using FreshlyBackendNew.DTOs.Order_DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<AddressDTO?> GetCustomerAddressAsync(Guid customerId);


    }
}
