using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance;

//var config = DefaultConfig.Instance.AddJob(Job.Dry); // Minimal overhead for quick validation
//var config = DefaultConfig.Instance.AddJob(
//    Job.Default.WithWarmupCount(5) // Reduced from default 6-15
//        .WithIterationCount(50) // Reduced from default 15-100
//        .WithInvocationCount(1024) // Number of method calls per iteration
//        .WithUnrollFactor(16) // Unroll factor for better performance
//);
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(config: config);
