module FTCStatsCLI.StatsPage

#r "nuget: FSharp.Data"

open FSharp.Data
open System
open System.IO

[<Literal>]
let private TEST_URL = "http://www.ftcstats.org/2023/georgia.html"
type STATS_HTML = HtmlProvider<TEST_URL>

[<Literal>]
let private CSV_SCHEMA = "A (int), B (string), C (string), D (decimal), E (decimal), F (decimal), G (decimal), H (decimal), I (decimal), J (decimal), K (decimal), L (decimal), M (decimal)"
type RowStatsCSV = CsvProvider<Schema = CSV_SCHEMA, HasHeaders=false>

let private GetLink (season: int) (region: string) : string =
    $"http://www.ftcstats.org/{season}/{region}.html"

let private GetRows (link: string) : STATS_HTML.Opr.Row[] =
    STATS_HTML.Load(link).Tables.Opr.Rows

let private GenCSVStr (rows: STATS_HTML.Opr.Row[])  =
    let header = "Team,Team Name,Event,OPR,Auto OPR,Auto Cones,Auto Nav,TeleOp OPR,TeleOp Cones,TeleOp Junctions,EndGame OPR,Junctions Owned,Circuits\n"
    let values = rows |> Array.map(fun r -> RowStatsCSV.Row(
        r.Team,
        r.``Team name``,
        r.Event,
        r.``Non Penalty OPRc *``,
        r.``Auto OPRc *``, r.``Auto Cones *``,
        r.``Auto Nav *``,
        r.``TeleOp OPRc *``,
        r.``Tele Cones *``,
        r.``Tele Jct *``,
        r.``End Game OPRc *``,
        r.``End Owned *``,
        r.``End Circuit *``
    ))
    let csv_str = (new RowStatsCSV(values)).SaveToString()

    header + csv_str

let GenCSV season_num region date file_name =
    let csv_str = GetLink season_num region |> GetRows |> GenCSVStr

    let csv_lines = 
        match date with
        | "" -> csv_str.Split('\n')
        | _ ->
            csv_str.Split('\n') |> Array.filter(fun l -> l.Contains date)

    let file_path = $"{file_name}.csv"
    File.WriteAllLines(file_path, csv_lines)

    file_path