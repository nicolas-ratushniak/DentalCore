using System.Threading.Tasks;

namespace DentalCore.Wpf.Abstract;

public interface IUpdateService
{
    public Task<bool> CheckForNewReleasesAsync();
    public Task UpdateAndRestartAsync();
}