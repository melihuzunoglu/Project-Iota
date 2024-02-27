using System;

namespace ProjectIota.Models
{
    public class WebMenu
    {
        public int WebMenuId { get; set; }
        public int? ParentId { get; set; }
        public string Text { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int Position { get; set; }
    }
}
