/// <summary>
/// Class that knows how to install and run the TestCentric GUI,
/// using either the NuGet or the Chocolatey package.
/// </summary>
public class GuiRunner
{
	public const string NuGetId = "TestCentric.GuiRunner";
	public const string ChocoId = "testcentric-gui";
	
	private const string RUNNER_EXE = "testcentric.exe";

	private BuildParameters _parameters;

	public GuiRunner(BuildParameters parameters, string packageId, string version = null)
	{
		if (packageId != null && packageId != NuGetId && packageId != ChocoId)
			throw new System.Exception($"Package Id invalid: {packageId}");

		_parameters = parameters;

		PackageId = packageId;
		Version = version;
	}

	public string PackageId { get; }
	public string Version { get; }
	public string InstallPath => _parameters.PackageTestDirectory;
	public string ExecutablePath =>
		$"{InstallPath}{PackageId}.{Version}/tools/{RUNNER_EXE}";
	public bool IsInstalled => System.IO.File.Exists(ExecutablePath);

	public int RunUnattended(string arguments)
	{
		if (!arguments.Contains(" --run"))
			arguments += " --run";
		if (!arguments.Contains(" --unattended"))
			arguments += " --unattended";

		return Run(arguments);
	}

	public int Run(string arguments)
	{
		Console.WriteLine(ExecutablePath);
		Console.WriteLine(arguments);
		Console.WriteLine();
		return _parameters.Context.StartProcess(ExecutablePath, new ProcessSettings()
		{
			Arguments = arguments,
			WorkingDirectory = _parameters.OutputDirectory
		});
	}

	public void InstallRunner()
    {
		if (!System.IO.Directory.Exists(InstallPath))
			throw new System.Exception($"Directory does not exist: {InstallPath}");

		_parameters.Context.NuGetInstall(PackageId,
			new NuGetInstallSettings()
			{
				Version = Version,
				Source = PACKAGE_SOURCES,
				Prerelease = true,
				OutputDirectory = InstallPath
			});
    }
}
