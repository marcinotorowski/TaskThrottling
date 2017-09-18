# TaskThrottling

This is a small library that contains useful methods to control throttled tasks.

# Usage
Executing tasks one-by-one
```csharp
ThrottlingBase seq = new ThrottleSequential();
seq.Add(token => Task.Run(DoMethod1, token));
seq.Add(token => Task.Run(DoMethod2, token));
seq.Add(token => Task.Run(DoMethod3, token));
await seq.ExecuteAsync(default(CancellationToken));
// Result: Method1, Method2, Method3
```

Executing tasks in parallel (with a given level of parallelism)

```csharp
ThrottlingBase seq = new ThrottleParallel(2);
seq.Add(token => Task.Run(DoMethod1, token));
seq.Add(token => Task.Run(DoMethod2, token));
seq.Add(token => Task.Run(DoMethod3, token));
await seq.ExecuteAsync(default(CancellationToken));
// Result: DoMethod3 is executed only after Method1 or Method2 finished.
```

They can be combined too:
```csharp
ThrottlingBase seq1 = new ThrottleSequential();
seq1.Add(token => Task.Run(DoMethod11, token));
seq1.Add(token => Task.Run(DoMethod12, token));
seq1.Add(token => Task.Run(DoMethod13, token));

ThrottlingBase seq2 = new ThrottleSequential();
seq2.Add(token => Task.Run(DoMethod21, token));
seq2.Add(token => Task.Run(DoMethod22, token));
seq2.Add(token => Task.Run(DoMethod23, token));

ThrottlingBase seq3 = new ThrottleParallel(3);
seq3.Add(token => Task.Run(DoMethod31, token));
seq3.Add(token => Task.Run(DoMethod32, token));
seq3.Add(token => Task.Run(DoMethod33, token));
seq3.Add(token => Task.Run(DoMethod34, token));

ThrottlingBase seq = new ThrottleParallel(2);
seq.Add(seq1, seq2, seq3); // params overload
await seq.ExecuteAsync();
```
