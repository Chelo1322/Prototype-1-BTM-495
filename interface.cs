using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        // Table selection variables
        private int selectedTableNumber = 0;
        private Button[] tableButtons;
        private Panel tableSelectionPanel;
        private Panel orderingPanel;
        private ListView cartListView;
        private List<OrderItem> currentOrder = new List<OrderItem>();
        private Label totalLabel;

        // Order item class to manage menu items
        private class OrderItem
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        // Dictionary to store menu items
        private Dictionary<string, decimal> menuItems = new Dictionary<string, decimal>
        {
            { "Kimchi Jjigae", 12.99m },
            { "Bibimbap", 10.99m },
            { "Korean BBQ", 15.99m },
            { "Bulgogi", 13.99m },
            { "Japchae", 11.99m }
        };

        // Constructor
        public Form1()
        {
            InitializeComponent();
            SetupInitialUI();
        }

        // Initialize Components
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Basic form configuration
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "Restaurant Ordering System";
            this.Text = "Restaurant Ordering System";

            this.ResumeLayout(false);
        }

        // Setup Initial Table Selection UI
        private void SetupInitialUI()
        {
            // Clear any existing controls
            this.Controls.Clear();

            // Create table selection panel
            tableSelectionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Title for table selection
            Label tableSelectionTitle = new Label
            {
                Text = "Select Your Table",
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(250, 50),
                AutoSize = true
            };
            tableSelectionPanel.Controls.Add(tableSelectionTitle);

            // Create table buttons
            tableButtons = new Button[10];
            int rows = 2;
            int cols = 5;
            int buttonSize = 100;
            int padding = 20;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int tableNumber = i * cols + j + 1;
                    Button tableButton = new Button
                    {
                        Text = $"Table {tableNumber}",
                        Size = new Size(buttonSize, buttonSize),
                        Location = new Point(
                            100 + j * (buttonSize + padding),
                            150 + i * (buttonSize + padding)
                        ),
                        Tag = tableNumber,
                        Font = new Font("Arial", 10)
                    };

                    tableButton.Click += TableButton_Click;
                    tableSelectionPanel.Controls.Add(tableButton);
                    tableButtons[tableNumber - 1] = tableButton;
                }
            }

            // Confirm Table Selection Button
            Button confirmTableButton = new Button
            {
                Text = "Confirm Table",
                Size = new Size(200, 50),
                Location = new Point(300, 500),
                Font = new Font("Arial", 12)
            };
            confirmTableButton.Click += ConfirmTable_Click;
            tableSelectionPanel.Controls.Add(confirmTableButton);

            // Add table selection panel to form
            this.Controls.Add(tableSelectionPanel);
        }

        // Table Button Selection
        private void TableButton_Click(object sender, EventArgs e)
        {
            // Deselect all buttons
            foreach (Button btn in tableButtons)
            {
                if (btn != null)
                {
                    btn.BackColor = SystemColors.Control;
                }
            }

            // Highlight selected table
            Button clickedButton = (Button)sender;
            clickedButton.BackColor = Color.Green;
            selectedTableNumber = (int)clickedButton.Tag;
        }

        // Confirm Table Selection
        private void ConfirmTable_Click(object sender, EventArgs e)
        {
            // Validate table selection
            if (selectedTableNumber == 0)
            {
                MessageBox.Show("Please select a table first.", "Table Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Clear previous controls
            this.Controls.Clear();

            // Show main ordering interface
            ShowOrderingInterface();
        }

        // Show Ordering Interface
        private void ShowOrderingInterface()
        {
            // Create ordering panel
            orderingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Display selected table number
            Label tableInfoLabel = new Label
            {
                Text = $"Selected Table: {selectedTableNumber}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(50, 20),
                AutoSize = true
            };
            orderingPanel.Controls.Add(tableInfoLabel);

            // Create menu buttons
            int yPosition = 100;
            foreach (var item in menuItems)
            {
                Button menuButton = new Button
                {
                    Text = $"{item.Key} - ${item.Value:F2}",
                    Location = new Point(50, yPosition),
                    Size = new Size(300, 50),
                    Tag = item.Key,
                    Font = new Font("Arial", 10)
                };
                menuButton.Click += MenuButton_Click;
                orderingPanel.Controls.Add(menuButton);
                yPosition += 60;
            }

            // Create cart ListView
            cartListView = new ListView
            {
                Location = new Point(400, 100),
                Size = new Size(300, 300),
                View = View.Details,
                Font = new Font("Arial", 10)
            };
            cartListView.Columns.Add("Item", 150);
            cartListView.Columns.Add("Quantity", 75);
            cartListView.Columns.Add("Price", 75);

            // Total Label
            totalLabel = new Label
            {
                Text = "Total: $0.00",
                Location = new Point(400, 410),
                Size = new Size(300, 30),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            orderingPanel.Controls.Add(totalLabel);

            // Place Order Button
            Button placeOrderButton = new Button
            {
                Text = "Confirm Order",
                Location = new Point(400, 450),
                Size = new Size(300, 50),
                Font = new Font("Arial", 12)
            };
            placeOrderButton.Click += PlaceOrder_Click;

            orderingPanel.Controls.Add(cartListView);
            orderingPanel.Controls.Add(placeOrderButton);

            this.Controls.Add(orderingPanel);
        }

        // Menu Item Button Click
        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string itemName = clickedButton.Tag.ToString();
            decimal itemPrice = menuItems[itemName];

            // Check if item already in order
            var existingItem = currentOrder.Find(x => x.Name == itemName);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                currentOrder.Add(new OrderItem { Name = itemName, Price = itemPrice, Quantity = 1 });
            }

            UpdateCart();
        }

        // Update Cart ListView and Total
        private void UpdateCart()
        {
            cartListView.Items.Clear();
            decimal total = 0;

            foreach (var item in currentOrder)
            {
                ListViewItem listViewItem = new ListViewItem(item.Name);
                listViewItem.SubItems.Add(item.Quantity.ToString());
                listViewItem.SubItems.Add($"${item.Price * item.Quantity:F2}");
                cartListView.Items.Add(listViewItem);
                total += item.Price * item.Quantity;
            }

            totalLabel.Text = $"Total: ${total:F2}";
        }

        // Place Order Click Event
        private void PlaceOrder_Click(object sender, EventArgs e)
        {
            // Validate order
            if (currentOrder.Count == 0)
            {
                MessageBox.Show("Your cart is empty!", "Order Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm order
            DialogResult result = MessageBox.Show(
                $"Confirm order for Table {selectedTableNumber}?",
                "Order Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Process the order
                MessageBox.Show(
                    $"Order Payment Confirmed for Table {selectedTableNumber}!",
                    "Order Confirmed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Reset order process
                ResetOrderProcess();
            }
        }

        // Reset Order Process
        private void ResetOrderProcess()
        {
            selectedTableNumber = 0;
            currentOrder.Clear();
            this.Controls.Clear();
            SetupInitialUI();
        }
    }
