﻿using Android.App;
using Android.Content;
using Android.Content.PM;

namespace ParkingBot;

[Activity(
    Theme = "@style/Maui.MainTheme.NoActionBar",
    MainLauncher = true,
    ConfigurationChanges =
    ConfigChanges.ScreenSize |
    ConfigChanges.Orientation |
    ConfigChanges.UiMode |
    ConfigChanges.ScreenLayout |
    ConfigChanges.SmallestScreenSize |
    ConfigChanges.Density)]
[IntentFilter(
    [Shiny.ShinyNotificationIntents.NotificationClickAction, Shiny.ShinyPushIntents.NotificationClickAction],
    Categories = ["android.intent.category.DEFAULT"])]
public class MainActivity : MauiAppCompatActivity { }
