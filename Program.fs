open Argu
open System

// NOTE: States only
type Region = 
    | AK | AZ | CA | CALA | CANO | CASD | FL | GA | HI | IA | IL | KY | MI | MN
    | MOKS | MT | NJ | NV | NY | NYNY | OH | OK | OR | TX | TXCE | TXHO | TXNO 
    | TXSO | TXWP | WA

let parse_region (region: Region) = 
    match region with
    | AK -> "alaska"
    | AZ -> "arizona"
    | CA -> "california"
    | CALA -> "california_los_angeles"
    | CANO -> "california_northern"
    | CASD -> "california_san_diego"
    | FL -> "florida"
    | GA -> "georgia"
    | HI -> "hawaii"
    | IA -> "iowa"
    | IL -> "illinois"
    | KY -> "kentucky"
    | MI -> "michigan"
    | MN -> "minnesota"
    | MOKS -> "missouri"
    | MT -> "montana"
    | NJ -> "new_jersey"
    | NV -> "nevada"
    | NY -> "new_york"
    | NYNY -> "new_york_city"
    | OH -> "ohio"
    | OK -> "oklahoma"
    | OR -> "oregon"
    | TX  -> "texas"
    | TXCE -> "texas_central"
    | TXHO -> "texas_houston"
    | TXNO -> "texas_north"
    | TXSO -> "texas_south"
    | TXWP -> "texas_west_&_panhandle"
    | WA -> "washington"


type Arguments =
    | Season of int
    | Region of Region
    | Date of string
    | Teams of string
    | No_Header
    | File_Name of string
    interface IArgParserTemplate with

        member s.Usage =
            match s with
            | Season _ -> "specify current year of the season, the year when the season ends"
            | Region _ -> "specify what region of competition you want data from"
            | Date _ -> "specify competition date you want data from in the format MM/DD/YY"
            | Teams _ -> "enter teams you want to be included in your data separated by commas (Ex: \"9686,12864\")"
            | No_Header _ -> "removes title row in csv file to make it easier to combine generated data"
            | File_Name _ -> "determine what the name of the generated csv file will be"

let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
let parser = ArgumentParser.Create<Arguments>(errorHandler = errorHandler)
let results = parser.ParseCommandLine()

let season = results.GetResult(Season, defaultValue=2023)
let region = results.GetResult Region |> parse_region
let teams = results.GetResult(Teams, defaultValue="")
let date = results.GetResult(Date, defaultValue="")
let file_name = results.GetResult(File_Name, defaultValue="ftc-stats")
let no_header = results.Contains No_Header

let file_path = FTCStatsCLI.StatsPage.GenCSV season region date teams no_header file_name