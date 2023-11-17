using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DentalCore.Wpf.Abstract;
using Squirrel;

namespace DentalCore.Wpf.Services;

public class UpdateService : IUpdateService
{
    private readonly string _repoUrl;
    private readonly string _configFileName;

    public UpdateService(string githubRepo, string configFileName)
    {
        _repoUrl = githubRepo;
        _configFileName = configFileName;
    }
    
    public static void RestoreSettingsFromBackup(string configFileName)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(Directory.GetParent(exeDir)!.FullName, "settings_backup", configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destFile = Path.Combine(exeDir, configFileName);
        
        File.Copy(sourceFile, destFile, true);
        File.Delete(sourceFile);
    }

    public async Task<bool> CheckForNewReleasesAsync()
    {
        using var manager = await UpdateManager.GitHubUpdateManager(_repoUrl);
        
        var updateInfo = await manager.CheckForUpdate();
        return updateInfo.ReleasesToApply.Any();
    }

    public async Task UpdateAndRestartAsync()
    {
        using var manager = await UpdateManager.GitHubUpdateManager(_repoUrl);
        
        BackupSettings();
        await manager.UpdateApp();
        UpdateManager.RestartApp();
    }
    
    private void BackupSettings()
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(exeDir, _configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destDir = Path.Combine(Directory.GetParent(exeDir)!.FullName, "settings_backup");

        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        
        var destFile = Path.Combine(destDir, _configFileName);
        File.Copy(sourceFile, destFile, true);
    }
}