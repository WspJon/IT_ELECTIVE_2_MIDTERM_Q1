using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MagicCardApp.Models;

namespace MagicCardApp.Controllers
{
    public class CardController : Controller
    {
        private readonly HttpClient _httpClient;

        public CardController()
        {
            _httpClient = new HttpClient();
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "MagicCardApp/1.0");
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Models.GlobalData.IsLoggedIn)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new List<MagicCard>());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchQuery)
        {
            if (!Models.GlobalData.IsLoggedIn)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View(new List<MagicCard>());
            }

            var cards = new List<MagicCard>();

            try
            {
                // Scryfall Search Endpoint
                string apiUrl = $"https://api.scryfall.com/cards/search?q={Uri.EscapeDataString(searchQuery)}";

                // Siguraduhing tumatanggap ng JSON ang request
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        if (doc.RootElement.TryGetProperty("data", out var data))
                        {
                            foreach (var element in data.EnumerateArray())
                            {
                                var card = new MagicCard
                                {
                                    Name = element.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown",
                                    TypeLine = element.TryGetProperty("type_line", out var type) ? type.GetString() : "",
                                    OracleText = element.TryGetProperty("oracle_text", out var oracle) ? oracle.GetString() : "",
                                    FlavorText = element.TryGetProperty("flavor_text", out var flavor) ? flavor.GetString() : "",
                                    Set = element.TryGetProperty("set", out var set) ? set.GetString().ToUpper() : "",
                                    Rarity = element.TryGetProperty("rarity", out var rarity) ? rarity.GetString() : ""
                                };

                                if (element.TryGetProperty("image_uris", out var images) && images.TryGetProperty("normal", out var normalImg))
                                {
                                    card.ImageUrl = normalImg.GetString();
                                }
                                else if (element.TryGetProperty("card_faces", out var faces) && faces.GetArrayLength() > 0)
                                {
                                    // Para sa mga double-faced cards
                                    var firstFace = faces[0];
                                    if (firstFace.TryGetProperty("image_uris", out var faceImages) && faceImages.TryGetProperty("normal", out var faceImg))
                                    {
                                        card.ImageUrl = faceImg.GetString();
                                    }
                                }

                                if (element.TryGetProperty("power", out var p) && element.TryGetProperty("toughness", out var t))
                                {
                                    card.PowerToughness = $"{p.GetString()}/{t.GetString()}";
                                }

                                cards.Add(card);
                            }
                        }
                    }
                }
                else
                {
                    // Kung may error mula sa Scryfall API (tulad ng 404), ilalagay natin ang detalye rito para makita
                    cards.Add(new MagicCard
                    {
                        Name = $"API Response Status: {response.StatusCode}",
                        TypeLine = "Scryfall Message",
                        OracleText = jsonString, // Ipapakita nito ang eksaktong mensahe mula sa API server
                        Set = "API",
                        Rarity = "common"
                    });
                }
            }
            catch (Exception ex)
            {
                cards.Add(new MagicCard
                {
                    Name = "Connection Exception",
                    TypeLine = "Error",
                    OracleText = ex.Message,
                    Set = "ERR",
                    Rarity = "rare"
                });
            }

            return View(cards);
        }
    }
}