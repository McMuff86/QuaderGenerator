using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Rhino;
using Rhino.Geometry;
using Rhino.UI;
using QuaderGenerator.Core;

namespace QuaderGenerator.UI
{
    /// <summary>
    /// Panel for generating Quader (box) geometry in Rhino
    /// </summary>
    [System.Runtime.InteropServices.Guid("A1B2C3D4-E5F6-4A5B-8C7D-9E0F1A2B3C4D")]
    public class QuaderPanel : Panel, IPanel
    {
        public static Guid PanelId => typeof(QuaderPanel).GUID;

        // Unit selection
        private RadioButtonList _unitSelector;
        private UnitConverter.Unit _currentUnit = UnitConverter.Unit.Meters;
        
        // Mode selection
        private RadioButtonList _modeSelector;
        
        // Input fields
        private TextBox _lengthTextBox;
        private TextBox _widthTextBox;
        private TextBox _heightTextBox;
        private TextBox _volumeTextBox;
        private TextBox _surfaceAreaTextBox;
        
        // Labels
        private Label _lengthLabel;
        private Label _widthLabel;
        private Label _heightLabel;
        private Label _volumeLabel;
        private Label _surfaceAreaLabel;
        private Label _infoLabel;
        
        // UI elements
        private Button _generateButton;
        private Label _statusLabel;
        
        // Dynamic layout container
        private Panel _inputPanel;
        private DropDown _presetDropDown;
        private Button _savePresetButton;
        private Button _deletePresetButton;
        private TextBox _presetNameTextBox;
        
        // Object info display
        private Label _objectInfoLabel;
        private Button _refreshInfoButton;
        private Button _generateCentroidButton;
        private Scrollable _objectInfoScrollable;
        private Rhino.DocObjects.RhinoObject _currentSelectedObject;

        public QuaderPanel()
        {
            InitializeComponents();
            LayoutComponents();
        }

        private void InitializeComponents()
        {
            // Unit selector
            _unitSelector = new RadioButtonList
            {
                Orientation = Orientation.Horizontal,
                Items = 
                {
                    new ListItem { Text = "mm", Key = "mm" },
                    new ListItem { Text = "cm", Key = "cm" },
                    new ListItem { Text = "m", Key = "m" }
                },
                SelectedIndex = 2 // Default to meters
            };
            _unitSelector.SelectedIndexChanged += OnUnitChanged;
            
            // Mode selector
            _modeSelector = new RadioButtonList
            {
                Orientation = Orientation.Vertical,
                Items = 
                {
                    new ListItem { Text = "Direct Dimensions", Key = "dimensions" },
                    new ListItem { Text = "From Volume", Key = "volume" },
                    new ListItem { Text = "From Surface Area", Key = "surface" }
                },
                SelectedIndex = 0
            };
            _modeSelector.SelectedIndexChanged += OnModeChanged;

            // Input fields with Enter key support for generation
            _lengthTextBox = new TextBox { PlaceholderText = "1.0" };
            _widthTextBox = new TextBox { PlaceholderText = "1.0" };
            _heightTextBox = new TextBox { PlaceholderText = "1.0" };
            _volumeTextBox = new TextBox { PlaceholderText = "1.0" };
            _surfaceAreaTextBox = new TextBox { PlaceholderText = "6.0" };

            // Add Enter key handler for generation
            Action<TextBox> generateOnEnter = (textBox) =>
            {
                textBox.KeyDown += (sender, e) =>
                {
                    if (e.Key == Keys.Enter)
                    {
                        OnGenerateButtonClick(sender, e);
                    }
                };
            };

            generateOnEnter(_lengthTextBox);
            generateOnEnter(_widthTextBox);
            generateOnEnter(_heightTextBox);
            generateOnEnter(_volumeTextBox);
            generateOnEnter(_surfaceAreaTextBox);

            // Labels
            _lengthLabel = new Label { Text = "Length (X):" };
            _widthLabel = new Label { Text = "Width (Y):" };
            _heightLabel = new Label { Text = "Height (Z):" };
            _volumeLabel = new Label { Text = "Volume:" };
            _surfaceAreaLabel = new Label { Text = "Surface Area:" };
            _infoLabel = new Label { Text = "Enter values in selected unit", TextColor = Colors.Gray };

            // Preset controls
            _presetDropDown = new DropDown();
            _presetDropDown.SelectedIndexChanged += OnPresetSelected;
            _savePresetButton = new Button { Text = "Save", Width = 60 };
            _deletePresetButton = new Button { Text = "Delete", Width = 60 };
            _presetNameTextBox = new TextBox { PlaceholderText = "Preset name" };
            
            _savePresetButton.Click += OnSavePreset;
            _deletePresetButton.Click += OnDeletePreset;

            // Buttons and status
            _generateButton = new Button { Text = "Generate Quader" };
            _statusLabel = new Label { Text = "💡 Tip: Press Enter in any input field to generate", TextColor = Colors.Gray };
            _generateButton.Click += OnGenerateButtonClick;
            
            // Object info
            _objectInfoLabel = new Label 
            { 
                Text = "No object selected",
                TextColor = Colors.Gray,
                Wrap = WrapMode.Word
            };
            
            // Make label scrollable
            _objectInfoScrollable = new Scrollable
            {
                Content = _objectInfoLabel,
                Border = BorderType.Line,
                Size = new Size(-1, -1) // Auto height, auto width - let it expand as needed
            };
            
            _refreshInfoButton = new Button { Text = "Refresh Info" };
            _refreshInfoButton.Click += OnRefreshInfo;
            
            _generateCentroidButton = new Button { Text = "Generate Centroid", Enabled = false };
            _generateCentroidButton.Click += OnGenerateCentroid;
            
            // Load presets
            LoadPresets();
        }

