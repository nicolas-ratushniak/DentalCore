using System;
using System.Diagnostics;
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

    private readonly IUpdateManager _manager;
    private readonly string? _configFileName;

    private GithubUpdateManager(IUpdateManager manager, string? configFileName)
    {
        _manager = manager;
        _configFileName = configFileName;
    }

    public static async Task<GithubUpdateManager> CreateAsync(string githubRepoUrl, string? configFileName)
    {
        var manager = await UpdateManager.GitHubUpdateManager(githubRepoUrl);
        return new GithubUpdateManager(manager, configFileName);
    }

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

    public async Task<bool> HasNewerReleaseAsync()
    {
        if (_manager is null)
        {
            throw new InvalidOperationException();
        }

        var updateInfo = await _manager.CheckForUpdate();
        return updateInfo.ReleasesToApply.Any();
    }

    public async Task UpdateAsync()
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
    }

    public string GetCurrentVersionInstalled(string defaultValue = "?.?.?")
    {
        var assembly = Assembly.GetExecutingAssembly();
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        return versionInfo.FileVersion ?? defaultValue;
    }

    public void Dispose()
    {
        _manager.Dispose();
    }
}