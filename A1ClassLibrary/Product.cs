using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1ClassLibrary
{
    /// <summary>
    /// Product data object. Contains data such as Id, Title and Unit Price
    /// </summary>
    class Product
    {
        #region Class Data
        public int Id { get; private set; }
        public string Title { get; private set; }
        public int Qty { get; set; }
        public int UnitPrice { get; private set; }
        #endregion

        /// <summary>
        /// Instatiates a new Product object.
        /// </summary>
        /// <param name="Id">The Unique Id of the Product</param>
        /// <param name="Title">The Title of the Product</param>
        /// <param name="Qty">The starting inventory total in units</param>
        /// <param name="UnitPrice">The unit price of the Product (in Sickles)</param>
        #region Constructor

        public Product(int Id, string Title, int Qty, int UnitPrice)
        {
            this.Id = Id;
            this.Title = Title;
            this.Qty = Qty;
            this.UnitPrice = UnitPrice;
        }
        #endregion
    }
}
