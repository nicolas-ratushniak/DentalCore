using System.Threading.Tasks;

namespace DentalCore.Wpf.Abstract;

public interface IMinimalUpdateManager
{
    public Task<bool> HasNewerReleaseAsync();
    public Task UpdateAsync();
    public string GetCurrentVersionInstalled(string defaultValue);
}