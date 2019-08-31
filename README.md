# TimeTracer
A small library for quickly adding timing metrics to sections of code. Sometimes a profiler is overkill (or you're unable to attach a profiler) and you just want to  quickly identify bits of code that are taking a long time to execute.

If you ever find yourself writing code like this...

```csharp
var stopwatch = Stopwatch.StartNew();
		
// Code you want to time
		
stopwatch.Stop();
		
Console.WriteLine($"Execution took {stopwatch.ElapsedMilliseconds}ms");
```

... then this is the library for you!

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
