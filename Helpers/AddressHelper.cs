using FreshlyBackendNew.Models;
using System.Collections.Generic;
using System.Linq;

namespace FreshlyBackendNew.Helpers
{
    public static class AddressHelper
    {
        public static string FormatAddress(Address? address)
        {
            if (address == null) return string.Empty;

            var addressParts = new List<string>();
            
            if (!string.IsNullOrEmpty(address.HouseNo)) addressParts.Add(address.HouseNo);
            if (!string.IsNullOrEmpty(address.Street)) addressParts.Add(address.Street);
            if (!string.IsNullOrEmpty(address.City)) addressParts.Add(address.City);
            if (!string.IsNullOrEmpty(address.PostalCode)) addressParts.Add(address.PostalCode);

            return string.Join(", ", addressParts);
        }

        public static string FormatFullName(string? firstName, string? lastName)
        {
            var nameParts = new List<string>();
            
            if (!string.IsNullOrEmpty(firstName)) nameParts.Add(firstName);
            if (!string.IsNullOrEmpty(lastName)) nameParts.Add(lastName);

            return string.Join(" ", nameParts);
        }
    }
}
