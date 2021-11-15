public abstract class PackageTester
{
    const string TEST_RESULT = "TestResult.xml";

    static readonly ExpectedResult EXPECTED_RESULT = new ExpectedResult("Failed")
    {
        Total = 36,
        Passed = 23,
        Failed = 5,
        Warnings = 1,
        Inconclusive = 1,
        Skipped = 7,
        Assemblies = new AssemblyResult[]
        {
            new AssemblyResult() { Name = MOCK_ASSEMBLY }
        }
    };

    protected BuildParameters _parameters;
    protected ICakeContext _context;
    protected GuiRunner _guiRunner;

    public PackageTester(BuildParameters parameters)
    {
        _parameters = parameters;
        _context = parameters.Context;
    }

    protected abstract string PackageId { get; }
    protected abstract string RunnerId { get; }
    
    public void RunAllTests()
    {
        _guiRunner.InstallRunner();

        int errors = 0;
        foreach (var runtime in new[] { "netcoreapp1.1", "netcoreapp2.1" })
        {
            _context.Information($"Running {runtime} mock-assembly tests");

            var actual = RunTest(runtime);

            var report = new TestReport(EXPECTED_RESULT, actual);
            errors += report.Errors.Count;
            report.DisplayErrors();
        }

        if (errors > 0)
            throw new System.Exception("A package test failed!");
    }

    private ActualResult RunTest(string runtime)
    {
        // Delete result file ahead of time so we don't mistakenly
        // read a left-over file from another test run. Leave the
        // file after the run in case we need it to debug a failure.
        if (_context.FileExists(_parameters.OutputDirectory + TEST_RESULT))
            _context.DeleteFile(_parameters.OutputDirectory + TEST_RESULT);

        _guiRunner.RunUnattended($"{_parameters.OutputDirectory}tests/{runtime}/{MOCK_ASSEMBLY}");

        return new ActualResult(_parameters.OutputDirectory + TEST_RESULT);
    }
}

public class NuGetPackageTester : PackageTester
{
    public NuGetPackageTester(BuildParameters parameters) : base(parameters)
    {
        _guiRunner = new GuiRunner(parameters, GuiRunner.NuGetId, "2.0.0-dev00079");
    }

    protected override string PackageId => NUGET_ID;
    protected override string RunnerId => GuiRunner.NuGetId;
}

public class ChocolateyPackageTester : PackageTester
{
    public ChocolateyPackageTester(BuildParameters parameters) : base(parameters)
    {
        _guiRunner = new GuiRunner(parameters, GuiRunner.ChocoId, "2.0.0-dev00079");
    }

    protected override string PackageId => CHOCO_ID;
    protected override string RunnerId => GuiRunner.ChocoId;
}
