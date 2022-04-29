using FractalsChat.Automaton.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FractalsChat.Automaton.Common.Test.ModelTests
{
    [TestClass]
    public class ChannelShould
    {
        [TestMethod]
        public void ProcessMembers()
        {
            string[] groupA = new string[] { "C", "D" };

            string[] groupB = new string[] { "A", "B" };

            string[] historical = groupA.Concat(groupB).ToArray();

            Channel channel = new Channel() { ChannelId = 1, Name = "#botplayground", Description = "Fractals Chat IRC Server", Created = DateTimeOffset.UtcNow };

            channel.UpdateMembers(groupA);

            Assert.IsTrue(channel.ActiveMembers.Equals(string.Join('\u002C', groupA)));

            Assert.IsTrue(channel.HistoricalMembers.Equals(string.Join('\u002C', groupA)));

            Assert.IsTrue(channel.GetActiveMembers().Length == groupA.Length);

            Assert.IsTrue(channel.GetHistoricalMembers().Length == groupA.Length);

            channel.UpdateMembers(groupB);

            Assert.IsTrue(channel.ActiveMembers.Equals(string.Join('\u002C', groupB)));

            Assert.IsTrue(channel.HistoricalMembers.Equals(string.Join('\u002C', historical)));

            Assert.IsTrue(channel.GetActiveMembers().Length == groupB.Length);

            Assert.IsTrue(channel.GetHistoricalMembers().Length == historical.Length);
        }
    }
}