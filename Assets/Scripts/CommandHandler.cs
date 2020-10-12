using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ConCommand
{
    public Func<string[],string> callback;
    public string help = "No help available.";
    public float helpOutputIncrement = 65f;
    public CommandBackend.CommandType type;
    public ConCommand(Func<string[],string> cb, string helpstring,CommandBackend.CommandType conType = CommandBackend.CommandType.Normal)
    {
        callback = cb;
        help = helpstring;
        type = conType;
    }
}
public static class CommandBackend
{
    public enum CommandType
    {
        Normal,
        Cheat,
        Developer
    }
    public static float consoleSize = 65f;
    public static float line = 65f;
    public static string consoleLog = "Type \"exit\" to exit this console window.";
    public static bool executing = false;
    public static bool currentlyActive = false;
    public static CommandType allowedCommands = CommandType.Normal;
    public static Dictionary<string,ConCommand> commands = new Dictionary<string,ConCommand>();
    public static void AddConCommand(string entry,Func<string[],string> command,string help = "No help available.",CommandBackend.CommandType conType = CommandBackend.CommandType.Normal)
    {
        commands.Add(entry, new ConCommand(command,help,conType));
    }
    public static void HandleConCommand(string entry,string[] args = null)
    {
        if(commands.ContainsKey(entry) && !executing)
        {
            executing = true;
            ConCommand command = commands[entry];
            switch(command.type)
            {
                case CommandBackend.CommandType.Cheat:
                    if(CommandBackend.allowedCommands < CommandBackend.CommandType.Cheat)
                        PrintOutput("Cheats must be enabled to use this command.");
                    else
                    {
                        string executedOutput = command.callback.Invoke(args);
                        if(executedOutput != "")
                            PrintOutput(executedOutput);
                    }
                    break;
                case CommandBackend.CommandType.Developer:
                    if(CommandBackend.allowedCommands >= CommandBackend.CommandType.Developer)
                    {
                        string commandOutput = command.callback.Invoke(args);
                        if(commandOutput != "")
                            PrintOutput(commandOutput);
                    }
                    PrintOutput("This command is only available within the editor.");
                    break;
                default:
                    string executedCommandOutput = command.callback.Invoke(args);
                    if(executedCommandOutput != "")
                        PrintOutput(executedCommandOutput);
                    break;
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
        consoleWindow.gameObject.SetActive(false);
    }
    #if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        Debug.Log("CommandHandler has been reloaded! Commands will not be available until play mode is restarted.");
    }
    #endif
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
                    if(conCommand.Value.type <= CommandBackend.allowedCommands)
                    {
                        helpmenu += "\n- "+conCommand.Key;
                        CommandBackend.IncreaseOutputSize(CommandBackend.line);
                    }
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
        CommandBackend.AddConCommand("quit",(string[] args) =>
        {
            Application.Quit();
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #endif
            return "Successfuly quit the game.";
        }, "Quits the game.");
        CommandBackend.AddConCommand("clear",(string[] args) =>
        {
            CommandBackend.consoleLog = "";
            consoleLog.text = "";
            CommandBackend.SetOutputSize(CommandBackend.line);
            return "Type \"exit\" to exit this console window.";
        }, "Clears and resets the console window.");
        CommandBackend.AddConCommand("fontsize",(string[] args) =>
        {
            if(args == null || args.Length == 0)
            {
                CommandBackend.IncreaseOutputSize(CommandBackend.line);
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
        CommandBackend.AddConCommand("allowcheats",(string[] args) =>
        {
            if(CommandBackend.allowedCommands < CommandBackend.CommandType.Cheat)
            {
                CommandBackend.allowedCommands = CommandBackend.CommandType.Cheat;
                return "Cheats have been enabled.";
            }
            else
                return "The allowed commands are set to a higher enum than the parameter.";
        }, "Activates cheats.");
        #if UNITY_EDITOR
        CommandBackend.AddConCommand("allowdev",(string[] args) =>
        {
            if(CommandBackend.allowedCommands < CommandBackend.CommandType.Developer)
            {
                CommandBackend.allowedCommands = CommandBackend.CommandType.Developer;
                return "Developer mode has been enabled.";
            }
            else
                return "The allowed commands are set to a higher enum than the parameter.";
        }, "Activates developer mode.");
        #endif
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