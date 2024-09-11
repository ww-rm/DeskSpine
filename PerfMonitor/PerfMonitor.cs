using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Timers;

namespace PerfMonitor
{
    /// <summary>
    /// 性能数据结构体
    /// </summary>
    public struct PerfData
    {
        public float cpuUsage;
        public float memUsage;
        public float uploadBytes;
        public float downloadBytes;

        public override string ToString()
        {
            return $"{{C: {cpuUsage:f2}%, M: {memUsage:f2}%, UP: {uploadBytes:n2}, DOWN: {downloadBytes:n2}}}";
        }
    }

    public partial class PerfMonitor : Form
    {
        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        private PerformanceCounter memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        private List<PerformanceCounter> uploadCounters = [];
        private List<PerformanceCounter> downloadCounters = [];
        private System.Timers.Timer updateTimer = new(1000);
        private Queue<PerfData> dataQueue = new();

        private Color[] gradientColors = [
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(233, 0, 255),
            Color.FromArgb(212, 0, 255),
            Color.FromArgb(190, 0, 255),
            Color.FromArgb(169, 0, 255),
            Color.FromArgb(147, 0, 255),
            Color.FromArgb(125, 0, 255),
            Color.FromArgb(104, 0, 255),
            Color.FromArgb(82, 0, 255),
            Color.FromArgb(61, 0, 255),
            Color.FromArgb(39, 0, 255),
            Color.FromArgb(17, 0, 255),
            Color.FromArgb(0, 4, 255),
            Color.FromArgb(0, 26, 255),
            Color.FromArgb(0, 48, 255),
            Color.FromArgb(0, 69, 255),
            Color.FromArgb(0, 91, 255),
            Color.FromArgb(0, 112, 255),
            Color.FromArgb(0, 134, 255),
            Color.FromArgb(0, 156, 255),
            Color.FromArgb(0, 177, 255),
            Color.FromArgb(0, 199, 255),
            Color.FromArgb(0, 220, 255),
            Color.FromArgb(0, 242, 255),
            Color.FromArgb(0, 255, 246),
            Color.FromArgb(0, 255, 225),
            Color.FromArgb(0, 255, 203),
            Color.FromArgb(0, 255, 182),
            Color.FromArgb(0, 255, 160),
            Color.FromArgb(0, 255, 138),
            Color.FromArgb(0, 255, 117),
            Color.FromArgb(0, 255, 95),
            Color.FromArgb(0, 255, 73),
            Color.FromArgb(0, 255, 52),
            Color.FromArgb(0, 255, 30),
            Color.FromArgb(0, 255, 9),
            Color.FromArgb(13, 255, 0),
            Color.FromArgb(35, 255, 0),
            Color.FromArgb(56, 255, 0),
            Color.FromArgb(78, 255, 0),
            Color.FromArgb(99, 255, 0),
            Color.FromArgb(121, 255, 0),
            Color.FromArgb(143, 255, 0),
            Color.FromArgb(164, 255, 0),
            Color.FromArgb(186, 255, 0),
            Color.FromArgb(207, 255, 0),
            Color.FromArgb(229, 255, 0),
            Color.FromArgb(251, 255, 0),
            Color.FromArgb(255, 238, 0),
            Color.FromArgb(255, 216, 0),
            Color.FromArgb(255, 194, 0),
            Color.FromArgb(255, 173, 0),
            Color.FromArgb(255, 151, 0),
            Color.FromArgb(255, 130, 0),
            Color.FromArgb(255, 108, 0),
            Color.FromArgb(255, 86, 0),
            Color.FromArgb(255, 65, 0),
            Color.FromArgb(255, 43, 0),
            Color.FromArgb(255, 22, 0),
            Color.FromArgb(255, 0, 0),
        ];
        private float gradientAngleStep;

        private Pen circlePen;
        private RectangleF circleRect;

        private Font font;
        private SolidBrush fontBrush = new(Color.Black);
        private StringFormat textFormat = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        private Pen barPen;

        public bool UseLightTheme 
        { 
            get => BackColor == Color.White; 
            set 
            {
                if (value)
                {
                    BackColor = Color.White;
                    ForeColor = Color.Black;
                    fontBrush.Color = Color.Black;
                }
                else
                {
                    BackColor = Color.Black;
                    ForeColor = Color.White;
                    fontBrush.Color = Color.White;
                }
            } 
        }

        public const int MaxPerfDataCount = 60;
        public float RowHeight { get => Size.Height / 4f; }
        public float CellPadding { get => RowHeight / 10f; }

        public PerfMonitor()
        {
            // 初始化窗体
            InitializeComponent();
            CreateHandle();

            // 初始化计数器
            foreach (var name in (new PerformanceCounterCategory("Network Interface")).GetInstanceNames())
            {
                uploadCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
                downloadCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
            }
            CollectPerfData();

            // 初始化绘制相关对象
            gradientAngleStep = 360f / gradientColors.Length;
            UseLightTheme = false;
            InitGraphicsObjects();

            // 开启定时更新
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
        }

