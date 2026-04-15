using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class VwProductStats
{
    public Guid ProductId { get; set; }

    public int? ReviewCount { get; set; }

    public decimal RatingAverage { get; set; }

    public int? VerifiedReviewCount { get; set; }
}
