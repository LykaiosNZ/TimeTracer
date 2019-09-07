var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0

var name = "TimeTracer";

var solutionPath = "./src/TimeTracer.sln";
var testProject = GetFiles("./src/TimeTracer.Tests/TimeTracer.Tests.csproj").Single();

Setup(_ => {
    Information("");
    Information("----------------------------------------");
    Information("Starting the cake build script");
    Information("Building: " + name);
    Information("----------------------------------------");
    Information("");
});

Teardown(_ => {
    Information("Finished running tasks.");
});

Task("Clean")
    .Does(() => 
    {
        Information("Cleaning {0}", solutionPath);

        DotNetCoreClean(solutionPath.ToString());
    });

Task("RestorePackages")
    .Does(() => {
        Information("Restoring NuGet Packages for {0}", solutionPath);
        DotNetCoreRestore(solutionPath.ToString());
    });

Task("BuildSolution")
    .Does(() => {
        Information("Building {0}", solutionPath);

        var buildSettings = new DotNetCoreBuildSettings 
        {
            Configuration = configuration,
            Verbosity = DotNetCoreVerbosity.Minimal,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings { TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error}
        };

        DotNetCoreBuild(solutionPath.ToString(), buildSettings);
    });

Task("RunTests")
    .Does(() => {
       var testSettings = new DotNetCoreTestSettings 
       {
           Configuration = configuration,
           NoBuild = true
       };

       DotNetCoreTest(testProject.FullPath, testSettings);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("BuildSolution")
    .IsDependentOn("RunTests");

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);