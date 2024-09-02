using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeskSpine
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class SystemConfig
    {
        public bool AutuRun { get; set; } = false;
        public bool Visible { get; set; } = true;
    }

    /// <summary>
    /// 基础设置
    /// </summary>
    public class BasicConfig
    {
        public bool WallpaperMode { get; set; } = false;
        public bool MouseClickThrough { get; set; } = false;
        public float SpineScale { get; set; } = 1f;
        public bool SpineFlip { get; set; } = false;
        public byte Opacity { get; set; } = 255;
        public uint MaxFps { get; set; } = 30;
        public SpineWindow.BackgroudColor BackgroudColor { get; set; } = SpineWindow.BackgroudColor.Gray;
        public SpineWindow.SpineWindowType WindowType { get; set; } = SpineWindow.SpineWindowType.AzurLaneSD;
        public string SpineVersion { get; set; } = "3.8.x";
    }

    /// <summary>
    /// Spine 设置
    /// </summary>
    public class SpineConfig
    {
        public const int SlotCount = 10;
        private string?[] skelPaths = new string?[SlotCount];

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
            var propertyName = $"SkelPath{index}";
            var propertyInfo = typeof(SpineConfig).GetProperty(propertyName);
            return propertyInfo?.GetValue(this) as string;
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

        public Config()
        {
            // 给一个默认的路径
            SpineConfig.SkelPath0 = Path.Combine(Program.ProgramDirectory, @"res\spines\AzurLaneSD\guanghui_2.skel");
        }

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
                    Console.WriteLine($"Error reading config file: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

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
