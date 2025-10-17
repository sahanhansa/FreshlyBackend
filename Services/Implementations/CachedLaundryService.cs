using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FreshlyBackendNew.Services.Implementations
{
    public class CachedLaundryService : ILaundryService
    {
        private readonly LaundryService _laundryService; // Changed from ILaundryService
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedLaundryService> _logger;
        private const int CacheExpirationMinutes = 15;

        public CachedLaundryService(
            LaundryService laundryService, // Changed parameter type
            IMemoryCache cache,
            ILogger<CachedLaundryService> logger)
        {
            _laundryService = laundryService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<LaundryWithAddressDTO>> GetLaundriesForCustomerAsync()
        {
            string cacheKey = "LaundryListForCustomer";

            if (_cache.TryGetValue(cacheKey, out List<LaundryWithAddressDTO> cachedResult))
            {
                _logger.LogInformation("Retrieved laundries from cache");
                return cachedResult;
            }

            var result = await _laundryService.GetLaundriesForCustomerAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes * 2))
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _logger.LogInformation($"Cache entry {key} evicted: {reason}");
                });

            _cache.Set(cacheKey, result, cacheOptions);

            return result;
        }

        // Delegate other methods to _laundryService
        public Task<List<LaundryAdminDTO>> GetLaundriesForAdminAsync() => 
            _laundryService.GetLaundriesForAdminAsync();

        public Task<LaundryAdminDTO> CreateLaundryAsync(LaundryAdminDTO laundryDto) => 
            _laundryService.CreateLaundryAsync(laundryDto);

        public Task<LaundryAdminDTO> GetLaundryByIdAsync(Guid id) => 
            _laundryService.GetLaundryByIdAsync(id);

        public Task<LaundryDetailsDTO> GetLaundryDetailsAsync(Guid laundryId) => 
            _laundryService.GetLaundryDetailsAsync(laundryId);
    }
}
