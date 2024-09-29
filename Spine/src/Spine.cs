using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections;
using System.Collections.ObjectModel;

namespace Spine
{
    /// <summary>
    /// SFML 混合模式
    /// </summary>
    internal static class SFMLBlendMode
    {
        public static SFML.Graphics.BlendMode Normal = new(
            SFML.Graphics.BlendMode.Factor.SrcAlpha, 
            SFML.Graphics.BlendMode.Factor.OneMinusSrcAlpha
        );
        public static SFML.Graphics.BlendMode Additive = new(
            SFML.Graphics.BlendMode.Factor.SrcAlpha,
            SFML.Graphics.BlendMode.Factor.One
        );
        public static SFML.Graphics.BlendMode Multiply = new(
            SFML.Graphics.BlendMode.Factor.DstColor,
            SFML.Graphics.BlendMode.Factor.OneMinusSrcAlpha
        );
        public static SFML.Graphics.BlendMode Screen = new(
            SFML.Graphics.BlendMode.Factor.One,
            SFML.Graphics.BlendMode.Factor.OneMinusSrcColor
        );

        public static SFML.Graphics.BlendMode NormalPma = new(
            SFML.Graphics.BlendMode.Factor.One,
            SFML.Graphics.BlendMode.Factor.OneMinusSrcAlpha
        );
        public static SFML.Graphics.BlendMode AdditivePma = new(
            SFML.Graphics.BlendMode.Factor.One,
            SFML.Graphics.BlendMode.Factor.One
        );
        public static SFML.Graphics.BlendMode MultiplyPma = new(
            SFML.Graphics.BlendMode.Factor.DstColor,
            SFML.Graphics.BlendMode.Factor.OneMinusSrcAlpha
        );
        public static SFML.Graphics.BlendMode ScreenPma = new(
            SFML.Graphics.BlendMode.Factor.One,
            SFML.Graphics.BlendMode.Factor.OneMinusSrcColor
        );
    }

    /// <summary>
    /// Spine 基类, 使用静态方法 New 来创建具体版本对象
    /// </summary>
    public abstract class Spine: SFML.Graphics.Drawable
    {
        /// <summary>
        /// 缩放最小值
        /// </summary>
        public static readonly float ScaleMin = 0.1f;

        /// <summary>
        /// 缩放最大值
        /// </summary>
        public static readonly float ScaleMax = 5f;

        /// <summary>
        /// 创建特定版本的 Spine
        /// </summary>
        public static Spine New(string version, string skelPath, string? atlasPath = null, float defaultMix = 0) 
        {
            if (version.StartsWith("3.6")) return new Spine36(skelPath, atlasPath, defaultMix);
            if (version.StartsWith("3.8")) return new Spine38(skelPath, atlasPath, defaultMix);
            throw new NotImplementedException($"Not implemented version: {version}");
        }

        /// <summary>
        /// 获取所属版本
        /// </summary>
        public string Version
        {
            get
            {
                var t = GetType();
                if (t == typeof(Spine36)) return "3.6.x";
                if (t == typeof(Spine38)) return "3.8.x";
                throw new InvalidOperationException($"Unknown Spine version {this}");
            }
        }

        /// <summary>
        /// skel 文件完整路径
        /// </summary>
        public string SkelPath { get; private set; }

        /// <summary>
        /// atlas 文件完整路径
        /// </summary>
        public string AtlasPath { get; private set; }

        /// <summary>
        /// png 文件完整路径
        /// </summary>
        public ReadOnlyCollection<string> PngPaths { get => pngPaths.AsReadOnly(); }
        protected List<string> pngPaths = [];

        /// <summary>
        /// 获取 Spine 对象纹理图集中每种颜色的数目, 不包含透明色, key 是 SFMl.Graphics.Color.ToInteger 整数, 且 A 赋值为 0
        /// </summary>
        public Dictionary<uint, uint> ColorTable
        {
            get
            {
                var colors = new Dictionary<uint, uint>();
                foreach (var p in PngPaths)
                {
                    var png = new SFML.Graphics.Image(p);
                    for (uint i = 0; i < png.Size.X; i++)
                    {
                        for (uint j = 0; j < png.Size.Y; j++)
                        {
                            var c = png.GetPixel(i, j);
                            if (c.A <= 0) continue;
                            c.A = 0;
                            var k = c.ToInteger();
                            if (colors.ContainsKey(k)) 
                                colors[k] += 1;
                            else 
                                colors[k] = 1;
                        }
                    }
                }

                return colors;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Spine(string skelPath, string? atlasPath = null)
        {
            atlasPath ??= Path.ChangeExtension(skelPath, ".atlas");
            SkelPath = Path.GetFullPath(skelPath);
            AtlasPath = Path.GetFullPath(atlasPath);

            var atlasDir = Path.GetDirectoryName(AtlasPath);
            var regex = new Regex(@".*?\.png");
            var content = File.ReadAllText(atlasPath);
            var matches = regex.Matches(content);
            foreach (Match match in matches)
                pngPaths.Add(Path.Combine(atlasDir, match.Value));
        }

        /// <summary>
        /// 缩放比例
        /// </summary>
        public abstract float Scale { get; set; }

        /// <summary>
        /// 横坐标
        /// </summary>
        public abstract float X { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        public abstract float Y { get; set; }

        /// <summary>
        /// 水平翻转
        /// </summary>
        public abstract bool FlipX { get; set; }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        public abstract bool FlipY { get; set; }

        /// <summary>
        /// 是否使用预乘Alpha
        /// </summary>
        public bool UsePremultipliedAlpha { get; set; }

        /// <summary>
        /// 包含的所有动画名称
        /// </summary>
        public ReadOnlyCollection<string> AnimationNames { get => animationNames.AsReadOnly(); }
        protected List<string> animationNames = [];

        /// <summary>
        /// 默认动画名称
        /// </summary>
        public string DefaultAnimationName { get => animationNames.Last(); }

        /// <summary>
        /// 当前动画名称
        /// </summary>
        public abstract string CurrentAnimation { get; set; }

        /// <summary>
        /// 动画队列末尾添加新动画, 会依次按序播放
        /// </summary>
        /// <param name="name">动画名称</param>
        public abstract void AddAnimation(string name);

        /// <summary>
        /// 更新内部状态
        /// </summary>
        /// <param name="delta">时间间隔</param>
        public abstract void Update(float delta);

        /// <summary>
        /// 顶点坐标缓冲区
        /// </summary>
        protected float[] worldVerticesBuffer = new float[1024];

        /// <summary>
        /// 顶点缓冲区
        /// </summary>
        protected SFML.Graphics.VertexArray vertexArray = new(SFML.Graphics.PrimitiveType.Triangles);

        /// <summary>
        /// SFML.Graphics.Drawable 接口实现
        /// </summary>
        public abstract void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states);
    }
}
