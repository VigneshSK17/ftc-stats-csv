module FTCStatsCLI.StatsPage

#r "nuget: FSharp.Data"

open FSharp.Data
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
    (new RowStatsCSV(values)).SaveToString()

let get_teams_list teams_str : string list = 
    match teams_str with
    | "" -> []
    | s ->
        s.Split(',')
        |> Array.map(fun t -> 
            try
                let _ = (int) t
                t
            with
            | :? System.FormatException ->
                failwith "Team numbers are not valid.")
        |> Array.toList 

let GenCSV season_num region date teams_str no_header file_name =
    let header = "Team,Team Name,Event,OPR,Auto OPR,Auto Cones,Auto Nav,TeleOp OPR,TeleOp Cones,TeleOp Junctions,EndGame OPR,Junctions Owned,Circuits"

    let csv_str = GetLink season_num region |> GetRows |> GenCSVStr
    let csv_lines = 
        match date with
        | "" -> csv_str.Split('\n') |> Array.toList
        | _ ->
            csv_str.Split('\n') |> Array.toList |> List.filter(fun l -> l.Contains date)

    let csv_lines_teams =
        match teams_str with
        | "" -> csv_lines
        | ts ->
            ts
            |> get_teams_list
            |> List.map(fun team ->
                csv_lines 
                |> List.filter(fun l -> l.Contains team)
            )
            |> List.map(fun rows -> rows |> String.concat "")

    let csv_lines_header = if no_header then csv_lines_teams else header :: csv_lines_teams

    let file_path = $"{file_name}.csv"
    File.WriteAllLines(file_path, csv_lines_header)

    file_path