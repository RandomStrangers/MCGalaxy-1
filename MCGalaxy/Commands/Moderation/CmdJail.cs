﻿/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Events;
using static MCGalaxy.Chat;

namespace MCGalaxy.Commands.Moderation
{
    public sealed class CmdJail : Command
    {
        public override string name { get { return "Jail"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);

            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;

            if (message.CaselessEq("set") && p != null)
            {
                p.level.Config.JailX = p.Pos.X; p.level.Config.JailY = p.Pos.Y; p.level.Config.JailZ = p.Pos.Z;
                p.level.Config.jailrotx = p.Rot.RotY; p.level.Config.jailroty = p.Rot.HeadX;
                MessageOps("Set Jail point.");
                return;
            }

            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;

            if (!who.jailed)
            {
                if (p != null && who.Rank >= p.Rank)
                {
                    p.Message("jail", false); return;
                }
                ModAction action = new ModAction(who.name, p, ModActionType.Jailed, reason);
                OnModActionEvent.Call(action);
            }
            else
            {
                ModAction action = new ModAction(who.name, p, ModActionType.Unjailed, reason);
                OnModActionEvent.Call(action);
            }
        }

        public override void Help(Player p)
        {
            p.Message("%T/Jail [user] <reason>");
            p.Message("%HPlaces [user] in jail unable to use commands.");
            p.Message("%HFor <reason>, @number can be used as a shortcut for that rule.");
            p.Message("%T/Jail set");
            p.Message("%HCreates the jail point for the map.");
            p.Message("%H  This has been deprecated in favor of %T/XJail");
        }
    }
}