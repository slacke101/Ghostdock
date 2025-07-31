using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GhostDock
{
    public class SettingsManager
    {
        private readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public Dictionary<string, int> LoadAppAssignments()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);
                    return settings.TryGetValue("AppAssignments", out var assignments) ? assignments : new Dictionary<string, int>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading app assignments: {ex.Message}");
            }
            return new Dictionary<string, int>();
        }

        public void SaveAppAssignments(Dictionary<int, string> assignments)
        {
            try
            {
                var settings = new Dictionary<string, Dictionary<string, int>>();
                var currentAssignments = LoadAppAssignments();
                var newAssignments = new Dictionary<string, int>();

                foreach (var assignment in assignments)
                {
                    if (!string.IsNullOrEmpty(assignment.Value))
                    {
                        newAssignments[assignment.Value] = assignment.Key;
                    }
                }

                settings["AppAssignments"] = newAssignments;

                // Preserve existing color scheme
                string colorScheme = LoadColorScheme();
                if (!string.IsNullOrEmpty(colorScheme))
                {
                    settings["ColorScheme"] = new Dictionary<string, int> { { colorScheme, 0 } };
                }

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving app assignments: {ex.Message}");
            }
        }

        public string LoadColorScheme()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json);
                    if (settings.TryGetValue("ColorScheme", out var colorSchemeDict))
                    {
                        return colorSchemeDict.Keys.FirstOrDefault() ?? "Navy Blue/Gold";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading color scheme: {ex.Message}");
            }
            return "Navy Blue/Gold";
        }

        public void SaveColorScheme(string colorScheme)
        {
            try
            {
                var settings = new Dictionary<string, Dictionary<string, int>>();

                // Preserve existing app assignments
                var currentAssignments = LoadAppAssignments();
                settings["AppAssignments"] = currentAssignments;

                // Save color scheme
                settings["ColorScheme"] = new Dictionary<string, int> { { colorScheme, 0 } };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving color scheme: {ex.Message}");
            }
        }
    }
}