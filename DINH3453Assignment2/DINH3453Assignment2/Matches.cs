using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;
using static Xamarin.Essentials.Permissions;

namespace DINH3453Assignment2
{
    public class Matches
    {    
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int OpponentID { get; set; } 
        public DateTime Date { get; set; }
        public string Comments { get; set; }
        public int GameID { get; set; } 
        public bool Win { get; set; }
    }
}
