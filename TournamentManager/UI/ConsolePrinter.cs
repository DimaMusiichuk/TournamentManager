using System;
using System.Collections.Generic;
using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class ConsolePrinter
{
    public static void PrintGroupStage(Dictionary<string, Dictionary<IParticipant, ParticipantStats>> allGroupStandings) 
    {
        foreach (var group in allGroupStandings)
        {
            Console.WriteLine($"\n{group.Key}");
            Console.WriteLine($"{"Place"} || {"Team Name"} || {"Wins"} || {"Draws"} || {"Losses"} || {"Points"}");
            
            int place = 1;
            foreach (var row in group.Value)
            {
                var participant = row.Key;
                var stats = row.Value;
                
                Console.WriteLine($"{place} || {participant.Name} || {stats.Wins} || {stats.Draws} || {stats.Losses} || {stats.Points}");
                place++;
            }
        }
    }
    
    public static void PrintPlayOff(List<Match> playOffMatches)
    {
        foreach (var match in playOffMatches)
        {
            PrintMatchStatus(match);
        }
    }
    
    public static void PrintMatchStatus(Match match)
    {
        Console.WriteLine($"{match.FirstParticipant.Name} VS {match.SecondParticipant.Name} || Status: {match.Status}");
    }
}