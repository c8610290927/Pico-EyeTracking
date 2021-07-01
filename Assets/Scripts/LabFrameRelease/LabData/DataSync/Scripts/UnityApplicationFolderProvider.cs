using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataSync;
using UnityEngine;

public class UnityApplicationFolderProvider : IApplicationFolderProvider
{

    private readonly string _storeFolderPath;

    public UnityApplicationFolderProvider(string storeFolderPath)
    {
        _storeFolderPath = storeFolderPath;
    }

    public IPlatformFolder GetApplicationFolder()
    {
        return CreateAndValidateApplicationFolder(_storeFolderPath);
    }

    private IPlatformFolder CreateAndValidateApplicationFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        IPlatformFolder result = null;
        var telemetryDirectory = new DirectoryInfo(path);
        TelemetryChannelEventSource.Log.StorageFolder(telemetryDirectory.FullName);
        result = new PlatformFolder(telemetryDirectory);
        return result;

    }
}
