using System.ComponentModel.DataAnnotations;

namespace ProjectIota.PrivateModels
{
	public class CategoryList
	{
		[Key]
		public int CategoryId { get; set; }
		public string? ParentId { get; set; } //String used instead of int because Syncfusion Dropdowntree control value property can not work with integers.
		public string Name { get; set; }
		public string? ParentCategoryName { get; set; }
	}
}
