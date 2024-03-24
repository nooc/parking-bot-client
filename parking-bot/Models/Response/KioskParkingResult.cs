﻿namespace ParkingBot.Models.Response;

public sealed class KioskParkingResult
{
    public string? EndTime { get; set; }
    public string? EndTimeText { get; set; }
    public bool IsLimited { get; set; }
    public string? LimitationText { get; set; }
}
