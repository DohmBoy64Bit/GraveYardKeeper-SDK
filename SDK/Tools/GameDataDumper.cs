using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GraveSDK.Tools
{
    /// <summary>
    /// Dumps all GameBalance data lists into individual JSON files.
    /// Press F10 in-game to trigger the dump.
    /// Output: GraveYardKeeper/DataDump/
    /// </summary>
    public class GameDataDumper : MonoBehaviour
    {
        private static bool _dumping = false;
        private static string _outputDir;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10) && !_dumping)
            {
                _dumping = true;
                try
                {
                    DumpAll();
                }
                catch (Exception ex)
                {
                    Debug.LogError("[GraveSDK] Dump failed: " + ex);
                }
                finally
                {
                    _dumping = false;
                }
            }
        }

        public static void DumpAll()
        {
            if (GameBalance.me == null)
            {
                Debug.LogError("[GraveSDK] GameBalance not loaded. Load a save first.");
                return;
            }

            _outputDir = Path.Combine(Application.dataPath, "..", "DataDump");
            Directory.CreateDirectory(_outputDir);

            Debug.Log("[GraveSDK] Starting GameBalance dump to: " + _outputDir);

            var settings = CreateJsonSettings();
            var gb = GameBalance.me;
            int totalFiles = 0;
            int totalEntries = 0;

            // Dump all 34 data lists
            totalEntries += DumpList(gb.items_data, "items", settings, ref totalFiles);
            totalEntries += DumpList(gb.craft_data, "crafts", settings, ref totalFiles);
            totalEntries += DumpList(gb.objs_data, "objects", settings, ref totalFiles);
            totalEntries += DumpList(gb.quests_data, "quests", settings, ref totalFiles);
            totalEntries += DumpList(gb.techs_data, "technologies", settings, ref totalFiles);
            totalEntries += DumpList(gb.tech_branches_data, "tech_branches", settings, ref totalFiles);
            totalEntries += DumpList(gb.vendors_data, "vendors", settings, ref totalFiles);
            totalEntries += DumpList(gb.buffs_data, "buffs", settings, ref totalFiles);
            totalEntries += DumpList(gb.perks_data, "perks", settings, ref totalFiles);
            totalEntries += DumpList(gb.bodies_data, "bodies", settings, ref totalFiles);
            totalEntries += DumpList(gb.souls_data, "souls", settings, ref totalFiles);
            totalEntries += DumpList(gb.fishes_data, "fishes", settings, ref totalFiles);
            totalEntries += DumpList(gb.workers_data, "workers", settings, ref totalFiles);
            totalEntries += DumpList(gb.craft_obj_data, "building_crafts", settings, ref totalFiles);
            totalEntries += DumpList(gb.tools_data, "tool_types", settings, ref totalFiles);
            totalEntries += DumpList(gb.auras_data, "auras", settings, ref totalFiles);
            totalEntries += DumpList(gb.object_groups, "object_groups", settings, ref totalFiles);
            totalEntries += DumpList(gb.spawners_data, "spawners", settings, ref totalFiles);
            totalEntries += DumpList(gb.projectiles_data, "projectiles", settings, ref totalFiles);
            totalEntries += DumpList(gb.works_data, "works", settings, ref totalFiles);
            totalEntries += DumpList(gb.logics_data, "logics", settings, ref totalFiles);
            totalEntries += DumpList(gb.product_types_data, "product_types", settings, ref totalFiles);
            totalEntries += DumpList(gb.world_zones_data, "world_zones", settings, ref totalFiles);
            totalEntries += DumpList(gb.reservoirs_data, "reservoirs", settings, ref totalFiles);
            totalEntries += DumpList(gb.pray_events_data, "prayer_events", settings, ref totalFiles);
            totalEntries += DumpList(gb.achievements_data, "achievements", settings, ref totalFiles);
            totalEntries += DumpList(gb.transport_paths, "transport_paths", settings, ref totalFiles);
            totalEntries += DumpList(gb.cutscenes_data, "cutscenes", settings, ref totalFiles);
            totalEntries += DumpList(gb.tavern_events, "tavern_events", settings, ref totalFiles);
            totalEntries += DumpList(gb.jobs_data, "jobs", settings, ref totalFiles);
            totalEntries += DumpList(gb.chars_data, "characters", settings, ref totalFiles);
            totalEntries += DumpList(gb.jobs_atom_data, "job_atoms", settings, ref totalFiles);
            totalEntries += DumpList(gb.grade_levels, "grade_levels", settings, ref totalFiles);
            totalEntries += DumpList(gb.grave_requirement_data, "grave_requirements", settings, ref totalFiles);

            // Write metadata
            var metadata = new Dictionary<string, object>
            {
                { "dump_version", "1.0" },
                { "sdk_version", "1.3" },
                { "timestamp", DateTime.UtcNow.ToString("o") },
                { "total_files", totalFiles },
                { "total_entries", totalEntries }
            };
            var metaJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);
            File.WriteAllText(Path.Combine(_outputDir, "_metadata.json"), metaJson);

            Debug.Log($"[GraveSDK] Dump complete! {totalFiles} files, {totalEntries} entries → {_outputDir}");
        }

        private static int DumpList<T>(List<T> list, string name, JsonSerializerSettings settings, ref int fileCount)
        {
            if (list == null || list.Count == 0)
            {
                Debug.Log($"[GraveSDK]   Skipping {name} (empty)");
                return 0;
            }

            try
            {
                var wrapper = new Dictionary<string, object>
                {
                    { "type", name },
                    { "count", list.Count },
                    { "data", list }
                };

                var json = JsonConvert.SerializeObject(wrapper, Formatting.Indented, settings);
                var path = Path.Combine(_outputDir, name + ".json");
                File.WriteAllText(path, json);
                fileCount++;
                Debug.Log($"[GraveSDK]   {name}.json → {list.Count} entries");
                return list.Count;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GraveSDK]   FAILED {name}: {ex.Message}");
                return 0;
            }
        }

        private static JsonSerializerSettings CreateJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MaxDepth = 10,
                ContractResolver = new GameDataContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new SmartExpressionConverter(),
                    new GameResConverter(),
                    new ItemConverter(),
                    new Vector3Converter(),
                    new Vector2Converter(),
                    new ColorConverter(),
                    new SpriteConverter(),
                    new UnityObjectConverter()
                }
            };
            return settings;
        }
    }

    // --- Contract Resolver: skip problematic Unity fields ---
    internal class GameDataContractResolver : DefaultContractResolver
    {
        private static readonly HashSet<string> SkipFields = new HashSet<string>
        {
            "hideFlags", "name", "m_CachedPtr", "_exp", "_wgo", "_character",
            "_items", "_simplified", "_simpified_float"
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            // Skip Unity internal fields
            if (SkipFields.Contains(prop.PropertyName))
            {
                prop.ShouldSerialize = _ => false;
                return prop;
            }

            // Skip private fields unless they have [SerializeField]
            if (member is FieldInfo fi && !fi.IsPublic)
            {
                if (fi.GetCustomAttribute<SerializeField>() == null)
                {
                    prop.ShouldSerialize = _ => false;
                }
            }

            // Skip UnityEngine.Object-derived types (Sprite, Texture, AudioClip, etc.)
            var propType = prop.PropertyType;
            if (propType != null && typeof(UnityEngine.Object).IsAssignableFrom(propType)
                && propType != typeof(SmartExpression))
            {
                prop.ShouldSerialize = _ => false;
            }

            return prop;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            // Serialize fields (Unity convention), not properties
            var props = new List<JsonProperty>();

            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var jp = CreateProperty(fi, memberSerialization);
                jp.Writable = true;
                jp.Readable = true;
                props.Add(jp);
            }

            // Also include [SerializeField] private fields
            foreach (var fi in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fi.GetCustomAttribute<SerializeField>() != null)
                {
                    var jp = CreateProperty(fi, memberSerialization);
                    jp.Writable = true;
                    jp.Readable = true;
                    props.Add(jp);
                }
            }

            return props;
        }
    }

    // --- Custom Converters ---

    internal class SmartExpressionConverter : JsonConverter<SmartExpression>
    {
        public override void WriteJson(JsonWriter writer, SmartExpression value, JsonSerializer serializer)
        {
            if (value == null || !value.has_expression)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value.GetRawExpressionString());
        }

        public override SmartExpression ReadJson(JsonReader reader, Type objectType, SmartExpression existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return null;
        }
    }

    internal class GameResConverter : JsonConverter<GameRes>
    {
        public override void WriteJson(JsonWriter writer, GameRes value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            // Dump all public fields via reflection
            foreach (var fi in typeof(GameRes).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var val = fi.GetValue(value);
                if (val != null)
                {
                    writer.WritePropertyName(fi.Name);
                    serializer.Serialize(writer, val);
                }
            }
            writer.WriteEndObject();
        }

        public override GameRes ReadJson(JsonReader reader, Type objectType, GameRes existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return null;
        }
    }

    internal class ItemConverter : JsonConverter<Item>
    {
        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(value.id);
            writer.WritePropertyName("value");
            writer.WriteValue(value.value);

            if (value.definition != null)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(value.definition.type.ToString());
            }
            writer.WriteEndObject();
        }

        public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return null;
        }
    }

    internal class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(Math.Round(value.x, 2));
            writer.WritePropertyName("y"); writer.WriteValue(Math.Round(value.y, 2));
            writer.WritePropertyName("z"); writer.WriteValue(Math.Round(value.z, 2));
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Vector3.zero;
        }
    }

    internal class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(Math.Round(value.x, 2));
            writer.WritePropertyName("y"); writer.WriteValue(Math.Round(value.y, 2));
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Vector2.zero;
        }
    }

    internal class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(value)}");
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Color.white;
        }
    }

    internal class SpriteConverter : JsonConverter<Sprite>
    {
        public override void WriteJson(JsonWriter writer, Sprite value, JsonSerializer serializer)
        {
            writer.WriteValue(value != null ? value.name : null);
        }

        public override Sprite ReadJson(JsonReader reader, Type objectType, Sprite existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return null;
        }
    }

    /// <summary>
    /// Fallback converter for any UnityEngine.Object-derived type we haven't handled.
    /// Just writes the object's name to avoid serialization crashes.
    /// </summary>
    internal class UnityObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var uo = value as UnityEngine.Object;
            writer.WriteValue(uo != null ? uo.name : value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }
    }
}
