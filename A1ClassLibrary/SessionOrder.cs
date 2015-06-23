using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1ClassLibrary
{
    /// <summary>
    /// SessionOrder represents a current cart container object.
    /// </summary>
    class SessionOrder
    {
        #region Class Data and properties

        private SortedDictionary<int, int> _CartContents;
        public int UniqueRecords
        {
            get
            {
                return _CartContents.Count;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instatiates a new SessionOrder object.
        /// </summary>
        public SessionOrder()
        {
            _CartContents = new SortedDictionary<int, int>();
        }
        #endregion


        #region Operations

        /// <summary>
        /// Prints a formatted list of the current contents of the cart. 
        /// </summary>
        /// <param name="CurrentProductInventory">A reference to the orders available inventory.</param>
        public void DisplayCart(Inventory CurrentProductInventory)
        {
            if (!(UniqueRecords > 0))
            {
                Console.WriteLine("There are currently no items in your cart.");
            }
            else
            {
                int qtySum = 0;
                int rowExtendedCost = 0;
                int grandTotalCost = 0;

                StringBuilder header = new StringBuilder();
                StringBuilder cartLine = null;

                header.Append(String.Format("{0, -7} ", "Record#"));
                header.Append(String.Format("{0, -20} ", "Title"));
                header.Append(String.Format("{0, -12} ", "Unit Cost"));
                header.Append(String.Format("{0, -10} ", "Quantity"));
                header.Append(String.Format("{0, -12} ", "Ext. Cost"));

                Console.WriteLine("\n************************** CART SUMMARY *************************");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine(header);
                Console.WriteLine("------- -------------------- ------------ ---------- ------------ ");

                foreach (KeyValuePair<int, int> cartRow in _CartContents)
                {
                    Product itemDetail = CurrentProductInventory.RetrieveProductById(cartRow.Key);

                    rowExtendedCost = itemDetail.UnitPrice * cartRow.Value;
                    qtySum += cartRow.Value;
                    grandTotalCost += rowExtendedCost;

                    cartLine = new StringBuilder();
                    cartLine.Append(String.Format("{0, -7} ", itemDetail.Id.ToString()));
                    cartLine.Append(String.Format("{0, -20} ", itemDetail.Title.Trim()));
                    cartLine.Append(String.Format("{0, -12} ", itemDetail.UnitPrice.ToString()));
                    cartLine.Append(String.Format("{0, -10} ", cartRow.Value.ToString()));
                    cartLine.Append(String.Format("{0, -12} ", rowExtendedCost.ToString()));

                    Console.WriteLine(cartLine);
                }

                Console.WriteLine("\nTotal Units in the cart: {0}", qtySum);
                Console.WriteLine("Cart Total Cost: {0} Sickles\n", grandTotalCost);
                Console.WriteLine("-----------------------------------------------------------------");
            }
        }

        /// <summary>
        /// Adds a Product and quantity to the cart.
        /// </summary>
        /// <param name="Id">The Id of the Product object that you wish to add to the cart.</param>
        /// <param name="Qty">The amount of units of the Product object to add to the cart.</param>
        public void AddToCart(int Id, int Qty)
        {
            _CartContents.Add(Id, Qty);
        }

        /// <summary>
        /// Increases the quantity of an item that is already existing in the cart.
        /// </summary>
        /// <param name="Id">The Id of the product that you need to increment the qty for.</param>
        /// <param name="Qty">The amount to increment by.</param>
        public void IncrementCartItem(int Id, int Qty)
        {
            _CartContents[Id] += Qty;
        }

        /// <summary>
        /// Removes the passed Product object and quantity from the cart.
        /// </summary>
        /// <param name="Id">The Id of the Product object that you want to remove from the cart.</param>
        /// <param name="Qty">The amount of units of the passed Product object that you want to remove from the cart.</param>
        public void RemoveFromCart(int Id, int Qty)
        {

            if (IsInCart(Id))
            {
                if (_CartContents[Id] < Qty)
                {
                    // The Qty requested to be removed is greater than what is in the cart
                    Console.WriteLine("\nYou have attempted to remove an invalid quantity of items from your cart.\nPlease check the quantity and try again.");
                }
                else if (_CartContents[Id] == Qty)
                {
                    // Removing all units
                    _CartContents.Remove(Id);
                    Console.WriteLine("All units of Record# {0} have been successfully removed from your cart", Id);
                }
                else
                {
                    // Partially removing units of an item
                    _CartContents[Id] -= Qty;
                    Console.WriteLine("\n{0} unit(s) of Record# {1} have been successfully removed from your cart", Qty, Id);
                }
            }
            else
            {
                // The item is not in the cart
                Console.WriteLine("\nRecord# {0} is not included in your cart. No changes have been made.", Id);
            }

        }

        /// <summary>
        /// Updates the inventory and writes the updated inventory to disk. 
        /// </summary>
        /// <param name="Inv">A referenece to the current Inventory object</param>
        public void ProcessCart(Inventory Inv)
        {
            // Update the inventory SOH values
            foreach (var cartRow in _CartContents)
                Inv.UpdateSOH(cartRow.Key, cartRow.Value);

            if (Inv.WriteToFile())
            {
                // Successful Checkout. Clear Cart
                _CartContents.Clear();
                Console.WriteLine("\nThank You! Your cart has been successfully checked out.");
            }
            else
            {
                // Roll Back Inventory Changes
                foreach (var cartRow in _CartContents)
                    Inv.UpdateSOH(cartRow.Key, (cartRow.Value * -1));

                Console.WriteLine("An error has occured whilst checking out.");
                Console.WriteLine("Please try again later. All items remain in your cart.");

            }

        }

        /// <summary>
        /// Performs a check to see if the passed Id is currently in the cart.
        /// </summary>
        /// <param name="Id">The Id of the Product that you wish to check.</param>
        /// <returns>True if the passed ID is currenty in the cart.</returns>
        public bool IsInCart(int Id)
        {
            bool inCart = false;

            foreach (KeyValuePair<int, int> cartItem in _CartContents)
            {
                if (cartItem.Key == Id)
                {
                    inCart = true;
                    break;
                }
            }
            return inCart;
        }

        /// <summary>
        /// Gets the current qty of the passed Id in the cart.
        /// </summary>
        /// <param name="Id">The Product ID that you wish to enquire about in the cart.</param>
        /// <returns>null if the item is not in the cart, otherwise the current qty of the ID in the cart.</returns>
        public int? GetCartQty(int Id)
        {
            int? inCartQty = null;

            foreach (KeyValuePair<int, int> cartItem in _CartContents)
            {
                if (cartItem.Key == Id)
                {
                    inCartQty = cartItem.Value;
                    break;
                }
            }
            return inCartQty;
        }

        /// <summary>
        /// Gets the total cost of the current cart
        /// </summary>
        /// <param name="CurrentProductInventory">Requires a reference to the current inventory object for current product pricing.</param>
        /// <returns>The total sum of the cost of all te itemsin the cart.</returns>
        public int GetCartTotalCost(Inventory CurrentProductInventory)
        {
            int rowCost = 0, grandTotal = 0;

            foreach (KeyValuePair<int, int> cartRow in _CartContents)
            {
                Product itemDetail = CurrentProductInventory.RetrieveProductById(cartRow.Key);

                rowCost = itemDetail.UnitPrice * cartRow.Value;
                grandTotal += rowCost;
            }

            return grandTotal;
        }

        #endregion
    }
}
