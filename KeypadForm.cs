using System;
using System.Drawing;
using System.Windows.Forms;

public partial class KeypadForm : Form
{
    private bool dragging = false;
    private Point dragCursorPoint;
    private Point dragFormPoint;

    public string EnteredValue { get; private set; } = "";

    public KeypadForm(string initialValue)
    {
        EnteredValue = initialValue;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Set form properties
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterParent;
        this.ClientSize = new Size(300, 380);
        this.BackColor = Color.DarkSlateGray;
        this.Padding = new Padding(5); // Inner border padding

        // Attach draggable events to the base form
        this.MouseDown += new MouseEventHandler(KeypadForm_MouseDown);
        this.MouseMove += new MouseEventHandler(KeypadForm_MouseMove);
        this.MouseUp += new MouseEventHandler(KeypadForm_MouseUp);

        // Outer border panel
        Panel outerBorderPanel = new Panel();
        outerBorderPanel.Dock = DockStyle.Fill;
        outerBorderPanel.BackColor = Color.DarkSlateGray;
        this.Controls.Add(outerBorderPanel);

        // Attach draggable events to the outer border panel
        outerBorderPanel.MouseDown += new MouseEventHandler(KeypadForm_MouseDown);
        outerBorderPanel.MouseMove += new MouseEventHandler(KeypadForm_MouseMove);
        outerBorderPanel.MouseUp += new MouseEventHandler(KeypadForm_MouseUp);

        // Inner border panel
        Panel innerBorderPanel = new Panel();
        innerBorderPanel.Dock = DockStyle.Fill;
        innerBorderPanel.BackColor = Color.Cyan;
        innerBorderPanel.Padding = new Padding(5); // Padding for the main content
        outerBorderPanel.Controls.Add(innerBorderPanel);

        // Attach draggable events to the inner border panel
        innerBorderPanel.MouseDown += new MouseEventHandler(KeypadForm_MouseDown);
        innerBorderPanel.MouseMove += new MouseEventHandler(KeypadForm_MouseMove);
        innerBorderPanel.MouseUp += new MouseEventHandler(KeypadForm_MouseUp);

        // Create Panel for main content
        Panel mainContentPanel = new Panel();
        mainContentPanel.Dock = DockStyle.Fill;
        mainContentPanel.BackColor = ColorTranslator.FromHtml("#0097a8");
        innerBorderPanel.Controls.Add(mainContentPanel);

        // Attach draggable events to the main content panel
        mainContentPanel.MouseDown += new MouseEventHandler(KeypadForm_MouseDown);
        mainContentPanel.MouseMove += new MouseEventHandler(KeypadForm_MouseMove);
        mainContentPanel.MouseUp += new MouseEventHandler(KeypadForm_MouseUp);

        // Create TextBox to display entered value
        TextBox displayTextBox = new TextBox();
        displayTextBox.ReadOnly = true;
        displayTextBox.Text = EnteredValue;
        displayTextBox.Size = new Size(240, 30);
        displayTextBox.Location = new Point(20, 20);
        displayTextBox.BackColor = Color.Ivory;
        displayTextBox.ForeColor = ColorTranslator.FromHtml("#0097a8");
        displayTextBox.Font = new Font("Sarasa Fixed J", 12);
        mainContentPanel.Controls.Add(displayTextBox);

        // Create buttons for digits 0-9 and other controls
        string[] buttonLabels = { "7", "8", "9", "4", "5", "6", "1", "2", "3", "0", ".", "+/-" };
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            Button button = new Button();
            button.Text = buttonLabels[i];
            button.Tag = buttonLabels[i];
            button.Size = new Size(50, 50);
            button.Location = new Point(20 + (i % 3) * 60, 60 + (i / 3) * 60);
            button.BackColor = Color.LightSlateGray;
            button.ForeColor = Color.Ivory;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#00bdd6");
            button.FlatAppearance.BorderSize = 2;
            button.Font = new Font("Sarasa Fixed J", 12);
            button.Click += (sender, e) => {
                if (button.Text == "+/-")
                {
                    if (!string.IsNullOrEmpty(EnteredValue))
                    {
                        if (EnteredValue[0] == '-')
                            EnteredValue = EnteredValue.Substring(1);
                        else
                            EnteredValue = "-" + EnteredValue;
                    }
                }
                else
                {
                    EnteredValue += button.Tag.ToString();
                }
                displayTextBox.Text = EnteredValue;
            };
            mainContentPanel.Controls.Add(button);
        }

        // Create Clear button
        Button clearButton = new Button();
        clearButton.Text = "CLR";
        clearButton.Size = new Size(60, 50);
        clearButton.Location = new Point(200, 60);
        clearButton.BackColor = Color.LightSlateGray;
        clearButton.ForeColor = Color.Ivory;
        clearButton.FlatStyle = FlatStyle.Flat;
        clearButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#00bdd6");
        clearButton.FlatAppearance.BorderSize = 2;
        clearButton.Font = new Font("Sarasa Fixed J", 12);
        clearButton.Click += (sender, e) => {
            EnteredValue = "";
            displayTextBox.Text = EnteredValue;
        };
        mainContentPanel.Controls.Add(clearButton);

        // Create OK button
        Button okButton = new Button();
        okButton.Text = "OK";
        okButton.Size = new Size(60, 110);
        okButton.Location = new Point(200, 180);
        okButton.BackColor = Color.LightSlateGray;
        okButton.ForeColor = Color.Ivory;
        okButton.FlatStyle = FlatStyle.Flat;
        okButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#00bdd6");
        okButton.FlatAppearance.BorderSize = 2;
        okButton.Font = new Font("Sarasa Fixed J", 12);
        okButton.Click += OkButton_Click;
        mainContentPanel.Controls.Add(okButton);

        // Create Cancel button
        Button cancelButton = new Button();
        cancelButton.Text = "ESC";
        cancelButton.Size = new Size(50, 50);
        cancelButton.Location = new Point(20, 300);
        cancelButton.BackColor = Color.LightSlateGray;
        cancelButton.ForeColor = Color.Ivory;
        cancelButton.FlatStyle = FlatStyle.Flat;
        cancelButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#00bdd6");
        cancelButton.FlatAppearance.BorderSize = 2;
        cancelButton.Font = new Font("Sarasa Fixed J", 12);
        cancelButton.Click += CancelButton_Click;
        mainContentPanel.Controls.Add(cancelButton);

        this.ResumeLayout(false);
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    private void KeypadForm_MouseDown(object sender, MouseEventArgs e)
    {
        dragging = true;
        dragCursorPoint = Cursor.Position;
        dragFormPoint = this.Location;
    }

    private void KeypadForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
            this.Location = Point.Add(dragFormPoint, new Size(diff));
        }
    }

    private void KeypadForm_MouseUp(object sender, MouseEventArgs e)
    {
        dragging = false;
    }
}