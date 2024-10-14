using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeskSpine
{
    public class BackgroundColorConverter : JsonConverter<SFML.Graphics.Color>
    {
        public override SFML.Graphics.Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 读取 RGB 值，忽略 A
            var jsonDocument = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDocument.RootElement;

            try
            {
                // 返回 Color 并将 Alpha 设置为 0
                byte r = jsonObject.GetProperty("R").GetByte();
                byte g = jsonObject.GetProperty("G").GetByte();
                byte b = jsonObject.GetProperty("B").GetByte();
                return new(r, g, b, 0);
            }
            catch (KeyNotFoundException)
            {
                return new(128, 128, 128, 0);
            }
        }

        public override void Write(Utf8JsonWriter writer, SFML.Graphics.Color value, JsonSerializerOptions options)
        {
            // 序列化 R、G、B 值，忽略 A
            writer.WriteStartObject();
            writer.WriteNumber("R", value.R);
            writer.WriteNumber("G", value.G);
            writer.WriteNumber("B", value.B);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// 系统设置
    /// </summary>
    public class SystemConfig
    {
        [JsonIgnore]
        public bool AutuRun { get; set; } = false;
        public bool Visible { get; set; } = true;
        public string? BalloonIconPath { get; set; }
        public bool HourlyChime = true;
    }

    /// <summary>
    /// 基础设置
    /// </summary>
    public class BasicConfig
    {
        public bool WallpaperMode { get; set; } = false;
        public bool MouseClickThrough { get; set; } = false;
        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;
        public uint SizeX { get; set; } = 1000;
        public uint SizeY { get; set; } = 1000;
        public float SpinePositionX { get; set; } = 0;
        public float SpinePositionY { get; set; } = 0;
        public bool SpineFlip { get; set; } = false;
        public float SpineScale { get; set; } = 1f;
        public byte Opacity { get; set; } = 255;
        public uint MaxFps { get; set; } = 30;
        [JsonConverter(typeof(BackgroundColorConverter))] // 结构体不会自动保存字段, 需要用自定义的转换器
        public SFML.Graphics.Color BackgroundColor { get; set; } = new(128, 128, 128, 0);
        public bool SpineUsePMA { get; set; } = false;

    }

    /// <summary>
    /// Spine 设置
    /// </summary>
    public class SpineConfig
    {
        public const int SlotCount = 10;
        private string?[] skelPaths = new string?[SlotCount];

        public Spine.SpineVersion SpineVersion { get; set; } = Spine.SpineVersion.V38;
        public TinyEngine.AnimatorType InteractMode { get; set; } = TinyEngine.AnimatorType.AzurLaneSD;

        public string? SkelPath0 { get => skelPaths[0]; set => skelPaths[0] = value; }
        public string? SkelPath1 { get => skelPaths[1]; set => skelPaths[1] = value; }
        public string? SkelPath2 { get => skelPaths[2]; set => skelPaths[2] = value; }
        public string? SkelPath3 { get => skelPaths[3]; set => skelPaths[3] = value; }
        public string? SkelPath4 { get => skelPaths[4]; set => skelPaths[4] = value; }
        public string? SkelPath5 { get => skelPaths[5]; set => skelPaths[5] = value; }
        public string? SkelPath6 { get => skelPaths[6]; set => skelPaths[6] = value; }
        public string? SkelPath7 { get => skelPaths[7]; set => skelPaths[7] = value; }
        public string? SkelPath8 { get => skelPaths[8]; set => skelPaths[8] = value; }
        public string? SkelPath9 { get => skelPaths[9]; set => skelPaths[9] = value; }

        public string? GetSkelPath(int index)
        {
            if (index > SlotCount) throw new ArgumentOutOfRangeException(nameof(index), $"Index must less than {SlotCount}");
            if (string.IsNullOrEmpty(skelPaths[index]))
                return null;
            return skelPaths[index];
        }

        public void SetSkelPath(int index, string? value)
        {
            if (index > SlotCount) throw new ArgumentOutOfRangeException(nameof(index), $"Index must less than {SlotCount}");
            skelPaths[index] = string.IsNullOrEmpty(value) ? null : value;
        }
    }

    /// <summary>
    /// DeskSpine 设置
    /// </summary>
    public class Config
    {
        private static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        public SystemConfig SystemConfig { get; set; } = new();
        public BasicConfig BasicConfig { get; set; } = new();
        public SpineConfig SpineConfig { get; set; } = new();

        /// <summary>
        /// 创建配置新实例并拥有合适的默认值
        /// </summary>
        public Config()
        {
            SystemConfig.BalloonIconPath = Path.Combine(Program.ProgramResourceDirectory, @"image\hourlychime.ico");
            SpineConfig.SkelPath0 = Path.Combine(Program.ProgramResourceDirectory, @"spine\AzueLaneSD\guanghui_2\guanghui_2.skel");
        }

        /// <summary>
        /// 从本地文件加载配置
        /// </summary>
        public bool Load(string configPath)
        {
            if (File.Exists(configPath))
            {
                try
                {
                    string jsonString = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<Config>(jsonString);

                    if (config != null)
                    {
                        if (config.SystemConfig is not null) SystemConfig = config.SystemConfig;
                        if (config.BasicConfig is not null) BasicConfig = config.BasicConfig;
                        if (config.SpineConfig is not null) SpineConfig = config.SpineConfig;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading config file: {ex}");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 保存配置至本地文件
        /// </summary>
        public bool Save(string configPath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(this, jsonOptions);
                File.WriteAllText(configPath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config file: {ex.Message}");
                return false;
            }
        }
    }
}
