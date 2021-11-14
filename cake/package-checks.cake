public static class Check
{
	public static void That(string testDir, params PackageCheck[] checks)
    {
		foreach (var check in checks)
			check.ApplyTo(testDir);
    }
}

public static FileCheck HasFile(string file) => HasFiles(new[] { file });
	
public static FileCheck HasFiles(params string[] files) => new FileCheck(files);

public static DirectoryCheck HasDirectory(string dir) => new DirectoryCheck(dir);

public abstract class PackageCheck
{
	public abstract void ApplyTo(string testDir);
}

public class FileCheck : PackageCheck
{
	string[] _files;

	public FileCheck(string[] files)
	{
		_files = files;
	}

	public override void ApplyTo(string testDir)
	{
		foreach (string file in _files)
		{
			if (!System.IO.File.Exists(System.IO.Path.Combine(testDir, file)))
				throw new Exception($"File {file} was not found.");
		}
	}
}

public class DirectoryCheck : PackageCheck
{
	private string _path;
	private List<string> _files = new List<string>();

	public DirectoryCheck(string path)
	{
		_path = path;
	}

	public DirectoryCheck WithFiles(params string[] files)
	{
		_files.AddRange(files);
		return this;
	}

	public DirectoryCheck WithFile(string file)
	{
		_files.Add(file);
		return this;
	}

	public override void ApplyTo(string testDir)
	{
		string combinedPath = System.IO.Path.Combine(testDir, _path);

		if (!System.IO.Directory.Exists(combinedPath))
			throw new Exception($"Directory {_path} was not found.");

		if (_files != null)
		{
			foreach (var file in _files)
			{
				if (!System.IO.File.Exists(System.IO.Path.Combine(combinedPath, file)))
					throw new Exception($"File {file} was not found in directory {_path}.");
			}
		}
	}
}
