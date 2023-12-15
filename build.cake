#tool NuGet.CommandLine&version=6.0.0

// Load the recipe
#load nuget:?package=TestCentric.Cake.Recipe&version=1.1.0-dev00066
// Comment out above line and uncomment below for local tests of recipe changes
//#load ../TestCentric.Cake.Recipe/recipe/*.cake

var target = Argument("target", Argument("t", "Default"));

BuildSettings.Initialize
(
	context: Context,
	title: "NetCore21PluggableAgent",
	solutionFile: "netcore21-pluggable-agent.sln",
	unitTests: "netcore21-agent-launcher.tests.exe",
	githubOwner: "TestCentric",
	githubRepository: "netcore21-pluggable-agent"
);

var MockAssemblyResult = new ExpectedResult("Failed")
{
	Total = 36, Passed = 23, Failed = 5, Warnings = 1, Inconclusive = 1, Skipped = 7,
	Assemblies = new ExpectedAssemblyResult[] { new ExpectedAssemblyResult("mock-assembly.dll") }
};

var	PackageTests = new List<PackageTest>();
PackageTests.Add(new PackageTest(
	1, "NetCore11PackageTest", "Run mock-assembly.dll targeting .NET Core 1.1",
	"tests/netcoreapp1.1/mock-assembly.dll", MockAssemblyResult));

PackageTests.Add(new PackageTest(
	1, "NetCore21PackageTest", "Run mock-assembly.dll targeting .NET Core 2.1",
	"tests/netcoreapp2.1/mock-assembly.dll", MockAssemblyResult));

BuildSettings.Packages.Add(new NuGetPackage(
	"TestCentric.Extension.NetCore21PluggableAgent",
	title: ".NET Core 2.1 Pluggable Agent",
	description: "TestCentric engine extension for running tests under .NET Core 2.1",
	tags: new [] { "testcentric", "pluggable", "agent", "netcpreapp2.1" },
	packageContent: new PackageContent()
		.WithRootFiles("../../LICENSE.txt", "../../README.md", "../../testcentric.png")
		.WithDirectories(
			new DirectoryContent("tools").WithFiles(
				"netcore21-agent-launcher.dll", "netcore21-agent-launcher.pdb",
				"testcentric.extensibility.api.dll", "testcentric.engine.api.dll" ),
			new DirectoryContent("tools/agent").WithFiles(
				"agent/netcore21-agent.dll", "agent/netcore21-agent.pdb", "agent/netcore21-agent.dll.config",
				"agent/netcore21-agent.deps.json", $"agent/netcore21-agent.runtimeconfig.json",
				"agent/TestCentric.Agent.Core.dll", "agent/testcentric.engine.core.dll",
				"agent/testcentric.engine.api.dll", "agent/testcentric.extensibility.api.dll",
				"agent/testcentric.extensibility.dll", "agent/testcentric.engine.metadata.dll",
				"agent/TestCentric.InternalTrace.dll") ),
	testRunner: new AgentRunner(BuildSettings.NuGetTestDirectory + "TestCentric.Extension.NetCore21PluggableAgent." + BuildSettings.PackageVersion + "/tools/agent/netcore21-agent.dll"),
	tests: PackageTests) );
	
BuildSettings.Packages.Add(new ChocolateyPackage(
	"testcentric-extension-netcore21-pluggable-agent",
	title: ".NET Core 2.1 Pluggable Agent",
	description: "TestCentric engine extension for running tests under .NET Core 2.1",
	tags: new [] { "testcentric", "pluggable", "agent", "netcoreapp2.1" },
	packageContent: new PackageContent()
		.WithRootFiles("../../testcentric.png")
		.WithDirectories(
			new DirectoryContent("tools").WithFiles(
				"../../LICENSE.txt", "../../README.md", "../../VERIFICATION.txt",
				"netcore21-agent-launcher.dll", "netcore21-agent-launcher.pdb",
				"testcentric.extensibility.api.dll", "testcentric.engine.api.dll" ),
			new DirectoryContent("tools/agent").WithFiles(
				"agent/netcore21-agent.dll", "agent/netcore21-agent.pdb", "agent/netcore21-agent.dll.config",
				"agent/netcore21-agent.deps.json", $"agent/netcore21-agent.runtimeconfig.json",
				"agent/TestCentric.Agent.Core.dll", "agent/testcentric.engine.core.dll",
				"agent/testcentric.engine.api.dll", "agent/testcentric.extensibility.api.dll",
				"agent/testcentric.extensibility.dll", "agent/testcentric.engine.metadata.dll",
				"agent/TestCentric.InternalTrace.dll") ),
	testRunner: new AgentRunner(BuildSettings.ChocolateyTestDirectory + "testcentric-extension-netcore21-pluggable-agent." + BuildSettings.PackageVersion + "/tools/agent/netcore21-agent.dll"),
	tests: PackageTests) );

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Appveyor")
	.IsDependentOn("DumpSettings")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package")
	.IsDependentOn("Publish")
	.IsDependentOn("CreateDraftRelease")
	.IsDependentOn("CreateProductionRelease");

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
