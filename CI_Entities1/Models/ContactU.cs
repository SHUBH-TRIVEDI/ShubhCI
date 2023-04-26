using System;
using System.Collections.Generic;

namespace CI_Entities1.Models;

public partial class ContactU
{
    public int ContactUsId { get; set; }

    public long UserId { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
