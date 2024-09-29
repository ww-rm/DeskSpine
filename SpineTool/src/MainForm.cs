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
            // ��ʼ��Ԥ������
            exporterFrameBuffer = new(panel_Preview.Handle);
            exporterFrameBuffer.SetFramerateLimit(30);
            exporterFrameBuffer.SetActive(false);
            FixPreviewPosition(true);

            comboBox_SpineVersion.DataSource = new BindingSource(comboBox_SpineVersion_KV, null);
            comboBox_SpineVersion.DisplayMember = "Key";
            comboBox_SpineVersion.ValueMember = "Value";
            comboBox_SpineVersion.SelectedValue = "3.8.x";

            exporterSpines[0] = Spine.Spine.New("3.8.x", @"D:\ACGN\AzurLane_Export\AzurLane_Dynamic\yueke_ger_3\yueke_ger_3.skel");
            exporterSpines[0].CurrentAnimation = "normal";
        }

        #region ������������ҳ��

        protected static Dictionary<string, string> comboBox_SpineVersion_KV = new()
        {
            { "3.6.x", "3.6.x" },
            { "3.8.x", "3.8.x" },
        };

        // ֡������
        private Mutex expoterMutex = new();
        private Spine.Spine[] exporterSpines = new Spine.Spine[10];
        private SFML.Graphics.RenderWindow exporterFrameBuffer;

        // Preview �߳�
        private Task exporterPreviewTask;
        private CancellationTokenSource exporterPreviewTaskCancelTokenSrc;
        private SFML.System.Clock expoterPreviewClock = new();

        private Point? exporterPreviewPressedPosition = null;
        private SFML.System.Vector2f? exporterPreviewViewPressedCenter = null;

        private void tabPage_Exporter_Enter(object sender, EventArgs e) { StartPreview(); }
        private void tabPage_Exporter_Leave(object sender, EventArgs e) { StopPreview(); }

        private void button_SelectSkels_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void comboBox_SpineVersion_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_SpineVersion.SelectedValue is string ver)
            {
                expoterMutex.WaitOne();
                for (int i = 0; i < exporterSpines.Length; i++)
                {
                    if (exporterSpines[i] is null)
                        continue;

                    Spine.Spine newSpine = null;
                    var skelPath = exporterSpines[i].SkelPath;
                    try { newSpine = Spine.Spine.New(ver, skelPath); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ�ܣ��汾δ�޸�\n\n{ex}", "Spine ��Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
        private void panel_PreviewContainer_SizeChanged(object sender, EventArgs e) { FixPreviewPosition(false); }

        private void button_ResetTimeline_Click(object sender, EventArgs e)
        {
            expoterMutex.WaitOne();
            for (int i = 0; i < exporterSpines.Length; i++)
            {
                if (exporterSpines[i] is null)
                    continue;

                exporterSpines[i].CurrentAnimation = exporterSpines[i].CurrentAnimation;
            }
            expoterMutex.ReleaseMutex();
        }

        private void panel_Preview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                exporterPreviewPressedPosition = e.Location;
                exporterPreviewViewPressedCenter = exporterFrameBuffer.GetView().Center;
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
                var src = exporterFrameBuffer.MapPixelToCoords(new SFML.System.Vector2i(srcClick.X, srcClick.Y));
                var dst = exporterFrameBuffer.MapPixelToCoords(new SFML.System.Vector2i(e.Location.X, e.Location.Y));
                var view = exporterFrameBuffer.GetView();
                view.Center = srcView - (dst - src);
                exporterFrameBuffer.SetView(view);
            }
        }

        private void Panel_Preview_MouseWheel(object sender, MouseEventArgs e)
        {
            var view = exporterFrameBuffer.GetView();
            view.Zoom(e.Delta < 0 ? 1.1f : 0.9f);
            var scale = numericUpDown_SizeX.Value / (decimal)view.Size.X * 100;
            if (scale < numericUpDown_PreviewScale.Minimum || scale > numericUpDown_PreviewScale.Maximum)
            {
                scale = Math.Clamp(scale, numericUpDown_PreviewScale.Minimum, numericUpDown_PreviewScale.Maximum);
                view.Size = new((float)(numericUpDown_SizeX.Value / scale * 100), -(float)(numericUpDown_SizeY.Value / scale * 100));
            }
            exporterFrameBuffer.SetView(view);
            label_PreviewSize.Text = $"�Ӵ���С��[{view.Size.X:f0}, {-view.Size.Y:f0}]";
            numericUpDown_PreviewScale.Enabled = false;
            numericUpDown_PreviewScale.Value = scale;
            numericUpDown_PreviewScale.Enabled = true;
        }

        private void numericUpDown_PreviewScale_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_PreviewScale.Enabled)
            {
                var view = exporterFrameBuffer.GetView();
                float scale = (float)numericUpDown_PreviewScale.Value;
                view.Size = new((float)numericUpDown_SizeX.Value / scale * 100, -(float)numericUpDown_SizeY.Value / scale * 100);
                exporterFrameBuffer.SetView(view);
                label_PreviewSize.Text = $"�Ӵ���С��[{view.Size.X:f0}, {-view.Size.Y:f0}]";
            }
        }

        private void button_Export_Click(object sender, EventArgs e)
        {
            FixPreviewPosition(true);
        }

        private void FixPreviewPosition(bool resetViewSize = false)
        {
            var sizeX = (float)numericUpDown_SizeX.Value;
            var sizeY = (float)numericUpDown_SizeY.Value;

            // ���������ط����仯ʱ����Ҫ�� View �Ĵ�С���óɺ�����һ�µĴ�С
            if (resetViewSize)
            {
                var view = exporterFrameBuffer.GetView();
                view.Size = new(sizeX, -sizeY);
                exporterFrameBuffer.SetView(view);
                label_PreviewSize.Text = $"�Ӵ���С��[{sizeX:f0}, {sizeY:f0}]";
                numericUpDown_PreviewScale.Enabled = false;
                numericUpDown_PreviewScale.Value = 100;
                numericUpDown_PreviewScale.Enabled = true;
            }

            // ����������Ԥ�����ڵĴ�С
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
            exporterFrameBuffer.Size = new((uint)sizeX, (uint)sizeY); // ������ʽ���� SFML �Ĵ��� Size
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

        static private void ExporterPreviewTask(MainForm self) { self.ExporterPreviewTask(); }
        private void ExporterPreviewTask()
        {
            exporterFrameBuffer.SetActive(true);
            while (!exporterPreviewTaskCancelTokenSrc.Token.IsCancellationRequested)
            {
                var delta = expoterPreviewClock.ElapsedTime.AsSeconds();
                expoterPreviewClock.Restart();

                exporterFrameBuffer.Clear(new SFML.Graphics.Color(0, 255, 0, 0));
                expoterMutex.WaitOne();
                for (int i = 0; i < exporterSpines.Length; i++)
                {
                    if (exporterSpines[i] is null)
                        continue;

                    exporterSpines[i].Update(delta);
                    exporterFrameBuffer.Draw(exporterSpines[i]);
                }
                expoterMutex.ReleaseMutex();
                exporterFrameBuffer.Display();
            }
        }

        #endregion

    }
}
