using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RestaurantManagementSystem
{
    // Enums
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Paid,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        MobilePayment
    }

    public enum MenuItemCategory
    {
        Appetizer,
        MainCourse,
        Dessert,
        Beverage
    }

    // Models
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MenuItemCategory Category { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice => Price * Quantity;
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<MenuItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int TableNumber { get; set; }
    }

    // Services
    public class MenuService
    {
        private List<MenuItem> _menuItems;

        public MenuService()
        {
            InitializeDefaultMenu();
        }

        private void InitializeDefaultMenu()
        {
            _menuItems = new List<MenuItem>
            {
                new MenuItem { Id = 1, Name = "Beef Udon", Price = 10.99m, Category = MenuItemCategory.MainCourse },
                new MenuItem { Id = 2, Name = "Shrimp Burger", Price = 12.99m, Category = MenuItemCategory.MainCourse },
                new MenuItem { Id = 3, Name = "Miso Soup", Price = 6.99m, Category = MenuItemCategory.Appetizer },
                new MenuItem { Id = 4, Name = "Soft Drink", Price = 2.50m, Category = MenuItemCategory.Beverage },
                new MenuItem { Id = 5, Name = "Kimchi", Price = 5.99m, Category = MenuItemCategory.Dessert }
            };
        }

        public List<MenuItem> GetMenuItems() => _menuItems;
    }

    public class OrderService
    {
        private List<Order> _orders = new List<Order>();
        private int _nextOrderId = 1;

        public Order CreateOrder(int tableNumber, List<MenuItem> items)
        {
            var order = new Order
            {
                Id = _nextOrderId++,
                OrderDate = DateTime.Now,
                Items = items.Select(item => new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Category = item.Category,
                    Quantity = item.Quantity
                }).ToList(), // Create a new list with copied items
                TableNumber = tableNumber,
                Status = OrderStatus.Pending,
                TotalAmount = items.Sum(item => item.TotalPrice)
            };

            _orders.Add(order);
            return order;
        }
    
        public List<Order> GetAllOrders() => _orders;

        public Order GetOrderById(int orderId)
        {
            return _orders.FirstOrDefault(o => o.Id == orderId);
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _orders.Where(o => o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date).ToList();
        }

        public void UpdateOrder(Order order)
        {
            var existingOrder = GetOrderById(order.Id);
            if (existingOrder != null)
            {
                _orders.Remove(existingOrder);
                _orders.Add(order);
            }
        }
    }

    // Main Form
    public class RestaurantManagementForm : Form
    {
        private MenuService _menuService;
        private OrderService _orderService;
        private List<MenuItem> _currentOrderItems;
        private int _selectedTable;

        public object BoxButtons { get; private set; }

        public RestaurantManagementForm()
        {
            InitializeComponent();
            InitializeServices();
            SetupMainInterface();
        }

        private void InitializeServices()
        {
            _menuService = new MenuService();
            _orderService = new OrderService();
            _currentOrderItems = new List<MenuItem>();
        }

        private void InitializeComponent()
        {
            Text = "Ewha Shiro";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 30);
        }

        private void SetupMainInterface()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40,40,40)
            };

            // Header Panel
            var headerPanel = CreateHeaderPanel();

            // Navigation Panel
            var navigationPanel = CreateNavigationPanel();

            // Content Panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 40)
            };

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(navigationPanel);
            mainPanel.Controls.Add(headerPanel);

            Controls.Add(mainPanel);
        }

        private Panel CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(52, 152, 219)
            };

            var titleLabel = new Label
            {
                Text = "Ewha Shiro",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(titleLabel);
            return headerPanel;
        }

        private FlowLayoutPanel CreateNavigationPanel()
        {
            var navigationPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(44, 62, 80)
            };

            var navButtons = new[]
            {
                CreateNavButton("Place Order"),
                CreateNavButton("View Orders"),
                CreateNavButton("Menu Management"),
                CreateNavButton("Sales Report"),
                CreateNavButton("Settings")
            };

            foreach (var button in navButtons)
            {
                button.Click += NavigationButton_Click;
                navigationPanel.Controls.Add(button);
            }

            return navigationPanel;
        }

        private Button CreateNavButton(string text)
        {
            return new Button
            {
                Text = text,
                Size = new Size(250, 60),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12)
            };
        }

        private void NavigationButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            switch (button.Text)
            {
                case "Place Order":
                    ShowOrderPlacementForm();
                    break;
                case "View Orders":
                    ShowOrdersListForm();
                    break;
                case "Menu Management":
                    ShowMenuManagementForm();
                    break;
                case "Sales Report":
                    ShowSalesReportForm();
                    break;
            }
        }

        private void ShowOrderPlacementForm()
        {
            var orderForm = new Form
            {
                Text = "Place Order",
                Size = new Size(800, 600)
            };

            // Table Selection
            var tableLabel = new Label
            {
                Text = "Select Table:",
                Location = new Point(20, 20)
            };

            var tableNumericUpDown = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 20,
                Location = new Point(150, 20)
            };

            // Menu Items ListView
            var menuListView = new ListView
            {
                Location = new Point(20, 60),
                Size = new Size(400, 300),
                View = View.Details
            };
            menuListView.Columns.Add("Item", 200);
            menuListView.Columns.Add("Price", 100);
            menuListView.Columns.Add("Category", 100);

            // Populate Menu Items
            var menuItems = _menuService.GetMenuItems();
            foreach (var item in menuItems)
            {
                var listViewItem = new ListViewItem(new[]
                {
            item.Name,
            item.Price.ToString("C"),
            item.Category.ToString()
        });
                menuListView.Items.Add(listViewItem);
            }

            // Order Summary ListView
            var orderSummaryListView = new ListView
            {
                Location = new Point(440, 60),
                Size = new Size(300, 250),
                View = View.Details
            };
            orderSummaryListView.Columns.Add("Item", 200);
            orderSummaryListView.Columns.Add("Quantity", 100);
            orderSummaryListView.Columns.Add("Total Price", 100);

            // Add Item to Order
            var quantityNumericUpDown = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10,
                Value = 1,
                Location = new Point(20, 380),
                Size = new Size(100, 30)
            };

            menuListView.DoubleClick += (s, e) =>
            {
                if (menuListView.SelectedItems.Count > 0)
                {
                    var selectedItem = menuItems[menuListView.SelectedIndices[0]];
                    int quantity = (int)quantityNumericUpDown.Value;

                    // Update current order items
                    var existingItem = _currentOrderItems.FirstOrDefault(item => item.Name == selectedItem.Name);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity; // Increase quantity if item already exists
                    }
                    else
                    {
                        var newItem = new MenuItem
                        {
                            Id = selectedItem.Id,
                            Name = selectedItem.Name,
                            Price = selectedItem.Price,
                            Category = selectedItem.Category,
                            Quantity = quantity
                        };
                        _currentOrderItems.Add(newItem);
                    }

                    // Update order summary list view
                    orderSummaryListView.Items.Clear();
                    foreach (var item in _currentOrderItems)
                    {
                        var orderItem = new ListViewItem(new[]
                        {
                    item.Name,
                    item.Quantity.ToString(),
                    item.TotalPrice.ToString("C")
                });
                        orderSummaryListView.Items.Add(orderItem);
                    }

                    
                }
            };

            // Delete Item Button
            var deleteItemButton = new Button
            {
                Text = "Delete Item",
                Location = new Point(440, 320),
                Size = new Size(100, 30)
            };
            deleteItemButton.Click += (s, e) =>
            {
                if (orderSummaryListView.SelectedItems.Count > 0)
                {
                    var selectedItem = orderSummaryListView.SelectedItems[0];
                    var itemToRemove = _currentOrderItems.FirstOrDefault(item =>
                        item.Name == selectedItem.SubItems[0].Text);

                    if (itemToRemove != null)
                    {
                        _currentOrderItems.Remove(itemToRemove);
                        orderSummaryListView.Items.Remove(selectedItem);
                    }
                }
            };

            // Confirm Order Button
            var confirmOrderButton = new Button
            {
                Text = "Send to Kitchen",
                Location = new Point(20, 420),
                Size = new Size(150, 30)
            };
            confirmOrderButton.Click += (s, e) =>
            {
                if (_currentOrderItems.Count == 0)
                {
                    MessageBox.Show("Please add items to the order first.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedTable = (int)tableNumericUpDown.Value;

                // Create a new list with copied items to ensure a clean order
                var orderItems = _currentOrderItems.Select(item => new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Category = item.Category,
                    Quantity = item.Quantity
                }).ToList();

                var order = _orderService.CreateOrder(_selectedTable, orderItems);

                // Change order status to Confirmed (sent to kitchen)
                order.Status = OrderStatus.Confirmed;

                MessageBox.Show($"Order for Table {_selectedTable} sent to kitchen successfully", "Order Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear current order
                _currentOrderItems.Clear();
                orderSummaryListView.Items.Clear();
            };

            orderForm.Controls.Add(tableLabel);
            orderForm.Controls.Add(tableNumericUpDown);
            orderForm.Controls.Add(menuListView);
            orderForm.Controls.Add(orderSummaryListView);
            orderForm.Controls.Add(quantityNumericUpDown);
            orderForm.Controls.Add(confirmOrderButton);
            orderForm.Controls.Add(deleteItemButton);
            orderForm.ShowDialog();
        }

        private void GenerateReceipt(Order order)
        {
            var receiptForm = new Form
            {
                Text = "Receipt",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            var receiptTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            var receiptContent = new StringBuilder();
            receiptContent.AppendLine("Receipt");
            receiptContent.AppendLine($"Order ID: {order.Id}");
            receiptContent.AppendLine($"Table Number: {order.TableNumber}");
            receiptContent.AppendLine($"Order Date: {order.OrderDate}");
            receiptContent.AppendLine("Items:");
            foreach (var item in order.Items)
            {
                receiptContent.AppendLine($"{item.Name} - {item.TotalPrice:C}");
            }
            receiptContent.AppendLine($"Total Amount: {order.TotalAmount:C}");
            receiptContent.AppendLine($"Payment Method: {order.PaymentMethod}");
            receiptContent.AppendLine($"Status: {order.Status}");

            receiptTextBox.Text = receiptContent.ToString();

            var saveButton = new Button
            {
                Text = "Save Receipt",
                Dock = DockStyle.Bottom
            };
            saveButton.Click += (s, e) =>
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Save Receipt"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, receiptTextBox.Text);
                    MessageBox.Show("Receipt saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            receiptForm.Controls.Add(receiptTextBox);
            receiptForm.Controls.Add(saveButton);
            receiptForm.ShowDialog();
        }

        private void ShowOrdersListForm()
        {
            var ordersForm = new Form
            {
                Text = "View Orders",
                Size = new Size(1000, 600),
                BackColor = Color.FromArgb(40, 40, 40) // Dark background for the form
            };

            var ordersListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(50, 50, 50), // Darker background for ListView
                ForeColor = Color.White // White text for better visibility
            };

            ordersListView.Columns.Add("Order ID", 100);
            ordersListView.Columns.Add("Table Number", 100);
            ordersListView.Columns.Add("Total Amount", 100);
            ordersListView.Columns.Add("Status", 100);

            foreach (var order in _orderService.GetAllOrders())
            {
                var listViewItem = new ListViewItem(order.Id.ToString())
                {
                    SubItems = {
                order.TableNumber.ToString(),
                order.TotalAmount.ToString("C"),
                order.Status.ToString()
            }
                };
                ordersListView.Items.Add(listViewItem);
            }

            // Add Confirm Payment Button
            var confirmPaymentButton = new Button
            {
                Text = "Confirm Payment",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219), // Button color
                ForeColor = Color.White // White text for visibility
            };

            confirmPaymentButton.Click += (s, e) => ConfirmPayment(ordersListView);

            // Add Edit Order Button
            var editOrderButton = new Button
            {
                Text = "Edit Order",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(44, 62, 80), // Button color
                ForeColor = Color.White // White text for visibility
            };

            editOrderButton.Click += (s, e) => EditOrder(ordersListView);

            // Add View Order Details Button
            var viewOrderDetailsButton = new Button
            {
                Text = "View Order Details",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219), // Button color
                ForeColor = Color.White // White text for visibility
            };

            viewOrderDetailsButton.Click += (s, e) => ViewOrderDetails(ordersListView);

            ordersForm.Controls.Add(ordersListView);
            ordersForm.Controls.Add(confirmPaymentButton);
            ordersForm.Controls.Add(editOrderButton);
            ordersForm.Controls.Add(viewOrderDetailsButton);
            ordersForm.ShowDialog();
        }
        private void ViewOrderDetails(ListView ordersListView)
        {
            if (ordersListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an order to view details.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedOrderId = int.Parse(ordersListView.SelectedItems[0].Text);
            var order = _orderService.GetOrderById(selectedOrderId);

            if (order == null)
            {
                MessageBox.Show("Order not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Debug: Check if items exist
            if (order.Items == null || order.Items.Count == 0)
            {
                MessageBox.Show($"No items found for Order ID {order.Id}. Total Items: {order.Items?.Count ?? 0}", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var detailsForm = new Form
            {
                Text = $"Order Details - ID: {order.Id}",
                Size = new Size(600, 400), // Increased size for better visibility
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(40, 40, 40)
            };

            var detailsListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FullRowSelect = true // Add this for better visibility
            };

            // Add more detailed columns
            detailsListView.Columns.Add("Item", 200);
            detailsListView.Columns.Add("Category", 100);
            detailsListView.Columns.Add("Unit Price", 100);
            detailsListView.Columns.Add("Quantity", 80);
            detailsListView.Columns.Add("Total Price", 100);

            // Detailed logging
            foreach (var item in order.Items)
            {
                var listViewItem = new ListViewItem(new[]
                {
            item.Name,
            item.Category.ToString(),
            item.Price.ToString("C"),
            item.Quantity.ToString(),
            item.TotalPrice.ToString("C")
        });
                detailsListView.Items.Add(listViewItem);
            }

            // Add total amount label
            var totalLabel = new Label
            {
                Text = $"Total Order Amount: {order.TotalAmount:C}",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                Height = 30
            };

            detailsForm.Controls.Add(detailsListView);
            detailsForm.Controls.Add(totalLabel);
            detailsForm.ShowDialog();
        }
        private void ConfirmPayment(ListView ordersListView)
        {
            if (ordersListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an order to confirm payment.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected order
            int selectedOrderId = int.Parse(ordersListView.SelectedItems[0].Text);
            var order = _orderService.GetOrderById(selectedOrderId);

            if (order == null)
            {
                MessageBox.Show("Order not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if order is already paid
            if (order.Status == OrderStatus.Paid)
            {
                MessageBox.Show("This order has already been paid.", "Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Payment Confirmation Dialog
            var paymentForm = new Form
            {
                Text = "Confirm Payment",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            // Order Details
            var orderDetailsLabel = new Label
            {
                Text = $"Order ID: {order.Id}\nTable Number: {order.TableNumber}\nTotal Amount: {order.TotalAmount:C}",
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Payment Method Selection
            var paymentMethodLabel = new Label
            {
                Text = "Select Payment Method:",
                Location = new Point(20, 100)
            };

            var paymentMethodComboBox = new ComboBox
            {
                Location = new Point(20, 130),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            paymentMethodComboBox.Items.AddRange(Enum.GetNames(typeof(PaymentMethod)));
            paymentMethodComboBox.SelectedIndex = 0;

            // Confirm Payment Button
            var confirmButton = new Button
            {
                Text = "Confirm Payment",
                Location = new Point(20, 200),
                Width = 150
            };

            confirmButton.Click += (s, e) =>
            {
                // Update order status and payment method
                order.Status = OrderStatus.Paid;
                order.PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), paymentMethodComboBox.SelectedItem.ToString());
                order.PaymentDate = DateTime.Now;

                // Refresh the orders list
                ordersListView.SelectedItems[0].SubItems[3].Text = OrderStatus.Paid.ToString();

                MessageBox.Show("Payment confirmed successfully!", "Payment Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                paymentForm.Close();

                // Generate receipt after payment confirmation
                GenerateReceipt(order);
            };

            // Cancel Button
            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(180, 200),
                Width = 150
            };
            cancelButton.Click += (s, e) => paymentForm.Close();

            paymentForm.Controls.AddRange(new Control[]
            {
                orderDetailsLabel,
                paymentMethodLabel,
                paymentMethodComboBox,
                confirmButton,
                cancelButton
            });

            paymentForm.ShowDialog();
        }

        private void EditOrder(ListView ordersListView)
        {
            if (ordersListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an order to edit.", "No Order Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedOrderId = int.Parse(ordersListView.SelectedItems[0].Text);
            var order = _orderService.GetOrderById(selectedOrderId);

            if (order == null)
            {
                MessageBox.Show("Order not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var editOrderForm = new Form
            {
                Text = "Edit Order",
                Size = new Size(800, 600)
            };

            // Table Number
            var tableLabel = new Label
            {
                Text = "Table Number:",
                Location = new Point(20, 20)
            };

            var tableNumericUpDown = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 20,
                Value = order.TableNumber,
                Location = new Point(150, 20)
            };

            // Total Price Label
            var totalPriceLabel = new Label
            {
                Text = $"Total Price: {order.TotalAmount:C}",
                Location = new Point(300, 20),
                AutoSize = true
            };

            // Menu Items ListView
            var menuListView = new ListView
            {
                Location = new Point(20, 60),
                Size = new Size(400, 300),
                View = View.Details
            };
            menuListView.Columns.Add("Item", 200);
            menuListView.Columns.Add("Price", 100);
            menuListView.Columns.Add("Category", 100);

            // Populate Menu Items
            var menuItems = _menuService.GetMenuItems();
            foreach (var item in menuItems)
            {
                var listViewItem = new ListViewItem(new[]
                {
            item.Name,
            item.Price.ToString("C"),
            item.Category.ToString()
        });
                menuListView.Items.Add(listViewItem);
            }

            // Order Summary ListView
            var orderSummaryListView = new ListView
            {
                Location = new Point(440, 60),
                Size = new Size(300, 250),
                View = View.Details,
                FullRowSelect = true // Allow selecting entire row
            };
            orderSummaryListView.Columns.Add("Item", 200);
            orderSummaryListView.Columns.Add("Quantity", 100);
            orderSummaryListView.Columns.Add("Total Price", 100);

            // Populate existing order items
            foreach (var item in order.Items)
            {
                var orderItem = new ListViewItem(new[]
                {
            item.Name,
            item.Quantity.ToString(),
            (item.Price * item.Quantity).ToString("C")
        });
                orderSummaryListView.Items.Add(orderItem);
            }

            // Add Item to Order
            var quantityNumericUpDown = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10,
                Value = 1,
                Location = new Point(20, 380),
                Size = new Size(100, 30)
            };

            // Delete Item Button
            var deleteItemButton = new Button
            {
                Text = "Delete Item",
                Location = new Point(440, 320),
                Size = new Size(100, 30)
            };
            deleteItemButton.Click += (s, e) =>
            {
                if (orderSummaryListView.SelectedItems.Count > 0)
                {
                    var selectedItem = orderSummaryListView.SelectedItems[0];
                    var itemToRemove = order.Items.FirstOrDefault(item =>
                        item.Name == selectedItem.SubItems[0].Text);

                    if (itemToRemove != null)
                    {
                        // Remove the item from the order
                        order.Items.Remove(itemToRemove);

                        // Recalculate total price
                        order.TotalAmount = order.Items.Sum(item => item.Price * item.Quantity);
                        totalPriceLabel.Text = $"Total Price: {order.TotalAmount:C}";

                        // Remove from the ListView
                        orderSummaryListView.Items.Remove(selectedItem);

                        MessageBox.Show($"{itemToRemove.Name} removed from the order.", "Item Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an item to delete.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            menuListView.DoubleClick += (s, e) =>
            {
                if (menuListView.SelectedItems.Count > 0)
                {
                    var selectedItem = menuItems[menuListView.SelectedIndices[0]];
                    int quantity = (int)quantityNumericUpDown.Value;

                    // Update current order items
                    var existingItem = order.Items.FirstOrDefault(item => item.Name == selectedItem.Name);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity; // Increase quantity if item already exists
                    }
                    else
                    {
                        var newItem = new MenuItem
                        {
                            Id = selectedItem.Id,
                            Name = selectedItem.Name,
                            Price = selectedItem.Price,
                            Category = selectedItem.Category,
                            Quantity = quantity
                        };
                        order.Items.Add(newItem);
                    }

                    // Recalculate total price
                    order.TotalAmount = order.Items.Sum(item => item.Price * item.Quantity);
                    totalPriceLabel.Text = $"Total Price: {order.TotalAmount:C}";

                    // Update order summary
                    orderSummaryListView.Items.Clear();
                    foreach (var item in order.Items)
                    {
                        var orderItem = new ListViewItem(new[]
                        {
                    item.Name,
                    item.Quantity.ToString(),
                    (item.Price * item.Quantity).ToString("C")
                });
                        orderSummaryListView.Items.Add(orderItem);
                    }
                }
            };

            // Save Changes Button
            var saveButton = new Button
            {
                Text = "Save Changes",
                Location = new Point(20, 420)
            };

            saveButton.Click += (s, e) =>
            {
                // Save the updated order
                _orderService.UpdateOrder(order);
                MessageBox.Show("Order updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the orders list in the main view
                RefreshOrdersList(ordersListView);
                editOrderForm.Close();
            };

            // Add controls to the form
            editOrderForm.Controls.Add(tableLabel);
            editOrderForm.Controls.Add(tableNumericUpDown);
            editOrderForm.Controls.Add(totalPriceLabel);
            editOrderForm.Controls.Add(menuListView);
            editOrderForm.Controls.Add(orderSummaryListView);
            editOrderForm.Controls.Add(quantityNumericUpDown);
            editOrderForm.Controls.Add(saveButton);
            editOrderForm.Controls.Add(deleteItemButton);

            editOrderForm.ShowDialog();
        }

        private void RefreshOrdersList(ListView ordersListView)
        {
            ordersListView.Items.Clear();
            var orders = _orderService.GetAllOrders();
            foreach (var order in orders)
            {
                var listViewItem = new ListViewItem(new[]
                {
            order.Id.ToString(),
            order.TableNumber.ToString(),
            order.TotalAmount.ToString("C"),
            order.OrderDate.ToString("g")
        });
                ordersListView.Items.Add(listViewItem);
            }
        }

        private void ShowMenuManagementForm()
        {
            // Placeholder for menu management functionality
            MessageBox.Show("Menu Management feature is not implemented yet.");
        }

        private void ShowSalesReportForm()
        {
            var salesReportForm = new Form
            {
                Text = "Sales Report",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            };

            var startDateLabel = new Label
            {
                Text = "Start Date:",
                Location = new Point(20, 20)
            };

            var startDatePicker = new DateTimePicker
            {
                Location = new Point(100, 20),
                Format = DateTimePickerFormat.Short
            };

            var endDateLabel = new Label
            {
                Text = "End Date:",
                Location = new Point(20, 60)
            };

            var endDatePicker = new DateTimePicker
            {
                Location = new Point(100, 60),
                Format = DateTimePickerFormat.Short
            };

            var generateReportButton = new Button
            {
                Text = "Generate Report",
                Location = new Point(20, 100),
                Width = 150
            };

            generateReportButton.Click += (s, e) =>
            {
                var startDate = startDatePicker.Value;
                var endDate = endDatePicker.Value;

                var orders = _orderService.GetOrdersByDateRange(startDate, endDate);
                if (orders.Count == 0)
                {
                    MessageBox.Show("No orders found for the selected date range.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var csvFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SalesReport.csv");
                using (var writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Order ID,Table Number,Total Amount,Order Date,Status");
                    foreach (var order in orders)
                    {
                        writer.WriteLine($"{order.Id},{order.TableNumber},{order.TotalAmount:C},{order.OrderDate},{order.Status}");
                    }
                }

                MessageBox.Show($"Sales report generated successfully at:\n{csvFilePath}", "Report Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            salesReportForm.Controls.AddRange(new Control[]
            {
                startDateLabel,
                startDatePicker,
                endDateLabel,
                endDatePicker,
                generateReportButton
            });

            salesReportForm.ShowDialog();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RestaurantManagementForm());
        }
    }
}
