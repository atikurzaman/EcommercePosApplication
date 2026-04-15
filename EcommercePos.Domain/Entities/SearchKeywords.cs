using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class SearchKeywords
{
    public Guid Id { get; set; }

    public string Keyword { get; set; } = null!;

    public int SearchCount { get; set; }

    public DateTime LastSearchedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }
}
