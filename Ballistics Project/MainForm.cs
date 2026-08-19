// MainForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BallisticSimulator
{
    public class MainForm : Form
    {
        private TabControl tabControl;
        private Panel inputPanel;
        private Panel resultPanel;
        private Chart trajectoryChart;
        private DataGridView resultGrid;
        private Button runButton;
        private Button clearButton;
        private Button exportButton;
        private TextBox[] inputFields;
        private Label[] inputLabels;
        private string[] fieldNames = new string[]
        {
            "Initial Velocity (m/s):",
            "Elevation Angle (degrees):",
            "Azimuth Angle (degrees):",
            "Projectile Mass (kg):",
            "Projectile Diameter (m):",
            "Ballistic Coefficient:",
            "Temperature (K):",
            "Pressure (Pa):",
            "Relative Humidity (0-1):",
            "North Wind (m/s):",
            "East Wind (m/s):",
            "Latitude (degrees):",
            "Cannon North (m):",
            "Cannon East (m):",
            "Cannon Altitude (m):",
            "Target Altitude (m):"
        };
        private double[] defaultValues = new double[]
        {
            820.0, 45.0, 0.0, 0.043, 0.012, 0.2,
            288.15, 101325.0, 0.5, 2.0, 1.5,
            -22.9, 1000.0, 2000.0, 800.0, 800.0
        };

        public MainForm()
        {
            InitializeComponent();
            SetupInputFields();
            SetupChart();
            SetupResultGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Ballistic Trajectory Simulator - Military Grade";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(45, 45, 48)
            };

            TabPage inputTab = new TabPage("Input Parameters");
            TabPage resultsTab = new TabPage("Results");
            TabPage chartTab = new TabPage("Trajectory Visualization");

            inputPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(20)
            };

            resultPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(20)
            };

            inputTab.Controls.Add(inputPanel);
            resultsTab.Controls.Add(resultPanel);
            chartTab.Controls.Add(new Panel { Dock = DockStyle.Fill });

            tabControl.TabPages.Add(inputTab);
            tabControl.TabPages.Add(chartTab);
            tabControl.TabPages.Add(resultsTab);

            this.Controls.Add(tabControl);
        }

        private void SetupInputFields()
        {
            int yPos = 20;
            int xPos = 20;
            int fieldWidth = 250;
            int labelWidth = 180;
            int spacing = 40;
            int columns = 2;

            inputFields = new TextBox[fieldNames.Length];
            inputLabels = new Label[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; i++)
            {
                int col = i % columns;
                int row = i / columns;
                int currentX = xPos + col * (labelWidth + fieldWidth + 50);
                int currentY = yPos + row * spacing;

                Label label = new Label
                {
                    Text = fieldNames[i],
                    Location = new Point(currentX, currentY),
                    Size = new Size(labelWidth, 25),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleRight
                };
                inputPanel.Controls.Add(label);
                inputLabels[i] = label;

                TextBox textBox = new TextBox
                {
                    Location = new Point(currentX + labelWidth + 5, currentY),
                    Size = new Size(fieldWidth, 25),
                    BackColor = Color.FromArgb(60, 60, 65),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Consolas", 10),
                    Text = defaultValues[i].ToString("F2")
                };
                inputPanel.Controls.Add(textBox);
                inputFields[i] = textBox;
            }

            int buttonY = yPos + ((fieldNames.Length + columns - 1) / columns) * spacing + 30;

            runButton = new Button
            {
                Text = "RUN SIMULATION",
                Location = new Point(xPos, buttonY),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            runButton.Click += RunButton_Click;
            inputPanel.Controls.Add(runButton);

            clearButton = new Button
            {
                Text = "CLEAR",
                Location = new Point(xPos + 170, buttonY),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            clearButton.Click += ClearButton_Click;
            inputPanel.Controls.Add(clearButton);

            exportButton = new Button
            {
                Text = "EXPORT DATA",
                Location = new Point(xPos + 340, buttonY),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(50, 150, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            exportButton.Click += ExportButton_Click;
            inputPanel.Controls.Add(exportButton);

            Label statusLabel = new Label
            {
                Text = "Status: Ready",
                Location = new Point(xPos, buttonY + 55),
                Size = new Size(600, 25),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10),
                Name = "statusLabel"
            };
            inputPanel.Controls.Add(statusLabel);
        }

        private void SetupChart()
        {
            TabPage chartTab = tabControl.TabPages[1];
            trajectoryChart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 48),
                Palette = ChartColorPalette.EarthTones
            };

            ChartArea chartArea = new ChartArea
            {
                BackColor = Color.FromArgb(30, 30, 35),
                AxisX = { Title = "Range (m)", TitleForeColor = Color.White, ForeColor = Color.White, 
                          LabelStyle = { ForeColor = Color.White } },
                AxisY = { Title = "Altitude (m)", TitleForeColor = Color.White, ForeColor = Color.White,
                          LabelStyle = { ForeColor = Color.White } }
            };
            trajectoryChart.ChartAreas.Add(chartArea);

            Series trajectorySeries = new Series
            {
                Name = "Trajectory",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(255, 200, 50),
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 3,
                MarkerColor = Color.Yellow
            };
            trajectoryChart.Series.Add(trajectorySeries);

            Series groundSeries = new Series
            {
                Name = "Ground Level",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(100, 200, 100),
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            trajectoryChart.Series.Add(groundSeries);

            chartTab.Controls.Add(trajectoryChart);
        }

        private void SetupResultGrid()
        {
            resultGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(80, 80, 85),
                Font = new Font("Consolas", 9),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            resultGrid.Columns.Add("Parameter", "Parameter", 200);
            resultGrid.Columns.Add("Value", "Value", 200);
            resultGrid.Columns.Add("Unit", "Unit", 150);

            resultGrid.Rows.Add("Impact North", "", "m");
            resultGrid.Rows.Add("Impact East", "", "m");
            resultGrid.Rows.Add("Impact Altitude", "", "m");
            resultGrid.Rows.Add("Impact Velocity", "", "m/s");
            resultGrid.Rows.Add("Impact Angle", "", "degrees");
            resultGrid.Rows.Add("Time of Flight", "", "s");
            resultGrid.Rows.Add("Total Range", "", "m");
            resultGrid.Rows.Add("Maximum Altitude", "", "m");
            resultGrid.Rows.Add("Maximum Velocity", "", "m/s");
            resultGrid.Rows.Add("Final Drag Force", "", "N");

            resultPanel.Controls.Add(resultGrid);

            Label legendLabel = new Label
            {
                Text = "IMPACT PARAMETERS",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                ForeColor = Color.LightBlue,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(legendLabel);
            legendLabel.BringToFront();
        }

        private InputData GetInputData()
        {
            try
            {
                InputData data = new InputData();
                int index = 0;
                data.V0 = double.Parse(inputFields[index++].Text);
                data.Elevation = double.Parse(inputFields[index++].Text);
                data.Azimuth = double.Parse(inputFields[index++].Text);
                data.Mass = double.Parse(inputFields[index++].Text);
                data.Diameter = double.Parse(inputFields[index++].Text);
                data.BC = double.Parse(inputFields[index++].Text);
                data.Temperature = double.Parse(inputFields[index++].Text);
                data.Pressure = double.Parse(inputFields[index++].Text);
                data.Humidity = double.Parse(inputFields[index++].Text);
                data.WindNorth = double.Parse(inputFields[index++].Text);
                data.WindEast = double.Parse(inputFields[index++].Text);
                data.Latitude = double.Parse(inputFields[index++].Text);
                data.CannonNorth = double.Parse(inputFields[index++].Text);
                data.CannonEast = double.Parse(inputFields[index++].Text);
                data.CannonAltitude = double.Parse(inputFields[index++].Text);
                data.TargetAltitude = double.Parse(inputFields[index++].Text);
                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading input data: {ex.Message}", "Input Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            try
            {
                InputData input = GetInputData();
                if (input == null) return;

                UpdateStatus("Running simulation...", Color.Yellow);

                Simulation sim = new Simulation();
                TrajectoryResult result = sim.Run(input);

                if (result != null)
                {
                    DisplayResults(result);
                    UpdateChart(result);
                    UpdateStatus("Simulation completed successfully!", Color.LightGreen);
                }
                else
                {
                    UpdateStatus("Simulation failed!", Color.Red);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}", Color.Red);
                MessageBox.Show($"Simulation error: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayResults(TrajectoryResult result)
        {
            resultGrid.Rows[0].Cells[1].Value = result.ImpactNorth.ToString("F2");
            resultGrid.Rows[1].Cells[1].Value = result.ImpactEast.ToString("F2");
            resultGrid.Rows[2].Cells[1].Value = result.ImpactAltitude.ToString("F2");
            resultGrid.Rows[3].Cells[1].Value = result.ImpactVelocity.ToString("F2");
            resultGrid.Rows[4].Cells[1].Value = result.ImpactAngle.ToString("F2");
            resultGrid.Rows[5].Cells[1].Value = result.TimeOfFlight.ToString("F2");
            resultGrid.Rows[6].Cells[1].Value = result.TotalRange.ToString("F2");
            resultGrid.Rows[7].Cells[1].Value = result.MaxAltitude.ToString("F2");
            resultGrid.Rows[8].Cells[1].Value = result.MaxVelocity.ToString("F2");
            resultGrid.Rows[9].Cells[1].Value = result.FinalDragForce.ToString("F2");

            tabControl.SelectedIndex = 2;
        }

        private void UpdateChart(TrajectoryResult result)
        {
            trajectoryChart.Series["Trajectory"].Points.Clear();
            trajectoryChart.Series["GroundLevel"].Points.Clear();

            double targetAltitude = result.TargetAltitude;
            double minAlt = double.MaxValue;
            double maxAlt = double.MinValue;
            double minRange = double.MaxValue;
            double maxRange = double.MinValue;

            foreach (var point in result.Trajectory)
            {
                double range = Math.Sqrt((point.North - result.CannonNorth) * (point.North - result.CannonNorth) +
                                        (point.East - result.CannonEast) * (point.East - result.CannonEast));
                
                trajectoryChart.Series["Trajectory"].Points.AddXY(range, point.Altitude);

                if (point.Altitude < minAlt) minAlt = point.Altitude;
                if (point.Altitude > maxAlt) maxAlt = point.Altitude;
                if (range < minRange) minRange = range;
                if (range > maxRange) maxRange = range;
            }

            trajectoryChart.Series["GroundLevel"].Points.AddXY(minRange - 10, targetAltitude);
            trajectoryChart.Series["GroundLevel"].Points.AddXY(maxRange + 10, targetAltitude);

            ChartArea ca = trajectoryChart.ChartAreas[0];
            ca.AxisX.Minimum = Math.Max(0, minRange - 50);
            ca.AxisX.Maximum = maxRange + 50;
            ca.AxisY.Minimum = Math.Max(0, minAlt - 10);
            ca.AxisY.Maximum = maxAlt + 50;

            tabControl.SelectedIndex = 1;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].Text = defaultValues[i].ToString("F2");
            }
            resultGrid.Rows.Clear();
            resultGrid.Rows.Add("Impact North", "", "m");
            resultGrid.Rows.Add("Impact East", "", "m");
            resultGrid.Rows.Add("Impact Altitude", "", "m");
            resultGrid.Rows.Add("Impact Velocity", "", "m/s");
            resultGrid.Rows.Add("Impact Angle", "", "degrees");
            resultGrid.Rows.Add("Time of Flight", "", "s");
            resultGrid.Rows.Add("Total Range", "", "m");
            resultGrid.Rows.Add("Maximum Altitude", "", "m");
            resultGrid.Rows.Add("Maximum Velocity", "", "m/s");
            resultGrid.Rows.Add("Final Drag Force", "", "N");
            trajectoryChart.Series["Trajectory"].Points.Clear();
            trajectoryChart.Series["GroundLevel"].Points.Clear();
            UpdateStatus("Cleared", Color.LightGreen);
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
                sfd.DefaultExt = "csv";
                sfd.FileName = $"BallisticData_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName);
                        sw.WriteLine("Ballistic Trajectory Simulation Data");
                        sw.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        sw.WriteLine();

                        sw.WriteLine("Input Parameters:");
                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            sw.WriteLine($"{fieldNames[i]},{inputFields[i].Text}");
                        }
                        sw.WriteLine();

                        sw.WriteLine("Results:");
                        foreach (DataGridViewRow row in resultGrid.Rows)
                        {
                            if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                            {
                                sw.WriteLine($"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value}");
                            }
                        }
                        sw.WriteLine();

                        sw.WriteLine("Trajectory Data:");
                        sw.WriteLine("Time,North,East,Altitude,VelocityNorth,VelocityEast,VelocityVertical,VelocityTotal,Angle");
                        
                        if (trajectoryChart.Series["Trajectory"].Points.Count > 0)
                        {
                            foreach (var point in GetTrajectoryData())
                            {
                                sw.WriteLine($"{point.Tempo:F3},{point.Norte:F2},{point.Leste:F2}," +
                                           $"{point.Altitude:F2},{point.VelocidadeNorte:F2}," +
                                           $"{point.VelocidadeLeste:F2},{point.VelocidadeVertical:F2}," +
                                           $"{point.VelocidadeTotal:F2},{point.Angulo:F2}");
                            }
                        }

                        sw.Close();
                        MessageBox.Show($"Data exported successfully to:\n{sfd.FileName}", "Export Complete", 
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}", "Error", 
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private List<TrajectoryPoint> GetTrajectoryData()
        {
            return new List<TrajectoryPoint>(); // Placeholder - actual data would come from simulation
        }

        private void UpdateStatus(string message, Color color)
        {
            foreach (Control ctrl in inputPanel.Controls)
            {
                if (ctrl is Label && ctrl.Name == "statusLabel")
                {
                    ctrl.Text = $"Status: {message}";
                    ctrl.ForeColor = color;
                    break;
                }
            }
        }
    }
}