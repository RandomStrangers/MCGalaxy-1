/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */

using MCGalaxy.Generator.fCraft;

namespace MCGalaxy.Commands.Moderation
{
    public sealed class CmdXJail : Command
    {
        public override string name { get { return "XJail"; } }
        public override string shortcut { get { return "xj"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            string xjailMap = ServerConfig.XJailLevel;
            if (xjailMap == "main") xjailMap = Server.mainLevel.name;
            if (message.Length == 0) { Help(p); return; }

            Command jail = Find("Jail");
            if (message == "set")
            {
                if (!p.level.IsMuseum)
                {
                    jail.Use(p, "set");
                    ServerConfig.XJailLevel = p.level.name;
                    SrvProperties.Save();
                    p.Message("The xjail map was set from '" + xjailMap + "' to '" + p.level.name + "'");
                }
                else
                {
                    p.Message("You are in a museum!");
                }
                return;
            }

            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (p != null && who.Rank >= p.Rank)
            {
                p.Message("xjail", false); return;
            }

            Command spawn = Find("Spawn");
            Command freeze = Find("Freeze");
          //  Command mute = Find("Mute");
          // The unmute feature for this is broken right now... So no mute.


            if (!Server.jailed.Contains(who.name))
            {
                // if (!who.muted) mute.Use(p, message);
                if (!who.frozen) freeze.Use(p, message + " 10000d");

                PlayerActions.ChangeMap(who, xjailMap);
                who.BlockUntilLoad(10);
                jail.Use(p, message);
                Chat.MessageGlobal("{0} %Swas XJailed!", who.ColoredName);
            }
            else
            {
              //  if (who.muted == true) Find("unmute").Use(p, message);
                if (who.frozen) freeze.Use(p, message);
                if (who.jailed) jail.Use(p, message);

                PlayerActions.ChangeMap(who, Server.mainLevel);
                who.BlockUntilLoad(10);
                spawn.Use(who, "");
                Chat.MessageGlobal("{0} %Swas released from XJail!", who.ColoredName);
            }
        }

        public override void Help(Player p)
        {
            p.Message("%T/XJail [player]");
            p.Message("%Hfreezes, jails, and sends <player> to the XJail map");
            p.Message("%HIf [player] is already jailed, [player] will be spawned, unfrozen and unjailed");
            p.Message("%T/XJail set");
            p.Message("%HSets the xjail map to your current map and sets jail to current location");
        }
    }
}