        private void LayoutComponents()
        {
            var mainLayout = new DynamicLayout { Padding = 10, Spacing = new Size(5, 5) };

            // Header
            mainLayout.AddRow(new Label { Text = "Quader Generator", Font = new Font(SystemFont.Bold, 14) });
            mainLayout.AddSpace();

            // Presets section
            var presetGroup = new GroupBox { Text = "Presets" };
            var presetLayout = new DynamicLayout { Padding = 5, Spacing = new Size(5, 5) };
            presetLayout.AddRow(_presetDropDown);
            var buttonLayout = new DynamicLayout { Spacing = new Size(5, 5) };
            buttonLayout.BeginHorizontal();
            buttonLayout.Add(_savePresetButton);
            buttonLayout.Add(_deletePresetButton);
            buttonLayout.EndHorizontal();
            presetLayout.AddRow(buttonLayout);
            presetLayout.AddRow(_presetNameTextBox);
            presetGroup.Content = presetLayout;
            mainLayout.AddRow(presetGroup);
            mainLayout.AddSpace();

            // Units section
            var unitGroup = new GroupBox { Text = "Units" };
            var unitLayout = new DynamicLayout { Padding = 5, Spacing = new Size(5, 5) };
            unitLayout.AddRow(_unitSelector);
            unitGroup.Content = unitLayout;
            mainLayout.AddRow(unitGroup);
            mainLayout.AddSpace();

            // Generation Mode section
            var modeGroup = new GroupBox { Text = "Generation Mode" };
            var modeLayout = new DynamicLayout { Padding = 5, Spacing = new Size(5, 5) };
            modeLayout.AddRow(_modeSelector);
            modeGroup.Content = modeLayout;
            mainLayout.AddRow(modeGroup);
            mainLayout.AddSpace();

            // Input area (will be dynamically updated)
            var inputGroup = new GroupBox { Text = "Parameters" };
            _inputPanel = new Panel();
            inputGroup.Content = _inputPanel;
            mainLayout.AddRow(inputGroup);
            
            // Generate button
            mainLayout.AddSpace();
            mainLayout.AddRow(_generateButton);
            mainLayout.AddSpace();
            mainLayout.AddRow(_statusLabel);
            
            // Object Info section
            mainLayout.AddSpace();
            var infoGroup = new GroupBox { Text = "Selected Object Info" };
            var infoLayout = new DynamicLayout { Padding = 5, Spacing = new Size(5, 5) };

            // Add scrollable with expanded row to allow it to grow
            infoLayout.AddRow(_objectInfoScrollable);
            infoLayout.AddRow(null); // This will allow the scrollable to expand

            // Button row
            var infoButtonLayout = new DynamicLayout { Spacing = new Size(5, 5) };
            infoButtonLayout.BeginHorizontal();
            infoButtonLayout.Add(_refreshInfoButton);
            infoButtonLayout.Add(_generateCentroidButton);
            infoButtonLayout.EndHorizontal();
            infoLayout.AddRow(infoButtonLayout);

            infoGroup.Content = infoLayout;
            mainLayout.AddRow(infoGroup);

            mainLayout.Add(null); // Stretch remaining space

            // Make entire panel scrollable
            var scrollablePanel = new Scrollable
            {
                Content = mainLayout,
                Border = BorderType.None
            };

            Content = scrollablePanel;
            
            // Initialize with dimensions mode
            UpdateInputFields();
            
            // Update object info on load
            UpdateObjectInfo();
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            UpdateInputFields();
            _statusLabel.Text = "✓ Mode changed - enter values";
            _statusLabel.TextColor = Colors.Green;
        }

        private void OnUnitChanged(object sender, EventArgs e)
        {
            // Update current unit based on selection
            switch (_unitSelector.SelectedKey)
            {
                case "mm":
                    _currentUnit = UnitConverter.Unit.Millimeters;
                    break;
                case "cm":
                    _currentUnit = UnitConverter.Unit.Centimeters;
                    break;
                case "m":
                    _currentUnit = UnitConverter.Unit.Meters;
                    break;
            }

            UpdateInputFields();
            _statusLabel.Text = $"✓ Units changed to {UnitConverter.GetUnitSymbol(_currentUnit)}";
            _statusLabel.TextColor = Colors.Green;
        }

        private void UpdateInputFields()
        {
            var inputLayout = new DynamicLayout { Spacing = new Size(5, 5) };
            
            var selectedMode = _modeSelector.SelectedKey;
            var unitSymbol = UnitConverter.GetUnitSymbol(_currentUnit);
            var volumeSymbol = UnitConverter.GetVolumeUnitSymbol(_currentUnit);
            var surfaceSymbol = UnitConverter.GetSurfaceAreaUnitSymbol(_currentUnit);

            switch (selectedMode)
            {
                case "dimensions":
                    _lengthLabel.Text = $"Length (X) [{unitSymbol}]:";
                    _widthLabel.Text = $"Width (Y) [{unitSymbol}]:";
                    _heightLabel.Text = $"Height (Z) [{unitSymbol}]:";
                    
                    inputLayout.AddRow(_lengthLabel);
                    inputLayout.AddRow(_lengthTextBox);
                    inputLayout.AddRow(_widthLabel);
                    inputLayout.AddRow(_widthTextBox);
                    inputLayout.AddRow(_heightLabel);
                    inputLayout.AddRow(_heightTextBox);
                    _infoLabel.Text = "Enter all three dimensions";
                    break;

                case "volume":
                    _volumeLabel.Text = $"Volume [{volumeSymbol}]:";
                    _lengthLabel.Text = $"Length (X) [{unitSymbol}]:";
                    _widthLabel.Text = $"Width (Y) [{unitSymbol}]:";
                    
                    inputLayout.AddRow(_volumeLabel);
                    inputLayout.AddRow(_volumeTextBox);
                    inputLayout.AddSpace();
                    inputLayout.AddRow(_lengthLabel);
                    inputLayout.AddRow(_lengthTextBox);
                    inputLayout.AddRow(_widthLabel);
                    inputLayout.AddRow(_widthTextBox);
                    _infoLabel.Text = "Height will be calculated from volume";
                    break;

                case "surface":
                    _surfaceAreaLabel.Text = $"Surface Area [{surfaceSymbol}]:";
                    _lengthLabel.Text = $"Length (X) [{unitSymbol}]:";
                    _widthLabel.Text = $"Width (Y) [{unitSymbol}]:";
                    
                    inputLayout.AddRow(_surfaceAreaLabel);
                    inputLayout.AddRow(_surfaceAreaTextBox);
                    inputLayout.AddSpace();
                    inputLayout.AddRow(_lengthLabel);
                    inputLayout.AddRow(_lengthTextBox);
                    inputLayout.AddRow(_widthLabel);
                    inputLayout.AddRow(_widthTextBox);
                    _infoLabel.Text = "Height will be calculated from surface area";
                    break;
            }
            
            inputLayout.AddSpace();
            inputLayout.AddRow(_infoLabel);
            
            _inputPanel.Content = inputLayout;
        }

