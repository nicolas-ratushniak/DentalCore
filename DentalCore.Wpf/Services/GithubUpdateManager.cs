using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DentalCore.Wpf.Abstract;
using Squirrel;

namespace DentalCore.Wpf.Services;

public class GithubUpdateManager : IMinimalUpdateManager, IDisposable
{
    private const string BackupFolderName = "settings_backup";

    private IUpdateManager? _manager;
    private string? _configFileName;
    
    public static void BackupSettings(string configFileName)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(exeDir, configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destDir = Path.Combine(Directory.GetParent(exeDir)!.FullName, BackupFolderName);

        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        
        var destFile = Path.Combine(destDir, configFileName);
        File.Copy(sourceFile, destFile, true);
    }

    public static void RestoreSettingsFromBackup(string configFileName)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(Directory.GetParent(exeDir)!.FullName, BackupFolderName, configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destFile = Path.Combine(exeDir, configFileName);
        
        File.Copy(sourceFile, destFile, true);
        File.Delete(sourceFile);
    }

    public async Task InitAsync(string githubRepoUrl, string? configFileName)
    {
        _manager = await UpdateManager.GitHubUpdateManager(githubRepoUrl);
        _configFileName = configFileName;
    }
    
    public async Task InitAsync(string githubRepoUrl)
    {
        await InitAsync(githubRepoUrl, null);
    }
    
    public async Task<bool> HasNewerReleaseAsync()
    {
        if (_manager is null)
        {
            throw new InvalidOperationException();
        }
        
        var updateInfo = await _manager.CheckForUpdate();
        return updateInfo.ReleasesToApply.Any();
    }

    public async Task UpdateAsync(bool restart = false)
    {
        if (_manager is null)
        {
            throw new InvalidOperationException();
        }

        if (_configFileName is not null)
        {
            BackupSettings(_configFileName);
        }
        
        await _manager.UpdateApp();

        if (restart)
        {
            UpdateManager.RestartApp();
        }
    }

    public void Dispose()
    {
        _manager?.Dispose();
    }
}