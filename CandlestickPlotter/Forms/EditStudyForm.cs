using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies;
using System.Reflection;

namespace CandleStickPlotter.Forms
{
    
    public partial class EditStudyForm : Form
    {
        private Study Study { get; set; }
        // Necessary to assign the default value of each ComboBox in the form, since using the SelectedValue
        // property in the Form's constructor doesn't work
        private readonly Dictionary<ComboBox, object> ComboBoxDefaultValues = [];
        public EditStudyForm(Study study)
        {
            InitializeComponent();
            Study = study;
            studyNameLabel.Text = study.Name;
            ShowStudyProperties();
            plotsGroupBox.Location = new Point(plotsGroupBox.Location.X, parametersGroupBox.Bottom);
                
            ShowStudyPlots();
            cancelButton.Location = new Point(parametersGroupBox.Right - cancelButton.Width, plotsGroupBox.Bottom);
            acceptButton.Location = new Point(cancelButton.Left - acceptButton.Width, cancelButton.Location.Y);
        }
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            string propertyName = "";
            // Set parameters
            foreach (Control control in parametersGroupBox.Controls)
            {
                if (control is Label) propertyName = control.Text.Remove(control.Text.Length - 1);
                else
                {
                    PropertyInfo propertyInfo = Study.GetType().GetProperty(propertyName) ?? 
                        throw new NullReferenceException($"Property '{propertyName}' couldn't be found.");
                    if (control is TextBox)
                        propertyInfo.SetValue(Study, Convert.ChangeType(control.Text, propertyInfo.PropertyType));
                    else if (control is ComboBox comboBox)
                        propertyInfo.SetValue(Study, comboBox.SelectedItem);
                }
            }
            // Set plot configuration
            for (int i = 0; i < plotsTabControl.Controls.Count; i++)
            {
                TabPage tabPage = (TabPage)plotsTabControl.Controls[i];
                ComboBox comboBox = (ComboBox)tabPage.Controls[1];
                if (comboBox.SelectedItem is null) throw new NullReferenceException($"Plot type wasn't set properly for '{tabPage.Text}'");
                Study.Plots[i].Type = (Plot.PlotType)comboBox.SelectedItem;
                CheckBox checkBox = (CheckBox)tabPage.Controls[2];
                Study.Plots[i].Visible = checkBox.Checked;
                for (int j = 4, k = 0; j < tabPage.Controls.Count; j += 2, k++)
                    Study.Plots[i].SubPlots[k].Color = tabPage.Controls[j].BackColor;
            }
            Close();
            DialogResult = DialogResult.OK;
        }
        private void ShowStudyPlots()
        {
            Plot[] plots = Study.Plots;
            foreach (Plot plot in plots)
            {
                // Add the tab page for the current plot
                TabPage tabPage = new()
                {
                    Padding = new Padding(10),
                    Text = plot.Name
                };
                plotsTabControl.Controls.Add(tabPage);
                // Create the controls for user input for plot type
                Label label = new()
                {
                    AutoSize = true,
                    //BackColor = Color.Cyan,
                    Location = new Point(tabPage.Padding.Left, tabPage.Padding.Top),
                    Margin = new Padding(5, 5, 5, 20),
                    MinimumSize = new Size(0, 25),
                    Text = "Draw as:",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                tabPage.Controls.Add(label);
                ComboBox comboBox = new()
                {
                    //BackColor = Color.Lime,
                    DataSource = Enum.GetValues(typeof(Plot.PlotType)),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(label.Right + label.Margin.Right, label.Bottom - 25),
                    Margin = new Padding(10, 0, 10, 0),
                    MinimumSize = new Size(0, 25)
                };
                tabPage.Controls.Add(comboBox);
                ComboBoxDefaultValues.Add(comboBox, plot.Type);
                // Controls for user input for plot visibility
                CheckBox checkBox = new()
                {
                    AutoSize = true,
                    //BackColor = Color.PeachPuff,
                    Checked = plot.Visible,
                    Location = new Point(label.Location.X, label.Bottom),
                    Margin = new Padding(5),
                    MinimumSize = new Size(0, 25),
                    Text = "Visible",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                tabPage.Controls.Add(checkBox);
                // Controls for user input for plot colors
                label = new()
                {
                    AutoSize = true,
                    //BackColor = Color.Maroon,
                    Location = new Point(tabPage.Padding.Left, tabPage.Controls[^1].Bottom),
                    Margin = new Padding(5, 5, 20, 5),
                    MinimumSize = new Size(0, 25),
                    Text = "Color:",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                tabPage.Controls.Add(label);
                foreach (SubPlot subPlot in plot.SubPlots)
                {
                    Button button = new()
                    {
                        AutoSize = false,
                        BackColor = subPlot.Color,
                        FlatStyle = FlatStyle.Flat,
                        Location = new Point(tabPage.Padding.Right, label.Bottom + label.Margin.Bottom),
                        Size = new Size(15, 15)
                    };
                    button.Click += ColorButton_Click;
                    tabPage.Controls.Add(button);
                    if (subPlot.Name != "")
                    {
                        label = new()
                        {
                            AutoSize = true,
                            //BackColor = Color.MediumOrchid,
                            Location = new Point(button.Right, button.Location.Y),
                            Margin = new Padding(5, 5, 20, 5),
                            MinimumSize = new Size(0, 25),
                            Text = subPlot.Name,
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        tabPage.Controls.Add(label);
                    }
                }
            }
        }
        private void ColorButton_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Button button = (Button)sender;
                button.BackColor = colorDialog1.Color;
            }
        }
        /// <summary>
        /// Reads the parameters from the given study and creates the necessary controls for user input.
        /// </summary>
        /// <param name="study"></param>
        private void ShowStudyProperties()
        {
            foreach (PropertyInfo property in Study.GetType().GetProperties())
            {
                // Not interested in these properties so ignore them
                if (property.Name == "Name" || property.Name == "Plots" || property.Name == "DefaultPlotLocation"
                    || property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Column<>)
                    || property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Table<>)) continue;
                // A label to indicate the parameter's name on the form
                Label label = new()
                {
                    AutoSize = false,
                    Margin = new Padding(5),
                    Size = new Size(parametersGroupBox.Width - parametersGroupBox.Padding.Horizontal, 30),
                    Text = property.Name + ":",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                if (parametersGroupBox.Controls.Count == 0)
                    label.Location = new Point(parametersGroupBox.Padding.Left, parametersGroupBox.Padding.Top);
                else
                    label.Location = new Point(parametersGroupBox.Padding.Left, parametersGroupBox.Controls[^1].Bottom);
                label.Width = parametersGroupBox.Width - parametersGroupBox.Padding.Horizontal - label.Margin.Horizontal;
                parametersGroupBox.Controls.Add(label);
                // If the parameter needs to be passed as an enum, then use it to create a ComboBox
                if (property.PropertyType.IsEnum)
                {
                    ComboBox comboBox = new()
                    {
                        DataSource = Enum.GetValues(property.PropertyType),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Location = new Point(parametersGroupBox.Padding.Left, label.Bottom),
                        Margin = new Padding(5)
                    };
                    comboBox.Width = parametersGroupBox.Width - parametersGroupBox.Padding.Horizontal - comboBox.Margin.Horizontal;
                    parametersGroupBox.Controls.Add(comboBox);
                    object? value = property.GetValue(Study);
                    if (value is not null)
                        ComboBoxDefaultValues.Add(comboBox, value);
                }
                // If the parameter is not enum, then simply use a TextBox
                else
                {
                    object? value = property.GetValue(Study);
                    TextBox textBox = new()
                    {
                        Location = new Point(parametersGroupBox.Padding.Left, label.Bottom),
                        Margin = new Padding(5),
                        Text = value is not null ? value.ToString() : "",
                        Width = parametersGroupBox.Width / 2
                    };
                    textBox.Width = parametersGroupBox.Width - parametersGroupBox.Padding.Horizontal - textBox.Margin.Horizontal;
                    parametersGroupBox.Controls.Add(textBox);
                }
            }
        }

        private void EditStudyForm_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<ComboBox, object> pair in ComboBoxDefaultValues)
            {
                ComboBox comboBox = pair.Key;
                comboBox.SelectedItem = pair.Value;
            }
            int sizeToGrow;
            // Adjust the size of the plotsGroupBox and the TabControl
            foreach (TabPage tabPage in plotsTabControl.Controls)
            {
                sizeToGrow = Math.Max(tabPage.Controls[^1].Bottom + tabPage.Padding.Bottom, tabPage.Height) - tabPage.Height;
                Height += sizeToGrow;
                plotsGroupBox.Height += sizeToGrow;
                plotsTabControl.Height += sizeToGrow;
                tabPage.Height += sizeToGrow;
            }
            cancelButton.Location = new Point(plotsGroupBox.Right - cancelButton.Width, plotsGroupBox.Bottom + cancelButton.Margin.Top);
            acceptButton.Location = new Point(cancelButton.Left - acceptButton.Margin.Right - acceptButton.Width, cancelButton.Location.Y);
            sizeToGrow = Math.Max(cancelButton.Bottom + Padding.Bottom + cancelButton.Margin.Bottom, Height) - Height;
            Height += sizeToGrow;
        }
    }
}