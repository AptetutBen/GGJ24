using UnityEditor;
using System.Linq;
using UnityEditor.Build.Reporting;
using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class BuildScript
{
    static BuildOptions developmentBuild = BuildOptions.Development;

    static string[] getScenes()
    {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }

    [MenuItem("Build/Linux64 [Development]")]
    static void PerformBuildLinux64()
    {
        string buildName = PlayerSettings.productName.Replace(" ", "-");
        string buildFolder = "linux";
        string extension = "x86_64";
        string buildArtifact = $"./builds/{buildFolder}/{buildName}.{extension}";
        BuildReport buildReport = null;
        startLoggingErrors();
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.IL2CPP);

        try{
            buildReport = BuildPipeline.BuildPlayer(getScenes(), buildArtifact,
                BuildTarget.StandaloneLinux64, developmentBuild);
        }catch(Exception error){
            Debug.LogError(error);
        }

        stopLoggingErrors();
        
        if(buildReport!= null && buildReport.summary.result == BuildResult.Succeeded){
            System.IO.File.WriteAllText(@"./builds/lastbuild.txt", buildArtifact);
        }else{
            EditorApplication.Exit(1);
        }
    }


    [MenuItem("Build/OSX [Development]")]
    static void PerformBuildOSX()
    {
        string buildName = PlayerSettings.productName.Replace(" ", "-");
        string buildFolder = "osx";
        string extension = "app";
        string buildArtifact = $"./builds/{buildFolder}/{buildName}.{extension}";
        BuildReport buildReport = null;
        startLoggingErrors();

        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.IL2CPP);

        try{
            buildReport = BuildPipeline.BuildPlayer(getScenes(), buildArtifact,
                BuildTarget.StandaloneOSX, developmentBuild);
        }catch(Exception error){
            Debug.LogError(error);
        }

        stopLoggingErrors();
        
        if(buildReport!= null && buildReport.summary.result == BuildResult.Succeeded){
            System.IO.File.WriteAllText(@"./builds/lastbuild.txt", $"./builds/{buildFolder}");
        }else{
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/Windows [Development]")]
    static void PerformBuildWindows()
    {
        string buildName = PlayerSettings.productName.Replace(" ", "-");
        string buildFolder = "windows";
        string extension = "exe";
        string buildArtifact = $"./builds/{buildFolder}/{buildName}.{extension}";
        BuildReport buildReport = null;
        startLoggingErrors();

        // The build server is osx so it can only build mono
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);

        try{
            buildReport = BuildPipeline.BuildPlayer(getScenes(), buildArtifact,
            BuildTarget.StandaloneWindows64, developmentBuild);
        }catch(Exception error){
            Debug.LogError(error);
        }

        stopLoggingErrors();
        
        if(buildReport!= null && buildReport.summary.result == BuildResult.Succeeded){
            System.IO.File.WriteAllText(@"./builds/lastbuild.txt", $"./builds/{buildFolder}");
        }else{
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/Android [Development]")]
    static void PerformBuildAndroid()
    {
        string buildName = PlayerSettings.productName.Replace(" ", "-");
        string buildFolder = "android";
        string extension = "apk";
        string buildArtifact = $"./builds/{buildFolder}/{buildName}.{extension}";
        BuildReport buildReport = null;
        startLoggingErrors();

        try{
            buildReport = BuildPipeline.BuildPlayer(getScenes(), buildArtifact,
                BuildTarget.Android, developmentBuild);
        }catch(Exception error){
            Debug.LogError(error);
        }

        stopLoggingErrors();
        
        if(buildReport!= null && buildReport.summary.result == BuildResult.Succeeded){
            System.IO.File.WriteAllText(@"./builds/lastbuild.txt", buildArtifact);
        }else{
            EditorApplication.Exit(1);
        } 
    }

    [MenuItem("Build/iOS [Development]")]
    static void PerformBuildIOS()
    {
        string buildName = "XcodeProject";
        string buildFolder = "ios";
        string buildArtifact = $"./builds/{buildFolder}/{buildName}";
        BuildReport buildReport = null;
        startLoggingErrors();

        try{
            buildReport = BuildPipeline.BuildPlayer(getScenes(), buildArtifact,
                BuildTarget.iOS, developmentBuild);
        }catch(Exception error){
            Debug.LogError(error);
        }

        stopLoggingErrors();
        
        if(buildReport!= null && buildReport.summary.result == BuildResult.Succeeded){
            System.IO.File.WriteAllText(@"./builds/lastbuild.txt", buildArtifact);
        }else{
            EditorApplication.Exit(1);
        } 
    }

    private static string errorOutput;
    private static void startLoggingErrors(){
        errorOutput = "";
        Application.logMessageReceived += logMessage;
    }

    private static void logMessage(string logString, string stackTrace, LogType type){
        if(type == LogType.Error){
            errorOutput += logString + "\n" + stackTrace + "\n";
        }
    }

    private static void stopLoggingErrors(){
        Application.logMessageReceived -= logMessage;
        if(errorOutput != ""){
            System.IO.File.WriteAllText(@"./errorOutput.txt", errorOutput);
        }
    }
}