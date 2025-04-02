using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text.Json;
using GameDbManager.Client.Models.Items;

namespace GameDbManager.Client.Services.Items
{
    public class ItemService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ItemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
                // Ya no necesitamos ReferenceHandler.Preserve
            };
        }

        public async Task<string> ImportItemsFromXml(string xmlContent, bool overwriteExisting = false)
        {
            try
            {
                var request = new ImportXmlRequest
                {
                    XmlContent = xmlContent,
                    OverwriteExisting = overwriteExisting
                };

                var response = await _httpClient.PostAsJsonAsync("api/item/import", request);
                response.EnsureSuccessStatusCode();
                var message = await response.Content.ReadAsStringAsync();
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing items: {ex.Message}");
                return $"Error importing items: {ex.Message}";
            }
        }

        public async Task<bool> ValidateXml(string xmlContent)
        {
            try
            {
                var request = new ValidateXmlRequest { XmlContent = xmlContent };
                var response = await _httpClient.PostAsJsonAsync("api/item/validate", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating XML: {ex.Message}");
                return false;
            }
        }

        public async Task<ItemDto> GetItemAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ItemDto>($"api/item/{id}", _options);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error getting item with ID {id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting item with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ItemDto>> GetItemsAsync()
        {
            try
            {
                // Podemos simplificarlo ahora que el servidor devuelve JSON directo
                return await _httpClient.GetFromJsonAsync<List<ItemDto>>("api/item", _options) ??
                       new List<ItemDto>();
            }
            catch (Exception ex)
            {
                // Si por algún motivo esto falla, registramos el error y devolvemos lista vacía
                Console.WriteLine($"Exception fetching items: {ex.Message}");

                // Para depuración, podemos intentar ver la respuesta real
                try
                {
                    var response = await _httpClient.GetAsync("api/item");
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Raw response: {content.Substring(0, Math.Min(content.Length, 200))}...");
                }
                catch { }

                return new List<ItemDto>();
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/item/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting item with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<ItemDto> CreateItemAsync(ItemDto item)
        {
            try
            {
                // Asegurar que las colecciones estén inicializadas
                if (item.Stats == null) item.Stats = new List<ItemStatDto>();
                if (item.Skills == null) item.Skills = new List<ItemSkillDto>();

                var response = await _httpClient.PostAsJsonAsync("api/item", item, _options);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ItemDto>(_options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating item: {ex.Message}");
                throw;
            }
        }

        public async Task<ItemDto> UpdateItemAsync(int id, ItemDto item)
        {
            try
            {
                // Asegurar que las colecciones estén inicializadas
                if (item.Stats == null) item.Stats = new List<ItemStatDto>();
                if (item.Skills == null) item.Skills = new List<ItemSkillDto>();

                var response = await _httpClient.PutAsJsonAsync($"api/item/{id}", item, _options);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ItemDto>(_options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item: {ex.Message}");
                throw;
            }
        }

        // Clases para peticiones
        private class ImportXmlRequest
        {
            public string XmlContent { get; set; }
            public bool OverwriteExisting { get; set; }
        }

        private class ValidateXmlRequest
        {
            public string XmlContent { get; set; }
        }
    }
}