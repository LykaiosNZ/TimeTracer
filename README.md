# TimeTracer
![Travis (.com)](https://img.shields.io/travis/com/LykaiosNZ/TimeTracer/master) 
![Nuget](https://img.shields.io/nuget/v/TimeTracer) 
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/TimeTracer)

A small .NET Standard library for quickly adding timing metrics to sections of code.

If you ever find yourself writing code like this...

```csharp
var stopwatch = Stopwatch.StartNew();
		
// Code you want to time
		
stopwatch.Stop();
		
Console.WriteLine($"Execution took {stopwatch.ElapsedMilliseconds}ms");
```

... then this is the library for you!

## Why?
There are, of course, many great libraries for collecting metrics that already exist, as well as profilers and the aforementioned trusty `Stopwatch`. The intent of this library is to provide a little more functionality than `Stopwatch`, while still being incredibly easy to get up and running. 

It provides two simple metrics for instrumented sections of code (scopes): the aggregate duration of all calls of the scope, and the number of calls made. Scopes can be nested, and the collected metrics provide information about the scope hierarchy.

Using the power of `AsyncLocal<T>`, scopes can be created using static methods and have their metrics fed back to a single `TimeTrace` instance in a thread- and async-safe manner, without having to inject the instance into every class that you want collect metrics for.

## Installation
Simply install the NuGet package and you're ready to go:

```install-package TimeTracer```

## Usage
```csharp
// Create a new trace to capture timing metrics
using (var trace = new TimeTrace())
{
   // Begin a scope within the current trace. This is a static method, no reference to
   // the specific TimeTrace instance required.
   using (TimeTrace.BeginScope("ParentScope"))
   {
      // Do some work
      
      for (int i = 0; i < 2; i++)
      {
         using (TimeTrace.BeginScope("ChildScope"))
         {
            // Do some work
         }
      }
   }
   
   foreach (var metric in trace.Metrics)
   {
       Console.WriteLine($"Scope: {metric.Name}, Duration: {metric.TotalDuration}, CallCount: {metric.Count}");   
   }
}

// Output:
// Scope: ParentScope, Duration: [...], CallCount: 1
// Scope: ParentScope/ChildScope, Duration: [...], CallCount: 2
```
## Credits
* Unit tests powered by [NUnit](https://github.com/nunit)

## Licence
Licenced under the terms of the [MIT Licence](https://opensource.org/licenses/MIT)