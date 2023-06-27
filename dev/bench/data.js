window.BENCHMARK_DATA = {
  "lastUpdate": 1687879771124,
  "repoUrl": "https://github.com/dotnet/Open-XML-SDK",
  "entries": {
    "Validation": [
      {
        "commit": {
          "author": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "committer": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "id": "51d05949db03b8861f430689678d124da36ec3d5",
          "message": "Only update benchmark results on main",
          "timestamp": "2023-06-26T08:41:16Z",
          "url": "https://github.com/dotnet/Open-XML-SDK/pull/1461/commits/51d05949db03b8861f430689678d124da36ec3d5"
        },
        "date": 1687879752654,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.ValidationTests.Validation",
            "value": 82293123.80952382,
            "unit": "ns",
            "range": "± 1222927.1366732018"
          }
        ]
      }
    ],
    "CompiledParticle": [
      {
        "commit": {
          "author": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "committer": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "id": "51d05949db03b8861f430689678d124da36ec3d5",
          "message": "Only update benchmark results on main",
          "timestamp": "2023-06-26T08:41:16Z",
          "url": "https://github.com/dotnet/Open-XML-SDK/pull/1461/commits/51d05949db03b8861f430689678d124da36ec3d5"
        },
        "date": 1687879763919,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.CompiledParticle.SetItem",
            "value": 133.31902027130127,
            "unit": "ns",
            "range": "± 2.8190117173950586"
          },
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.CompiledParticle.AddToCollection",
            "value": 104.75993156433105,
            "unit": "ns",
            "range": "± 2.038127058557232"
          }
        ]
      }
    ],
    "Documents": [
      {
        "commit": {
          "author": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "committer": {
            "name": "dotnet",
            "username": "dotnet"
          },
          "id": "51d05949db03b8861f430689678d124da36ec3d5",
          "message": "Only update benchmark results on main",
          "timestamp": "2023-06-26T08:41:16Z",
          "url": "https://github.com/dotnet/Open-XML-SDK/pull/1461/commits/51d05949db03b8861f430689678d124da36ec3d5"
        },
        "date": 1687879769015,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.Documents.Create",
            "value": 80208.89408172123,
            "unit": "ns",
            "range": "± 3640.9114726227926"
          },
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.Documents.CreateNoSave",
            "value": 82278.58973911831,
            "unit": "ns",
            "range": "± 649.9698241565359"
          },
          {
            "name": "DocumentFormat.OpenXml.Benchmarks.Documents.ReadFile",
            "value": 6221925.765625,
            "unit": "ns",
            "range": "± 2758510.991745672"
          }
        ]
      }
    ]
  }
}