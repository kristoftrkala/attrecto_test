using System;
using System.Collections.Generic;

namespace Attrecto.Data
{
    public partial class User
    {
        public User()
        {
            Todos = new HashSet<Todo>();
        }

        public int IdUser { get; set; }
        public int FkRole { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }

        public virtual Role FkRoleNavigation { get; set; } = null!;
        public virtual ICollection<Todo> Todos { get; set; }
    }
}
