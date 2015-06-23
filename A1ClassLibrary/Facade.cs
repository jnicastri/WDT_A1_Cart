using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1ClassLibrary
{
    /// <summary>
    /// Provides access to all business logic operations through a single facade.
    /// </summary>
    public class Facade
    {
        #region Class Data
        private SessionOrder _Cart;
        private Inventory _Inv;
        #endregion


        #region Constr.

        /// <summary>
        /// Instatiates a new Facade object, which will initialise a cart and an inventory.
        /// </summary>
        public Facade()
        {
            _Cart = new SessionOrder();
            _Inv = new Inventory();

        }
        #endregion


        #region Program Operations

        /// <summary>
        /// Adds a Product and quantity to the cart.
        /// </summary>
        /// <returns>True if no errors or issues occured whilst adding to the cart.</returns>
        public bool AddToCart()
        {
            _Inv.DisplayUnallocatedInventory(_Cart);

            // Collect the record number of the item to add
            Console.Write("\nPlease enter the Record # of the item to add to the cart: ");
            string IdString = Console.ReadLine();
            int userRequestedId, userRequestedQty;

            try
            {
                userRequestedId = Int32.Parse(IdString.Trim());
            }
            catch (FormatException)
            {
                // User has not entered an int value
                Console.WriteLine("\nYou have not entered an integer. Valid record numbers are integers.");
                return false;
            }

            Console.Write("\nPlease enter the quantity of record# {0} to add to cart: ", IdString.Trim());
            string qtyString = Console.ReadLine();

            try
            {
                userRequestedQty = Int32.Parse(qtyString.Trim());
            }
            catch (FormatException)
            {
                // User has not entered an int value
                Console.WriteLine("\nYou have not entered an integer. Quantity must be an integer.");
                return false;
            }

            if (_Inv.ValidateId(userRequestedId))
            {
                // Check if cart is currently empty.
                if (_Cart.UniqueRecords == 0)
                {
                    // Check available SOH
                    if (_Inv.GetInventorySOH(userRequestedId) >= userRequestedQty)
                    {
                        _Cart.AddToCart(userRequestedId, userRequestedQty);
                        return true;
                    }
                    else
                    {
                        // If there is not enough stock.
                        Console.WriteLine("\nThere is insufficient stock available of Product {0} for this operation to be performed.", IdString.Trim());
                        return false;
                    }
                }
                else
                {
                    //Cart is not empty.
                    if (_Cart.IsInCart(userRequestedId))
                    {
                        if (((int)_Inv.GetInventorySOH(userRequestedId) - (int)_Cart.GetCartQty(userRequestedId)) - userRequestedQty >= 0)
                        {
                            // OK to increment an existing cart item.
                            _Cart.IncrementCartItem(userRequestedId, userRequestedQty);
                            return true;
                        }
                        else
                        {
                            // Allowing requested qty to be added to the cart would leave a negative SOH remainder.
                            Console.WriteLine("\nThere is insufficient stock available of Product {0} for this operation to be performed.", IdString.Trim());
                            return false;
                        }
                    }
                    else
                    {
                        // Check available SOH
                        if (_Inv.GetInventorySOH(userRequestedId) >= userRequestedQty)
                        {
                            _Cart.AddToCart(userRequestedId, userRequestedQty);
                            return true;
                        }
                        else
                        {
                            // If there is not enough stock.
                            Console.WriteLine("\nThere is insufficient stock available of Product {0} for this operation to be performed.", IdString.Trim());
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\nCan't add Record # {0} because there is no Product with that Record #.", IdString.Trim());
                return false;
            }
        }

        /// <summary>
        /// Removes Products & quantities of Products from the cart.
        /// </summary>
        public void RemoveFromCart()
        {
            DisplayCartSummary();

            Console.Write("\nPlease enter the Record # of the item to remove from the cart: ");
            string IdString = Console.ReadLine();
            int userRequestedId, userRequestedQty;

            try
            {
                userRequestedId = Int32.Parse(IdString.Trim());
            }
            catch (FormatException)
            {
                // User has not entered an int value
                Console.WriteLine("\nYou have not entered an integer. Valid record numbers are integers.");
                return;
            }

            Console.Write("\nPlease enter the quantity of record# {0} to remove from the cart: ", IdString.Trim());
            string qtyString = Console.ReadLine();

            try
            {
                userRequestedQty = Int32.Parse(qtyString.Trim());
            }
            catch (FormatException)
            {
                // User has not entered an int value
                Console.WriteLine("\nYou have not entered an integer. Quantity must be an integer.");
                return;
            }

            _Cart.RemoveFromCart(userRequestedId, userRequestedQty);
        }

        /// <summary>
        /// Displays a formatted summary of the current contents of the cart.
        /// </summary>
        public void DisplayCartSummary()
        {
            _Cart.DisplayCart(_Inv);
        }

        /// <summary>
        /// Processes/Checks out the current order in the cart.
        /// </summary>
        public void ProcessCart()
        {
            _Cart.DisplayCart(_Inv);

            Console.WriteLine("\nPlease confirm that you would like to proceed to checkout with the above cart.");
            Console.Write("Enter \"y\" to proceed to checkout, or any other key to modify the cart: ");
            ConsoleKeyInfo option = Console.ReadKey();
            Console.WriteLine();

            if (option.KeyChar.ToString().ToLower() == "y")
            {
                Console.WriteLine("\nThe total cost of your cart is {0} Sickles.", _Cart.GetCartTotalCost(_Inv));
                Console.Write("\nEnter your Credit Cart No. to checkout: ");
                string creditCardNo = Console.ReadLine();
                _Cart.ProcessCart(_Inv);
            }


        }

        /// <summary>
        /// Displays the programs main menu.
        /// </summary>
        public static void DisplayMainMenu()
        {
            Console.WriteLine("\nWELCOME TO THE SHOPPING CART!\n");
            Console.WriteLine("The following options are available to you:\n");
            Console.WriteLine("   1. Add an item to cart,");
            Console.WriteLine("   2. Remove an item from the cart,");
            Console.WriteLine("   3. View the cart");
            Console.WriteLine("   4. Checkout and Pay");
            Console.WriteLine("   5. Exit\n");
            Console.Write("Please choose an option: ");
        }
        #endregion
    }
}