        private void OnGenerateButtonClick(object sender, EventArgs e)
        {
            try
            {
                double length, width, height;
                var selectedMode = _modeSelector.SelectedKey;

                switch (selectedMode)
                {
                    case "dimensions":
                        if (!TryParseDimensions(out length, out width, out height))
                            return;
                        break;

                    case "volume":
                        if (!TryCalculateFromVolume(out length, out width, out height))
                            return;
                        break;

                    case "surface":
                        if (!TryCalculateFromSurfaceArea(out length, out width, out height))
                            return;
                        break;

                    default:
                        _statusLabel.Text = "Error: Invalid mode selected";
                        return;
                }

                // Create and add the Quader
                CreateQuader(length, width, height);
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                RhinoApp.WriteLine($"ERROR: Quader generation failed: {ex.Message}");
            }
        }

        private bool TryParseDimensions(out double length, out double width, out double height)
        {
            length = width = height = 0;

            if (!double.TryParse(_lengthTextBox.Text, out double lengthInput) || lengthInput <= 0)
            {
                _statusLabel.Text = "Error: Length must be a positive number";
                return false;
            }

            if (!double.TryParse(_widthTextBox.Text, out double widthInput) || widthInput <= 0)
            {
                _statusLabel.Text = "Error: Width must be a positive number";
                return false;
            }

            if (!double.TryParse(_heightTextBox.Text, out double heightInput) || heightInput <= 0)
            {
                _statusLabel.Text = "Error: Height must be a positive number";
                return false;
            }

            // Convert to millimeters (Rhino's internal unit)
            length = UnitConverter.ToMillimeters(lengthInput, _currentUnit);
            width = UnitConverter.ToMillimeters(widthInput, _currentUnit);
            height = UnitConverter.ToMillimeters(heightInput, _currentUnit);

            return true;
        }

        private bool TryCalculateFromVolume(out double length, out double width, out double height)
        {
            length = width = height = 0;

            if (!double.TryParse(_volumeTextBox.Text, out double volumeInput) || volumeInput <= 0)
            {
                _statusLabel.Text = "Error: Volume must be a positive number";
                return false;
            }

            if (!double.TryParse(_lengthTextBox.Text, out double lengthInput) || lengthInput <= 0)
            {
                _statusLabel.Text = "Error: Length must be a positive number";
                return false;
            }

            if (!double.TryParse(_widthTextBox.Text, out double widthInput) || widthInput <= 0)
            {
                _statusLabel.Text = "Error: Width must be a positive number";
                return false;
            }

            // Convert inputs to millimeters
            double volumeMm3 = UnitConverter.VolumeToMillimeters(volumeInput, _currentUnit);
            length = UnitConverter.ToMillimeters(lengthInput, _currentUnit);
            width = UnitConverter.ToMillimeters(widthInput, _currentUnit);

            try
            {
                height = QuaderCalculator.CalculateDimensionFromVolume(volumeMm3, length, width);
                RhinoApp.WriteLine($"Input: Volume={volumeInput} {UnitConverter.GetVolumeUnitSymbol(_currentUnit)}, " +
                                 $"Length={lengthInput} {UnitConverter.GetUnitSymbol(_currentUnit)}, " +
                                 $"Width={widthInput} {UnitConverter.GetUnitSymbol(_currentUnit)}");
                RhinoApp.WriteLine($"Calculated height: {height:F3} mm = {UnitConverter.FromMillimeters(height, _currentUnit):F3} {UnitConverter.GetUnitSymbol(_currentUnit)}");
            }
            catch (ArgumentException ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                return false;
            }

            return true;
        }

        private bool TryCalculateFromSurfaceArea(out double length, out double width, out double height)
        {
            length = width = height = 0;

            if (!double.TryParse(_surfaceAreaTextBox.Text, out double surfaceAreaInput) || surfaceAreaInput <= 0)
            {
                _statusLabel.Text = "Error: Surface area must be a positive number";
                return false;
            }

            if (!double.TryParse(_lengthTextBox.Text, out double lengthInput) || lengthInput <= 0)
            {
                _statusLabel.Text = "Error: Length must be a positive number";
                return false;
            }

            if (!double.TryParse(_widthTextBox.Text, out double widthInput) || widthInput <= 0)
            {
                _statusLabel.Text = "Error: Width must be a positive number";
                return false;
            }

            // Convert inputs to millimeters
            double surfaceAreaMm2 = UnitConverter.SurfaceAreaToMillimeters(surfaceAreaInput, _currentUnit);
            length = UnitConverter.ToMillimeters(lengthInput, _currentUnit);
            width = UnitConverter.ToMillimeters(widthInput, _currentUnit);

            try
            {
                height = QuaderCalculator.CalculateDimensionFromSurfaceArea(surfaceAreaMm2, length, width);
                RhinoApp.WriteLine($"Input: Surface Area={surfaceAreaInput} {UnitConverter.GetSurfaceAreaUnitSymbol(_currentUnit)}, " +
                                 $"Length={lengthInput} {UnitConverter.GetUnitSymbol(_currentUnit)}, " +
                                 $"Width={widthInput} {UnitConverter.GetUnitSymbol(_currentUnit)}");
                RhinoApp.WriteLine($"Calculated height: {height:F3} mm = {UnitConverter.FromMillimeters(height, _currentUnit):F3} {UnitConverter.GetUnitSymbol(_currentUnit)}");
            }
            catch (ArgumentException ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                return false;
            }

            return true;
        }

        private void CreateQuader(double length, double width, double height)
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null)
            {
                _statusLabel.Text = "Error: No active Rhino document";
                return;
            }

