# FTC Stats CSV
A simple CLI tool to generate a usable CSV from data on ftcstats.org

# Help

```
USAGE: ftc_stats_csv [--help] [--season <int>] [--region <ak|az|ca|cala|cano|casd|fl|ga|hi|ia|il|ky|mi|mn|moks|mt|nj|nv|ny|nyny|oh|ok|or|tx|txce|txho|txno|txso|txwp|wa>] [--date <string>] [--file-name <string>]

OPTIONS:

    --season <int>        specify current year of the season, the year when the season ends
    --region <ak|az|ca|cala|cano|casd|fl|ga|hi|ia|il|ky|mi|mn|moks|mt|nj|nv|ny|nyny|oh|ok|or|tx|txce|txho|txno|txso|txwp|wa>
                          specify what region of competition you want data from
    --date <string>       specify competition date you want data from in the format MM/DD/YY
    --file-name <string>  determine what the name of the generated csv file will be
    --help                display this list of options.
```

# How to build locally

1) Note: Install the [dotnet-sdk](https://dotnet.microsoft.com/en-us/download) on your device.
2) Run ```dotnet build``` to create the executable
- The newly-created executable will be located at ```./bin/Debug/net7.0/``` and will be called ftc_stats_csv with the appropriate extension