using System;
using System.Collections.Generic;

namespace Attrecto.Data
{
    public partial class Todo
    {
        public int IdTodo { get; set; }
        public int FkUser { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime? LastModifyDate { get; set; }

        public virtual User FkUserNavigation { get; set; } = null!;
    }
}