            // Create box at origin
            var plane = Plane.WorldXY;
            var interval_x = new Interval(0, length);
            var interval_y = new Interval(0, width);
            var interval_z = new Interval(0, height);
            var box = new Box(plane, interval_x, interval_y, interval_z);

            // Add to document
            var undoRecord = doc.BeginUndoRecord("Add Quader");
            var brep = box.ToBrep();
            var objId = doc.Objects.AddBrep(brep);
            doc.EndUndoRecord(undoRecord);

            if (objId != Guid.Empty)
            {
                doc.Views.Redraw();
                
                // Calculate and display volume and surface area in both mm and selected unit
                var volumeMm3 = QuaderCalculator.CalculateVolume(length, width, height);
                var surfaceAreaMm2 = QuaderCalculator.CalculateSurfaceArea(length, width, height);
                
                // Convert to display unit
                var lengthDisplay = UnitConverter.FromMillimeters(length, _currentUnit);
                var widthDisplay = UnitConverter.FromMillimeters(width, _currentUnit);
                var heightDisplay = UnitConverter.FromMillimeters(height, _currentUnit);
                
                var unitSymbol = UnitConverter.GetUnitSymbol(_currentUnit);
                _statusLabel.Text = $"✓ Quader: {lengthDisplay:F2} × {widthDisplay:F2} × {heightDisplay:F2} {unitSymbol}";
                
                // Convert to display units
                var volumeDisplay = UnitConverter.ConvertVolumeFromMillimeters(volumeMm3, _currentUnit);
                var surfaceDisplay = UnitConverter.ConvertSurfaceAreaFromMillimeters(surfaceAreaMm2, _currentUnit);
                
                RhinoApp.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                RhinoApp.WriteLine($"Quader created:");
                RhinoApp.WriteLine($"  Internal (mm): {length:F3} × {width:F3} × {height:F3}");
                RhinoApp.WriteLine($"  Display ({unitSymbol}): {lengthDisplay:F3} × {widthDisplay:F3} × {heightDisplay:F3}");
                RhinoApp.WriteLine($"  Volume (mm³): {volumeMm3:F3}");
                RhinoApp.WriteLine($"  Volume ({unitSymbol}³): {volumeDisplay:F6}");
                RhinoApp.WriteLine($"  Surface (mm²): {surfaceAreaMm2:F3}");
                RhinoApp.WriteLine($"  Surface ({unitSymbol}²): {surfaceDisplay:F6}");
                RhinoApp.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            }
            else
            {
                _statusLabel.Text = "Error: Failed to add Quader to document";
            }
        }

        #region Preset Management

        private void LoadPresets()
        {
            try
            {
                var presets = PresetManager.LoadPresets();
                _presetDropDown.Items.Clear();
                _presetDropDown.Items.Add(new ListItem { Text = "-- Select Preset --", Key = "" });
                
                foreach (var preset in presets)
                {
                    _presetDropDown.Items.Add(new ListItem { Text = preset.Name, Key = preset.Name });
                }
                
                _presetDropDown.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ERROR: Failed to load presets: {ex.Message}");
            }
        }

        private void OnPresetSelected(object sender, EventArgs e)
        {
            try
            {
                if (_presetDropDown.SelectedIndex <= 0)
                    return;

                var presetName = _presetDropDown.SelectedKey;
                var presets = PresetManager.LoadPresets();
                var preset = presets.FirstOrDefault(p => p.Name == presetName);

                if (preset == null)
                {
                    _statusLabel.Text = "Error: Preset not found";
                    return;
                }

                // Set unit first
                switch (preset.Unit?.ToLower())
                {
                    case "mm":
                        _unitSelector.SelectedIndex = 0;
                        _currentUnit = UnitConverter.Unit.Millimeters;
                        break;
                    case "cm":
                        _unitSelector.SelectedIndex = 1;
                        _currentUnit = UnitConverter.Unit.Centimeters;
                        break;
                    case "m":
                    default:
                        _unitSelector.SelectedIndex = 2;
                        _currentUnit = UnitConverter.Unit.Meters;
                        break;
                }

                // Set mode
                switch (preset.Mode)
                {
                    case "dimensions":
                        _modeSelector.SelectedIndex = 0;
                        _lengthTextBox.Text = preset.Length.ToString();
                        _widthTextBox.Text = preset.Width.ToString();
                        _heightTextBox.Text = preset.Height.ToString();
                        break;
                    case "volume":
                        _modeSelector.SelectedIndex = 1;
                        _volumeTextBox.Text = preset.Volume.ToString();
                        _lengthTextBox.Text = preset.Length.ToString();
                        _widthTextBox.Text = preset.Width.ToString();
                        break;
                    case "surface":
                        _modeSelector.SelectedIndex = 2;
                        _surfaceAreaTextBox.Text = preset.SurfaceArea.ToString();
                        _lengthTextBox.Text = preset.Length.ToString();
                        _widthTextBox.Text = preset.Width.ToString();
                        break;
                }

                _statusLabel.Text = $"Loaded preset: {preset.Name}";
                RhinoApp.WriteLine($"Loaded preset: {preset.Name} (Unit: {preset.Unit})");
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error loading preset: {ex.Message}";
                RhinoApp.WriteLine($"ERROR: Failed to load preset: {ex.Message}");
            }
        }

        private void OnSavePreset(object sender, EventArgs e)
        {
            try
            {
                var presetName = _presetNameTextBox.Text?.Trim();
                
                if (string.IsNullOrEmpty(presetName))
                {
                    _statusLabel.Text = "Error: Enter a preset name";
                    return;
                }

                var selectedMode = _modeSelector.SelectedKey;
                var currentUnitString = UnitConverter.GetUnitSymbol(_currentUnit);
                QuaderPreset preset = null;

                switch (selectedMode)
                {
                    case "dimensions":
                        if (!double.TryParse(_lengthTextBox.Text, out double l) ||
                            !double.TryParse(_widthTextBox.Text, out double w) ||
                            !double.TryParse(_heightTextBox.Text, out double h))
                        {
                            _statusLabel.Text = "Error: Invalid dimensions";
                            return;
                        }
                        preset = new QuaderPreset(presetName, "dimensions", currentUnitString, l, w, h);
                        break;

                    case "volume":
                        if (!double.TryParse(_volumeTextBox.Text, out double vol) ||
                            !double.TryParse(_lengthTextBox.Text, out double l2) ||
                            !double.TryParse(_widthTextBox.Text, out double w2))
                        {
                            _statusLabel.Text = "Error: Invalid values";
                            return;
                        }
                        preset = new QuaderPreset(presetName, "volume", currentUnitString, l2, w2, 0, vol, 0);
                        break;

                    case "surface":
                        if (!double.TryParse(_surfaceAreaTextBox.Text, out double surf) ||
                            !double.TryParse(_lengthTextBox.Text, out double l3) ||
                            !double.TryParse(_widthTextBox.Text, out double w3))
                        {
                            _statusLabel.Text = "Error: Invalid values";
                            return;
                        }
                        preset = new QuaderPreset(presetName, "surface", currentUnitString, l3, w3, 0, 0, surf);
                        break;
                }

                if (preset != null && PresetManager.AddPreset(preset))
                {
                    LoadPresets();
                    _statusLabel.Text = $"✓ Saved preset: {presetName}";
                    _presetNameTextBox.Text = "";
                    RhinoApp.WriteLine($"Preset saved: {presetName} (Unit: {currentUnitString})");
                }
                else
                {
                    _statusLabel.Text = "Error: Failed to save preset";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                RhinoApp.WriteLine($"ERROR: Failed to save preset: {ex.Message}");
            }
        }

        private void OnDeletePreset(object sender, EventArgs e)
        {
            try
            {
                if (_presetDropDown.SelectedIndex <= 0)
                {
                    _statusLabel.Text = "Error: Select a preset to delete";
                    return;
                }

                var presetName = _presetDropDown.SelectedKey;
                
                var result = MessageBox.Show(
                    $"Delete preset '{presetName}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxType.Question
                );

                if (result == DialogResult.Yes)
                {
                    if (PresetManager.DeletePreset(presetName))
                    {
                        LoadPresets();
                        _statusLabel.Text = $"✓ Deleted preset: {presetName}";
                        RhinoApp.WriteLine($"Preset deleted: {presetName}");
                    }
                    else
                    {
                        _statusLabel.Text = "Error: Failed to delete preset";
                    }
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                RhinoApp.WriteLine($"ERROR: Failed to delete preset: {ex.Message}");
            }
        }

        #endregion

        #region Object Info

        private void OnRefreshInfo(object sender, EventArgs e)
        {
            UpdateObjectInfo();
        }

        private void UpdateObjectInfo()
        {
            try
            {
                var doc = RhinoDoc.ActiveDoc;
                if (doc == null)
                {
                    _objectInfoLabel.Text = "No active document";
                    _objectInfoLabel.TextColor = Colors.Gray;
                    _currentSelectedObject = null;
                    _generateCentroidButton.Enabled = false;
                    return;
                }

                var selectedObjects = doc.Objects.GetSelectedObjects(false, false).ToArray();
                
                if (selectedObjects == null || selectedObjects.Length == 0)
                {
                    _objectInfoLabel.Text = "No object selected\nSelect an object and click Refresh Info";
                    _objectInfoLabel.TextColor = Colors.Gray;
                    _currentSelectedObject = null;
                    _generateCentroidButton.Enabled = false;
                    return;
                }

                // Handle multiple selected objects
                if (selectedObjects.Length > 1)
                {
                    var infoBuilder = new System.Text.StringBuilder();
                    infoBuilder.AppendLine($"═══════════════════════════");
                    infoBuilder.AppendLine($"  {selectedObjects.Length} OBJECTS SELECTED");
                    infoBuilder.AppendLine($"═══════════════════════════");
                    infoBuilder.AppendLine();

                    for (int i = 0; i < selectedObjects.Length; i++)
                    {
                        infoBuilder.AppendLine($"┌─── OBJECT {i + 1}/{selectedObjects.Length} ───┐");
                        infoBuilder.AppendLine(GetObjectInfo(selectedObjects[i]));
                        
                        if (i < selectedObjects.Length - 1)
                        {
                            infoBuilder.AppendLine();
                            infoBuilder.AppendLine("─────────────────────────────");
                            infoBuilder.AppendLine();
                        }
                    }

                    _objectInfoLabel.Text = infoBuilder.ToString();
                    _objectInfoLabel.TextColor = Colors.Orange;
                    _currentSelectedObject = null;
                    _generateCentroidButton.Enabled = false;
                    _generateCentroidButton.Text = "Generate Centroid (Select 1)";
                    return;
                }

                // Single object selected
                var obj = selectedObjects[0];
                _currentSelectedObject = obj;
                var info = GetObjectInfo(obj);
                _objectInfoLabel.Text = info;
                _objectInfoLabel.TextColor = Colors.LightGreen;
                _generateCentroidButton.Enabled = true;
                _generateCentroidButton.Text = "Generate Centroid";
            }
            catch (Exception ex)
            {
                _objectInfoLabel.Text = $"Error: {ex.Message}";
                _objectInfoLabel.TextColor = Colors.Red;
                _currentSelectedObject = null;
                _generateCentroidButton.Enabled = false;
                RhinoApp.WriteLine($"ERROR: Failed to get object info: {ex.Message}");
            }
        }

        private string GetObjectInfo(Rhino.DocObjects.RhinoObject obj)
        {
            var info = new System.Text.StringBuilder();
            var unitSymbol = UnitConverter.GetUnitSymbol(_currentUnit);
            var doc = RhinoDoc.ActiveDoc;
            
            // ═══ BASIC INFO ═══
            info.AppendLine("═══ OBJECT INFO ═══");
            info.AppendLine($"ID: {obj.Id}");
            
            // Name
            if (!string.IsNullOrEmpty(obj.Name))
            {
                info.AppendLine($"Name: {obj.Name}");
            }
            else
            {
                info.AppendLine("Name: (none)");
            }
            
            info.AppendLine($"Type: {obj.ObjectType}");
            
            // Layer
            if (doc != null && obj.Attributes.LayerIndex >= 0 && obj.Attributes.LayerIndex < doc.Layers.Count)
            {
                var layer = doc.Layers[obj.Attributes.LayerIndex];
                info.AppendLine($"Layer: {layer.Name} (Index: {obj.Attributes.LayerIndex})");
            }
            else
            {
                info.AppendLine($"Layer: Index {obj.Attributes.LayerIndex}");
            }
            
            // Color
            if (obj.Attributes.ColorSource == Rhino.DocObjects.ObjectColorSource.ColorFromObject)
            {
                var color = obj.Attributes.ObjectColor;
                info.AppendLine($"Color: RGB({color.R},{color.G},{color.B})");
            }
            else if (obj.Attributes.ColorSource == Rhino.DocObjects.ObjectColorSource.ColorFromLayer)
            {
                info.AppendLine("Color: From Layer");
            }
            
            // Material
            if (obj.Attributes.MaterialSource == Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject)
            {
                if (doc != null && obj.Attributes.MaterialIndex >= 0 && obj.Attributes.MaterialIndex < doc.Materials.Count)
                {
                    var material = doc.Materials[obj.Attributes.MaterialIndex];
                    var matName = string.IsNullOrEmpty(material.Name) ? $"Material {obj.Attributes.MaterialIndex}" : material.Name;
                    info.AppendLine($"Material: {matName}");
                }
                else
                {
                    info.AppendLine($"Material: Index {obj.Attributes.MaterialIndex}");
                }
            }
            else if (obj.Attributes.MaterialSource == Rhino.DocObjects.ObjectMaterialSource.MaterialFromLayer)
            {
                info.AppendLine("Material: From Layer");
            }
            else
            {
                info.AppendLine("Material: (none)");
            }
            
            // Status
            info.AppendLine($"Locked: {(obj.IsLocked ? "Yes" : "No")}");
            info.AppendLine($"Hidden: {(obj.IsHidden ? "Yes" : "No")}");
            
            // Groups (with detailed information)
            var groupIndices = obj.Attributes.GetGroupList();
            if (groupIndices != null && groupIndices.Length > 0)
            {
                info.AppendLine($"Groups: {groupIndices.Length}");
                if (doc != null)
                {
                    foreach (var groupIndex in groupIndices)
                    {
                        var group = doc.Groups.FindIndex(groupIndex);
                        if (group != null)
                        {
                            var groupName = group.Name;
                            if (string.IsNullOrEmpty(groupName))
                            {
                                info.AppendLine($"  • Group {groupIndex} (Unnamed)");
                            }
                            else
                            {
                                info.AppendLine($"  • Group {groupIndex}: {groupName}");
                            }
                        }
                        else
                        {
                            info.AppendLine($"  • Group {groupIndex}");
                        }
                    }
                }
            }
            else
            {
                info.AppendLine("Groups: None");
            }
            
            info.AppendLine();

            // Get geometry
            var geom = obj.Geometry;

            // Check for Brep (Polysurface, Extrusion)
            if (geom is Rhino.Geometry.Brep brep)
            {
                info.AppendLine("═══ BREP / POLYSURFACE ═══");
                
                // Surface Area
                var areaProperties = Rhino.Geometry.AreaMassProperties.Compute(brep);
                if (areaProperties != null)
                {
                    double areaMm2 = areaProperties.Area;
                    double areaInUnit = 0;
                    
                    switch (_currentUnit)
                    {
                        case UnitConverter.Unit.Millimeters:
                            areaInUnit = areaMm2;
                            break;
                        case UnitConverter.Unit.Centimeters:
                            areaInUnit = areaMm2 / 100.0; // mm² to cm²
                            break;
                        case UnitConverter.Unit.Meters:
                            areaInUnit = areaMm2 / 1000000.0; // mm² to m²
                            break;
                    }
                    
                    info.AppendLine($"Surface Area: {areaInUnit:F3} {unitSymbol}²");
                }

                // Volume (if solid)
                if (brep.IsSolid)
                {
                    var volumeProperties = Rhino.Geometry.VolumeMassProperties.Compute(brep);
                    if (volumeProperties != null)
                    {
                        double volumeMm3 = volumeProperties.Volume;
                        double volumeInUnit = 0;
                        
                        switch (_currentUnit)
                        {
                            case UnitConverter.Unit.Millimeters:
                                volumeInUnit = volumeMm3;
                                break;
                            case UnitConverter.Unit.Centimeters:
                                volumeInUnit = volumeMm3 / 1000.0; // mm³ to cm³
                                break;
                            case UnitConverter.Unit.Meters:
                                volumeInUnit = volumeMm3 / 1000000000.0; // mm³ to m³
                                break;
                        }
                        
                        info.AppendLine($"Volume: {volumeInUnit:F6} {unitSymbol}³");
                        info.AppendLine($"Solid: Yes");
                    }
                }
                else
                {
                    info.AppendLine("Solid: No (Open surface)");
                }

                info.AppendLine($"Faces: {brep.Faces.Count}");
                info.AppendLine($"Edges: {brep.Edges.Count}");
            }
            // Check for Extrusion
            else if (geom is Rhino.Geometry.Extrusion extrusion)
            {
                info.AppendLine("═══ EXTRUSION ═══");
                
                var brepForm = extrusion.ToBrep();
                if (brepForm != null)
                {
                    // Surface Area
                    var areaProperties = Rhino.Geometry.AreaMassProperties.Compute(brepForm);
                    if (areaProperties != null)
                    {
                        double areaMm2 = areaProperties.Area;
                        double areaInUnit = areaMm2 / Math.Pow(1000.0 / UnitConverter.ToMillimeters(1, _currentUnit), 2);
                        info.AppendLine($"Surface Area: {areaInUnit:F3} {unitSymbol}²");
                    }

                    // Volume
                    if (brepForm.IsSolid)
                    {
                        var volumeProperties = Rhino.Geometry.VolumeMassProperties.Compute(brepForm);
                        if (volumeProperties != null)
                        {
                            double volumeMm3 = volumeProperties.Volume;
                            double volumeInUnit = volumeMm3 / Math.Pow(1000.0 / UnitConverter.ToMillimeters(1, _currentUnit), 3);
                            info.AppendLine($"Volume: {volumeInUnit:F6} {unitSymbol}³");
                        }
                    }
                }
            }
            // Check for Curve
            else if (geom is Rhino.Geometry.Curve curve)
            {
                info.AppendLine("═══ CURVE ═══");
                
                // Length
                double lengthMm = curve.GetLength();
                double lengthInUnit = UnitConverter.FromMillimeters(lengthMm, _currentUnit);
                info.AppendLine($"Total Length: {lengthInUnit:F3} {unitSymbol}");
                
                // Check if PolyCurve (multiple segments)
                if (curve is Rhino.Geometry.PolyCurve polyCurve)
                {
                    int segmentCount = polyCurve.SegmentCount;
                    info.AppendLine($"Segments: {segmentCount}");
                    
                    // Show individual segment lengths
                    if (segmentCount > 0 && segmentCount <= 20) // Limit to 20 to avoid too much output
                    {
                        info.AppendLine("Segment Lengths:");
                        for (int i = 0; i < segmentCount; i++)
                        {
                            var segment = polyCurve.SegmentCurve(i);
                            if (segment != null)
                            {
                                double segLengthMm = segment.GetLength();
                                double segLengthUnit = UnitConverter.FromMillimeters(segLengthMm, _currentUnit);
                                
                                // Get segment type
                                string segType = "Curve";
                                if (segment.IsLinear())
                                    segType = "Line";
                                else if (segment is Rhino.Geometry.ArcCurve)
                                    segType = "Arc";
                                else if (segment is Rhino.Geometry.NurbsCurve)
                                    segType = "NURBS";
                                
                                info.AppendLine($"  [{i + 1}] {segType}: {segLengthUnit:F3} {unitSymbol}");
                            }
                        }
                    }
                    else if (segmentCount > 20)
                    {
                        info.AppendLine($"  (Too many segments to display: {segmentCount})");
                    }
                }
                
                // Degree
                if (curve is Rhino.Geometry.NurbsCurve nurbsCurve)
                {
                    info.AppendLine($"Degree: {nurbsCurve.Degree}");
                    info.AppendLine($"Control Points: {nurbsCurve.Points.Count}");
                }
                
                // Is Closed
                info.AppendLine($"Closed: {(curve.IsClosed ? "Yes" : "No")}");
                
                // Is Planar
                if (curve.IsPlanar())
                {
                    info.AppendLine("Planar: Yes");
                }
                else
                {
                    info.AppendLine("Planar: No");
                }
                
                // Is Linear
                info.AppendLine($"Linear: {(curve.IsLinear() ? "Yes" : "No")}");
            }
            // Check for Surface
            else if (geom is Rhino.Geometry.Surface surface)
            {
                info.AppendLine("═══ SURFACE ═══");
                
                var surfaceBrep = surface.ToBrep();
                if (surfaceBrep != null)
                {
                    var areaProperties = Rhino.Geometry.AreaMassProperties.Compute(surfaceBrep);
                    if (areaProperties != null)
                    {
                        double areaMm2 = areaProperties.Area;
                        double areaInUnit = areaMm2 / Math.Pow(1000.0 / UnitConverter.ToMillimeters(1, _currentUnit), 2);
                        info.AppendLine($"Surface Area: {areaInUnit:F3} {unitSymbol}²");
                    }
                }
            }
            // Check for Mesh
            else if (geom is Rhino.Geometry.Mesh mesh)
            {
                info.AppendLine("═══ MESH ═══");
                
                // Surface Area
                var areaProperties = Rhino.Geometry.AreaMassProperties.Compute(mesh);
                if (areaProperties != null)
                {
                    double areaMm2 = areaProperties.Area;
                    double areaInUnit = areaMm2 / Math.Pow(1000.0 / UnitConverter.ToMillimeters(1, _currentUnit), 2);
                    info.AppendLine($"Surface Area: {areaInUnit:F3} {unitSymbol}²");
                }

                // Volume (if closed)
                if (mesh.IsClosed)
                {
                    var volumeProperties = Rhino.Geometry.VolumeMassProperties.Compute(mesh);
                    if (volumeProperties != null)
                    {
                        double volumeMm3 = volumeProperties.Volume;
                        double volumeInUnit = volumeMm3 / Math.Pow(1000.0 / UnitConverter.ToMillimeters(1, _currentUnit), 3);
                        info.AppendLine($"Volume: {volumeInUnit:F6} {unitSymbol}³");
                    }
                }

                info.AppendLine($"Vertices: {mesh.Vertices.Count}");
                info.AppendLine($"Faces: {mesh.Faces.Count}");
                info.AppendLine($"Closed: {(mesh.IsClosed ? "Yes" : "No")}");
            }
            else
            {
                info.AppendLine($"Geometry type not fully supported");
                info.AppendLine($"Type: {geom.GetType().Name}");
            }
            
            // ═══ BOUNDING BOX ═══
            info.AppendLine();
            info.AppendLine("═══ BOUNDING BOX ═══");
            var bbox = obj.Geometry.GetBoundingBox(true);
            if (bbox.IsValid)
            {
                var minPt = bbox.Min;
                var maxPt = bbox.Max;
                var centerPt = bbox.Center;
                
                var minX = UnitConverter.FromMillimeters(minPt.X, _currentUnit);
                var minY = UnitConverter.FromMillimeters(minPt.Y, _currentUnit);
                var minZ = UnitConverter.FromMillimeters(minPt.Z, _currentUnit);
                var maxX = UnitConverter.FromMillimeters(maxPt.X, _currentUnit);
                var maxY = UnitConverter.FromMillimeters(maxPt.Y, _currentUnit);
                var maxZ = UnitConverter.FromMillimeters(maxPt.Z, _currentUnit);
                var cenX = UnitConverter.FromMillimeters(centerPt.X, _currentUnit);
                var cenY = UnitConverter.FromMillimeters(centerPt.Y, _currentUnit);
                var cenZ = UnitConverter.FromMillimeters(centerPt.Z, _currentUnit);
                
                info.AppendLine($"Min: ({minX:F3}, {minY:F3}, {minZ:F3}) {unitSymbol}");
                info.AppendLine($"Max: ({maxX:F3}, {maxY:F3}, {maxZ:F3}) {unitSymbol}");
                info.AppendLine($"Center: ({cenX:F3}, {cenY:F3}, {cenZ:F3}) {unitSymbol}");
                
                var dx = UnitConverter.FromMillimeters(maxPt.X - minPt.X, _currentUnit);
                var dy = UnitConverter.FromMillimeters(maxPt.Y - minPt.Y, _currentUnit);
                var dz = UnitConverter.FromMillimeters(maxPt.Z - minPt.Z, _currentUnit);
                info.AppendLine($"Size: {dx:F3} × {dy:F3} × {dz:F3} {unitSymbol}");
            }
            
            // ═══ USER TEXT ═══
            var userStrings = obj.Attributes.GetUserStrings();
            if (userStrings != null && userStrings.Count > 0)
            {
                info.AppendLine();
                info.AppendLine("═══ USER TEXT ═══");
                foreach (var key in userStrings.AllKeys)
                {
                    var value = userStrings.Get(key);
                    info.AppendLine($"{key}: {value}");
                }
            }

            return info.ToString();
        }

        private void OnGenerateCentroid(object sender, EventArgs e)
        {
            try
            {
                if (_currentSelectedObject == null)
                {
                    _statusLabel.Text = "Error: No object selected";
                    return;
                }

                var doc = RhinoDoc.ActiveDoc;
                if (doc == null)
                {
                    _statusLabel.Text = "Error: No active document";
                    return;
                }

                var geom = _currentSelectedObject.Geometry;
                Rhino.Geometry.Point3d centroid = Rhino.Geometry.Point3d.Unset;
                string centroidType = "";

                // Calculate centroid based on geometry type
                if (geom is Rhino.Geometry.Brep brep)
                {
                    if (brep.IsSolid)
                    {
                        // Volume centroid for solids
                        var volumeProps = Rhino.Geometry.VolumeMassProperties.Compute(brep);
                        if (volumeProps != null)
                        {
                            centroid = volumeProps.Centroid;
                            centroidType = "Volume Centroid";
                        }
                    }
                    else
                    {
                        // Area centroid for surfaces
                        var areaProps = Rhino.Geometry.AreaMassProperties.Compute(brep);
                        if (areaProps != null)
                        {
                            centroid = areaProps.Centroid;
                            centroidType = "Surface Centroid";
                        }
                    }
                }
                else if (geom is Rhino.Geometry.Extrusion extrusion)
                {
                    var brepForm = extrusion.ToBrep();
                    if (brepForm != null && brepForm.IsSolid)
                    {
                        var volumeProps = Rhino.Geometry.VolumeMassProperties.Compute(brepForm);
                        if (volumeProps != null)
                        {
                            centroid = volumeProps.Centroid;
                            centroidType = "Volume Centroid";
                        }
                    }
                }
                else if (geom is Rhino.Geometry.Surface surface)
                {
                    var brepForm = surface.ToBrep();
                    if (brepForm != null)
                    {
                        var areaProps = Rhino.Geometry.AreaMassProperties.Compute(brepForm);
                        if (areaProps != null)
                        {
                            centroid = areaProps.Centroid;
                            centroidType = "Surface Centroid";
                        }
                    }
                }
                else if (geom is Rhino.Geometry.Curve curve)
                {
                    // Curve centroid (center of mass along curve)
                    var curveProps = Rhino.Geometry.AreaMassProperties.Compute(curve);
                    if (curveProps != null)
                    {
                        centroid = curveProps.Centroid;
                        centroidType = "Curve Centroid";
                    }
                }
                else if (geom is Rhino.Geometry.Mesh mesh)
                {
                    if (mesh.IsClosed)
                    {
                        var volumeProps = Rhino.Geometry.VolumeMassProperties.Compute(mesh);
                        if (volumeProps != null)
                        {
                            centroid = volumeProps.Centroid;
                            centroidType = "Volume Centroid";
                        }
                    }
                    else
                    {
                        var areaProps = Rhino.Geometry.AreaMassProperties.Compute(mesh);
                        if (areaProps != null)
                        {
                            centroid = areaProps.Centroid;
                            centroidType = "Surface Centroid";
                        }
                    }
                }

                // Add centroid point to document
                if (centroid.IsValid)
                {
                    var undoRecord = doc.BeginUndoRecord("Add Centroid Point");
                    var pointId = doc.Objects.AddPoint(centroid);
                    doc.EndUndoRecord(undoRecord);

                    if (pointId != Guid.Empty)
                    {
                        doc.Views.Redraw();
                        
                        var centroidDisplay = new Rhino.Geometry.Point3d(
                            UnitConverter.FromMillimeters(centroid.X, _currentUnit),
                            UnitConverter.FromMillimeters(centroid.Y, _currentUnit),
                            UnitConverter.FromMillimeters(centroid.Z, _currentUnit)
                        );
                        
                        var unitSymbol = UnitConverter.GetUnitSymbol(_currentUnit);
                        _statusLabel.Text = $"✓ {centroidType} created";
                        
                        RhinoApp.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                        RhinoApp.WriteLine($"{centroidType} generated:");
                        RhinoApp.WriteLine($"  Position (mm): ({centroid.X:F3}, {centroid.Y:F3}, {centroid.Z:F3})");
                        RhinoApp.WriteLine($"  Position ({unitSymbol}): ({centroidDisplay.X:F3}, {centroidDisplay.Y:F3}, {centroidDisplay.Z:F3})");
                        RhinoApp.WriteLine($"  Point ID: {pointId}");
                        RhinoApp.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    }
                    else
                    {
                        _statusLabel.Text = "Error: Failed to create centroid point";
                    }
                }
                else
                {
                    _statusLabel.Text = "Error: Could not calculate centroid";
                    RhinoApp.WriteLine("ERROR: Centroid calculation failed for this geometry type");
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
                RhinoApp.WriteLine($"ERROR: Failed to generate centroid: {ex.Message}");
            }
        }

        #endregion

        #region IPanel implementation
        public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Panel was shown - update object info
            UpdateObjectInfo();
        }

        public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason)
        {
            // Panel was hidden
        }

        public void PanelClosing(uint documentSerialNumber, bool onCloseDocument)
        {
            // Panel is closing
        }
        #endregion
    }
}

