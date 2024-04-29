using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if Unity_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;
 
namespace SH.Ads.Editor
{
#if Unity_IOS
    public class BuildProcessor
   {
      [PostProcessBuild(45)]
      private static void OnPostProcessBuildCocoaPodsAdjustments(BuildTarget buildTarget, string pathToBuiltProject)
      {
         if (buildTarget != BuildTarget.iOS) return;
 
         // https://stackoverflow.com/a/51416359
         var content = "\n\npost_install do |installer|\n" +
                       "installer.pods_project.targets.each do |target|\n" +
                       "  target.build_configurations.each do |config|\n" +
                       $"    config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '{PlayerSettings.iOS.targetOSVersionString}'\n" +
                       "    config.build_settings['ENABLE_BITCODE'] = 'NO'\n" +
                       "  end\n" +
                       " end\n" +
                       "end\n";
 
         using var streamWriter = File.AppendText(Path.Combine(pathToBuiltProject, "Podfile"));
         streamWriter.WriteLine(content);
       
         Debug.Log(">> Automation, CocoaPodsAdjustments ... <<");
      }
     
      [PostProcessBuild(44)]
      public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
      {
         if (buildTarget == BuildTarget.iOS)
         {
            string plistPath = path + "/Info.plist";
           
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
           
            PlistElementDict rootDict = plist.root;
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            File.WriteAllText(plistPath, plist.WriteToString());
           
            Debug.Log(">> Automation, Plist ... <<");
         }
      }
 
      [PostProcessBuild]
      private static void OnPostProcessBuildDisableBitCode(BuildTarget buildTarget, string pathToBuiltProject)
      {
         if (buildTarget != BuildTarget.iOS) return;
 
         string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
 
         var pbxProject = new PBXProject();
         pbxProject.ReadFromFile(projPath);
 
         // Main
         string target = pbxProject.GetUnityMainTargetGuid();
         pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
 
         // Unity Tests
         target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
         pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
       
         // Unity Framework
         target = pbxProject.GetUnityFrameworkTargetGuid();
         pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
       
         // Unity GameAssembly
         //target = pbxProject.TargetGuidByName("GameAssembly");
         //pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
       
         pbxProject.WriteToFile(projPath);
       
         Debug.Log(">> Automation, DisableBitCode ... <<");
      }
   }
#endif
}
 