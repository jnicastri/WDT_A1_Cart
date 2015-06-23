using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace A1ClassLibrary
{
    /// <summary>
    /// The Inventory Class holds a private list of Product Objects.
    /// </summary>
    class Inventory
    {
        #region Class Data
        private List<Product> _InventoryList;
        #endregion


        #region Constr.
        public Inventory()
        {
            _InventoryList = new List<Product>();

            if (!LoadInventory())
                Environment.Exit(0);
        }
        #endregion


        #region Operations
        /// <summary>
        /// Loads the inventory data in from a file & populates the Inventory objects private list with data.
        /// </summary>
        public bool LoadInventory()
        {

            StreamReader fp = null;

            try
            {
                fp = new StreamReader("product.inventory.txt");
                string currentLine, title;
                char[] delimiter = { ',' };
                string[] lineArray;
                int id, qty, price;

                while ((currentLine = fp.ReadLine()) != null)
                {
                    lineArray = currentLine.Split(delimiter);
                    id = Int32.Parse(lineArray[0]);
                    title = lineArray[1];
                    qty = Int32.Parse(lineArray[2]);
                    price = Int32.Parse(lineArray[3]);

                    _InventoryList.Add(new Product(id, title, qty, price));
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("An Error has occured during initialisation. The program will now exit.\nError details: \n" + ioe.Message);
                Console.WriteLine("Please check the file is located in the correct place. \nPress any key......");
                Console.ReadKey(false);

                if (fp != null)
                    fp.Close();

                return false;
            }
            catch (FormatException fe)
            {
                Console.WriteLine("A Parsing Error has occured during initialisation. The program will now exit.\nError details: \n" + fe.Message);
                Console.WriteLine("Please check the contents of the file and ensure the data is in the correct sequence. \nPress any key......");
                Console.ReadKey(false);
                fp.Close();
                return false;
            }

            fp.Close();

            return true;
        }
        /// <summary>
        /// Updates the available stock on hand value in the current Inventory list.
        /// </summary>
        /// <param name="Id">The Id of the Product object that you need to update the SOH for</param>
        /// <param name="AmmendQty">The amount you want to adjust the SOH value by (can be a negative number)</param>
        public void UpdateSOH(int Id, int AmmendQty)
        {
            foreach (var product in _InventoryList)
            {
                if (product.Id == Id)
                {
                    product.Qty -= AmmendQty;
                    break;
                }
            }
        }
        /// <summary>
        /// Writes the current contents of this object to file in its current state.
        /// </summary>
        public bool WriteToFile()
        {
            StreamWriter fp = null;

            try
            {
                fp = new StreamWriter("product.inventory.txt", false);

                foreach (var p in _InventoryList)
                {
                    fp.WriteLine("{0},{1},{2},{3}", p.Id.ToString(), p.Title.Trim(), p.Qty.ToString(), p.UnitPrice.ToString());
                }
            }
            catch (IOException)
            {
                fp.Close();
                return false;
            }

            fp.Close();
            return true;
        }

        /// <summary>
        /// Checks that the passed ID is a valid ID of a product in the current inventory object.
        /// </summary>
        /// <param name="Id">The ID of the Product that you want to check.</param>
        /// <returns>True if the passed ID is an iventory item. False if the Product does not exist.</returns>
        public bool ValidateId(int Id)
        {
            bool match = false;

            foreach (Product item in _InventoryList)
            {
                if (item.Id == Id)
                    match = true;
            }

            return match;
        }

        /// <summary>
        /// Returns the current stock on hand value of the ID passed in the inventory. 
        /// </summary>
        /// <param name="Id">The ID of the Product that you wish to enquire for.</param>
        /// <returns>null if the ID is invalid, otherwise the current available SOH in the inventory.</returns>
        public int? GetInventorySOH(int Id)
        {
            int? SOH = null;

            foreach (Product item in _InventoryList)
            {
                if (item.Id == Id)
                    SOH = item.Qty;
            }

            return SOH;
        }

        /// <summary>
        /// Retrieves a Product object that is currently in the inventory.
        /// </summary>
        /// <param name="Id">The Id of the Product you want returned</param>
        /// <returns>null if not found, otherwise the matching product in the inventory.</returns>
        public Product RetrieveProductById(int Id)
        {

            Product item = null;

            foreach (var invItem in _InventoryList)
            {
                if (invItem.Id == Id)
                {
                    item = invItem;
                    break;
                }
            }
            return item;
        }

        /// <summary>
        /// Displays a table of the current available items that are available to add to the cart.
        /// </summary>
        /// <param name="Cart">A reference to the current SessionOrder object</param>
        public void DisplayUnallocatedInventory(SessionOrder Cart)
        {
            StringBuilder header = new StringBuilder();
            StringBuilder inventoryLine = null;

            header.Append(String.Format("{0, -7} ", "Record#"));
            header.Append(String.Format("{0, -20} ", "Title"));
            header.Append(String.Format("{0, -12} ", "Unit Cost"));
            header.Append(String.Format("{0, -13} ", "Available Qty"));

            Console.WriteLine("\n******************** AVAILABLE PRODUCTS ***************");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(header);
            Console.WriteLine("------- -------------------- ------------ ------------- ");

            foreach (Product CurrentProduct in _InventoryList)
            {
                int? cartQty = Cart.GetCartQty(CurrentProduct.Id);
                int availableQty = 0;

                if (cartQty != null)
                    availableQty = CurrentProduct.Qty - (int)cartQty;
                else
                    availableQty = CurrentProduct.Qty;

                inventoryLine = new StringBuilder();
                inventoryLine.Append(String.Format("{0, -7} ", CurrentProduct.Id.ToString()));
                inventoryLine.Append(String.Format("{0, -20} ", CurrentProduct.Title.Trim()));
                inventoryLine.Append(String.Format("{0, -12} ", CurrentProduct.UnitPrice.ToString()));
                inventoryLine.Append(String.Format("{0, -10} ", availableQty.ToString()));

                Console.WriteLine(inventoryLine);
            }

            Console.WriteLine("-------------------------------------------------------");
        }
        #endregion
    }
}
