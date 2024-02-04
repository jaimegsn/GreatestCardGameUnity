using System.Collections.Generic;
using Newtonsoft.Json;


public class Matchmaking
{
    public string id { get; set; }
    public string leader { get; set; }
    public string maxplayers { get; set; }
    public List<int> challenges { get; set; }
    public Game game { get; set; }
    public Dictionary<string, Player> players { get; set; }

    public override string ToString()
    {
        return $"Matchmaking {{ {id} {leader} {maxplayers} {JsonConvert.SerializeObject(challenges)} {JsonConvert.SerializeObject(players)}}}";
    }

}

