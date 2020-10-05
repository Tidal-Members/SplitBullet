using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ConCommand
{
    public Func<string[],string> callback;
    public string help = "No help available.";
    public float helpOutputIncrement = 65f;
    public ConCommand(Func<string[],string> cb, string helpstring)
    {
        callback = cb;
        help = helpstring;
    }
}
public static class CommandBackend
{
    public static float consoleSize = 65f;
    public static float line = 65f;
    public static string consoleLog = "Type \"exit\" to exit this console window.";
    public static bool executing = false;
    public static bool currentlyActive = false;
    public static Dictionary<string,ConCommand> commands = new Dictionary<string,ConCommand>();
    public static void AddConCommand(string entry,Func<string[],string> command,string help = "No help available.")
    {
        commands.Add(entry, new ConCommand(command,help));
    }
    public static void HandleConCommand(string entry,string[] args)
    {
        if(commands.ContainsKey(entry) && !executing)
        {
            executing = true;
            ConCommand command = commands[entry];
            string executedCommandOutput = command.callback.Invoke(args);
            if(executedCommandOutput != "")
            {
                PrintOutput(executedCommandOutput);
            }
            executing = false;
        }
        else
        {
            PrintOutput("Unknown command!");
        }
    }
    public static void PrintOutput(string printout,float sizeIncrease = 65f)
    {
        IncreaseOutputSize(sizeIncrease);
        consoleLog += "\n"+printout;
    }
    public static void SetOutputSize(float inc)
    {
        consoleSize = inc;
    }
    public static void IncreaseOutputSize(float inc)
    {
        consoleSize += inc;
    }
}
public class CommandHandler : MonoBehaviour
{
    private Text consoleLog;
    private InputField consoleInput;
    private Transform consoleWindow;
    public static CommandHandler handler;
    public void Awake()
    {
        handler = this;
        consoleWindow = transform.Find("Console");
        consoleLog = consoleWindow.Find("Scroll View").Find("Viewport").Find("Content").Find("Text").GetComponent<Text>();
        consoleInput = consoleWindow.Find("InputField").GetComponent<InputField>();
    }
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        Debug.Log("CommandHandler has been reloaded! Commands will not be available until play mode is restarted.");
    }
    public void Start()
    {
        CommandBackend.AddConCommand("help",(string[] args) =>
        {
            string helpmenu = "";
            if(args == null || args.Length == 0)
            {
                helpmenu = "Commands:";
                foreach(KeyValuePair<string,ConCommand> conCommand in CommandBackend.commands)
                {
                    helpmenu += "\n- "+conCommand.Key;
                    CommandBackend.IncreaseOutputSize(CommandBackend.line);
                }
                helpmenu += "\nUsage: help [string]";
            }
            else
            {
                CommandBackend.IncreaseOutputSize(CommandBackend.commands[args[0]].helpOutputIncrement);
                helpmenu += "\""+CommandBackend.commands[args[0]].help+"\"";
            }
            return helpmenu;
        }, "Displays the help menu.");
        CommandBackend.AddConCommand("exit",(string[] args) =>
        {
            consoleWindow.gameObject.SetActive(false);
            CommandBackend.currentlyActive = false;
            return "Exiting console window...";
        }, "Exits the console window.");
        CommandBackend.AddConCommand("clear",(string[] args) =>
        {
            consoleLog.text = "";
            CommandBackend.SetOutputSize(CommandBackend.line);
            return "Type \"exit\" to exit this console window.";
        }, "Clears and resets the console window.");
        CommandBackend.AddConCommand("fontsize",(string[] args) =>
        {
            if(args == null || args.Length == 0)
            {
                CommandBackend.SetOutputSize(CommandBackend.line);
                return "Font size is set to "+consoleLog.fontSize+".\nUsage: fontsize [int]";
            }
            else
            {
                consoleLog.fontSize = int.Parse(args[0]);
                return "Desired size has been set.";
            }
        }, "Sets the font size of the console window.");
        CommandBackend.AddConCommand("nextmap",(string[] args) =>
        {
            int index = SceneManager.GetActiveScene().buildIndex+1;
            SceneManager.LoadScene(index);
            return "Loaded "+index+"!";
        }, "Loads the next map in order.");
        CommandBackend.AddConCommand("lastmap",(string[] args) =>
        {
            int index = SceneManager.GetActiveScene().buildIndex-1;
            if(index >= 0)
            {
                SceneManager.LoadScene(index);
                return "Loaded "+index+"!";
            }
            else
                return "Map index "+index+" is out of range!";
        }, "Loads the next map in order.");
    }
    public void Update()
    {
        consoleLog.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(consoleLog.transform.parent.GetComponent<RectTransform>().sizeDelta.x,CommandBackend.consoleSize);
        if(consoleLog.text != CommandBackend.consoleLog)
            consoleLog.transform.parent.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
        consoleLog.text = CommandBackend.consoleLog;
        if(Input.GetButtonDown("Submit"))
        {
            if(!CommandBackend.currentlyActive)
            {
                consoleWindow.gameObject.SetActive(true);
                CommandBackend.currentlyActive = true;
            }
            else if(!CommandBackend.executing && consoleInput.text != "")
            {
                string command = consoleInput.text.Split(' ')[0];
                int i = consoleInput.text.IndexOf(" ")+1;
                string[] args;
                if(i == 0) //repeats the original command ???
                    args = new string[0];
                else
                    args = consoleInput.text.Substring(i).Split(' ');
                CommandBackend.IncreaseOutputSize(CommandBackend.line);
                CommandBackend.PrintOutput("] "+consoleInput.text);
                consoleInput.text = "";
                CommandBackend.HandleConCommand(command,args);
            }
            consoleInput.Select();
            consoleInput.ActivateInputField();
        }
        else if(Input.GetButtonDown("Pause"))
        {
            consoleWindow.gameObject.SetActive(false);
            CommandBackend.currentlyActive = false;
        }
    }
}