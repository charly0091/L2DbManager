using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GameDbManager.API.Models.Items;
using System.Collections.Generic;
using System;
using System.Text.Json;

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
                Converters =
                {
                    new ItemConverter()
                }
            };
        }

        public async Task<string> ImportItemsFromXml(string xmlContent)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/item/import", xmlContent);
                response.EnsureSuccessStatusCode();
                var message = await response.Content.ReadAsStringAsync();
                return message;
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                Console.WriteLine($"Error importing items: {ex.Message}");
                return $"Error importing items: {ex.Message}";
            }
        }

        public async Task<List<Item>> GetItemsAsync()
        {
            try
            {
                var responseStream = await _httpClient.GetStreamAsync("api/item");
                var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseStream, _options);
                return items;
            }
            catch (NotSupportedException nsex)
            {
                // Log o handle the error appropriately
                Console.WriteLine($"NotSupportedException fetching items: {nsex.Message}, StackTrace: {nsex.StackTrace}");
                return new List<Item>();
            }
            catch (JsonException jex)
            {
                // Log or handle the error appropriately
                Console.WriteLine($"JsonException fetching items: {jex.Message}, Path: {jex.Path}, LineNumber: {jex.LineNumber}, BytePositionInLine: {jex.BytePositionInLine}");
                return new List<Item>();
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                Console.WriteLine($"Exception fetching items: {ex.Message}, StackTrace: {ex.StackTrace}");
                return new List<Item>();
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
                // Log or handle the error appropriately
                Console.WriteLine($"Error deleting item with ID {id}: {ex.Message}");
            }
        }
    }
}