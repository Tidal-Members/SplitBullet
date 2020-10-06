using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public static class Presence
{
    public static long clientID = 762750152594292737;
    public static Discord.Discord discord;
    public static ActivityManager activityManager;
    public static Discord.Activity activity;
    public static void UpdatePresence()
    {
        activityManager.UpdateActivity(activity, (res) =>
		{
			switch(res)
            {
                case Discord.Result.Ok:
				    Debug.Log("Discord returned OK");
                    break;
                default:
                    Debug.LogError("Discord returned "+res.ToString());
                    break;
			}
		});
    }
}
public class PresenceHandler : MonoBehaviour
{
    public void CreateInstance() 
    {
        Presence.discord = new Discord.Discord(Presence.clientID, (UInt64)Discord.CreateFlags.Default);
        Presence.activityManager = Presence.discord.GetActivityManager();
		Presence.activity = new Activity();
        Presence.activity.State = "Test Level"; 
		Presence.activity.Details = "Prototype";
        Presence.UpdatePresence();
    }
    void Awake()
    {
        CreateInstance();
    }
	void Update ()
    {
        if(Presence.discord != null)
		    Presence.discord.RunCallbacks();
        else
            CreateInstance();
	}
    void OnApplicationQuit()
    {
        Presence.discord.Dispose();
    }
}