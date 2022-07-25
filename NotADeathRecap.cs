using System.Collections.Generic;
using ExileCore.PoEMemory.Components;
using ImGuiNET;
using ExileCore;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using System.Runtime.Caching;
using System;

namespace NotADeathRecap
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        private List<Buff> currentBuffs;
        private List<Buff> buffsLastAlive = new List<Buff>();
        Dictionary<string, string> buff_dict = new Dictionary<string, string>();
        MemoryCache cache = new MemoryCache("cacheWithBuffs");
        bool wasAlive = false;
        public override bool Initialise()
        {
            using (TextFieldParser parser = new TextFieldParser(@"Plugins\Source\NotADeathRecap\BuffDefinitions.csv")) //change for debugging
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    buff_dict.Add(fields[0], fields[1]);
                }
            }
            return base.Initialise();
        }
        public override void DrawSettings()
        {
            base.DrawSettings();
            if (buffsLastAlive != null && buffsLastAlive.Count > 0)
            {
                ImGui.Text("Buffs from last death:");
                ImGui.Separator();
                buffsLastAlive = buffsLastAlive.GroupBy(p => p.Name).Select(g => g.First()).ToList(); //remove duplicates

                foreach (var str in buffsLastAlive)
                {
                    if(ImGui.MenuItem(str.Name, buff_dict[str.Name]))
                    {
                        ImGui.SetClipboardText(str.Name);
                    }
                }
            }
            else
            {
                ImGui.MenuItem("You either haven't died yet or you had 0 buffs");
            }
        }
        public override void Render()
        {
            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            if (localPlayer.IsAlive){
                if (Settings.BuffTTL.Value != 0)
                {
                    foreach (var buff in localPlayer.GetComponent<Buffs>().BuffsList)
                    {
                        cache.Add(buff.Name, buff, DateTime.Now.AddSeconds(Settings.BuffTTL.Value));
                    }
                }
                else
                {
                    currentBuffs = localPlayer.GetComponent<Buffs>().BuffsList;
                }
                wasAlive = true;
            }
            else
            {
                if (wasAlive)
                {
                    if (Settings.BuffTTL.Value != 0)
                    {
                        buffsLastAlive.Clear();
                        foreach (KeyValuePair<string, object> entry in cache)
                        {
                            buffsLastAlive.Add((Buff)entry.Value);
                        }
                    }
                    else
                    {
                        buffsLastAlive = currentBuffs;
                    }
                    wasAlive = false;
                }
            }
        }
    }
}
