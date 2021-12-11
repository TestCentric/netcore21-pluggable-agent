#tool nuget:?package=GitVersion.CommandLine&version=5.0.0
#tool nuget:?package=GitReleaseManager&version=0.11.0
#tool nuget:?package=TestCentric.GuiRunner&version=2.0.0-dev00089

#load nuget:?package=TestCentric.Cake.Recipe&version=1.0.0-dev00030

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// SETUP
//////////////////////////////////////////////////////////////////////

Setup<BuildSettings>((context) =>
{
	var settings = BuildSettings.Initialize
	(
		context: context,
		title: "NetCore21PluggableAgent",
		solutionFile: "netcore21-pluggable-agent.sln",
		unitTest: "netcore21-agent-launcher.tests.exe",
		guiVersion: "2.0.0-dev00089",
		githubOwner: "TestCentric",
		githubRepository: "netcore21-pluggable-agent",
		copyright: "Copyright (c) Charlie Poole and TestCentric Engine contributors.",
		packages: new PackageDefinition[] { NuGetAgentPackage, ChocolateyAgentPackage }
		//packageTests: new PackageTest[] { NetCore11PackageTest, NetCore21PackageTest }
	);

	Information($"NetCore21PluggableAgent {settings.Configuration} version {settings.PackageVersion}");

	if (BuildSystem.IsRunningOnAppVeyor)
		AppVeyor.UpdateBuildVersion(settings.PackageVersion);

	return settings;
});

var NuGetAgentPackage = new NuGetPackage(
		id: "NUnit.Extension.NetCore21PluggableAgent",
		source: "nuget/NetCore21PluggableAgent.nuspec",
		checks: new PackageCheck[] {
			HasFiles("LICENSE.txt", "CHANGES.txt"),
			HasDirectory("tools").WithFiles("netcore21-agent-launcher.dll", "nunit.engine.api.dll"),
			HasDirectory("tools/agent").WithFiles(
				"netcore21-pluggable-agent.dll", "netcore21-pluggable-agent.dll.config",
				"nunit.engine.api.dll", "testcentric.engine.core.dll")
		});

var ChocolateyAgentPackage = new ChocolateyPackage(
		id: "nunit-extension-netcore21-pluggable-agent",
		source: "choco/netcore21-pluggable-agent.nuspec",
		checks: new PackageCheck[] {
			HasDirectory("tools").WithFiles("netcore21-agent-launcher.dll", "nunit.engine.api.dll")
				.WithFiles("LICENSE.txt", "CHANGES.txt", "VERIFICATION.txt"),
			HasDirectory("tools/agent").WithFiles(
				"netcore21-pluggable-agent.dll", "netcore21-pluggable-agent.dll.config",
				"nunit.engine.api.dll", "testcentric.engine.core.dll")
		});

var NetCore11PackageTest = new PackageTest(
	1, "Run mock-assembly.dll targeting .NET Core 1.1", GUI_RUNNER,
	"tests/netcoreapp1.1/mock-assembly.dll", CommonResult);

var NetCore21PackageTest = new PackageTest(
	1, "Run mock-assembly.dll targeting .NET Core 2.1", GUI_RUNNER,
	"tests/netcoreapp2.1/mock-assembly.dll", CommonResult);

static readonly string GUI_RUNNER = "tools/TestCentric.GuiRunner.2.0.0-dev00089/tools/testcentric.exe";

ExpectedResult CommonResult => new ExpectedResult("Failed")
{
	Total = 36,
	Passed = 23,
	Failed = 5,
	Warnings = 1,
	Inconclusive = 1,
	Skipped = 7,
	Assemblies = new ExpectedAssemblyResult[]
	{
		new ExpectedAssemblyResult("mock-assembly.dll", "Net40AgentLauncher")
	}
};

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Appveyor")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package")
	.IsDependentOn("Publish");

Task("Full")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package");

//Task("Travis")
//	.IsDependentOn("Build")
//	.IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
