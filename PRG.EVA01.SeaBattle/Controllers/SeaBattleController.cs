using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class SeaBattleController : Controller
    {

        private static List<Boat> _boats = new List<Boat>();

        private readonly SeaBattleDbContext _context;
        public SeaBattleController(SeaBattleDbContext context)
        {
            _context = context;
        }


        static SeaBattleController()
        {
            LoadBoats();
        }
        private static Game _game = new Game(1, "test", DateTime.Now, _boats);

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ThrowBomb(string letter, string number)
        {
            //nog functie schrijven die checkt of het valide letters of nummers zijn

            bool hit = CheckBombHit(letter, number);
            ViewBag.Location = $"{letter}{number}";
            ViewBag.Result = hit ? "Hit" : "Miss";
            ViewBag.SunkBoatsCount = SunkBoats();
            ViewBag.GameId = _game.Id;
            ViewBag.Player = _game.PlayerName;
            ViewBag.Date = _game.StartedPlayingOn;

            var gameLog = new GameLog
            {
                GameId = _game.Id,
                PlayerName = _game.PlayerName,
                LocationLetter = letter,
                LocationNumber = number.ToString(),
                Result = hit ? "Hit" : "Miss",
                CreatedOn = DateTime.Now 
            };

            _context.Add(gameLog);
            _context.SaveChanges();


            return View();
        }

        private bool CheckBombHit(string letter, string number)
        {
            foreach (var boat in _game.Boats)
            {
                if (boat.Location.Letter == letter && boat.Location.Number == number && boat.Status != BoatStatus.Sunk)
                {
                    boat.Status = BoatStatus.Sunk;

                    return true;
                }
            }
            return false;
        }

        private int SunkBoats()
        {
            int count = 0;
            foreach (var boat in _game.Boats)
            {
                if (boat.Status == BoatStatus.Sunk)
                {
                    count++;
                }
            }
            return count;
        }
        public IActionResult Hit()
        {
            return View();
        }

        public IActionResult Miss()
        {
            return View();
        }

        public static async Task LoadBoats()
        {
            for (int i = 0; i < 3; i++)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://mgp32-api.azurewebsites.net/");

                var response = await client.GetAsync("randomlocation/get/6");
                Console.Write(response);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonString);

                    // Deserialize the JSON string to an anonymous object
                    var location = JsonConvert.DeserializeAnonymousType(jsonString, new { Letter = "", Number = "" });

                    // Create a new Location object
                    var newLocation = new Location { Letter = location.Letter, Number = location.Number };

                    // Check if a boat with the same location already exists
                    bool boatExists = _boats.Any(b => b.Location.Equals(newLocation));

                    // Add the boat only if it does not already exist
                    if (!boatExists)
                    {
                        _boats.Add(new Boat { Location = newLocation, Status = BoatStatus.Active });
                    }
                    else
                    {
                        Console.WriteLine("Boat already exists at this location.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to load boats from the API.");
                }
            }

        }
            /*
            _boats.Add(new Boat { Location = new Location { Letter = "A", Number = "3" }, Status = BoatStatus.Active });
            _boats.Add(new Boat { Location = new Location { Letter = "B", Number = "4" }, Status = BoatStatus.Active });
            _boats.Add(new Boat { Location = new Location { Letter = "C", Number = "5" }, Status = BoatStatus.Active });
            _boats.Add(new Boat { Location = new Location { Letter = "D", Number = "6" }, Status = BoatStatus.Active });
            */
    }
}
