export type ButtonStyle = "primary" | "secondary" | "success" | "danger" | "warning" | "info";

export enum TurnStatusType {
    None = "None",
    Speak = "Speak",
    FirstVote = "FirstVote",
    Defense = "Defense",
    FinalVote = "FinalVote",
    LastChance = "LastChance",
    Action = "Action",
    Report = "Report",
}

export enum HasMeetingType {
    Init = "Init",
    Remove = "Remove",
    Start = "Start",
    Stop = "Stop",
}
