using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces;

public interface IAddressService
{
    Task<OperationResult<AddressDto>> CreateAddressAsync(AddressDto address);
    Task<OperationResult<AddressDto>> GetAddressByIdAsync(int addressId);
    Task<OperationResult<IEnumerable<AddressDto>>> GetAllAddressesAsync();
    Task<OperationResult<AddressDto>> UpdateAddressAsync(AddressDto addressDto);
    Task<OperationResult<bool>> DeleteAddressAsync(int addressId);
    Task<OperationResult<bool>> AddressHasCustomersAsync(int addressId);
}
