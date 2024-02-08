using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces;

public interface IContactPreferenceService
{
    Task<OperationResult<ContactPreferenceDto>> CreateContactPreferenceAsync(ContactPreferenceDto contactPreference);
    Task<OperationResult<ContactPreferenceDto>> GetContactPreferenceByIdAsync(int contactPreferenceId);
    Task<OperationResult<IEnumerable<ContactPreferenceDto>>> GetAllContactPreferencesAsync();
    Task<OperationResult<ContactPreferenceDto>> UpdateContactPreferenceAsync(ContactPreferenceDto contactPreferenceDto);
    Task<OperationResult<bool>> DeleteContactPreferenceAsync(int contactPreferenceId);
}
