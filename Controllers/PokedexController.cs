using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using PokedexBE.Models;

namespace PokedexBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokedexController : ControllerBase
    {
        string baseUrl = "https://pokeapi.co/api/v2";
        private IHostingEnvironment _hostingEnvironment;
        public PokedexController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        // GET: api/Pokedex
        [HttpGet]
        public async Task<PokemonItem[]> Get()
        {
            string url = $"{baseUrl}/pokemon?limit=150";
            PokemonResponse pokemonResponse;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    pokemonResponse = JsonConvert.DeserializeObject<PokemonResponse>(apiResponse);
                }
            }
            return pokemonResponse.PokemonItems;
        }

        // GET: api/Pokedex/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Pokemon> Get(int id)
        {
            string url = $"{baseUrl}/pokemon/{id}";
            Pokemon pokemonResponse;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    pokemonResponse = JsonConvert.DeserializeObject<Pokemon>(apiResponse);
                }
            }
            return pokemonResponse;
        }
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> Download([FromQuery] string pokemonName)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
            if(!System.IO.Directory.Exists(uploads))
            {
                System.IO.Directory.CreateDirectory(uploads);
            }
            var filePath = Path.Combine(uploads, $"{pokemonName}.txt");

            string url = $"{baseUrl}/pokemon/{pokemonName}";
            Pokemon pokemonResponse;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    pokemonResponse = JsonConvert.DeserializeObject<Pokemon>(apiResponse);
                }
            }

            System.IO.File.WriteAllText(filePath, $"Pokemon name: {pokemonResponse.Name}");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), $"{pokemonName}.txt");
        }

        // POST: api/Pokedex
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pokedex/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
