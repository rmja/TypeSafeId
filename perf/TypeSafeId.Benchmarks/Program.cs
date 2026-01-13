using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance;

//var config = DefaultConfig.Instance.AddJob(Job.Dry); // Minimal overhead for quick validation
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(config: config);