        /// <summary>
        /// 设置绘制相关对象属性
        /// </summary>
        private void InitGraphicsObjects()
        {
            circlePen = new(Color.Transparent, RowHeight / 10);
            circleRect = new(CellPadding, CellPadding, RowHeight - CellPadding * 2, RowHeight - CellPadding * 2);
            font = new Font("Consolas", RowHeight / 6, GraphicsUnit.Pixel);
            barPen = new(Color.Transparent, (Size.Width - RowHeight - CellPadding * 2) / MaxPerfDataCount - 1);
        }

        /// <summary>
        /// 触发一次数据收集, 加入数据队列
        /// </summary>
        private void CollectPerfData()
        {
            var data = new PerfData();
            data.cpuUsage = cpuCounter.NextValue();
            data.memUsage = memCounter.NextValue();
            data.uploadBytes = uploadCounters.Sum(e => e.NextValue());
            data.downloadBytes = downloadCounters.Sum(e => e.NextValue());
            dataQueue.Enqueue(data);
            while (dataQueue.Count > MaxPerfDataCount)
                dataQueue.Dequeue();
        }

        /// <summary>
        /// 获取速度圆圈百分比
        /// </summary>
        private static float GetSpeedPercent(float bytes)
        {
            // < 1 KB/s
            if (bytes < 0x400)
                return bytes / 0x400 * 0.05f;
            // < 1 MB/s
            else if (bytes < 0x100000)
                return 0.05f + (bytes - 0x400) / (0x100000 - 0x400) * 0.45f;
            // < 128 MB/s (1000 Mbps)
            else if (bytes < 0x8000000)
                return 0.5f + (bytes - 0x100000) / (0x8000000 - 0x100000) * 0.5f;
            // >= 128 MB/s
            else
                return 1f;
            //return (float)Math.Min(Math.Log2(bytes) / 30, 1f);
        }

        /// <summary>
        /// 获取速度文本
        /// </summary>
        private static string GetSpeedText(float bytes)
        {
            string[] speedUnits = ["B/s", "KB/s", "MB/s", "GB/s"];
            int level = 0;
            while (bytes >= 1000 && level < speedUnits.Length - 1)
            {
                bytes /= 1024;
                level++;
            }
            return $"{bytes:f1}\n{speedUnits[level]}";
        }

        /// <summary>
        /// 在指定行绘制圆圈数据
        /// </summary>
        private void DrawCircleWithText(Graphics g, int row, float percent, string centerText)
        {
            circleRect.Y = RowHeight * row + CellPadding;
            int arcCount = (int)(gradientColors.Length * Math.Clamp(percent, 0, 1));
            for (int i = 0; i < arcCount; i++)
            {
                circlePen.Color = gradientColors[i];
                g.DrawArc(circlePen, circleRect, -86.5f - i * gradientAngleStep, -gradientAngleStep - 6f); // 角度有一些偏移补足
            }
            g.DrawString(centerText, font, fontBrush, circleRect, textFormat);
        }

        /// <summary>
        /// 绘制数据历史柱状图
        /// </summary>
        private void DrawHistoryBar(Graphics g, int row, float[] percents)
        {
            var barHeightStep = Math.Max((RowHeight - CellPadding * 2) / gradientColors.Length, 1f);
            var x = RowHeight + CellPadding + (barPen.Width + 1) * (MaxPerfDataCount - percents.Length);
            for (int i = 0; i < percents.Length; i++)
            {
                var percent = percents[i];
                int lineCount = (int)(gradientColors.Length * Math.Clamp(percent, 0, 1));
                var y = RowHeight * (row + 1) - CellPadding;
                for (int j = 0; j < lineCount; j++)
                {
                    barPen.Color = gradientColors[j];
                    g.DrawLine(barPen, x, y, x, y - barHeightStep);
                    y -= barHeightStep;
                }
                x += barPen.Width + 1;
            }
        }

        /// <summary>
        /// 定时收集数据并强制重绘
        /// </summary>
        private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            CollectPerfData();
            if (Visible) if (InvokeRequired) Invoke(Invalidate); else Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InitGraphicsObjects();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (dataQueue.Count > 0)
            {
                // 绘制最新时刻圆圈图
                var lastData = dataQueue.Last();
                DrawCircleWithText(g, 0, lastData.cpuUsage / 100, $"C:{lastData.cpuUsage:f0}%");
                DrawCircleWithText(g, 1, lastData.memUsage / 100, $"M:{lastData.memUsage:f0}%");
                DrawCircleWithText(g, 2, GetSpeedPercent(lastData.uploadBytes), $"{GetSpeedText(lastData.uploadBytes)}↑");
                DrawCircleWithText(g, 3, GetSpeedPercent(lastData.downloadBytes), $"{GetSpeedText(lastData.downloadBytes)}↓");

                // 绘制历史柱状图
                DrawHistoryBar(g, 0, dataQueue.Select(v => v.cpuUsage / 100).ToArray());
                DrawHistoryBar(g, 1, dataQueue.Select(v => v.memUsage / 100).ToArray());
                DrawHistoryBar(g, 2, dataQueue.Select(v => GetSpeedPercent(v.uploadBytes)).ToArray());
                DrawHistoryBar(g, 3, dataQueue.Select(v => GetSpeedPercent(v.downloadBytes)).ToArray());
            }
        }
    }
}
