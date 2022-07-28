using Microsoft.VisualBasic;
using SuperGus.Dtos;
using SuperGus.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperGus
{
    public partial class MainBillerForm : Form
    {
        private ProductService productService;
        private TransactionService transactionService;
        private ReportService reportService;

        private List<TransactionResponseDto> transactionResponses = new List<TransactionResponseDto>();  
        public MainBillerForm()
        {
            
            InitializeComponent();
            focusBillerBarCode();
            billerTab.SelectedIndex = 1;
            productService = new ProductService();
            transactionService = new TransactionService();
            reportService = new ReportService();
        }

        #region Stock Tab
        private void barCodeStockFocus()
        {
            barCodeStockTxt.Select();
            this.ActiveControl = quantityBillerTxt;
            barCodeStockTxt.Focus();
        }
            

        #region keyPress Txt ABM
        private void barCodeStockTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!ifExistProductLoadInfoToUpdate())
                nextTab(e);

            succesCreateStockButtonLabel.Text = "";
        }
        private void nameStockTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            nextTab(e);
        }

        private void nameTicketStockTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            nextTab(e);
        }

        private void quantityStockTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            nextTab(e);
        }

        private void amountStockTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            nextTab(e);
        }

        #endregion

        #region click Buttons ABM
        private void createProductButton_Click(object sender, EventArgs e)
        {
            String operation = "Creado";
            ProductDto response = null;

            ProductService productService = new ProductService();
            String barCode = barCodeStockTxt.Text;
            String name = nameStockTxt.Text;
            String nameTicket = nameTicketStockTxt.Text;
            int quantity = quantityStockTxt.Text.Length > 0 ? Convert.ToInt32(quantityStockTxt.Text) : 0;
            Double amount = amountStockTxt.Text.Length > 0 ? Convert.ToDouble(amountStockTxt.Text) : 0;

            if (barCode.Length > 0)
            {
                ProductDto product = productService.getProduct(barCode);

                if (product != null)
                {
                    operation = "Actualizado";
                    if (name.Length > 0)
                    {
                        product.name = name;
                    }

                    if (nameTicket.Length > 0)
                    {
                        product.nameTicket = nameTicket;
                    }

                    if (quantity > 0)
                    {
                        product.quantity = quantity;
                    }

                    if (amount > 0)
                    {
                        product.amount = amount;
                    }
                    response = productService.save(product);
                }
                else if ((name.Length > 0) && (quantity > 0) && (amount > 0))
                {
                    product = new ProductDto(barCode, name, nameTicket.Length > 0 ? nameTicket : name, quantity, amount);
                    response = productService.save(product);
                }
                else
                {
                    MessageBox.Show("Complete todos los campos para crear un producto nuevo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                barCodeStockTxt.Text = "";
                nameStockTxt.Text = "";
                nameTicketStockTxt.Text = "";
                quantityStockTxt.Text = "";
                amountStockTxt.Text = "";
                succesCreateStockButtonLabel.Text = "Producto " + operation + " Exitosamente.";
                barCodeStockFocus();
            }
        }
        private void searchProductButton_Click(object sender, EventArgs e)
        {
            productStockListView.Items.Clear();
            String barCode = barCodeStockTxt.Text;
            String name = nameStockTxt.Text;
            String nameTicket = nameTicketStockTxt.Text;
            int quantity = quantityStockTxt.Text.Length > 0 ? Convert.ToInt32(quantityStockTxt.Text) : 0;
            Decimal amount = amountStockTxt.Text.Length > 0 ? Convert.ToDecimal(amountStockTxt.Text) : 0;

            if (barCode.Length > 0)
            {
                ProductDto product = productService.getProduct(barCode);
                if (product != null)
                {
                    addItemProductStockListView(product);
                }
                else
                    MessageBox.Show("Codigo de barras no encontrado!", "Producto inexistente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                List<ProductDto> products = productService.searchProductsByName(name);
                if (products != null & products.Count > 0)
                {
                    products.OrderBy(p => p.name).ToList().ForEach(addItemProductStockListView);
                    drawStockListView();
                }
                else
                    MessageBox.Show("No se encontro ningun producto con ese nombre!", "Producto inexistente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region listViewProduct Stock
        private void addItemProductStockListView(ProductDto product)
        {
            ListViewItem row = new ListViewItem();
            row.SubItems.Add(product.barCode);
            row.SubItems.Add(product.name);
            row.SubItems.Add(product.nameTicket);
            row.SubItems.Add(product.quantity.ToString());
            row.SubItems.Add(product.amount.ToString());
            productStockListView.Items.Add(row);
        }

        private void drawStockListView()
        {
            for (int i = 0; i < productStockListView.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    productStockListView.Items[i].BackColor = Color.White;
                }
                else
                {
                    productStockListView.Items[i].BackColor = Color.FromArgb(255, 192, 128);
                }
            }

        }

        #endregion

        private bool ifExistProductLoadInfoToUpdate()
        {
            String barCode = barCodeStockTxt.Text;
            ProductDto product = productService.getProduct(barCode);

            if(product != null)
            {
                nameStockTxt.Text = product.name;
                nameTicketStockTxt.Text = product.nameTicket;
                quantityStockTxt.Text = product.quantity.ToString();
                amountStockTxt.Text = product.amount.ToString();

                return true;
            }

            return false;
            
        }


        #endregion


        #region Biller Tab

        #region keyPress Txt BILLER
        private void barCodeBillerTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!deleteItemCheckBox.Checked)
                {
                    if (!checkIfExistIntoList())
                    {

                        ProductDto product = productService.getProduct(barCodeBillerTxt.Text);
                        if (product != null)
                        {
                            addItemProductBillerListView(product);
                            drawBillerListView();
                        }
                        else
                            MessageBox.Show("Codigo de barras no encontrado!", "Producto inexistente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        updateItemIntoBillerListView();
                }
                else if (barCodeBillerTxt.Text.Length > 0)
                {
                    deleteProductBillerListView(barCodeBillerTxt.Text);
                    drawBillerListView();
                }
                barCodeBillerTxt.Text = "";
            }
        }

        private void quantityBillerTxt_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                focusBillerBarCode();
            }
        }

        private void conceptBillerTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!deleteItemCheckBox.Checked)
                {
                    focusAmountConceptBilletTxt();
                }
                else
                {
                    deleteProductBillerListView(conceptBillerTxt.Text);
                    drawBillerListView();
                }
                
            }
        }

        private void amountConecptBillerTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!deleteItemCheckBox.Checked)
                {
                    if (conceptBillerTxt.Text.Length > 0 && amountConecptBillerTxt.Text.Length > 0)
                    {
                        var product = new ProductDto(DateTime.Now.Ticks.ToString(), conceptBillerTxt.Text, conceptBillerTxt.Text, 1, amountConecptBillerTxt.Text.Length > 0 ? Convert.ToDouble(amountConecptBillerTxt.Text) : 0);
                        var response = productService.save(product);
                        if (response != null) { 
                            addItemProductBillerListView(product);
                            drawBillerListView();
                         }
                    }
                }
                else
                {
                    deleteProductBillerListView(conceptBillerTxt.Text);
                    drawBillerListView();
                }
                conceptBillerTxt.Text = "";
                amountConecptBillerTxt.Text = "";
                focusBillerBarCode();
            }
        }
        #endregion

        #region click Buttons BILLER
        private void billButton_Click(object sender, EventArgs e)
        {
            if(billerProductListView.Items.Count > 0)
            {
                List<ProductDto> products = new List<ProductDto>();

                foreach(ListViewItem item in billerProductListView.Items)
                {
                    products.Add(new ProductDto(item.SubItems[1].Text, Convert.ToInt32(item.SubItems[3].Text), Convert.ToDouble(item.SubItems[5].Text.Replace("$", ""))));
                }

                TransactionDto transactionDto = transactionService.createTransaction(new TransactionDto(products, Convert.ToDouble(billerTotalAmountLabel.Text)));

                if(transactionDto != null)
                {
                    reportService.printTransaction(transactionDto.id);
                    MessageBox.Show("Factura a Imprimiendose", "Facturado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    billerProductListView.Items.Clear();
                    billerTotalAmountLabel.Text = "00.00";
                }
                else{
                    MessageBox.Show("No se pudo guardar correctamente la factura!!!", "Facturado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        #endregion

        #region listViewProduct Biller
        private Boolean checkIfExistIntoList()
        {
            foreach (ListViewItem item in billerProductListView.Items)
            {
                if (item.SubItems[1].Text == barCodeBillerTxt.Text)
                    return true;
            }

            return false;
        }

        private void updateItemIntoBillerListView()
        {
            foreach (ListViewItem item in billerProductListView.Items)
            {
                if (item.SubItems[1].Text == barCodeBillerTxt.Text)
                {
                    int quantityNow = Convert.ToInt32(item.SubItems[3].Text);
                    int quantityIncrement = (quantityBillerTxt.Text.Length > 0 ? Convert.ToInt32(quantityBillerTxt.Text) : 1);
                    int quantityNew = quantityNow + quantityIncrement;

                    if (quantityNew <= Convert.ToInt32(item.SubItems[6].Text))
                    {
                        Double amountUnit = Convert.ToDouble(item.SubItems[4].Text.Replace("$", ""));
                        Double amountTotal = Convert.ToDouble(item.SubItems[5].Text.Replace("$", ""));

                        item.SubItems[3].Text = quantityNew.ToString();
                        item.SubItems[5].Text = "$" + (amountTotal + (amountUnit * quantityIncrement)).ToString();
                        billerTotalAmountLabel.Text = (Convert.ToDouble(billerTotalAmountLabel.Text) + (amountUnit * quantityIncrement)).ToString();
                    }
                    else
                    {
                        MessageBox.Show("Stock insuficiente!!", "Falta de Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void addItemProductBillerListView(ProductDto product)
        {
            if (product.quantity >= (quantityBillerTxt.Text.Length > 0 ? Convert.ToInt32(quantityBillerTxt.Text) : 1))
            {
                Double totalAmount = quantityBillerTxt.Text.Length > 0 ? (product.amount * Convert.ToInt32(quantityBillerTxt.Text)) : product.amount;

                ListViewItem row = new ListViewItem();
                row.SubItems.Add(product.barCode);
                row.SubItems.Add(product.name);
                row.SubItems.Add(quantityBillerTxt.Text.Length > 0 ? quantityBillerTxt.Text : "1");
                row.SubItems.Add("$" + product.amount.ToString());
                row.SubItems.Add("$" + totalAmount.ToString());
                row.SubItems.Add(product.quantity.ToString());

                billerProductListView.Items.Add(row);
                billerTotalAmountLabel.Text = (Convert.ToDouble(billerTotalAmountLabel.Text) + totalAmount).ToString();
            }
            else
            {
                MessageBox.Show("Stock insuficiente!!", "Falta de Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void deleteProductBillerListView(string barCode)
        {
            foreach (ListViewItem item in billerProductListView.Items)
            {
                if (item.SubItems[1].Text == barCode)
                {
                    int quantityDecrement = quantityBillerTxt.Text.Length > 0 ? Convert.ToInt32(quantityBillerTxt.Text) : 1;
                    int quantityNow = Convert.ToInt32(item.SubItems[3].Text) - quantityDecrement;
                    Double amountUnit = Convert.ToDouble(item.SubItems[4].Text.Replace("$", ""));


                    if (quantityNow > 0)
                    {

                        item.SubItems[3].Text = quantityNow.ToString();
                        item.SubItems[5].Text = "$" + (amountUnit * quantityNow).ToString();
                        billerTotalAmountLabel.Text = (Convert.ToDouble(billerTotalAmountLabel.Text) - (amountUnit * quantityDecrement)).ToString();
                    }
                    else if (quantityNow == 0)
                    {
                        item.Remove();
                        billerTotalAmountLabel.Text = (Convert.ToDouble(billerTotalAmountLabel.Text) - (amountUnit * quantityDecrement)).ToString();
                    }
                    else
                        MessageBox.Show("El numero a eliminar es mayor al existente ingresado", "Error en cantidad", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            drawBillerListView();
        }

        private void drawBillerListView()
        {
            for (int i = 0; i < billerProductListView.Items.Count; i++)
            {
                if (i % 2 != 0)
                {
                    billerProductListView.Items[i].BackColor = Color.FromArgb(255, 192, 128);
                }
            }
        }

        #endregion
        
        #region focus Biller
        private void focusBillerBarCode()
        {
            barCodeBillerTxt.Select();
            this.ActiveControl = barCodeBillerTxt;
            barCodeBillerTxt.Focus();
        }

        private void focusQuantityBiller()
        {
            quantityBillerTxt.Select();
            this.ActiveControl = quantityBillerTxt;
            quantityBillerTxt.Focus();
        }
        private void focusAmountConceptBilletTxt()
        {
            amountConecptBillerTxt.Select();
            this.ActiveControl = amountConecptBillerTxt;
            amountConecptBillerTxt.Focus();
        }
        #endregion

        private void deleteItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            focusBillerBarCode();
        }
        #endregion

        #region closeBox
        #region buttons
        private void searchTransactionCloseBoxButton_Click(object sender, EventArgs e)
        {
            if(idTransactionCloseBoxTxt.Text.Length > 0 )
            {
                transactionCloseBoxListView.Items.Clear();
                TransactionResponseDto transaction = transactionService.getTransaction(Convert.ToInt64(idTransactionCloseBoxTxt.Text));
                if(transaction != null)
                {
                    detailTransactionCloseBoxListView.Items.Clear();
                    addItemTransactionToListView(transaction);
                    transaction.transactionitems.ForEach(addItemTransactionListViewDetail);
                    totalCloseBox.Text = transaction.totalAmount.ToString();
                    drawTransactionrListView();
                    drawDetailTransactionListView();
                }
                else
                    MessageBox.Show("El id ingresado no existe", "Compra inexistente", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
        }

        private void closeBoxButton_Click(object sender, EventArgs e)
        {
            string dateFrom = fromDateCloseBoxTxt.Text.Length > 0 ? fromDateCloseBoxTxt.Text : DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
            string dateTo = fromDateCloseBoxTxt.Text.Length > 0 ? fromDateCloseBoxTxt.Text : DateTime.Now.Date.AddHours(23.9).ToString("yyyy-MM-dd HH:mm:ss");
            
            transactionResponses = transactionService.getFromToDate(dateFrom, dateTo);
          
            transactionCloseBoxListView.Items.Clear();
            
            transactionResponses.ForEach(addItemTransactionToListView);

            drawTransactionrListView();
            totalCloseBox.Text = transactionResponses.Sum(t => t.totalAmount).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (transactionResponses.Count > 0) {
                string dateFrom = fromDateCloseBoxTxt.Text.Length > 0 ? fromDateCloseBoxTxt.Text : DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                string dateTo = fromDateCloseBoxTxt.Text.Length > 0 ? fromDateCloseBoxTxt.Text : DateTime.Now.Date.AddHours(23.9).ToString("yyyy-MM-dd HH:mm:ss");
                DialogResult dialogResult = MessageBox.Show(String.Format("Se va a imprimir el cierre de daja de {0} al {1}", dateFrom, dateTo), "Imprimir cierre ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    reportService.printCloseBox(transactionResponses, dateFrom, dateTo);
                }
            }
        }

        #endregion

        private void transactionCloseBoxListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(transactionCloseBoxListView.SelectedItems.Count > 0)
            {
                detailTransactionCloseBoxListView.Items.Clear();
                TransactionResponseDto tr = transactionResponses.Where(t => t.id == Convert.ToInt64(transactionCloseBoxListView.SelectedItems[0].SubItems[1].Text)).First(); 
                tr.transactionitems.ForEach(addItemTransactionListViewDetail);
                drawDetailTransactionListView();
            }

        }


        private void addItemTransactionToListView(TransactionResponseDto transaction)
        {
            ListViewItem transactionItemList = new ListViewItem();
            transactionItemList.SubItems.Add(transaction.id.ToString());
            transactionItemList.SubItems.Add(transaction.creationDate.ToString());
            transactionItemList.SubItems.Add(transaction.transactionitems.Count.ToString());
            transactionItemList.SubItems.Add(transaction.totalAmount.ToString());
            transactionCloseBoxListView.Items.Add(transactionItemList);

        }

        private void addItemTransactionListViewDetail(TransactionItemDto transactionItem)
        {
            ListViewItem transactionItemList = new ListViewItem();
            transactionItemList.SubItems.Add(transactionItem.item.barCode.ToString());
            transactionItemList.SubItems.Add(transactionItem.item.name);
            transactionItemList.SubItems.Add(transactionItem.quantity.ToString());
            transactionItemList.SubItems.Add(transactionItem.amount.ToString());

            detailTransactionCloseBoxListView.Items.Add(transactionItemList);
        }

        private void drawTransactionrListView()
        {
            for (int i = 0; i < transactionCloseBoxListView.Items.Count; i++)
            {
                if (i % 2 != 0)
                {
                    transactionCloseBoxListView.Items[i].BackColor = Color.FromArgb(255, 192, 128);
                }
            }
        }

        private void drawDetailTransactionListView()
        {
            for (int i = 0; i < detailTransactionCloseBoxListView.Items.Count; i++)
            {
                if (i % 2 != 0)
                {
                    detailTransactionCloseBoxListView.Items[i].BackColor = Color.FromArgb(255, 192, 128);
                }
            }
        }


        #endregion

        private void nextTab(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void clearStockProductListviewButton_Click(object sender, EventArgs e)
        {
            productStockListView.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            billerProductListView.Items.Clear();
        }
    }
}
