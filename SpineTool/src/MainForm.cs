using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace SpineTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 初始化预览窗口
            exporterPreviewer = new(panel_Preview.Handle);
            exporterPreviewer.SetFramerateLimit(30);
            exporterPreviewer.SetActive(false);
            FixPreviewPosition(true);

            comboBox_SpineVersion.DataSource = new BindingSource(comboBox_SpineVersion_KV, null);
            comboBox_SpineVersion.DisplayMember = "Key";
            comboBox_SpineVersion.ValueMember = "Value";
            comboBox_SpineVersion.SelectedValue = "3.8.x";

            exporterSpines[0] = Spine.Spine.New("3.8.x", @"D:\ACGN\AzurLane_Export\AzurLane_Dynamic\aerhangeersike_3\aerhangeersike_3.skel");
            exporterSpines[0].CurrentAnimation = "normal";
        }

        #region 动画导出工具页面

        protected static Dictionary<string, string> comboBox_SpineVersion_KV = new()
        {
            { "3.6.x", "3.6.x" },
            { "3.8.x", "3.8.x" },
        };

        // 帧缓冲区
        private Mutex expoterMutex = new();
        private Spine.Spine[] exporterSpines = new Spine.Spine[10];
        private SFML.Graphics.RenderWindow exporterPreviewer;

        // Preview 线程
        private Task exporterPreviewTask;
        private CancellationTokenSource exporterPreviewTaskCancelTokenSrc;
        private SFML.System.Clock expoterPreviewClock = new();

        private Point? exporterPreviewPressedPosition = null;
        private SFML.System.Vector2f? exporterPreviewViewPressedCenter = null;

        // 导出线程
        private Task exportSpineTask;
        private CancellationTokenSource exportSpineTaskCancelTokenSrc;

        private void tabPage_Exporter_Enter(object sender, EventArgs e) { StartPreview(); }
        private void tabPage_Exporter_Leave(object sender, EventArgs e) { StopPreview(); }

        #region 导出设置项

        private void textBox_SkelPath_MouseHover(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var text = textBox.Text;
            Size textSize = TextRenderer.MeasureText(text, textBox.Font);

            // 如果内容超出文本框宽度，设置 Tooltip
            if (textSize.Width > textBox.ClientSize.Width)
                toolTip1.SetToolTip(textBox, text);
            else
                toolTip1.SetToolTip(textBox, null);
        }

        private void textBox_SkelPath_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (!textBox.Enabled)
                return;

            int index = int.Parse(textBox.Name.Substring(textBox.Name.Length - 1));
            var comboBox_SelectAnime = this.Controls.Find($"comboBox_SelectAnime{index}", true).FirstOrDefault() as ComboBox;

            expoterMutex.WaitOne();
            if (string.IsNullOrEmpty(textBox.Text))
            {
                exporterSpines[index] = null;
                comboBox_SelectAnime.Items.Clear();
                comboBox_SelectAnime.Enabled = false;
            }
            else
            {
                var skelPath = textBox.Text;
                Spine.Spine newSpine = null;

                // 尝试加载
                try { newSpine = Spine.Spine.New((string)comboBox_SpineVersion.SelectedValue, skelPath); }
                catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败，资源未修改\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                // 成功加载新模型
                if (newSpine is not null)
                {
                    exporterSpines[index] = newSpine;
                    exporterSpines[index].UsePremultipliedAlpha = checkBox_UsePMA.Checked;
                    comboBox_SelectAnime.Items.Clear();
                    comboBox_SelectAnime.Items.AddRange(newSpine.AnimationNames.ToArray());
                    comboBox_SelectAnime.Enabled = false;
                    comboBox_SelectAnime.SelectedItem = newSpine.CurrentAnimation;
                    comboBox_SelectAnime.Enabled = true;
                }

                // 还原路径框内容
                textBox.Enabled = false;
                if (exporterSpines[index] is null) textBox.Text = null;
                else textBox.Text = exporterSpines[index].SkelPath;
                textBox.Enabled = true;
            }
            expoterMutex.ReleaseMutex();
        }

        private void button_SelectSkel_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Name.Substring(button.Name.Length - 1));
            var textBox = this.Controls.Find($"textBox_SkelPath{index}", true).FirstOrDefault() as TextBox;

            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }

        private void comboBox_SelectAnime_SelectedValueChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (!comboBox.Enabled)
                return;

            int index = int.Parse(comboBox.Name.Substring(comboBox.Name.Length - 1));
            expoterMutex.WaitOne();
            exporterSpines[index].CurrentAnimation = comboBox.SelectedItem as string;
            expoterMutex.ReleaseMutex();
        }

        private void comboBox_SpineVersion_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!comboBox_SpineVersion.Enabled)
                return;

            if (comboBox_SpineVersion.SelectedValue is string ver)
            {
                expoterMutex.WaitOne();
                for (int i = 0; i < exporterSpines.Length; i++)
                {
                    if (exporterSpines[i] is null)
                        continue;

                    Spine.Spine newSpine = null;
                    var skelPath = exporterSpines[i].SkelPath;
                    try
                    {
                        newSpine = Spine.Spine.New(ver, skelPath);
                    }
                    catch (Exception ex)
                    {
                        comboBox_SpineVersion.Enabled = false;
                        comboBox_SpineVersion.SelectedValue = exporterSpines[i].Version;
                        comboBox_SpineVersion.Enabled = true;
                        MessageBox.Show($"{skelPath} 加载失败，版本未修改\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (newSpine is not null) exporterSpines[i] = newSpine;
                }
                expoterMutex.ReleaseMutex();
            }
        }

        private void checkBox_UsePMA_CheckedChanged(object sender, EventArgs e)
        {
            expoterMutex.WaitOne();
            for (int i = 0; i < exporterSpines.Length; i++)
            {
                if (exporterSpines[i] is null)
                    continue;

                exporterSpines[i].UsePremultipliedAlpha = checkBox_UsePMA.Checked;
            }
            expoterMutex.ReleaseMutex();
        }

        private void numericUpDown_SizeX_ValueChanged(object sender, EventArgs e) { FixPreviewPosition(true); }
        private void numericUpDown_SizeY_ValueChanged(object sender, EventArgs e) { FixPreviewPosition(true); }

        private void label_ExportDuration_Click(object sender, EventArgs e)
        {
            numericUpDown_ExportDuration.Value = 1;
        }

        private void button_Export_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog_Export.ShowDialog() != DialogResult.OK)
                return;
            StartExportSpine(folderBrowserDialog_Export.SelectedPath);
        }

        #endregion

        #region 导出预览界面

        private void panel_PreviewContainer_SizeChanged(object sender, EventArgs e) { FixPreviewPosition(false); }

        private void panel_Preview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                exporterPreviewPressedPosition = e.Location;
                exporterPreviewViewPressedCenter = exporterPreviewer.GetView().Center;
            }
        }

        private void panel_Preview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                exporterPreviewPressedPosition = null;
                exporterPreviewViewPressedCenter = null;
            }
        }

        private void panel_Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (exporterPreviewPressedPosition is Point srcClick && exporterPreviewViewPressedCenter is SFML.System.Vector2f srcView)
            {
                var src = exporterPreviewer.MapPixelToCoords(new SFML.System.Vector2i(srcClick.X, srcClick.Y));
                var dst = exporterPreviewer.MapPixelToCoords(new SFML.System.Vector2i(e.Location.X, e.Location.Y));
                var view = exporterPreviewer.GetView();
                view.Center = srcView - (dst - src);
                exporterPreviewer.SetView(view);
            }
        }

        private void panel_Preview_MouseWheel(object sender, MouseEventArgs e)
        {
            var view = exporterPreviewer.GetView();
            view.Zoom(e.Delta < 0 ? 1.1f : 0.9f);
            var scale = numericUpDown_SizeX.Value / (decimal)view.Size.X * 100;
            if (scale < numericUpDown_PreviewScale.Minimum || scale > numericUpDown_PreviewScale.Maximum)
            {
                scale = Math.Clamp(scale, numericUpDown_PreviewScale.Minimum, numericUpDown_PreviewScale.Maximum);
                view.Size = new((float)(numericUpDown_SizeX.Value / scale * 100), -(float)(numericUpDown_SizeY.Value / scale * 100));
            }
            exporterPreviewer.SetView(view);
            label_PreviewSize.Text = $"视窗大小：[{view.Size.X:f0}, {-view.Size.Y:f0}]";
            numericUpDown_PreviewScale.Enabled = false;
            numericUpDown_PreviewScale.Value = scale;
            numericUpDown_PreviewScale.Enabled = true;
        }

        private void numericUpDown_PreviewScale_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown_PreviewScale.Enabled)
                return;

            var view = exporterPreviewer.GetView();
            float scale = (float)numericUpDown_PreviewScale.Value;
            view.Size = new((float)numericUpDown_SizeX.Value / scale * 100, -(float)numericUpDown_SizeY.Value / scale * 100);
            exporterPreviewer.SetView(view);
            label_PreviewSize.Text = $"视窗大小：[{view.Size.X:f0}, {-view.Size.Y:f0}]";
        }

        private void button_ResetTimeline_Click(object sender, EventArgs e)
        {
            expoterMutex.WaitOne();
            for (int i = 0; i < exporterSpines.Length; i++)
            {
                if (exporterSpines[i] is null)
                    continue;

                exporterSpines[i].CurrentAnimation = exporterSpines[i].CurrentAnimation;
                exporterSpines[i].Update(0);
            }
            expoterMutex.ReleaseMutex();
        }

        #endregion

        private void FixPreviewPosition(bool resetViewSize = false)
        {
            var sizeX = (float)numericUpDown_SizeX.Value;
            var sizeY = (float)numericUpDown_SizeY.Value;

            // 当长宽像素发生变化时才需要将 View 的大小重置成和像素一致的大小
            if (resetViewSize)
            {
                var view = exporterPreviewer.GetView();
                view.Size = new(sizeX, -sizeY);
                exporterPreviewer.SetView(view);
                label_PreviewSize.Text = $"视窗大小：[{sizeX:f0}, {sizeY:f0}]";
                numericUpDown_PreviewScale.Enabled = false;
                numericUpDown_PreviewScale.Value = 100;
                numericUpDown_PreviewScale.Enabled = true;
            }

            // 按比例计算预览窗口的大小
            if (sizeX > sizeY)
            {
                sizeY = panel_PreviewContainer.Width * sizeY / sizeX;
                sizeX = panel_PreviewContainer.Width;
            }
            else
            {
                sizeX = panel_PreviewContainer.Height * sizeX / sizeY;
                sizeY = panel_PreviewContainer.Height;
            }
            panel_Preview.Location = new((int)(panel_PreviewContainer.Width - sizeX) / 2, (int)(panel_PreviewContainer.Height - sizeY) / 2);
            panel_Preview.Size = new((int)sizeX, (int)sizeY);
            exporterPreviewer.Size = new((uint)sizeX, (uint)sizeY); // 必须显式设置 SFML 的窗口 Size
        }

        private void StartPreview()
        {
            if (exporterPreviewTask is not null)
                return;

            exporterPreviewTaskCancelTokenSrc = new();
            exporterPreviewTask = Task.Run(() => ExporterPreviewTask(this));
        }
        private void StopPreview()
        {
            if (exporterPreviewTask is null)
                return;

            exporterPreviewTaskCancelTokenSrc.Cancel();
            exporterPreviewTask.Wait();
            exporterPreviewTaskCancelTokenSrc = null;
            exporterPreviewTask = null;
        }

        private static void ExporterPreviewTask(MainForm self) { self.ExporterPreviewTask(); }
        private void ExporterPreviewTask()
        {
            exporterPreviewer.SetActive(true);
            while (!exporterPreviewTaskCancelTokenSrc.Token.IsCancellationRequested)
            {
                var delta = expoterPreviewClock.ElapsedTime.AsSeconds();
                expoterPreviewClock.Restart();

                exporterPreviewer.Clear(new SFML.Graphics.Color(0, 255, 0, 0));
                expoterMutex.WaitOne();
                for (int i = exporterSpines.Length - 1; i >= 0; i--)
                {
                    if (exporterSpines[i] is null)
                        continue;

                    exporterSpines[i].Update(delta);
                    exporterPreviewer.Draw(exporterSpines[i]);
                }
                expoterMutex.ReleaseMutex();
                exporterPreviewer.Display();
            }
            exporterPreviewer.SetActive(false);
        }

        private void StartExportSpine(string exportFolder)
        {
            if (exportSpineTask is not null)
            {
                MessageBox.Show("正在导出中");
                return;
            }

            exportSpineTaskCancelTokenSrc = new();
            exportSpineTask = Task.Run(() => ExportSpine(this, exportFolder));
        }
        private void StopExportSpine(object? sender, EventArgs e) { StopExportSpine(); }
        private void StopExportSpine()
        {
            if (exportSpineTask is null)
                return;

            exportSpineTaskCancelTokenSrc?.Cancel();
            exportSpineTask?.Wait();
            exportSpineTaskCancelTokenSrc = null;
            exportSpineTask = null;
        }

        private static void ExportSpine(MainForm self, string exportFolder) { self.ExportSpine(exportFolder); }
        private void ExportSpine(string exportFolder)
        {
            string spineName = null;
            expoterMutex.WaitOne();
            for (int i = 0; i < exporterSpines.Length; i++)
            {
                if (exporterSpines[i] is null) continue;
                spineName = Path.GetFileNameWithoutExtension(exporterSpines[i].SkelPath);
                break;
            }
            expoterMutex.ReleaseMutex();

            if (!string.IsNullOrEmpty(spineName))
            {
                Debug.WriteLine(exportFolder);
                Directory.CreateDirectory(exportFolder);

                // 停止动画预览并禁用面板
                StopPreview();
                tabControl_Tools.BeginInvoke(() => tabControl_Tools.Enabled = false);
                button_CancelTask.Click += StopExportSpine;
                button_CancelTask.BeginInvoke(() => button_CancelTask.Enabled = true);

                // 初始化导出需要的内容
                button_ResetTimeline_Click(this, EventArgs.Empty);
                var tex = new SFML.Graphics.RenderTexture((uint)numericUpDown_SizeX.Value, (uint)numericUpDown_SizeY.Value);
                tex.SetView(exporterPreviewer.GetView());
                var duration = (float)numericUpDown_ExportDuration.Value;
                var fps = (int)numericUpDown_Fps.Value;
                var delta = 1f / fps;
                var frameCount = 1 + (int)(duration / delta); // 零帧开始导出

                progressBar_SpineTool.BeginInvoke(() => progressBar_SpineTool.Maximum = frameCount);
                progressBar_SpineTool.BeginInvoke(() => progressBar_SpineTool.Value = 0);
                label_ExportDuration.BeginInvoke(() => label_ProgressBar.Text = $"已导出 {0}/{frameCount} 帧：");
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    tex.Clear(SFML.Graphics.Color.Transparent);

                    expoterMutex.WaitOne();
                    for (int i = exporterSpines.Length - 1; i >= 0; i--)
                    {
                        if (exporterSpines[i] is null)
                            continue;
                        tex.Draw(exporterSpines[i]);
                        exporterSpines[i].Update(delta);
                    }
                    expoterMutex.ReleaseMutex();

                    tex.Display();
                    var img = tex.Texture.CopyToImage();
                    img.SaveToFile(Path.Combine(exportFolder, $"{spineName}_{fps}_{frameIndex:d6}.png"));
                    img.Dispose();

                    progressBar_SpineTool.BeginInvoke(progressBar_SpineTool.PerformStep);
                    var count = frameIndex + 1;
                    label_ExportDuration.BeginInvoke(() => label_ProgressBar.Text = $"已导出 {count}/{frameCount} 帧：");

                    if (exportSpineTaskCancelTokenSrc.Token.IsCancellationRequested)
                        break;
                }

                // 恢复面板功能和动画预览
                button_CancelTask.BeginInvoke(() => button_CancelTask.Enabled = false);
                button_CancelTask.Click -= StopExportSpine;
                tabControl_Tools.BeginInvoke(() => tabControl_Tools.Enabled = true);
                StartPreview();
            }

            exportSpineTaskCancelTokenSrc = null;
            exportSpineTask = null;
        }

        #endregion
    }
}
