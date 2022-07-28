using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.utils
{
    internal class PrintUtil
    {
            ArrayList headerLines = new ArrayList();
            ArrayList subHeaderLines = new ArrayList();
            ArrayList items = new ArrayList();
            ArrayList totales = new ArrayList();
            ArrayList footerLines = new ArrayList();
            private Image headerImage = null;

            int count = 0;

            int maxChar = 35;
            int maxCharDescription = 20;

            int imageHeight = 0;

            float leftMargin = 0;
            float topMargin = 3;

            string fontName = "Lucida Console";
            int fontSize = 10;

            Font printFont = null;
            SolidBrush myBrush = new SolidBrush(Color.Black);

            Graphics gfx = null;

            string line = null;

            public Image HeaderImage
            {
                get { return headerImage; }
                set { if (headerImage != value) headerImage = value; }
            }

            public int MaxChar
            {
                get { return maxChar; }
                set { if (value != maxChar) maxChar = value; }
            }

            public int MaxCharDescription
            {
                get { return maxCharDescription; }
                set { if (value != maxCharDescription) maxCharDescription = value; }
            }

            public int FontSize
            {
                get { return fontSize; }
                set { if (value != fontSize) fontSize = value; }
            }

            public string FontName
            {
                get { return fontName; }
                set { if (value != fontName) fontName = value; }
            }

            public void AddHeaderLine(string line)
            {
                headerLines.Add(line);
            }

            public void AddSubHeaderLine(string line)
            {
                subHeaderLines.Add(line);
            }

            public void AddItem(string barCode, string name, string quantity, string amountUnit, string amount)
            {
                OrderItem newItem = new OrderItem('?');
                items.Add(newItem.GenerateItem(barCode, name, quantity, amount, amount));
            }

            public void AddTotal(string name, string price)
            {
                OrderTotal newTotal = new OrderTotal('?');
                totales.Add(newTotal.GenerateTotal(name, price));
            }

            public void AddFooterLine(string line)
            {
                footerLines.Add(line);
            }

            private string AlignRightText(int lenght)
            {
                string espacios = "";
                int spaces = maxChar - lenght;
                for (int x = 0; x < spaces; x++)
                    espacios += " ";
                return espacios;
            }

            private string DottedLine()
            {
                string dotted = "";
                for (int x = 0; x < maxChar; x++)
                    dotted += "=";
                return dotted;
            }

            public bool PrinterExists(string impresora)
            {
                foreach (String strPrinter in PrinterSettings.InstalledPrinters)
                {
                    if (impresora == strPrinter)
                        return true;
                }
                return false;
            }

            public void PrintTicket(string impresora = "")
            {
                printFont = new Font(fontName, fontSize, FontStyle.Regular);
                PrintDocument pr = new PrintDocument();
                if (!impresora.Equals(""))
                {
                    pr.PrinterSettings.PrinterName = impresora;
                }
                pr.PrintPage += new PrintPageEventHandler(pr_PrintPage);
                pr.Print();
            }

            private void pr_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
            {
                e.Graphics.PageUnit = GraphicsUnit.Millimeter;
                gfx = e.Graphics;
                DrawImage();
                DrawHeader();
                DrawSubHeader();
                DrawItems();
                DrawTotales();
                DrawFooter();

                if (headerImage != null)
                {
                    HeaderImage.Dispose();
                    headerImage.Dispose();
                }
            }

            private float YPosition()
            {
                return topMargin + (count * printFont.GetHeight(gfx) + imageHeight);
            }

            private void DrawImage()
            {
                if (headerImage != null)
                {
                    try
                    {
                        //gfx.DrawImage(headerImage, new Point((int)leftMargin, (int)YPosition()));
                        gfx.DrawImage(headerImage, new Point((int)7, (int)YPosition()));
                        double height = ((double)headerImage.Height / 58) * 15;
                        imageHeight = (int)Math.Round(height) + 3;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            private void DrawHeader()
            {
                foreach (string header in headerLines)
                {
                    if (header.Length > maxChar)
                    {
                        int currentChar = 0;
                        int headerLenght = header.Length;

                        while (headerLenght > maxChar)
                        {
                            line = header.Substring(currentChar, maxChar);
                            gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                            count++;
                            currentChar += maxChar;
                            headerLenght -= maxChar;
                        }
                        line = header;
                        gfx.DrawString(line.Substring(currentChar, line.Length - currentChar), printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                        count++;
                    }
                    else
                    {
                        line = header;
                        //gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                        float XPosition = (maxChar/2) - (line.Length / 2);
                        gfx.DrawString(line, printFont, myBrush, XPosition, YPosition(), new StringFormat());

                        count++;
                    }
                }
                DrawEspacio();
            }

            private void DrawSubHeader()
            {
                foreach (string subHeader in subHeaderLines)
                {
                    if (subHeader.Length > maxChar)
                    {
                        int currentChar = 0;
                        int subHeaderLenght = subHeader.Length;

                        while (subHeaderLenght > maxChar)
                        {
                            line = subHeader;
                            gfx.DrawString(line.Substring(currentChar, maxChar), printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                            count++;
                            currentChar += maxChar;
                            subHeaderLenght -= maxChar;
                        }
                        line = subHeader;
                        gfx.DrawString(line.Substring(currentChar, line.Length - currentChar), printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                        count++;
                    }
                    else
                    {
                        line = subHeader;

                        gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                        count++;

                        line = DottedLine();

                        gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                        count++;
                    }
                }
            }

            private void DrawItems()
            {
                OrderItem ordIt = new OrderItem('?');

                foreach (string item in items)
                {
                    line = ordIt.GetItemInfo(item, 0);
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                    count++;
                    leftMargin = 0;

                    line = ordIt.GetItemInfo(item, 1);
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                    count++;
                    leftMargin = 0;


                    line = ordIt.GetItemInfo(item, 3) + " P/U      " + ordIt.GetItemInfo(item, 4);
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                    count++;
                    leftMargin = 0;
                    line = "cant " + ordIt.GetItemInfo(item, 2);
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                    count += 2;
            }

            leftMargin = 0;
  
                line = DottedLine();

                gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                count++;
                DrawEspacio();
            }

            private void DrawTotales()
            {
                OrderTotal ordTot = new OrderTotal('?');

                foreach (string total in totales)
                {
                    line = ordTot.GetTotalName(total);
                
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                    line = line = ordTot.GetTotalCantidad(total);
                    leftMargin += line.Length + 10;
                    gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                    count++;
                }
                leftMargin = 0;
                DrawEspacio();
            }

            private void DrawFooter()
            {
                foreach (string footer in footerLines)
                {
                    if (footer.Length > maxChar)
                    {
                        int currentChar = 0;
                        int footerLenght = footer.Length;

                        while (footerLenght > maxChar)
                        {
                            line = footer;
                            gfx.DrawString(line.Substring(currentChar, maxChar), printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                            count++;
                            currentChar += maxChar;
                            footerLenght -= maxChar;
                        }
                        line = footer;
                        gfx.DrawString(line.Substring(currentChar, line.Length - currentChar), printFont, myBrush, leftMargin, YPosition(), new StringFormat());
                        count++;
                    }
                    else
                    {
                        line = footer;
                        gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                        count++;
                    }
                }
                leftMargin = 0;
                DrawEspacio();
            }

            private void DrawEspacio()
            {
                line = "";

                gfx.DrawString(line, printFont, myBrush, leftMargin, YPosition(), new StringFormat());

                count++;
            }
        }

        public class OrderItem
        {
            char[] delimitador = new char[] { '?' };

            public OrderItem(char delimit)
            {
                delimitador = new char[] { delimit };
            }

            public string GetItemInfo(string orderItem, int index)
            {
                string[] delimitado = orderItem.Split(delimitador);
                return delimitado[index];
            }

            public string GenerateItem(string barCoder, string name, string quantity, string amountUnit, string amountTotal)
            {
                return barCoder + delimitador[0] + name + delimitador[0] + quantity + delimitador[0] + amountUnit + delimitador[0] + amountTotal;
            }
        }

        public class OrderTotal
        {
            char[] delimitador = new char[] { '?' };

            public OrderTotal(char delimit)
            {
                delimitador = new char[] { delimit };
            }

            public string GetTotalName(string totalItem)
            {
                string[] delimitado = totalItem.Split(delimitador);
                return delimitado[0];
            }

            public string GetTotalCantidad(string totalItem)
            {
                string[] delimitado = totalItem.Split(delimitador);
                return delimitado[1];
            }

            public string GenerateTotal(string totalName, string price)
            {
                return totalName + delimitador[0] + price;
            }
        }
    }