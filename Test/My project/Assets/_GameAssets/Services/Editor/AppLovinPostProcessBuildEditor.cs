#if UNITY_IOS
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

public static class AppLovinPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            Debug.Log("[AppLovinPPB_Debug]: Starting post build tasks for iOS...");

            // Đường dẫn tới file project.pbxproj
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
            string targetGUID = project.GetUnityMainTargetGuid();
#else
            string targetGUID = project.TargetGuidByName("Unity-iPhone");
#endif

            // Đường dẫn tới thư mục Pods
            string podsFrameworksPath = "Pods/";

            // Kiểm tra sự tồn tại của AppLovinSDK và InMobiSDK từ Dependencies.xml
            string appLovinVersion = GetFrameworkVersion("MaxSdk/AppLovin/Editor/Dependencies.xml", "AppLovinSDK", "com.applovin.mediation.ads@");
            string inMobiVersion = GetFrameworkVersion("MaxSdk/Mediation/InMobi/Editor/Dependencies.xml", "AppLovinMediationInMobiAdapter", "com.applovin.mediation.adapters.inmobi");

            // Nếu tồn tại AppLovinSDK version, tạo đường dẫn động tới AppLovinSDK.xcframework trong Pods
            if (!string.IsNullOrEmpty(appLovinVersion))
            {
                string appLovinSDKPath = $"AppLovinSDK/applovin-ios-sdk-{appLovinVersion}/AppLovinSDK.xcframework";
                string appLovinFullPath = Path.Combine(podsFrameworksPath, appLovinSDKPath);

                Debug.Log($"[AppLovinPPB_Debug]: Adding AppLovinSDK.xcframework from {appLovinFullPath}");
                project.AddFileToBuild(targetGUID, project.AddFile(appLovinFullPath, "AppLovinSDK.xcframework", PBXSourceTree.Source));
                // Thêm Embed and Sign
                project.AddFileToEmbedFrameworks(targetGUID, project.AddFile(appLovinFullPath, "AppLovinSDK.xcframework", PBXSourceTree.Source));
            }
            else
            {
                Debug.Log("[AppLovinPPB_Debug] Can not find applovin");
            }

            if (!string.IsNullOrEmpty(inMobiVersion))
            {
                string inMobiSDKPath = "InMobiSDK/InMobiSDK.xcframework";
                string inMobiFullPath = Path.Combine(podsFrameworksPath, inMobiSDKPath);

                Debug.Log($"[AppLovinPPB_Debug]: Adding InMobiSDK.xcframework from {inMobiFullPath}");
                project.AddFileToBuild(targetGUID, project.AddFile(inMobiFullPath, "InMobiSDK.xcframework", PBXSourceTree.Source));
                // Thêm Embed and Sign
                project.AddFileToEmbedFrameworks(targetGUID, project.AddFile(inMobiFullPath, "InMobiSDK.xcframework", PBXSourceTree.Source));
            }
            else
            {
                Debug.Log("[AppLovinPPB_Debug] Can not find inmobi");
            }

            // Ghi lại thay đổi vào project.pbxproj
            project.WriteToFile(projectPath);

            Debug.Log("[AppLovinPPB_Debug]: Post build tasks completed successfully.");
        }
    }

    private static string GetFrameworkVersion(string dependenciesPath, string frameworkName, string packagePrefix)
    {
        // Đường dẫn root của project
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;

        // Đường dẫn tới thư mục PackageCache
        string packageCachePath = Path.Combine(projectRoot, "Library/PackageCache");

        // Tìm thư mục bắt đầu với prefix chỉ định (ví dụ: com.applovin.mediation.ads@ hoặc com.applovin.mediation.adapters.inmobi@)
        string packagePath = Directory
            .GetDirectories(packageCachePath, packagePrefix + "*")
            .OrderByDescending(d => d) // Sắp xếp giảm dần để lấy phiên bản mới nhất
            .FirstOrDefault();

        // Các đường dẫn tiềm năng cho Dependencies.xml
        string[] possiblePaths = {
            Path.Combine(Application.dataPath, dependenciesPath),
            packagePath != null ? Path.Combine(packagePath, dependenciesPath) : null
        };

        foreach (string fullPath in possiblePaths)
        {
            Debug.Log($"Check Path: {fullPath}");
            if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fullPath);

                XmlNode iosPodNode = xmlDoc.SelectSingleNode($"//iosPod[@name='{frameworkName}']");
                if (iosPodNode != null && iosPodNode.Attributes["version"] != null)
                {
                    return iosPodNode.Attributes["version"].Value;
                }
            }
        }

        return string.Empty; // Trả về chuỗi rỗng nếu không tìm thấy
    }

}
#endif