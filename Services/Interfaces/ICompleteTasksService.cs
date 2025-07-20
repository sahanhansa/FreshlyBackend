using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.DTOs.Driver_DTOs;

namespace FreshlyBackendNew.Services.Interfaces
{
   

        public interface ICompleteTasksService
        {
            Task<List<CompleteTasksDetailsDto>> GetAllCompleteTasks();

            Task<CompleteTasksDetailsByIdDto> GetAllCompleteTasksBYId(string orderID);
        }

    }

