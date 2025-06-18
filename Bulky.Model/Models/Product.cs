using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Model.Models;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Model.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string ? Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author  { get; set; }
        [Required]
        [Display(Name ="List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name = "Price price 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "List Price 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "List Price 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        public int categoryID { get; set; }
        [ForeignKey("categoryID")]
        [ValidateNever]
        public Category category { get; set; }
        [ValidateNever]
        [AllowNull]
        public string imageUrl { get; set; }
    }
}
