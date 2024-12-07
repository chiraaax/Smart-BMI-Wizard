using System;
using System.Drawing;
using System.Windows.Forms;

namespace BMI
{
    public partial class Form1 : Form
    {
        private int age;
        private string gender;
        

        // BMI range constants
        private const double UnderweightThreshold = 18.5;
        private const double NormalWeightThreshold = 24.9;
        private const double OverweightThreshold = 29.9;
        private const double ObesityThreshold = 40;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate and parse inputs
                if (!ValidateInputs(out int feet, out int inches, out double weight))
                {
                    return;
                }

                // Convert height to centimeters
                double totalHeightInCm = ((feet * 12) + inches) * 2.54;

                // Calculate BMI
                double bmi = weight / Math.Pow(totalHeightInCm / 100, 2);

                // Display BMI
                txtBMI.Text = bmi.ToString("F2");

                // Update Feedback and ProgressBar
                UpdateFeedback(bmi);

                // Update the chart
                UpdateBMIChart(bmi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs(out int feet, out int inches, out double weight)
        {
            feet = inches = 0;
            weight = 0;

            if (!int.TryParse(txtFeet.Text, out feet) || feet < 0)
            {
                ShowValidationError("Please enter a valid numeric value for feet.");
                return false;
            }

            if (!int.TryParse(txtInches.Text, out inches) || inches < 0 || inches >= 12)
            {
                ShowValidationError("Please enter a valid numeric value for inches (0-11).");
                return false;
            }

            if (!double.TryParse(txtWeight.Text, out weight) || weight <= 0)
            {
                ShowValidationError("Please enter a valid numeric value for weight.");
                return false;
            }

            if (!int.TryParse(txtAge.Text, out age) || age <= 0)
            {
                ShowValidationError("Please enter a valid age.");
                return false;
            }

            gender = rbMale.Checked ? "Male" : rbFemale.Checked ? "Female" : "Not specified";
            return true;
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateFeedback(double bmi)
        {
            var feedback = new (double MaxValue, string Category, Color FeedbackColor, string Tips)[]
            {
                (UnderweightThreshold, "Underweight", Color.Blue, "You are underweight. Consider increasing your calorie intake with nutrient-rich foods."),
                (NormalWeightThreshold, "Normal weight", Color.Green, "You are in the healthy weight range! Maintain a balanced diet and regular exercise."),
                (OverweightThreshold, "Overweight", Color.Orange, "You are overweight. Focus on a balanced diet and increased physical activity."),
                (ObesityThreshold, "Obesity", Color.Red, "You are in the obesity range. Consult a healthcare provider for guidance on a weight loss plan."),
                (double.MaxValue, "Extremely Obese", Color.DarkRed, "Your BMI is extremely high. Seek medical advice for potential health risks.")
            };

            foreach (var (max, category, color, tips) in feedback)
            {
                if (bmi <= max)
                {
                    lblFeedback.Text = $"-> Gender: {gender}\n-> Age: {age}\n\n{category}";
                    lblFeedback.ForeColor = color;
                    lblHealthTips.Text = tips;
                    break;
                }
            }

            progressBarBMI.Value = Math.Min((int)bmi, progressBarBMI.Maximum);
        }

        private void UpdateBMIChart(double bmi)
        {
            var chartPoints = new (string Label, double Value, Color Color)[]
            {
        ("Underweight", UnderweightThreshold, Color.Black),
        ("Normal weight", NormalWeightThreshold, Color.Green),
        ("Overweight", OverweightThreshold, Color.Orange),
        ("Obesity", ObesityThreshold, Color.Red),
        ("Your BMI", bmi, Color.Purple)
            };

            // Clear previous chart points
            chartBMI.Series["BMI Ranges"].Points.Clear();

            foreach (var (label, value, color) in chartPoints)
            {
                var pointIndex = chartBMI.Series["BMI Ranges"].Points.AddXY(label, value);
                var point = chartBMI.Series["BMI Ranges"].Points[pointIndex];
                point.Color = color; // Correctly setting the color for the point
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Reset inputs and UI elements
            txtFeet.Clear();
            txtInches.Clear();
            txtWeight.Clear();
            txtBMI.Clear();
            txtAge.Clear();
            rbMale.Checked = false;
            rbFemale.Checked = false;

            progressBarBMI.Value = 0;
            chartBMI.Series["BMI Ranges"].Points.Clear();
            lblFeedback.Text = "";
            lblHealthTips.Text = "";
        }

        private void TextChanged(object sender, EventArgs e)
        {
            // Enable or disable Calculate button based on input
            btnCalculate.Enabled = !string.IsNullOrWhiteSpace(txtFeet.Text) &&
                                   !string.IsNullOrWhiteSpace(txtInches.Text) &&
                                   !string.IsNullOrWhiteSpace(txtWeight.Text) &&
                                   !string.IsNullOrWhiteSpace(txtAge.Text);
        }

        private bool isDarkMode = false; // Tracks the current theme mode

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode; // Toggle the theme mode

            if (isDarkMode)
            {
                ApplyDarkMode();
                btnToggleTheme.Text = "Light Mode";
            }
            else
            {
                ApplyLightMode();
                btnToggleTheme.Text = "Dark Mode";
            }
        }

        private void ApplyDarkMode()
        {
            // Form background
            this.BackColor = Color.Black;

            // Textboxes and Labels
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox textBox)
                {
                    textBox.BackColor = Color.FromArgb(40, 40, 40);
                    textBox.ForeColor = Color.White;
                }
                else if (ctrl is Label label)
                {
                    label.ForeColor = Color.White;
                }
                else if (ctrl is Button button)
                {
                    button.BackColor = Color.FromArgb(60, 60, 60);
                    button.ForeColor = Color.White;
                }
                else if (ctrl is ProgressBar progressBar)
                {
                    progressBar.BackColor = Color.FromArgb(50, 50, 50);
                }
                else if (ctrl is RadioButton radioButton)
                {
                    radioButton.ForeColor = Color.White;
                }
            }

            // Update specific UI elements
            lblFeedback.ForeColor = Color.LightBlue;
            lblHealthTips.ForeColor = Color.LightGray;
            chartBMI.BackColor = Color.FromArgb(30, 30, 30);
            chartBMI.ChartAreas[0].BackColor = Color.FromArgb(40, 40, 40);
        }

        private void ApplyLightMode()
        {
            // Form background
            this.BackColor = SystemColors.Control;

            // Textboxes and Labels
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox textBox)
                {
                    textBox.BackColor = Color.White;
                    textBox.ForeColor = Color.Black;
                }
                else if (ctrl is Label label)
                {
                    label.ForeColor = Color.Black;
                }
                else if (ctrl is Button button)
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = Color.Black;
                }
                else if (ctrl is ProgressBar progressBar)
                {
                    progressBar.BackColor = SystemColors.Control;
                }
                else if (ctrl is RadioButton radioButton)
                {
                    radioButton.ForeColor = Color.Black;
                }
            }

            // Update specific UI elements
            lblFeedback.ForeColor = Color.Black;
            lblHealthTips.ForeColor = Color.Gray;
            chartBMI.BackColor = Color.White;
            chartBMI.ChartAreas[0].BackColor = Color.White;
        }

    }
}
